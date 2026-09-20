using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using SkiaSharp;

WindowOptions options = WindowOptions.Default with
{
    Size = new Vector2D<int>(1280, 720),
    Title = "NADIR",
    PreferredStencilBufferBits = 8
};

using IWindow window = Window.Create(options);

GL? gl = null;
GRGlInterface? glInterface = null;
GRContext? grContext = null;
GRBackendRenderTarget? renderTarget = null;
SKSurface? surface = null;
SKPaint? fpsPaint = null;
SKFont? fpsFont = null;
double fpsTimer = 0;
int fpsFrameCount = 0;
double displayedFps = 0;

window.Load += () =>
{
    gl = window.CreateOpenGL();

    GLEnum error = gl.GetError();

    Console.WriteLine($"OpenGL error state: {error}");

    glInterface = GRGlInterface.Create(name =>
        window.GLContext!.TryGetProcAddress(name, out nint address)
            ? address
            : 0);

    if (glInterface is null || !glInterface.Validate())
    {
        throw new InvalidOperationException("Failed to create Skia OpenGL interface.");
    }

    grContext = GRContext.CreateGl(glInterface);

    if (grContext is null)
    {
        throw new InvalidOperationException("Failed to create Skia GPU context.");
    }

    Console.WriteLine("Skia GPU context initialized.");

    gl.GetInteger(GetPName.SampleBuffers, out int sampleBuffers);
    gl.GetInteger(GetPName.Samples, out int sampleCount);

    if (sampleBuffers == 0)
    {
        sampleCount = 0;
    }

    gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

    gl.GetFramebufferAttachmentParameter(
     FramebufferTarget.Framebuffer,
     (FramebufferAttachment)GLEnum.Stencil,
     FramebufferAttachmentParameterName.StencilSize,
     out int stencilBits);

    GLEnum framebufferError = gl.GetError();

    if (framebufferError != GLEnum.NoError)
    {
        throw new InvalidOperationException(
            $"OpenGL framebuffer error: {framebufferError}");
    }

    GRGlFramebufferInfo framebufferInfo = new(
     0,
     (uint)InternalFormat.Rgba8);

    Vector2D<int> framebufferSize = window.FramebufferSize;

    renderTarget = new GRBackendRenderTarget(
        framebufferSize.X,
        framebufferSize.Y,
        sampleCount,
        stencilBits,
        framebufferInfo);

    surface = SKSurface.Create(
        grContext,
        renderTarget,
        GRSurfaceOrigin.BottomLeft,
        SKColorType.Rgba8888);

    if (surface is null)
    {
        throw new InvalidOperationException(
            "Failed to create Skia surface.");
    }

    fpsPaint = new SKPaint
    {
        IsAntialias = true,
        Color = SKColors.White
    };

    fpsFont = new SKFont
    {
        Size = 24
    };

    Console.WriteLine("Skia surface initialized.");

};

window.Render += deltaTime =>
{
    if (surface is null ||
        grContext is null ||
        fpsPaint is null ||
        fpsFont is null)
    {
        return;
    }

    fpsTimer += deltaTime;
    fpsFrameCount++;

    if (fpsTimer >= 0.5)
    {
        displayedFps = fpsFrameCount / fpsTimer;

        fpsTimer = 0;
        fpsFrameCount = 0;
    }

    SKCanvas canvas = surface.Canvas;

    canvas.Clear(SKColors.Black);

    canvas.DrawText(
        $"FPS: {displayedFps:F0}",
        20,
        40,
        SKTextAlign.Left,
        fpsFont,
        fpsPaint);

    canvas.Flush();
    grContext.Flush();
};

window.Run();


fpsFont?.Dispose();
fpsPaint?.Dispose();
surface?.Dispose();
renderTarget?.Dispose();
grContext?.Dispose();
glInterface?.Dispose();
gl?.Dispose();