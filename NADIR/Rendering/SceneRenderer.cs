using NADIR.Domain;
using SkiaSharp;

namespace NADIR.Rendering;

internal sealed class SceneRenderer : IDisposable
{
    private readonly FpsOverlay _fpsOverlay = new();
    private readonly PlayerRenderer _playerRenderer = new();
    private readonly MapRenderer _mapRenderer = new();

    public void Render(
        SKCanvas canvas,
        double framesPerSecond,
        PlayerState localPlayer,
        IReadOnlyList<PlayerState> remotePlayers,
        SvgMap map)
    {
        // Фон всего окна.
        canvas.Clear(SKColors.Black);

        // Карта служит фоном для маркеров.
        _mapRenderer.Draw(canvas, map);

        // Пока используем тестовое расположение игроков
        // без привязки к карте.
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