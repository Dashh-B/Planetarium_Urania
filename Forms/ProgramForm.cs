using Planetarium_Urania.Core;
using Planetarium_Urania.UI;

namespace Planetarium_Urania.Forms;

public class ProgramForm : Form
{
    private readonly int? programId;
    private readonly bool isEditMode;

    private TextBox txtTitle = null!;
    private ComboBox cmbTheme = null!;
    private NumericUpDown numDuration = null!;
    private ComboBox cmbAgeRating = null!;
    private NumericUpDown numPrice = null!;
    private TextBox txtDescription = null!;

    public ProgramForm()
    {
        isEditMode = false;
        BuildForm("Добавление программы");
    }

    public ProgramForm(int id, string title, string theme, int duration,
        string ageRating, decimal price, string description)
    {
        isEditMode = true;
        programId = id;
        BuildForm("Редактирование программы");
        FillFields(title, theme, duration, ageRating, price, description);
    }

    private void BuildForm(string title)
    {
        Text = title;
        Size = new Size(600, 550);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        BackColor = AppTheme.Bg;
        BuildUI();
    }

    private void BuildUI()
    {
        Controls.Add(UIFactory.CreateLabel("ДАННЫЕ ПРОГРАММЫ", 20, 20, 14, FontStyle.Bold));

        Controls.Add(UIFactory.CreateLabel("Название программы:", 20, 70));
        txtTitle = UIFactory.CreateTextBox(20, 95, 540);
        Controls.Add(txtTitle);

        Controls.Add(UIFactory.CreateLabel("Тематика:", 20, 135));
        cmbTheme = UIFactory.CreateComboBox(20, 160, 260);
        cmbTheme.Items.AddRange(new object[]
        {
            "Солнечная система", "дальний космос", "история астрономии",
            "детская программа", "специальная программа"
        });
        Controls.Add(cmbTheme);

        Controls.Add(UIFactory.CreateLabel("Длительность (мин):", 300, 135));
        numDuration = MakeNumeric(300, 160, 260, min: 10, max: 180, value: 45);
        Controls.Add(numDuration);

        Controls.Add(UIFactory.CreateLabel("Возрастной рейтинг:", 20, 200));
        cmbAgeRating = UIFactory.CreateComboBox(20, 225, 260);
        cmbAgeRating.Items.AddRange(new object[] { "0+", "6+", "12+", "16+" });
        Controls.Add(cmbAgeRating);

        Controls.Add(UIFactory.CreateLabel("Цена (руб):", 300, 200));
        numPrice = MakeNumeric(300, 225, 260, min: 0, max: 10000, value: 500);
        numPrice.DecimalPlaces = 2;
        numPrice.Increment = 50;
        Controls.Add(numPrice);

        Controls.Add(UIFactory.CreateLabel("Описание:", 20, 265));
        txtDescription = new TextBox
        {
            Location = new Point(20, 290),
            Size = new Size(540, 120),
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            BackColor = AppTheme.Panel,
            ForeColor = AppTheme.Text,
            Font = AppTheme.FontInput,
            BorderStyle = BorderStyle.FixedSingle
        };
        Controls.Add(txtDescription);

        var btnSave = UIFactory.CreateDialogButton("Сохранить", 300, 430, AppTheme.Success);
        var btnCancel = UIFactory.CreateDialogButton("Отмена", 430, 430, AppTheme.Danger);
        btnSave.Click += BtnSave_Click;
        btnCancel.Click += (_, _) => Close();
        Controls.Add(btnSave);
        Controls.Add(btnCancel);
    }

    private void FillFields(string title, string theme, int duration,
        string ageRating, decimal price, string description)
    {
        txtTitle.Text = title;
        cmbTheme.Text = theme;
        numDuration.Value = duration;
        cmbAgeRating.Text = ageRating;
        numPrice.Value = price;
        txtDescription.Text = description;
    }

    // NumericUpDown специфичен для этой формы — в UIFactory не выносится
    private static NumericUpDown MakeNumeric(int x, int y, int width,
        decimal min, decimal max, decimal value)
        => new()
        {
            Location = new Point(x, y),
            Size = new Size(width, 30),
            Minimum = min,
            Maximum = max,
            Value = value,
            BackColor = AppTheme.Panel,
            ForeColor = AppTheme.Text,
            Font = AppTheme.FontInput,
            BorderStyle = BorderStyle.FixedSingle
        };

    private void BtnSave_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtTitle.Text))
        {
            MessageBox.Show("Введите название программы", "Предупреждение",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (cmbTheme.SelectedIndex == -1)
        {
            MessageBox.Show("Выберите тематику", "Предупреждение",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (cmbAgeRating.SelectedIndex == -1)
        {
            MessageBox.Show("Выберите возрастной рейтинг", "Предупреждение",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        bool success = isEditMode && programId.HasValue
            ? DatabaseHelper.UpdateProgram(programId.Value, txtTitle.Text, cmbTheme.Text,
                (int)numDuration.Value, cmbAgeRating.Text, numPrice.Value, txtDescription.Text)
            : DatabaseHelper.AddProgram(txtTitle.Text, cmbTheme.Text,
                (int)numDuration.Value, cmbAgeRating.Text, numPrice.Value, txtDescription.Text);

        if (success)
        {
            MessageBox.Show(
                isEditMode ? "Программа успешно обновлена!" : "Программа успешно добавлена!",
                "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}