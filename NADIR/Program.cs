using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using SkiaSharp;

WindowOptions options = WindowOptions.Default with
{
    Size = new Vector2D<int>(1280, 720),
    Title = "NADIR"
};

using IWindow window = Window.Create(options);

GL? gl = null;
GRGlInterface? glInterface = null;
GRContext? grContext = null;

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
};

window.Run();

grContext?.Dispose();
glInterface?.Dispose();
gl?.Dispose();