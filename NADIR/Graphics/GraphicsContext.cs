
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using SkiaSharp;

namespace NADIR.Graphics;

internal sealed class GraphicsContext : IDisposable
{
    private readonly IWindow _window;
    private GL? _gl;
    private GRGlInterface? _glInterface;
    private GRContext? _grContext;
    private GRBackendRenderTarget? _renderTarget;
    private SKSurface? _surface;
    private Vector2D<int> _surfaceSize;
    private bool _disposed;

    public GraphicsContext(IWindow window)
    {
        _window = window;
    }

    public bool IsReady => !_disposed && _surface is not null;

    public SKCanvas Canvas => IsReady
        ? _surface!.Canvas
        : throw new InvalidOperationException("Graphics are not ready.");

    public void Initialize()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_gl is not null)
            throw new InvalidOperationException("Graphics initialization has already started.");

        // Called from Load, while the window's OpenGL context is current.
        _gl = _window.CreateOpenGL();
        Console.WriteLine($"OpenGL error state: {_gl.GetError()}");

        _glInterface = GRGlInterface.Create(name =>
            _window.GLContext!.TryGetProcAddress(name, out nint address)
                ? address
                : 0);

        if (_glInterface is null || !_glInterface.Validate())
            throw new InvalidOperationException("Failed to create Skia OpenGL interface.");

        _grContext = GRContext.CreateGl(_glInterface)
            ?? throw new InvalidOperationException("Failed to create Skia GPU context.");

        Console.WriteLine("Skia GPU context initialized.");
        Resize(_window.FramebufferSize);
    }

    public void Resize(Vector2D<int> size)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_grContext is null)
            throw new InvalidOperationException("Graphics have not been initialized.");

        if (IsReady && _surfaceSize == size)
            return;

        if (_surface is not null)
            Flush();

        ReleaseSurface();

        if (size.X <= 0 || size.Y <= 0)
            return;

        try
        {
            CreateSurface(size);
            _surfaceSize = size;
        }
        catch
        {
            ReleaseSurface();
            throw;
        }
    }

    private void CreateSurface(Vector2D<int> size)
    {
        GL gl = _gl!;

        gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        // Direct GL calls can invalidate Skia's cached OpenGL state.
        _grContext!.ResetContext();
        gl.GetInteger(GetPName.SampleBuffers, out int sampleBuffers);
        gl.GetInteger(GetPName.Samples, out int sampleCount);

        if (sampleBuffers == 0)
            sampleCount = 0;

        gl.GetFramebufferAttachmentParameter(
            FramebufferTarget.Framebuffer,
            (FramebufferAttachment)GLEnum.Stencil,
            FramebufferAttachmentParameterName.StencilSize,
            out int stencilBits);

        GLEnum error = gl.GetError();

        if (error != GLEnum.NoError)
            throw new InvalidOperationException($"OpenGL framebuffer error: {error}");

        GRGlFramebufferInfo framebufferInfo = new(0, (uint)InternalFormat.Rgba8);

        _renderTarget = new GRBackendRenderTarget(
            size.X, size.Y, sampleCount, stencilBits, framebufferInfo);

        _surface = SKSurface.Create(
            _grContext!,
            _renderTarget,
            GRSurfaceOrigin.BottomLeft,
            SKColorType.Rgba8888)
            ?? throw new InvalidOperationException("Failed to create Skia surface.");
    }

    public void Flush()
    {
        Canvas.Flush();
        _grContext!.Flush();
    }

    private void ReleaseSurface()
    {
        _surface?.Dispose();
        _surface = null;
        _renderTarget?.Dispose();
        _renderTarget = null;
        _surfaceSize = default;
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        // GPU resources must be released before the window destroys its context.
        ReleaseSurface();
        _grContext?.Dispose();
        _glInterface?.Dispose();
        _gl?.Dispose();
        _disposed = true;
    }
}