
using NADIR.Diagnostics;
using NADIR.Graphics;
using NADIR.Rendering;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using NADIR.Data;
using NADIR.Domain;
using NADIR.Maps;
using System.Numerics;

namespace NADIR.Application;

internal sealed class NadirApp : IDisposable
{
    private readonly IWindow _window;
    private readonly GraphicsContext _graphics;
    private readonly FpsCounter _fpsCounter = new();
    private readonly FakeSceneSource _sceneSource = new();
    private SceneRenderer? _renderer;
    private SvgMap? _map;
    private Vector2D<int>? _pendingFramebufferSize;
    private bool _hasRun;
    private bool _disposed;

    public NadirApp()
    {
        WindowOptions options = WindowOptions.Default with
        {
            Size = new Vector2D<int>(1280, 720),
            Title = "NADIR",
            PreferredStencilBufferBits = 8,
            WindowState = WindowState.Maximized
        };

        _window = Window.Create(options);
        _graphics = new GraphicsContext(_window);

        _window.Load += OnLoad;
        _window.Render += OnRender;
        _window.FramebufferResize += OnFramebufferResize;
        _window.Closing += ReleaseResources;
    }

    public void Run()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_hasRun)
            throw new InvalidOperationException("The application can only run once.");

        _hasRun = true;

        try
        {
            _window.Run();
        }
        finally
        {
            // Also release partially initialized resources after an exception.
            ReleaseResources();
        }
    }

    private void OnLoad()
    {
        // Путь к конфигурации относительно каталога приложения.
        string configPath = Path.Combine(
            AppContext.BaseDirectory,
            "Assets", "Maps", "TestMap", "config.json");

        MapConfig mapConfig = MapConfigLoader.Load(configPath);
        var mapProjection = new MapProjection(mapConfig);

        Console.WriteLine(
            $"Map loaded: {mapConfig.Id}; SVG: {mapConfig.SvgFile}");

        // SvgFile считается относительно папки конфигурации.
        string mapDirectory = Path.GetDirectoryName(configPath)!;

        string svgPath = Path.GetFullPath(
            Path.Combine(mapDirectory, mapConfig.SvgFile));

        _map = SvgMap.Load(svgPath);

        Console.WriteLine(
            $"SVG loaded: {_map.Bounds.Width} x {_map.Bounds.Height}");

        _graphics.Initialize();
        _pendingFramebufferSize = null;
        _renderer = new SceneRenderer();

        // Однократная проверка мировых координат и координат карты.
        PrintPlayer(_sceneSource.LocalPlayer, mapProjection);

        Console.WriteLine(
            $"Remote players: {_sceneSource.RemotePlayers.Count}");

        foreach (PlayerState player in _sceneSource.RemotePlayers)
        {
            PrintPlayer(player, mapProjection);
        }
    }

    private void OnFramebufferResize(Vector2D<int> size)
    {
        // Apply only the latest size at the start of a render callback.
        _pendingFramebufferSize = size;
    }

    private void OnRender(double deltaTime)
    {
        if (_renderer is null || _map is null)
            return;

        if (_pendingFramebufferSize is { } size)
        {
            _graphics.Resize(size);
            _pendingFramebufferSize = null;
        }

        if (!_graphics.IsReady)
            return;

        _fpsCounter.Update(deltaTime);

        _renderer.Render(
            _graphics.Canvas,
            _fpsCounter.FramesPerSecond,
            _sceneSource.LocalPlayer,
            _sceneSource.RemotePlayers,
            _map);

        _graphics.Flush();
    }

    private void ReleaseResources()
    {
        _renderer?.Dispose();
        _renderer = null;

        _map?.Dispose();
        _map = null;

        _graphics.Dispose();
    }

    private static void PrintPlayer(
    PlayerState player,
    MapProjection projection)
    {
        Vector2 mapPosition = projection.WorldToMap(player.Position);

        Console.WriteLine(
            $"Player: {player.Id}; Local: {player.IsLocalPlayer}; " +
            $"World: X={player.Position.X}, Y={player.Position.Y}, Z={player.Position.Z}; " +
            $"Map: X={mapPosition.X}, Y={mapPosition.Y}");
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        // Call on the window thread, after Run has finished.
        ReleaseResources();
        _window.Load -= OnLoad;
        _window.Render -= OnRender;
        _window.FramebufferResize -= OnFramebufferResize;
        _window.Closing -= ReleaseResources;
        _window.Dispose();
        _disposed = true;
    }
}