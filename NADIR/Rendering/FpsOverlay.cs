using SkiaSharp;

namespace NADIR.Rendering;

internal sealed class FpsOverlay : IDisposable
{
    private readonly SKPaint _paint = new()
    {
        IsAntialias = true,
        Color = SKColors.White
    };

    private readonly SKFont _font = new() { Size = 24 };

    public void Draw(SKCanvas canvas, double framesPerSecond)
    {
        canvas.DrawText(
            $"FPS: {framesPerSecond:F0}",
            20,
            40,
            SKTextAlign.Left,
            _font,
            _paint);
    }

    public void Dispose()
    {
        _font.Dispose();
        _paint.Dispose();
    }
}
