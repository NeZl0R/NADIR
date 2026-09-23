
using NADIR.Diagnostics;
using NADIR.Graphics;
using NADIR.Rendering;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace NADIR.Application;

internal sealed class NadirApp : IDisposable
{
    private readonly IWindow _window;
    private readonly GraphicsContext _graphics;
    private readonly FpsCounter _fpsCounter = new();
    private SceneRenderer? _renderer;
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
        _graphics.Initialize();
        _pendingFramebufferSize = null;
        _renderer = new SceneRenderer();
    }

    private void OnFramebufferResize(Vector2D<int> size)
    {
        // Apply only the latest size at the start of a render callback.
        _pendingFramebufferSize = size;
    }

    private void OnRender(double deltaTime)
    {
        if (_renderer is null)
            return;

        if (_pendingFramebufferSize is { } size)
        {
            _graphics.Resize(size);
            _pendingFramebufferSize = null;
        }

        if (!_graphics.IsReady)
            return;

        _fpsCounter.Update(deltaTime);
        _renderer.Render(_graphics.Canvas, _fpsCounter.FramesPerSecond);
        _graphics.Flush();
    }

    private void ReleaseResources()
    {
        _renderer?.Dispose();
        _renderer = null;
        _graphics.Dispose();
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