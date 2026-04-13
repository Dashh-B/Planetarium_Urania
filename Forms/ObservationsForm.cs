using Planetarium_Urania.Core;
using Planetarium_Urania.Forms.Observations;
using Planetarium_Urania.UI;

namespace Planetarium_Urania.Forms;

public class ObservationsForm : Form
{
    private DataGridView dgvObservations = null!;
    private Button btnAdd = null!;
    private Button btnAddMetadata = null!;
    private Button btnViewMetadata = null!;
    private Button btnRefresh = null!;
    private Label lblStatus = null!;

    public ObservationsForm()
    {
        Text = "Планетарий \"Урания\" - Наблюдения";
        Size = new Size(1400, 700);
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(1200, 600);
        BackColor = AppTheme.Bg;

        BuildUI();
        Load += ObservationsForm_Load;
    }

    private void BuildUI()
    {
        // ── Верхняя панель ────────────────────────────────────────────────
        var panelTop = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = AppTheme.Header };

        panelTop.Controls.AddRange(new Control[]
        {
            new Label
            {
                Text = "🔭 ЖУРНАЛ НАБЛЮДЕНИЙ", Font = AppTheme.FontTitle,
                ForeColor = AppTheme.Text, Location = new Point(20, 12), AutoSize = true
            },
            new Label
            {
                Text = "Астрономические наблюдения и метаданные", Font = AppTheme.FontSubtitle,
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

        btnAdd = UIFactory.CreateButton("➕ Добавить наблюдение", 10, 15, 230, AppTheme.Success);
        btnAdd.Click += BtnAdd_Click;
        btnAdd.Enabled = AuthService.CanWrite;
        btnAdd.BackColor = AuthService.CanWrite ? AppTheme.Success : AppTheme.SuccessMuted;

        btnAddMetadata = UIFactory.CreateButton("📝 Добавить метаданные", 250, 15, 230, AppTheme.Warning);
        btnAddMetadata.Click += BtnAddMetadata_Click;
        btnAddMetadata.Enabled = AuthService.CanWrite;
        btnAddMetadata.BackColor = AuthService.CanWrite ? AppTheme.Warning : AppTheme.WarningMuted;

        btnViewMetadata = UIFactory.CreateButton("👁️ Просмотр метаданных", 490, 15, 230, AppTheme.Accent);
        btnViewMetadata.Click += BtnViewMetadata_Click;

        btnRefresh = UIFactory.CreateIconButton("🔄");
        btnRefresh.Location = new Point(1400 - 56, 15);
        btnRefresh.Click += (_, _) => LoadObservations();

        panelButtons.Controls.AddRange(new Control[]
            { btnAdd, btnAddMetadata, btnViewMetadata, btnRefresh });

        panelButtons.Layout += (_, _) =>
            btnRefresh.Location = new Point(panelButtons.Width - 56, 15);

        // ── DataGridView ──────────────────────────────────────────────────
        dgvObservations = new DataGridView
        { Dock = DockStyle.Fill, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
        UIFactory.ApplyGridStyle(dgvObservations);

        // ── Статус-бар ────────────────────────────────────────────────────
        var (panelBottom, lbl) = UIFactory.CreateStatusBar();
        lblStatus = lbl;

        Controls.Add(dgvObservations);
        Controls.Add(panelButtons);
        Controls.Add(panelTop);
        Controls.Add(panelBottom);
    }

    private void ObservationsForm_Load(object sender, EventArgs e)
    {
        UpdateStatus("Проверка подключения к базам данных...", AppTheme.TextSecondary);

        if (!DatabaseHelper.TestConnection())
        {
            UpdateStatus("✗ Ошибка подключения к PostgreSQL", AppTheme.Danger);
            return;
        }

        if (!MongoDbHelper.TestConnection())
            UpdateStatus("⚠ MongoDB недоступна (метаданные будут недоступны)", AppTheme.Warning);
        else
            MongoDbHelper.Initialize();

        UpdateStatus("✓ Подключения установлены", AppTheme.Success);
        LoadObservations();
    }

    private void BtnAdd_Click(object sender, EventArgs e)
    {
        if (!AuthService.CanWrite) { ShowAccessDenied("добавления наблюдений"); return; }
        var form = new AddObservationForm();
        if (form.ShowDialog() == DialogResult.OK) LoadObservations();
    }

    private void BtnAddMetadata_Click(object sender, EventArgs e)
    {
        if (!AuthService.CanWrite) { ShowAccessDenied("добавления метаданных"); return; }

        if (dgvObservations.SelectedRows.Count == 0)
        {
            MessageBox.Show("Выберите наблюдение для добавления метаданных",
                "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        int observationId = Convert.ToInt32(dgvObservations.SelectedRows[0].Cells["ID"].Value);
        var existing = MongoDbHelper.GetMetadataByObservationId(observationId);

        if (existing is not null &&
            MessageBox.Show(
                "Метаданные для этого наблюдения уже существуют. Открыть для редактирования?",
                "Метаданные существуют", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            == DialogResult.No) return;

        var form = new MetadataForm(observationId);
        if (form.ShowDialog() == DialogResult.OK)
            UpdateStatus("✓ Метаданные успешно сохранены", AppTheme.Success);
    }

    private void BtnViewMetadata_Click(object sender, EventArgs e)
    {
        if (dgvObservations.SelectedRows.Count == 0)
        {
            MessageBox.Show("Выберите наблюдение для просмотра метаданных",
                "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        int observationId = Convert.ToInt32(dgvObservations.SelectedRows[0].Cells["ID"].Value);
        var metadata = MongoDbHelper.GetMetadataByObservationId(observationId);

        if (metadata is null)
        {
            MessageBox.Show(
                "Метаданные для этого наблюдения отсутствуют.\nИспользуйте кнопку 'Добавить метаданные'.",
                "Метаданные не найдены", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        new ViewMetadataForm(metadata).ShowDialog();
    }

    private void LoadObservations()
    {
        UpdateStatus("Загрузка наблюдений...", AppTheme.TextSecondary);
        Application.DoEvents();

        var dt = DatabaseHelper.GetObservations();
        dgvObservations.DataSource = dt;

        if (dgvObservations.Columns.Count > 0)
        {
            dgvObservations.Columns["ID"].Width = 50;
            dgvObservations.Columns["Дата"].Width = 100;
            dgvObservations.Columns["Время"].Width = 80;
            dgvObservations.Columns["Объект"].Width = 180;
            dgvObservations.Columns["Телескоп"].Width = 150;
            dgvObservations.Columns["Астроном"].Width = 180;
            dgvObservations.Columns["Тип"].Width = 120;
            dgvObservations.Columns["Погода"].Width = 150;
            dgvObservations.Columns["Участников"].Width = 100;
            dgvObservations.Columns["Примечания"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        var withMetadata = MongoDbHelper.GetAllObservationIdsWithMetadata();
        int withoutMetadata = 0;

        foreach (DataGridViewRow row in dgvObservations.Rows)
        {
            if (!withMetadata.Contains(Convert.ToInt32(row.Cells["ID"].Value)))
            {
                withoutMetadata++;
                row.DefaultCellStyle.BackColor = Color.FromArgb(60, 40, 40);
                row.DefaultCellStyle.SelectionBackColor = Color.FromArgb(120, 60, 60);
            }
        }

        UpdateStatus(
            $"✓ Загружено наблюдений: {dt.Rows.Count} (без метаданных: {withoutMetadata})",
            AppTheme.Success);
    }

    private void UpdateStatus(string message, Color color)
    {
        lblStatus.Text = message;
        lblStatus.ForeColor = color;
    }

    private void ShowAccessDenied(string action) =>
        MessageBox.Show(
            $"У вас нет прав для {action}.\n\nВаша роль: {AuthService.RoleDisplayName}",
            "Доступ запрещен", MessageBoxButtons.OK, MessageBoxIcon.Warning);
}