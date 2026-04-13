using Planetarium_Urania.Core;
using Planetarium_Urania.UI;

namespace Planetarium_Urania.Forms.Observations;

public class ViewMetadataForm : Form
{
    private readonly ObservationMetadata metadata;

    public ViewMetadataForm(ObservationMetadata metadata)
    {
        this.metadata = metadata;

        Text = "Просмотр метаданных наблюдения";
        Size = new Size(700, 600);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        BackColor = AppTheme.Bg;

        BuildUI();
    }

    private void BuildUI()
    {
        var rtb = new RichTextBox
        {
            Location = new Point(20, 20),
            Size = new Size(640, 500),
            ReadOnly = true,
            BackColor = AppTheme.Panel,
            ForeColor = AppTheme.Text,
            Font = AppTheme.FontMono,
            BorderStyle = BorderStyle.FixedSingle,
            Text = BuildText()
        };

        var btnClose = UIFactory.CreateDialogButton("Закрыть", 540, 530, AppTheme.Accent);
        btnClose.Click += (_, _) => Close();

        Controls.Add(rtb);
        Controls.Add(btnClose);
    }

    private string BuildText()
    {
        var sb = new System.Text.StringBuilder();

        sb.AppendLine("═══════════════════════════════════════════════════");
        sb.AppendLine("           МЕТАДАННЫЕ НАБЛЮДЕНИЯ");
        sb.AppendLine("═══════════════════════════════════════════════════");
        sb.AppendLine();
        sb.AppendLine($"ID наблюдения: {metadata.ObservationId}");
        sb.AppendLine($"Создано:       {metadata.CreatedAt:dd.MM.yyyy HH:mm}");
        sb.AppendLine($"Обновлено:     {metadata.UpdatedAt:dd.MM.yyyy HH:mm}");

        sb.AppendLine();
        sb.AppendLine("───────────────────────────────────────────────────");
        sb.AppendLine("  ПОГОДНЫЕ УСЛОВИЯ");
        sb.AppendLine("───────────────────────────────────────────────────");

        if (metadata.Weather is { } w)
        {
            sb.AppendLine($"Температура:        {w.Temperature}°C");
            sb.AppendLine($"Влажность:          {w.Humidity}%");
            sb.AppendLine($"Давление:           {w.Pressure} мм рт.ст.");
            sb.AppendLine($"Качество видимости: {w.SeeingQuality}");
            sb.AppendLine($"Прозрачность:       {w.Transparency}");
        }
        else
        {
            sb.AppendLine("Нет данных");
        }

        sb.AppendLine();
        sb.AppendLine("───────────────────────────────────────────────────");
        sb.AppendLine("  ПАРАМЕТРЫ СЪЕМКИ");
        sb.AppendLine("───────────────────────────────────────────────────");

        if (metadata.Photography is { } p)
        {
            sb.AppendLine($"Камера:   {p.CameraModel}");
            sb.AppendLine($"Выдержка: {p.ExposureTime} сек");
            sb.AppendLine($"ISO:      {p.Iso}");
            sb.AppendLine($"Фильтр:   {p.Filter}");
            sb.AppendLine($"Кадров:   {p.FramesCount}");
        }
        else
        {
            sb.AppendLine("Нет данных");
        }

        sb.AppendLine();
        sb.AppendLine("───────────────────────────────────────────────────");
        sb.AppendLine("  ДЕТАЛЬНЫЕ ЗАМЕТКИ");
        sb.AppendLine("───────────────────────────────────────────────────");
        sb.AppendLine(metadata.DetailedNotes ?? "Нет заметок");

        if (metadata.Photos is { Count: > 0 } photos)
        {
            sb.AppendLine();
            sb.AppendLine("───────────────────────────────────────────────────");
            sb.AppendLine("  ФОТОГРАФИИ");
            sb.AppendLine("───────────────────────────────────────────────────");
            foreach (var photo in photos)
            {
                sb.AppendLine($"• {photo.Filename} ({photo.Resolution})");
                sb.AppendLine($"  {photo.Description}");
            }
        }

        return sb.ToString();
    }
}