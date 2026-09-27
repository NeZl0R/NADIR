using SkiaSharp;

namespace NADIR.Rendering;

internal sealed class MapRenderer
{
    // Отступ от краёв области рисования в пикселях framebuffer.
    private const float Padding = 24f;

    public void Draw(SKCanvas canvas, SvgMap map)
    {
        SKRect viewport = canvas.LocalClipBounds;
        SKRect bounds = map.Bounds;

        float availableWidth = viewport.Width - Padding * 2;
        float availableHeight = viewport.Height - Padding * 2;

        // В слишком маленьком окне для карты не осталось места.
        if (availableWidth <= 0 || availableHeight <= 0)
            return;

        // Единый масштаб сохраняет пропорции
        // и позволяет вместить карту целиком.
        float scale = MathF.Min(
            availableWidth / bounds.Width,
            availableHeight / bounds.Height);

        canvas.Save();

        try
        {
            // Центр картинки совмещаем с центром области рисования.
            canvas.Translate(viewport.MidX, viewport.MidY);
            canvas.Scale(scale);
            canvas.Translate(-bounds.MidX, -bounds.MidY);

            canvas.DrawPicture(map.Picture);
        }
        finally
        {
            // Преобразования карты не должны влиять на игроков и FPS.
            canvas.Restore();
        }
    }
}