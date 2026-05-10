using Planetarium_Urania.Core;
using Planetarium_Urania.UI;

namespace Planetarium_Urania.Forms;

public class MainForm : Form
{
    private DataGridView dgvPrograms = null!;
    private Button btnLoadPrograms = null!;
    private Button btnLoadStatistics = null!;
    private Button btnAdd = null!;
    private Button btnEdit = null!;
    private Button btnDelete = null!;
    private Button btnObservations = null!;
    private Button btnRefresh = null!;
    private Label lblStatus = null!;

    public MainForm()
    {
        Text = "Планетарий \"Урания\" - Управление программами";
        Size = new Size(1200, 700);
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(1000, 600);
        BackColor = AppTheme.Bg;

        BuildUI();
        Load += MainForm_Load;
    }

    private void BuildUI()
    {
        // ── Верхняя панель ────────────────────────────────────────────────
        var panelTop = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = AppTheme.Header };

        panelTop.Controls.AddRange(new Control[]
        {
            new Label
            {
                Text = "🌟 ПЛАНЕТАРИЙ \"УРАНИЯ\"", Font = AppTheme.FontTitle,
                ForeColor = AppTheme.Text, Location = new Point(20, 12), AutoSize = true
            },
            new Label
            {
                Text = "Управление показами и наблюдениями", Font = AppTheme.FontSubtitle,
                ForeColor = AppTheme.TextSecondary, Location = new Point(20, 48), AutoSize = true
            }
        });

        var lblUser = new Label
        {
            Text = $"👤 {AuthService.Username}  •  {AuthService.RoleDisplayName}",
            Font = AppTheme.FontUserInfo,
            ForeColor = AppTheme.TextSecondary,
            AutoSize = true
        };
        panelTop.Controls.Add(lblUser);

        Label? lblReadOnly = null;
        if (!AuthService.CanWrite)
        {
            lblReadOnly = new Label
            {
                Text = "🔒 Режим просмотра",
                Font = AppTheme.FontReadOnly,
                ForeColor = AppTheme.TextWarning,
                AutoSize = true
            };
            panelTop.Controls.Add(lblReadOnly);
        }

        panelTop.Layout += (_, _) =>
        {
            lblUser.Location = new Point(panelTop.Width - lblUser.Width - 20, 28);
            if (lblReadOnly is not null)
                lblReadOnly.Location = new Point(panelTop.Width - lblReadOnly.Width - 20, 52);
        };

        // ── Панель кнопок ─────────────────────────────────────────────────
        var panelButtons = new Panel
        { Dock = DockStyle.Top, Height = 70, BackColor = AppTheme.Panel, Padding = new Padding(10) };

        btnLoadPrograms = UIFactory.CreateButton("📋 Список программ", 10, 15, 180, AppTheme.Accent);
        btnLoadStatistics = UIFactory.CreateButton("📊 Статистика", 200, 15, 140, AppTheme.Warning);
        btnAdd = UIFactory.CreateButton("➕ Добавить", 350, 15, 130, AppTheme.Success);
        btnEdit = UIFactory.CreateButton("✏️ Изменить", 490, 15, 130, AppTheme.Accent);
        btnDelete = UIFactory.CreateButton("🗑️ Удалить", 630, 15, 130, AppTheme.Danger);

        btnLoadPrograms.Click += (_, _) => LoadPrograms();
        btnLoadStatistics.Click += (_, _) => LoadStatistics();
        btnAdd.Click += BtnAdd_Click;
        btnEdit.Click += BtnEdit_Click;
        btnDelete.Click += BtnDelete_Click;

        int observationsX = AuthService.CanWrite ? 770 : 630;
        btnObservations = UIFactory.CreateButton("🔭 Наблюдения", observationsX, 15, 150, AppTheme.Purple);
        btnObservations.Click += (_, _) => new ObservationsForm().ShowDialog();

        btnRefresh = UIFactory.CreateIconButton("🔄");
        btnRefresh.Location = new Point(1200 - 56, 15);
        btnRefresh.Click += (_, _) => { if (dgvPrograms.DataSource is not null) LoadPrograms(); };

        panelButtons.Controls.AddRange(new Control[]
            { btnLoadPrograms, btnLoadStatistics, btnAdd, btnEdit, btnDelete, btnObservations, btnRefresh });

        panelButtons.Layout += (_, _) =>
            btnRefresh.Location = new Point(panelButtons.Width - 56, 15);

        // ── DataGridView ──────────────────────────────────────────────────
        dgvPrograms = new DataGridView
        { Dock = DockStyle.Fill, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
        UIFactory.ApplyGridStyle(dgvPrograms);

        // ── Статус-бар ────────────────────────────────────────────────────
        var (panelBottom, lbl) = UIFactory.CreateStatusBar();
        lblStatus = lbl;

        Controls.Add(dgvPrograms);
        Controls.Add(panelButtons);
        Controls.Add(panelTop);
        Controls.Add(panelBottom);
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
        UpdateStatus("Проверка подключения к базе данных...", AppTheme.TextSecondary);

        if (DatabaseHelper.TestConnection())
        {
            UpdateStatus("✓ Подключение установлено", AppTheme.Success);
            ApplyPermissions();
            LoadPrograms();
        }
        else
        {
            UpdateStatus("✗ Ошибка подключения", AppTheme.Danger);
        }
    }

    private void ApplyPermissions()
    {
        btnAdd.Enabled = AuthService.CanWrite;
        btnEdit.Enabled = AuthService.CanWrite;
        if (!AuthService.CanWrite)
        {
            btnAdd.BackColor = AppTheme.SuccessMuted;
            btnEdit.BackColor = AppTheme.AccentMuted;
        }

        switch (AuthService.Role)
        {
            case UserRole.Admin:
                btnDelete.Visible = true;
                btnDelete.Enabled = true;
                break;
            case UserRole.Manager:
                btnDelete.Visible = true;
                btnDelete.Enabled = false;
                btnDelete.BackColor = AppTheme.DangerMuted;
                break;
            case UserRole.Viewer:
                btnDelete.Visible = false;
                break;
        }

        btnLoadStatistics.Enabled = AuthService.CanWrite;
        btnLoadStatistics.BackColor = AuthService.CanWrite ? AppTheme.Warning : AppTheme.WarningMuted;

        Text = $"Планетарий «Урания»  —  {AuthService.Username} ({AuthService.RoleDisplayName})";
    }

    private void BtnAdd_Click(object sender, EventArgs e)
    {
        if (!AuthService.CanWrite) { ShowAccessDenied(); return; }
        var form = new ProgramForm();
        if (form.ShowDialog() == DialogResult.OK) LoadPrograms();
    }

    private void BtnEdit_Click(object sender, EventArgs e)
    {
        if (!AuthService.CanWrite) { ShowAccessDenied(); return; }
        if (dgvPrograms.SelectedRows.Count == 0)
        {
            MessageBox.Show("Выберите программу для редактирования", "Предупреждение",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (!dgvPrograms.Columns.Contains("ID"))
        {
            MessageBox.Show("Переключитесь в режим «Список программ» для редактирования.",
                "Недоступно", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        var row = dgvPrograms.SelectedRows[0];
        var form = new ProgramForm(
            Convert.ToInt32(row.Cells["ID"].Value),
            row.Cells["Название"].Value!.ToString()!,
            row.Cells["Тематика"].Value!.ToString()!,
            Convert.ToInt32(row.Cells["Длительность (мин)"].Value),
            row.Cells["Возраст"].Value!.ToString()!,
            Convert.ToDecimal(row.Cells["Цена (руб)"].Value),
            row.Cells["Описание"].Value?.ToString() ?? "");
        if (form.ShowDialog() == DialogResult.OK) LoadPrograms();
    }

    private void BtnDelete_Click(object sender, EventArgs e)
    {
        if (!AuthService.CanDelete) { ShowAccessDenied(); return; }
        if (dgvPrograms.SelectedRows.Count == 0)
        {
            MessageBox.Show("Выберите программу для удаления", "Предупреждение",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        var row = dgvPrograms.SelectedRows[0];
        string title = row.Cells["Название"].Value!.ToString()!;

        if (MessageBox.Show($"Вы уверены, что хотите удалить программу \"{title}\"?",
                "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            == DialogResult.Yes)
        {
            if (DatabaseHelper.DeleteProgram(Convert.ToInt32(row.Cells["ID"].Value)))
            {
                MessageBox.Show("Программа успешно удалена!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadPrograms();
            }
        }
    }

    private void LoadPrograms()
    {
        UpdateStatus("Загрузка списка программ...", AppTheme.TextSecondary);
        Application.DoEvents();

        var dt = DatabaseHelper.GetActivePrograms();
        dgvPrograms.DataSource = dt;

        if (dgvPrograms.Columns.Count > 0)
        {
            dgvPrograms.Columns["ID"].Width = 60;
            dgvPrograms.Columns["Название"].Width = 250;
            dgvPrograms.Columns["Тематика"].Width = 180;
            dgvPrograms.Columns["Длительность (мин)"].Width = 140;
            dgvPrograms.Columns["Возраст"].Width = 80;
            dgvPrograms.Columns["Цена (руб)"].Width = 100;
            dgvPrograms.Columns["Описание"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        UpdateStatus($"✓ Загружено программ: {dt.Rows.Count}", AppTheme.Success);
    }

    private void LoadStatistics()
    {
        if (!AuthService.CanWrite) { ShowAccessDenied(); return; }

        UpdateStatus("Загрузка статистики...", AppTheme.TextSecondary);
        Application.DoEvents();

        var dt = DatabaseHelper.GetProgramsStatistics();
        dgvPrograms.DataSource = dt;

        if (dgvPrograms.Columns.Count > 0)
        {
            dgvPrograms.Columns["Программа"].Width = 280;
            dgvPrograms.Columns["Тематика"].Width = 200;
            dgvPrograms.Columns["Количество сеансов"].Width = 160;
            dgvPrograms.Columns["Продано билетов"].Width = 150;
            dgvPrograms.Columns["Выручка (руб)"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        UpdateStatus("✓ Статистика загружена", AppTheme.Success);
    }

    private void UpdateStatus(string message, Color color)
    {
        lblStatus.Text = message;
        lblStatus.ForeColor = color;
    }

    private void ShowAccessDenied() =>
        MessageBox.Show(
            $"У вас нет прав для выполнения этого действия.\n\nВаша роль: {AuthService.RoleDisplayName}",
            "Доступ запрещен", MessageBoxButtons.OK, MessageBoxIcon.Warning);
}