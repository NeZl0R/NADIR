using NADIR.Domain;
using SkiaSharp;

namespace NADIR.Rendering;

internal sealed class SceneRenderer : IDisposable
{
    private readonly FpsOverlay _fpsOverlay = new();
    private readonly PlayerRenderer _playerRenderer = new();
    private readonly MapRenderer _mapRenderer = new();
    private readonly MapCamera _mapCamera = new();

    public void Render(
        SKCanvas canvas,
        double framesPerSecond,
        PlayerState localPlayer,
        IReadOnlyList<PlayerState> remotePlayers,
        SvgMap map)
    {
        canvas.Clear(SKColors.Black);

        // Обновляем камеру по текущему размеру холста,
        // включая изменения после resize.
        _mapCamera.FitToViewport(
            map.Bounds,
            canvas.LocalClipBounds);

        _mapRenderer.Draw(canvas, map, _mapCamera);

        // Пока сохраняем прежнее тестовое расположение игроков.
        _playerRenderer.Draw(canvas, localPlayer, remotePlayers);

        _fpsOverlay.Draw(canvas, framesPerSecond);
    }

    public void Dispose()
    {
        _playerRenderer.Dispose();
        _fpsOverlay.Dispose();
    }
}