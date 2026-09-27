using System.Numerics;
using SkiaSharp;

namespace NADIR.Rendering;

// Хранит общий масштаб и смещение
// для перехода из карты в экран.
internal sealed class MapCamera
{
    private const float Padding = 24f;

    public float Scale { get; private set; }
    public Vector2 Offset { get; private set; }
    public bool IsReady { get; private set; }

    public void FitToViewport(SKRect mapBounds, SKRect viewport)
    {
        // Не оставляем старое преобразование,
        // если окно стало слишком маленьким.
        IsReady = false;
        Scale = 0;
        Offset = Vector2.Zero;

        float availableWidth = viewport.Width - Padding * 2;
        float availableHeight = viewport.Height - Padding * 2;

        if (availableWidth <= 0 || availableHeight <= 0)
            return;

        // Границы карты уже проверены при загрузке SvgMap.
        Scale = MathF.Min(
            availableWidth / mapBounds.Width,
            availableHeight / mapBounds.Height);

        // Совмещаем центр карты с центром области рисования.
        Offset = new Vector2(
            viewport.MidX - mapBounds.MidX * Scale,
            viewport.MidY - mapBounds.MidY * Scale);

        IsReady = true;
    }

    public Vector2 MapToScreen(Vector2 mapPosition)
    {
        if (!IsReady)
            throw new InvalidOperationException(
                "Map camera is not ready.");

        return mapPosition * Scale + Offset;
    }
}