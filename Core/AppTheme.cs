namespace Planetarium_Urania.Core;

// Единая палитра и шрифты приложения
public static class AppTheme
{
    // ── Фоны ──────────────────────────────────────────────────────────────
    public static readonly Color Bg = Color.FromArgb(30, 30, 30);
    public static readonly Color Panel = Color.FromArgb(45, 45, 48);
    public static readonly Color Header = Color.FromArgb(28, 28, 28);

    // ── Акценты ───────────────────────────────────────────────────────────
    public static readonly Color Accent = Color.FromArgb(0, 122, 204);
    public static readonly Color AccentHover = Color.FromArgb(0, 150, 255);
    public static readonly Color Success = Color.FromArgb(76, 175, 80);
    public static readonly Color SuccessHover = Color.FromArgb(100, 200, 104);
    public static readonly Color Warning = Color.FromArgb(255, 152, 0);
    public static readonly Color WarningHover = Color.FromArgb(255, 178, 50);
    public static readonly Color Danger = Color.FromArgb(244, 67, 54);
    public static readonly Color DangerHover = Color.FromArgb(255, 100, 88);
    public static readonly Color Purple = Color.FromArgb(103, 58, 183);
    public static readonly Color PurpleHover = Color.FromArgb(130, 85, 210);

    // ── Текст ─────────────────────────────────────────────────────────────
    public static readonly Color Text = Color.FromArgb(220, 220, 220);
    public static readonly Color TextSecondary = Color.FromArgb(150, 150, 150);
    public static readonly Color TextWarning = Color.FromArgb(255, 152, 0);

    // ── Прочее ────────────────────────────────────────────────────────────
    public static readonly Color Border = Color.FromArgb(70, 70, 70);
    public static readonly Color GridLine = Color.FromArgb(60, 60, 60);
    public static readonly Color RowAlt = Color.FromArgb(40, 40, 40);
    public static readonly Color InputBg = Color.FromArgb(40, 40, 40);
    public static readonly Color InputBgFocus = Color.FromArgb(50, 50, 55);
    public static readonly Color ReadOnlyBg = Color.FromArgb(38, 38, 38);

    // ── Приглушенные акценты (заблокированные кнопки) ─────────────────────
    public static readonly Color SuccessMuted = Color.FromArgb(50, 80, 55);
    public static readonly Color AccentMuted = Color.FromArgb(30, 50, 70);
    public static readonly Color WarningMuted = Color.FromArgb(70, 60, 35);
    public static readonly Color DangerMuted = Color.FromArgb(90, 40, 40);

    // ── Шрифты ────────────────────────────────────────────────────────────
    public static readonly Font FontTitle = new("Segoe UI", 18, FontStyle.Bold);
    public static readonly Font FontSubtitle = new("Segoe UI", 10);
    public static readonly Font FontUserInfo = new("Segoe UI", 9);
    public static readonly Font FontReadOnly = new("Segoe UI", 9, FontStyle.Bold);
    public static readonly Font FontButton = new("Segoe UI", 10, FontStyle.Bold);
    public static readonly Font FontButtonIcon = new("Segoe UI", 14);
    public static readonly Font FontInput = new("Segoe UI", 10);
    public static readonly Font FontInputLg = new("Segoe UI", 11);
    public static readonly Font FontLabel = new("Segoe UI", 9);
    public static readonly Font FontGrid = new("Segoe UI", 9);
    public static readonly Font FontGridHeader = new("Segoe UI", 10, FontStyle.Bold);
    public static readonly Font FontStatus = new("Segoe UI", 9);
    public static readonly Font FontMono = new("Consolas", 10);

    // Возвращает hover-цвет по базовому цвету кнопки
    public static Color HoverFor(Color baseColor)
    {
        if (baseColor == Accent) return AccentHover;
        if (baseColor == Success) return SuccessHover;
        if (baseColor == Warning) return WarningHover;
        if (baseColor == Danger) return DangerHover;
        if (baseColor == Purple) return PurpleHover;

        // Для незнакомых цветов осветляет на фиксированный шаг
        return Color.FromArgb(
            Math.Min(baseColor.R + 30, 255),
            Math.Min(baseColor.G + 30, 255),
            Math.Min(baseColor.B + 30, 255));
    }
}