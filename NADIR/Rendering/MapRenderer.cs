using SkiaSharp;

namespace NADIR.Rendering;

internal sealed class MapRenderer
{
    public void Draw(
        SKCanvas canvas,
        SvgMap map,
        MapCamera camera)
    {
        if (!camera.IsReady)
            return;

        canvas.Save();

        try
        {
            // Та же формула, что в MapToScreen:
            // точка * масштаб + смещение.
            canvas.Translate(camera.Offset.X, camera.Offset.Y);
            canvas.Scale(camera.Scale);

            canvas.DrawPicture(map.Picture);
        }
        finally
        {
            // Преобразования карты не влияют на игроков и FPS.
            canvas.Restore();
        }
    }
}