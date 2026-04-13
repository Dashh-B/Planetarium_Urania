using Planetarium_Urania.Core;
using Planetarium_Urania.UI;

namespace Planetarium_Urania.Forms.Observations;

public class AddObservationForm : Form
{
    private DateTimePicker dtpDate = null!;
    private DateTimePicker dtpTime = null!;
    private ComboBox cmbObject = null!;
    private ComboBox cmbTelescope = null!;
    private ComboBox cmbAstronomer = null!;
    private ComboBox cmbObservationType = null!;
    private TextBox txtWeatherConditions = null!;
    private NumericUpDown numParticipants = null!;
    private TextBox txtNotes = null!;
    private CheckBox chkAddMetadata = null!;

    public AddObservationForm()
    {
        if (!AuthService.CanWrite)
        {
            MessageBox.Show("У вас нет прав для добавления наблюдений.",
                "Доступ запрещен", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Load += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };
            return;
        }

        Text = "Добавление наблюдения";
        Size = new Size(600, 600);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        BackColor = AppTheme.Bg;

        BuildUI();
        LoadComboBoxData();
    }

    private void BuildUI()
    {
        int y = 20;

        Controls.Add(UIFactory.CreateLabel("НОВОЕ НАБЛЮДЕНИЕ", 20, y, 14, FontStyle.Bold));
        y += 40;

        Controls.Add(UIFactory.CreateLabel("Дата наблюдения:", 20, y));
        dtpDate = new DateTimePicker
        { Location = new Point(200, y), Size = new Size(360, 25), Format = DateTimePickerFormat.Short, Value = DateTime.Now };
        Controls.Add(dtpDate);
        y += 35;

        Controls.Add(UIFactory.CreateLabel("Время:", 20, y));
        dtpTime = new DateTimePicker
        { Location = new Point(200, y), Size = new Size(360, 25), Format = DateTimePickerFormat.Time, ShowUpDown = true, Value = DateTime.Now };
        Controls.Add(dtpTime);
        y += 35;

        Controls.Add(UIFactory.CreateLabel("Астрономический объект:", 20, y));
        cmbObject = UIFactory.CreateComboBox(200, y, 360);
        Controls.Add(cmbObject);
        y += 35;

        Controls.Add(UIFactory.CreateLabel("Телескоп:", 20, y));
        cmbTelescope = UIFactory.CreateComboBox(200, y, 360);
        Controls.Add(cmbTelescope);
        y += 35;

        Controls.Add(UIFactory.CreateLabel("Астроном:", 20, y));
        cmbAstronomer = UIFactory.CreateComboBox(200, y, 360);
        Controls.Add(cmbAstronomer);
        y += 35;

        Controls.Add(UIFactory.CreateLabel("Тип наблюдения:", 20, y));
        cmbObservationType = UIFactory.CreateComboBox(200, y, 360);
        cmbObservationType.Items.AddRange(new object[]
            { "публичное", "научное", "образовательное", "фотосессия" });
        Controls.Add(cmbObservationType);
        y += 35;

        Controls.Add(UIFactory.CreateLabel("Погодные условия:", 20, y));
        txtWeatherConditions = UIFactory.CreateTextBox(200, y, 360);
        Controls.Add(txtWeatherConditions);
        y += 35;

        Controls.Add(UIFactory.CreateLabel("Участников:", 20, y));
        numParticipants = new NumericUpDown
        { Location = new Point(200, y), Size = new Size(100, 25), Minimum = 0, Maximum = 100 };
        Controls.Add(numParticipants);
        y += 35;

        Controls.Add(UIFactory.CreateLabel("Примечания:", 20, y));
        txtNotes = new TextBox
        {
            Location = new Point(20, y + 25),
            Size = new Size(540, 80),
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            BackColor = AppTheme.Panel,
            ForeColor = AppTheme.Text,
            Font = AppTheme.FontInput,
            BorderStyle = BorderStyle.FixedSingle
        };
        Controls.Add(txtNotes);
        y += 115;

        chkAddMetadata = new CheckBox
        {
            Text = "Добавить детальные метаданные после сохранения",
            Location = new Point(20, y),
            Size = new Size(400, 25),
            ForeColor = AppTheme.Text,
            Font = AppTheme.FontInput,
            Checked = true
        };
        Controls.Add(chkAddMetadata);
        y += 40;

        var btnSave = UIFactory.CreateDialogButton("Сохранить", 300, y, AppTheme.Success);
        var btnCancel = UIFactory.CreateDialogButton("Отмена", 430, y, AppTheme.Danger);
        btnSave.Click += BtnSave_Click;
        btnCancel.Click += (_, _) => Close();
        Controls.Add(btnSave);
        Controls.Add(btnCancel);
    }

    private void LoadComboBoxData()
    {
        var objects = DatabaseHelper.GetAstronomicalObjects();
        cmbObject.DataSource = objects;
        cmbObject.DisplayMember = "name";
        cmbObject.ValueMember = "id";

        var telescopes = DatabaseHelper.GetTelescopes();
        cmbTelescope.DataSource = telescopes;
        cmbTelescope.DisplayMember = "name";
        cmbTelescope.ValueMember = "id";

        var astronomers = DatabaseHelper.GetAstronomers();
        cmbAstronomer.DataSource = astronomers;
        cmbAstronomer.DisplayMember = "full_name";
        cmbAstronomer.ValueMember = "id";
    }

    private void BtnSave_Click(object sender, EventArgs e)
    {
        if (!AuthService.CanWrite)
        {
            MessageBox.Show("Недостаточно прав для сохранения.",
                "Доступ запрещен", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (cmbObject.SelectedIndex == -1 || cmbTelescope.SelectedIndex == -1 ||
            cmbAstronomer.SelectedIndex == -1 || cmbObservationType.SelectedIndex == -1)
        {
            MessageBox.Show("Заполните все обязательные поля", "Предупреждение",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        int participants = (int)numParticipants.Value;
        int observationId = DatabaseHelper.AddObservation(
            dtpDate.Value.Date,
            dtpTime.Value.TimeOfDay,
            Convert.ToInt32(cmbTelescope.SelectedValue),
            Convert.ToInt32(cmbObject.SelectedValue),
            Convert.ToInt32(cmbAstronomer.SelectedValue),
            txtWeatherConditions.Text,
            cmbObservationType.Text,
            participants > 0 ? participants : null,
            txtNotes.Text);

        if (observationId > 0)
        {
            MessageBox.Show("Наблюдение успешно добавлено!", "Успех",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (chkAddMetadata.Checked)
            {
                Hide();
                new MetadataForm(observationId).ShowDialog();
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}