using Planetarium_Urania.Core;

namespace Planetarium_Urania.UI;

// Фабрика стандартных UI-контролов приложения
// Гарантирует единообразие стилей и hover-эффектов во всех формах
public static class UIFactory
{
    // ══ КНОПКИ ════════════════════════════════════════════════════════════

    /// Стандартная кнопка с hover-эффектом
    public static Button CreateButton(string text, int x, int y, int width, Color color)
    {
        var btn = new Button
        {
            Text = text,
            Size = new Size(width, 40),
            Location = new Point(x, y),
            BackColor = color,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = AppTheme.FontButton,
            Cursor = Cursors.Hand
        };
        btn.FlatAppearance.BorderSize = 0;
        AttachHover(btn, color);
        return btn;
    }

    // Кнопка-иконка (используется для обновления таблицы)
    public static Button CreateIconButton(string icon, int size = 46)
    {
        var btn = new Button
        {
            Text = icon,
            Size = new Size(size, 40),
            BackColor = AppTheme.Panel,
            ForeColor = AppTheme.Text,
            FlatStyle = FlatStyle.Flat,
            Font = AppTheme.FontButtonIcon,
            Cursor = Cursors.Hand,
            Anchor = AnchorStyles.Top | AnchorStyles.Right
        };
        btn.FlatAppearance.BorderColor = AppTheme.Border;
        btn.FlatAppearance.BorderSize = 1;

        var hover = Color.FromArgb(65, 65, 68);
        btn.MouseEnter += (_, _) => { if (btn.Enabled) btn.BackColor = hover; };
        btn.MouseLeave += (_, _) => btn.BackColor = AppTheme.Panel;
        return btn;
    }

    // Кнопка диалога Save/Cancel
    public static Button CreateDialogButton(string text, int x, int y, Color color, int width = 120)
        => CreateButton(text, x, y, width, color);

    // ══ МЕТКИ ═════════════════════════════════════════════════════════════

    // Метка с текстом AppTheme.Text, размер и стиль шрифта настраиваются
    public static Label CreateLabel(string text, int x, int y,
        int fontSize = 10, FontStyle style = FontStyle.Regular)
        => new()
        {
            Text = text,
            Location = new Point(x, y),
            AutoSize = true,
            Font = new Font("Segoe UI", fontSize, style),
            ForeColor = AppTheme.Text
        };

    // Подпись поля ввода
    public static Label CreateFieldLabel(string text, int x, int y)
        => new()
        {
            Text = text,
            Location = new Point(x, y),
            AutoSize = true,
            Font = AppTheme.FontLabel,
            ForeColor = AppTheme.TextSecondary
        };

    // ══ ПОЛЯ ВВОДА ════════════════════════════════════════════════════════

    // Однострочное поле с подсветкой фокуса; опционально — режим пароля
    public static TextBox CreateTextBox(int x, int y, int width, bool isPassword = false)
    {
        var tb = new TextBox
        {
            Location = new Point(x, y),
            Size = new Size(width, 25),
            BackColor = AppTheme.Panel,
            ForeColor = AppTheme.Text,
            Font = AppTheme.FontInput,
            BorderStyle = BorderStyle.FixedSingle,
            UseSystemPasswordChar = isPassword
        };
        tb.Enter += (_, _) => tb.BackColor = AppTheme.InputBgFocus;
        tb.Leave += (_, _) => tb.BackColor = AppTheme.Panel;
        return tb;
    }

    // Поле для формы входа
    public static TextBox CreateLoginTextBox(int x, int y, bool isPassword = false)
    {
        var tb = new TextBox
        {
            Location = new Point(x, y),
            Size = new Size(300, 32),
            Font = AppTheme.FontInputLg,
            BackColor = AppTheme.InputBg,
            ForeColor = AppTheme.Text,
            BorderStyle = BorderStyle.FixedSingle,
            UseSystemPasswordChar = isPassword
        };
        tb.Enter += (_, _) => tb.BackColor = AppTheme.InputBgFocus;
        tb.Leave += (_, _) => tb.BackColor = AppTheme.InputBg;
        return tb;
    }

    // ══ ВЫПАДАЮЩИЕ СПИСКИ ═════════════════════════════════════════════════

    public static ComboBox CreateComboBox(int x, int y, int width)
        => new()
        {
            Location = new Point(x, y),
            Size = new Size(width, 25),
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = AppTheme.Panel,
            ForeColor = AppTheme.Text,
            Font = AppTheme.FontInput,
            FlatStyle = FlatStyle.Flat
        };

    // ══ DATAGRIDVIEW ══════════════════════════════════════════════════════

    // Применяет единый темный стиль к переданному DataGridView
    public static void ApplyGridStyle(DataGridView dgv)
    {
        dgv.BackgroundColor = AppTheme.Bg;
        dgv.GridColor = AppTheme.GridLine;
        dgv.BorderStyle = BorderStyle.None;
        dgv.RowHeadersVisible = false;
        dgv.AllowUserToAddRows = false;
        dgv.AllowUserToDeleteRows = false;
        dgv.ReadOnly = true;
        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgv.Font = AppTheme.FontGrid;
        dgv.ColumnHeadersHeight = 45;
        dgv.EnableHeadersVisualStyles = false;
        dgv.RowTemplate.Height = 40;

        dgv.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
        { BackColor = AppTheme.RowAlt, ForeColor = AppTheme.Text };

        dgv.DefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = AppTheme.Bg,
            ForeColor = AppTheme.Text,
            SelectionBackColor = AppTheme.Accent,
            SelectionForeColor = Color.White,
            Padding = new Padding(5),
            WrapMode = DataGridViewTriState.True
        };

        dgv.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = AppTheme.Header,
            ForeColor = AppTheme.Text,
            Font = AppTheme.FontGridHeader,
            Padding = new Padding(8, 10, 8, 10),
            SelectionBackColor = AppTheme.Header,
            SelectionForeColor = AppTheme.Text,
            Alignment = DataGridViewContentAlignment.MiddleLeft
        };
    }

    // ══ ВСПОМОГАТЕЛЬНОЕ ═══════════════════════════════════════════════════

    // Привязывает hover-эффект
    public static void AttachHover(Button btn, Color baseColor)
    {
        var hoverColor = AppTheme.HoverFor(baseColor);
        btn.MouseEnter += (_, _) => { if (btn.Enabled) btn.BackColor = hoverColor; };
        btn.MouseLeave += (_, _) => btn.BackColor = baseColor;
    }

    // Создает нижнюю панель статуса, возвращает (panel, label)
    public static (Panel panel, Label label) CreateStatusBar()
    {
        var panel = new Panel { Dock = DockStyle.Bottom, Height = 35, BackColor = AppTheme.Header };
        var label = new Label
        {
            Text = "Готово к работе",
            Location = new Point(15, 9),
            AutoSize = true,
            Font = AppTheme.FontStatus,
            ForeColor = AppTheme.TextSecondary
        };
        panel.Controls.Add(label);
        return (panel, label);
    }
}