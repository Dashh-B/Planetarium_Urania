using System.Data;
using Npgsql;

namespace Planetarium_Urania.Core;

public static class DatabaseHelper
{
    // ── Строки подключения ────────────────────────────────────────────────

    internal const string AuthConnectionString =
        "Host=localhost;Port=4242;Database=postgres;" +
        "Username=urania_auth;Password=auth_password";

    private static readonly Dictionary<UserRole, string> ConnectionStrings = new()
    {
        [UserRole.Admin] = "Host=localhost;Port=4242;Database=postgres;Username=urania_admin;Password=admin_password",
        [UserRole.Manager] = "Host=localhost;Port=4242;Database=postgres;Username=urania_manager;Password=manager_password",
        [UserRole.Viewer] = "Host=localhost;Port=4242;Database=postgres;Username=urania_viewer;Password=viewer_password"
    };

    private static string _connectionString = "";

    public static void SetConnectionByRole(UserRole role)
    {
        if (!ConnectionStrings.TryGetValue(role, out var cs))
            throw new ArgumentException($"Неизвестная роль: {role}");
        _connectionString = cs;
    }

    private static NpgsqlConnection GetConnection()
    {
        if (string.IsNullOrEmpty(_connectionString))
            throw new InvalidOperationException("Строка подключения не установлена. Выполните вход.");
        return new NpgsqlConnection(_connectionString);
    }

    // ── Проверка подключения ──────────────────────────────────────────────

    public static bool TestConnection()
    {
        try
        {
            using var conn = GetConnection();
            conn.Open();
            return true;
        }
        catch (Exception ex) { ShowError("проверке подключения", ex); return false; }
    }

    // ── Авторизация ───────────────────────────────────────────────────────

    // Единственная точка проверки учетных данных
    public static UserRole? ValidateUser(string username, string passwordHash)
    {
        try
        {
            using var conn = new NpgsqlConnection(AuthConnectionString);
            conn.Open();

            const string sql = """
                SELECT role FROM app_users
                WHERE  username      = @username
                  AND  password_hash = @hash
                  AND  is_active     = TRUE
                LIMIT 1
                """;

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@hash", passwordHash);

            var value = cmd.ExecuteScalar();
            return value is null or DBNull ? null : value.ToString() switch
            {
                "admin" => UserRole.Admin,
                "manager" => UserRole.Manager,
                "viewer" => UserRole.Viewer,
                _ => null
            };
        }
        catch { return null; }
    }

    // ══ ПРОГРАММЫ ══════════════════════════════════════════════════════════

    public static DataTable GetActivePrograms()
    {
        var dt = new DataTable();
        try
        {
            using var conn = GetConnection();
            conn.Open();
            const string query = """
                SELECT id          AS "ID",
                       title       AS "Название",
                       theme       AS "Тематика",
                       duration    AS "Длительность (мин)",
                       age_rating  AS "Возраст",
                       price       AS "Цена (руб)",
                       description AS "Описание"
                FROM  programs
                WHERE is_active = TRUE
                ORDER BY id
                """;
            using var adapter = new NpgsqlDataAdapter(query, conn);
            adapter.Fill(dt);
        }
        catch (Exception ex) { ShowError("получении программ", ex); }
        return dt;
    }

    public static bool AddProgram(string title, string theme, int duration,
        string ageRating, decimal price, string description)
    {
        try
        {
            using var conn = GetConnection();
            conn.Open();
            const string query = """
                INSERT INTO programs (title, theme, duration, age_rating, price, description, is_active)
                VALUES (@title, @theme, @duration, @ageRating, @price, @description, TRUE)
                """;
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@theme", theme);
            cmd.Parameters.AddWithValue("@duration", duration);
            cmd.Parameters.AddWithValue("@ageRating", ageRating);
            cmd.Parameters.AddWithValue("@price", price);
            cmd.Parameters.AddWithValue("@description", (object?)description ?? DBNull.Value);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception ex) { ShowError("добавлении программы", ex); return false; }
    }

    public static bool UpdateProgram(int id, string title, string theme, int duration,
        string ageRating, decimal price, string description)
    {
        try
        {
            using var conn = GetConnection();
            conn.Open();
            const string query = """
                UPDATE programs
                SET title       = @title,
                    theme       = @theme,
                    duration    = @duration,
                    age_rating  = @ageRating,
                    price       = @price,
                    description = @description
                WHERE id = @id
                """;
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@theme", theme);
            cmd.Parameters.AddWithValue("@duration", duration);
            cmd.Parameters.AddWithValue("@ageRating", ageRating);
            cmd.Parameters.AddWithValue("@price", price);
            cmd.Parameters.AddWithValue("@description", (object?)description ?? DBNull.Value);
            return cmd.ExecuteNonQuery() > 0;
        }
        catch (Exception ex) { ShowError("обновлении программы", ex); return false; }
    }

    public static bool DeleteProgram(int id)
    {
        try
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new NpgsqlCommand(
                "UPDATE programs SET is_active = FALSE WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            return cmd.ExecuteNonQuery() > 0;
        }
        catch (Exception ex) { ShowError("удалении программы", ex); return false; }
    }

    // ══ СТАТИСТИКА ═════════════════════════════════════════════════════════

    public static DataTable GetProgramsStatistics()
    {
        var dt = new DataTable();
        try
        {
            using var conn = GetConnection();
            conn.Open();
            const string query = """
                SELECT p.title                                    AS "Программа",
                       p.theme                                    AS "Тематика",
                       COUNT(DISTINCT s.id)                       AS "Количество сеансов",
                       COALESCE(SUM(s.tickets_sold), 0)           AS "Продано билетов",
                       COALESCE(SUM(s.tickets_sold * p.price), 0) AS "Выручка (руб)"
                FROM  programs p
                LEFT JOIN sessions s
                       ON p.id = s.program_id
                      AND s.status IN ('запланирован', 'завершен')
                WHERE p.is_active = TRUE
                GROUP BY p.id, p.title, p.theme
                ORDER BY SUM(s.tickets_sold) DESC NULLS LAST
                """;
            using var adapter = new NpgsqlDataAdapter(query, conn);
            adapter.Fill(dt);
        }
        catch (Exception ex) { ShowError("получении статистики", ex); }
        return dt;
    }

    // ══ НАБЛЮДЕНИЯ ═════════════════════════════════════════════════════════

    // Возвращает наблюдения, отсортированные по убыванию даты и времени
    public static DataTable GetObservations()
    {
        var dt = new DataTable();
        try
        {
            using var conn = GetConnection();
            conn.Open();
            const string query = """
                SELECT o.id                 AS "ID",
                       o.observation_date   AS "Дата",
                       o.observation_time   AS "Время",
                       ao.name              AS "Объект",
                       t.name               AS "Телескоп",
                       e.full_name          AS "Астроном",
                       o.observation_type   AS "Тип",
                       o.weather_conditions AS "Погода",
                       o.participants_count AS "Участников",
                       o.notes              AS "Примечания"
                FROM  observations o
                JOIN  astronomical_objects ao ON o.object_id     = ao.id
                JOIN  telescopes           t  ON o.telescope_id  = t.id
                JOIN  employees            e  ON o.astronomer_id = e.id
                ORDER BY o.observation_date DESC, o.observation_time DESC
                """;
            using var adapter = new NpgsqlDataAdapter(query, conn);
            adapter.Fill(dt);
        }
        catch (Exception ex) { ShowError("получении наблюдений", ex); }
        return dt;
    }

    public static DataTable GetAstronomicalObjects()
    {
        var dt = new DataTable();
        try
        {
            using var conn = GetConnection();
            conn.Open();
            using var adapter = new NpgsqlDataAdapter(
                "SELECT id, name FROM astronomical_objects ORDER BY id", conn);
            adapter.Fill(dt);
        }
        catch (Exception ex) { ShowError("получении объектов", ex); }
        return dt;
    }

    public static DataTable GetTelescopes()
    {
        var dt = new DataTable();
        try
        {
            using var conn = GetConnection();
            conn.Open();
            using var adapter = new NpgsqlDataAdapter(
                "SELECT id, name FROM telescopes WHERE status = 'доступен' ORDER BY name", conn);
            adapter.Fill(dt);
        }
        catch (Exception ex) { ShowError("получении телескопов", ex); }
        return dt;
    }

    public static DataTable GetAstronomers()
    {
        var dt = new DataTable();
        try
        {
            using var conn = GetConnection();
            conn.Open();
            const string q = """
                SELECT id, full_name FROM employees
                WHERE  position  IN ('астроном', 'лектор')
                  AND  is_active = TRUE
                ORDER BY full_name
                """;
            using var adapter = new NpgsqlDataAdapter(q, conn);
            adapter.Fill(dt);
        }
        catch (Exception ex) { ShowError("получении астрономов", ex); }
        return dt;
    }

    public static int AddObservation(DateTime date, TimeSpan time,
        int telescopeId, int objectId, int astronomerId,
        string? weatherConditions, string observationType,
        int? participantsCount, string? notes)
    {
        try
        {
            using var conn = GetConnection();
            conn.Open();
            const string query = """
                INSERT INTO observations
                    (observation_date, observation_time, telescope_id, object_id,
                     astronomer_id, weather_conditions, observation_type,
                     participants_count, notes)
                VALUES
                    (@date, @time, @telescopeId, @objectId, @astronomerId,
                     @weatherConditions, @observationType, @participantsCount, @notes)
                RETURNING id
                """;
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@date", date);
            cmd.Parameters.AddWithValue("@time", time);
            cmd.Parameters.AddWithValue("@telescopeId", telescopeId);
            cmd.Parameters.AddWithValue("@objectId", objectId);
            cmd.Parameters.AddWithValue("@astronomerId", astronomerId);
            cmd.Parameters.AddWithValue("@weatherConditions", (object?)weatherConditions ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@observationType", observationType);
            cmd.Parameters.AddWithValue("@participantsCount", (object?)participantsCount ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@notes", (object?)notes ?? DBNull.Value);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }
        catch (Exception ex) { ShowError("добавлении наблюдения", ex); return -1; }
    }

    private static void ShowError(string operation, Exception ex) =>
        MessageBox.Show($"Ошибка при {operation}:\n{ex.Message}",
            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
}