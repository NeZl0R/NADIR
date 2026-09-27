using System.Numerics;
using NADIR.Domain;
using SkiaSharp;

namespace NADIR.Rendering;

internal sealed class PlayerRenderer : IDisposable
{
    // Временный масштаб для просмотра игроков без карты.
    private const float PixelsPerWorldUnit = 4f;

    // Радиус каждого маркера в пикселях.
    private const float MarkerRadius = 6f;

    private readonly SKPaint _localPaint = new()
    {
        IsAntialias = true,
        Color = SKColors.LimeGreen,
        Style = SKPaintStyle.Fill
    };

    private readonly SKPaint _remotePaint = new()
    {
        IsAntialias = true,
        Color = SKColors.Orange,
        Style = SKPaintStyle.Fill
    };

    public void Draw(
        SKCanvas canvas,
        PlayerState localPlayer,
        IReadOnlyList<PlayerState> remotePlayers)
    {
        // Сейчас холст без преобразований и дополнительных ограничений.
        SKRect bounds = canvas.LocalClipBounds;
        SKPoint center = new(bounds.MidX, bounds.MidY);

        foreach (PlayerState player in remotePlayers)
        {
            DrawPlayer(canvas, player, localPlayer.Position, center);
        }

        // Локальный маркер рисуем последним,
        // чтобы он не терялся при наложении.
        DrawPlayer(canvas, localPlayer, localPlayer.Position, center);
    }

    private void DrawPlayer(
        SKCanvas canvas,
        PlayerState player,
        Vector3 origin,
        SKPoint center)
    {
        SKPoint screenPosition =
            ToScreenPosition(player.Position, origin, center);

        SKPaint paint = player.IsLocalPlayer
            ? _localPaint
            : _remotePaint;

        canvas.DrawCircle(
            screenPosition.X,
            screenPosition.Y,
            MarkerRadius,
            paint);
    }

    private static SKPoint ToScreenPosition(
        Vector3 position,
        Vector3 origin,
        SKPoint center)
    {
        // Временный вид сверху: X вправо, Z вверх.
        // Высоту Y пока игнорируем.
        Vector3 offset = position - origin;

        return new SKPoint(
            center.X + offset.X * PixelsPerWorldUnit,
            center.Y - offset.Z * PixelsPerWorldUnit);
    }

    public void Dispose()
    {
        _localPaint.Dispose();
        _remotePaint.Dispose();
    }
}