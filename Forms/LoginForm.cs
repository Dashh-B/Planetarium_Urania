using System.Security.Cryptography;
using System.Text;
using Planetarium_Urania.Core;
using Planetarium_Urania.UI;

namespace Planetarium_Urania.Forms;

public class LoginForm : Form
{
    private TextBox txtUsername = null!;
    private TextBox txtPassword = null!;
    private Button btnLogin = null!;
    private Label lblError = null!;
    private CheckBox chkShowPassword = null!;

    public UserRole AuthenticatedRole { get; private set; }
    public string AuthenticatedUser { get; private set; } = "";

    public LoginForm()
    {
        Text = "Планетарий «Урания» — Вход";
        Size = new Size(440, 480);
        MinimumSize = new Size(440, 480);
        MaximumSize = new Size(440, 480);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        BackColor = AppTheme.Bg;

        BuildUI();
    }

    private void BuildUI()
    {
        SuspendLayout();

        // ── Шапка ─────────────────────────────────────────────────────────
        var panelTop = new Panel { Dock = DockStyle.Top, Height = 120, BackColor = AppTheme.Header };
        panelTop.Controls.AddRange(new Control[]
        {
            new Label
            {
                Text = "🌟", Font = new Font("Segoe UI", 28), ForeColor = AppTheme.Text,
                Location = new Point(0, 18), Size = new Size(440, 50),
                TextAlign = ContentAlignment.MiddleCenter
            },
            new Label
            {
                Text = "ПЛАНЕТАРИЙ «УРАНИЯ»",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = AppTheme.Text,
                Location = new Point(0, 68), Size = new Size(440, 28),
                TextAlign = ContentAlignment.MiddleCenter
            },
            new Label
            {
                Text = "Управление показами и наблюдениями",
                Font = AppTheme.FontLabel, ForeColor = AppTheme.TextSecondary,
                Location = new Point(0, 94), Size = new Size(440, 22),
                TextAlign = ContentAlignment.MiddleCenter
            }
        });

        // ── Панель формы ──────────────────────────────────────────────────
        var panelForm = new Panel
        { Location = new Point(40, 140), Size = new Size(360, 280), BackColor = AppTheme.Panel };
        panelForm.Paint += (_, e) =>
        {
            using var pen = new Pen(Color.FromArgb(60, 60, 60), 1);
            e.Graphics.DrawRectangle(pen, 0, 0, panelForm.Width - 1, panelForm.Height - 1);
        };

        panelForm.Controls.Add(new Label
        {
            Text = "Авторизация",
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            ForeColor = AppTheme.Text,
            Location = new Point(0, 20),
            Size = new Size(360, 28),
            TextAlign = ContentAlignment.MiddleCenter
        });

        panelForm.Controls.Add(UIFactory.CreateFieldLabel("Имя пользователя", 30, 58));
        txtUsername = UIFactory.CreateLoginTextBox(30, 80);
        txtUsername.KeyDown += (_, e) => { if (e.KeyCode == Keys.Enter) txtPassword.Focus(); };

        panelForm.Controls.Add(UIFactory.CreateFieldLabel("Пароль", 30, 120));
        txtPassword = UIFactory.CreateLoginTextBox(30, 142, isPassword: true);
        txtPassword.KeyDown += (_, e) => { if (e.KeyCode == Keys.Enter) DoLogin(); };

        chkShowPassword = new CheckBox
        {
            Text = "Показать пароль",
            Font = AppTheme.FontLabel,
            ForeColor = AppTheme.TextSecondary,
            Location = new Point(30, 178),
            AutoSize = true,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        chkShowPassword.CheckedChanged += (_, _) =>
            txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;

        lblError = new Label
        {
            Text = "",
            Font = AppTheme.FontLabel,
            ForeColor = AppTheme.Danger,
            Location = new Point(30, 208),
            Size = new Size(300, 18),
            TextAlign = ContentAlignment.MiddleLeft
        };

        btnLogin = new Button
        {
            Text = "Войти",
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            Location = new Point(30, 232),
            Size = new Size(300, 36),
            BackColor = AppTheme.Accent,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnLogin.FlatAppearance.BorderSize = 0;
        btnLogin.Click += (_, _) => DoLogin();
        btnLogin.MouseEnter += (_, _) => btnLogin.BackColor = AppTheme.AccentHover;
        btnLogin.MouseLeave += (_, _) => btnLogin.BackColor = AppTheme.Accent;

        panelForm.Controls.AddRange(new Control[]
            { txtUsername, txtPassword, chkShowPassword, lblError, btnLogin });

        Controls.AddRange(new Control[]
        {
            panelTop, panelForm,
            new Label
            {
                Text = "Обратитесь к администратору для получения учетных данных",
                Font = new Font("Segoe UI", 8), ForeColor = AppTheme.TextSecondary,
                Location = new Point(0, 432), Size = new Size(440, 20),
                TextAlign = ContentAlignment.MiddleCenter
            }
        });

        ResumeLayout();
        ActiveControl = txtUsername;
    }

    private void DoLogin()
    {
        lblError.Text = "";

        if (string.IsNullOrWhiteSpace(txtUsername.Text))
        { ShowError("Введите имя пользователя"); txtUsername.Focus(); return; }
        if (string.IsNullOrWhiteSpace(txtPassword.Text))
        { ShowError("Введите пароль"); txtPassword.Focus(); return; }

        btnLogin.Enabled = false;
        btnLogin.Text = "Проверка...";

        try
        {
            var role = DatabaseHelper.ValidateUser(
                txtUsername.Text.Trim(),
                HashPassword(txtPassword.Text));

            if (role is null)
            {
                ShowError("Неверное имя пользователя или пароль");
                txtPassword.Clear();
                txtPassword.Focus();
                return;
            }

            AuthenticatedRole = role.Value;
            AuthenticatedUser = txtUsername.Text.Trim();
            AuthService.SetCurrentUser(AuthenticatedUser, AuthenticatedRole);
            DatabaseHelper.SetConnectionByRole(AuthenticatedRole);

            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            ShowError("Ошибка подключения к БД");
            MessageBox.Show($"Не удалось проверить учетные данные:\n{ex.Message}",
                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnLogin.Enabled = true;
            btnLogin.Text = "Войти";
        }
    }

    // SHA-256 хэш пароля в hex-строке нижнего регистра
    internal static string HashPassword(string password)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(hash).ToLower();
    }

    private void ShowError(string message) => lblError.Text = "⚠ " + message;
}