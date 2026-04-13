namespace Planetarium_Urania.Core;

// Роли пользователей (соответствуют полю role в таблице app_users)
public enum UserRole
{
    Viewer,   // только чтение
    Manager,  // чтение + запись + статистика
    Admin     // полный доступ, включая удаление
}

// Статический сервис авторизации, хранит текущего пользователя на время сессии
public static class AuthService
{
    private static UserRole? _role;
    private static string _username = "";

    public static string Username => _username;

    // Роль текущего пользователя; бросает исключение до входа
    public static UserRole Role =>
        _role ?? throw new InvalidOperationException("Попытка получить роль до авторизации.");

    public static bool IsAuthenticated => _role.HasValue;

    public static void SetCurrentUser(string username, UserRole role)
    {
        _username = username;
        _role = role;
    }

    public static void Logout()
    {
        _username = "";
        _role = null;
    }

    // ── Проверки прав ──────────────────────────────────────────────────────

    // Добавление/редактирование данных и просмотр статистики (Manager и Admin)
    public static bool CanWrite => _role is UserRole.Manager or UserRole.Admin;

    // Удаление записей (только Admin)
    public static bool CanDelete => _role == UserRole.Admin;

    public static bool IsAdmin => _role == UserRole.Admin;

    public static string RoleDisplayName => _role switch
    {
        UserRole.Admin => "Администратор",
        UserRole.Manager => "Менеджер",
        UserRole.Viewer => "Наблюдатель",
        _ => "Не авторизован"
    };
}