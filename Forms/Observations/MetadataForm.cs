using Planetarium_Urania.Core;
using Planetarium_Urania.UI;

namespace Planetarium_Urania.Forms.Observations;

public class MetadataForm : Form
{
    private readonly int observationId;
    private ObservationMetadata? metadata;
    private bool isEditMode;

    private TextBox txtTemperature = null!;
    private TextBox txtHumidity = null!;
    private TextBox txtPressure = null!;
    private ComboBox cmbSeeingQuality = null!;
    private ComboBox cmbTransparency = null!;
    private TextBox txtCameraModel = null!;
    private TextBox txtExposureTime = null!;
    private TextBox txtIso = null!;
    private TextBox txtFilter = null!;
    private TextBox txtFramesCount = null!;
    private TextBox txtDetailedNotes = null!;
    private Button btnSave = null!;
    private Button btnCancel = null!;

    public MetadataForm(int observationId)
    {
        this.observationId = observationId;

        Text = "Метаданные наблюдения";
        Size = new Size(700, 700);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        BackColor = AppTheme.Bg;

        BuildUI();
        LoadExistingMetadata();

        if (!AuthService.CanWrite)
            SetReadOnlyMode();
    }

    private void BuildUI()
    {
        var scroll = new Panel
        { Location = new Point(0, 0), Size = new Size(684, 610), AutoScroll = true, BackColor = AppTheme.Bg };

        int y = 20;

        scroll.Controls.Add(UIFactory.CreateLabel("МЕТАДАННЫЕ НАБЛЮДЕНИЯ", 20, y, 14, FontStyle.Bold));
        y += 40;

        // ── Погода ────────────────────────────────────────────────────────
        scroll.Controls.Add(UIFactory.CreateLabel("Детальные погодные условия", 20, y, 12, FontStyle.Bold));
        y += 30;

        scroll.Controls.Add(UIFactory.CreateLabel("Температура (°C):", 20, y));
        txtTemperature = UIFactory.CreateTextBox(200, y, 150); scroll.Controls.Add(txtTemperature); y += 35;

        scroll.Controls.Add(UIFactory.CreateLabel("Влажность (%):", 20, y));
        txtHumidity = UIFactory.CreateTextBox(200, y, 150); scroll.Controls.Add(txtHumidity); y += 35;

        scroll.Controls.Add(UIFactory.CreateLabel("Давление (мм рт.ст.):", 20, y));
        txtPressure = UIFactory.CreateTextBox(200, y, 150); scroll.Controls.Add(txtPressure); y += 35;

        scroll.Controls.Add(UIFactory.CreateLabel("Качество видимости:", 20, y));
        cmbSeeingQuality = UIFactory.CreateComboBox(200, y, 200);
        cmbSeeingQuality.Items.AddRange(new object[] { "Отличное", "Хорошее", "Среднее", "Плохое" });
        scroll.Controls.Add(cmbSeeingQuality); y += 35;

        scroll.Controls.Add(UIFactory.CreateLabel("Прозрачность атмосферы:", 20, y));
        cmbTransparency = UIFactory.CreateComboBox(200, y, 200);
        cmbTransparency.Items.AddRange(new object[] { "Отличная", "Хорошая", "Средняя", "Плохая" });
        scroll.Controls.Add(cmbTransparency); y += 45;

        // ── Фотосъемка ────────────────────────────────────────────────────
        scroll.Controls.Add(UIFactory.CreateLabel("Параметры фотосъемки", 20, y, 12, FontStyle.Bold));
        y += 30;

        scroll.Controls.Add(UIFactory.CreateLabel("Модель камеры:", 20, y));
        txtCameraModel = UIFactory.CreateTextBox(200, y, 400); scroll.Controls.Add(txtCameraModel); y += 35;

        scroll.Controls.Add(UIFactory.CreateLabel("Выдержка (сек):", 20, y));
        txtExposureTime = UIFactory.CreateTextBox(200, y, 150); scroll.Controls.Add(txtExposureTime); y += 35;

        scroll.Controls.Add(UIFactory.CreateLabel("ISO:", 20, y));
        txtIso = UIFactory.CreateTextBox(200, y, 150); scroll.Controls.Add(txtIso); y += 35;

        scroll.Controls.Add(UIFactory.CreateLabel("Фильтр:", 20, y));
        txtFilter = UIFactory.CreateTextBox(200, y, 200); scroll.Controls.Add(txtFilter); y += 35;

        scroll.Controls.Add(UIFactory.CreateLabel("Количество кадров:", 20, y));
        txtFramesCount = UIFactory.CreateTextBox(200, y, 150); scroll.Controls.Add(txtFramesCount); y += 45;

        // ── Заметки ───────────────────────────────────────────────────────
        scroll.Controls.Add(UIFactory.CreateLabel("Детальные заметки", 20, y, 12, FontStyle.Bold));
        y += 30;

        txtDetailedNotes = new TextBox
        {
            Location = new Point(20, y),
            Size = new Size(620, 120),
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            BackColor = AppTheme.Panel,
            ForeColor = AppTheme.Text,
            Font = AppTheme.FontInput,
            BorderStyle = BorderStyle.FixedSingle
        };
        scroll.Controls.Add(txtDetailedNotes);
        Controls.Add(scroll);

        btnSave = UIFactory.CreateDialogButton("Сохранить", 380, 620, AppTheme.Success, 130);
        btnCancel = UIFactory.CreateDialogButton("Отмена", 530, 620, AppTheme.Danger, 130);
        btnSave.Click += BtnSave_Click;
        btnCancel.Click += (_, _) => Close();
        Controls.Add(btnSave);
        Controls.Add(btnCancel);
    }

    private void SetReadOnlyMode()
    {
        Text = "Просмотр метаданных наблюдения (только чтение)";

        foreach (var ctrl in AllControls(this))
        {
            if (ctrl is TextBox tb) { tb.ReadOnly = true; tb.BackColor = AppTheme.ReadOnlyBg; }
            if (ctrl is ComboBox cb) cb.Enabled = false;
        }

        btnSave.Visible = false;
        btnCancel.Text = "Закрыть";
        btnCancel.Location = new Point(530, 620);

        Controls.Add(new Label
        {
            Text = "🔒 Режим просмотра",
            Font = AppTheme.FontReadOnly,
            ForeColor = AppTheme.TextWarning,
            Location = new Point(20, 628),
            AutoSize = true
        });
    }

    private static IEnumerable<Control> AllControls(Control parent)
    {
        foreach (Control c in parent.Controls)
        {
            yield return c;
            foreach (var child in AllControls(c)) yield return child;
        }
    }

    private void LoadExistingMetadata()
    {
        metadata = MongoDbHelper.GetMetadataByObservationId(observationId);
        if (metadata is null) return;

        isEditMode = true;
        Text = "Редактирование метаданных наблюдения";

        if (metadata.Weather is not null)
        {
            txtTemperature.Text = metadata.Weather.Temperature?.ToString() ?? "";
            txtHumidity.Text = metadata.Weather.Humidity?.ToString() ?? "";
            txtPressure.Text = metadata.Weather.Pressure?.ToString() ?? "";
            cmbSeeingQuality.Text = metadata.Weather.SeeingQuality ?? "";
            cmbTransparency.Text = metadata.Weather.Transparency ?? "";
        }

        if (metadata.Photography is not null)
        {
            txtCameraModel.Text = metadata.Photography.CameraModel ?? "";
            txtExposureTime.Text = metadata.Photography.ExposureTime?.ToString() ?? "";
            txtIso.Text = metadata.Photography.Iso?.ToString() ?? "";
            txtFilter.Text = metadata.Photography.Filter ?? "";
            txtFramesCount.Text = metadata.Photography.FramesCount?.ToString() ?? "";
        }

        txtDetailedNotes.Text = metadata.DetailedNotes ?? "";
    }

    private void BtnSave_Click(object sender, EventArgs e)
    {
        if (!AuthService.CanWrite)
        {
            MessageBox.Show("Недостаточно прав для сохранения метаданных.",
                "Доступ запрещен", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        metadata ??= new ObservationMetadata { ObservationId = observationId };

        metadata.Weather = new WeatherDetails
        {
            Temperature = Parse<double?>(txtTemperature.Text),
            Humidity = Parse<int?>(txtHumidity.Text),
            Pressure = Parse<double?>(txtPressure.Text),
            SeeingQuality = cmbSeeingQuality.Text,
            Transparency = cmbTransparency.Text
        };

        metadata.Photography = new PhotographySettings
        {
            CameraModel = txtCameraModel.Text,
            ExposureTime = Parse<double?>(txtExposureTime.Text),
            Iso = Parse<int?>(txtIso.Text),
            Filter = txtFilter.Text,
            FramesCount = Parse<int?>(txtFramesCount.Text)
        };

        metadata.DetailedNotes = txtDetailedNotes.Text;

        bool success = isEditMode
            ? MongoDbHelper.UpdateMetadata(metadata)
            : MongoDbHelper.AddMetadata(metadata);

        if (success)
        {
            MessageBox.Show(
                isEditMode ? "Метаданные успешно обновлены!" : "Метаданные успешно добавлены!",
                "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }
    }

    private static T? Parse<T>(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return default;
        try
        {
            var underlying = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
            return (T)Convert.ChangeType(
                text.Replace(',', '.'), underlying,
                System.Globalization.CultureInfo.InvariantCulture);
        }
        catch { return default; }
    }
}