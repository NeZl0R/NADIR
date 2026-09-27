using NADIR.Domain;
using SkiaSharp;

namespace NADIR.Rendering;

internal sealed class SceneRenderer : IDisposable
{
    private readonly FpsOverlay _fpsOverlay = new();
    private readonly PlayerRenderer _playerRenderer = new();

    public void Render(
        SKCanvas canvas,
        double framesPerSecond,
        PlayerState localPlayer,
        IReadOnlyList<PlayerState> remotePlayers)
    {
        // Сначала фон.
        canvas.Clear(SKColors.Black);

        // Затем игроки.
        _playerRenderer.Draw(canvas, localPlayer, remotePlayers);

        // Поверх сцены — FPS.
        _fpsOverlay.Draw(canvas, framesPerSecond);
    }

    public void Dispose()
    {
        _playerRenderer.Dispose();
        _fpsOverlay.Dispose();
    }
}