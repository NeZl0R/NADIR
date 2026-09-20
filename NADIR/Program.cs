using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.Core.Native;

WindowOptions options = WindowOptions.Default with
{
    Size = new Vector2D<int>(1280, 720),
    Title = "NADIR"
};

using IWindow window = Window.Create(options);

GL? gl = null;

window.Load += () =>
{
    gl = window.CreateOpenGL();

    unsafe
    {
        byte* versionPtr = gl.GetString(StringName.Version);
        string? version = SilkMarshal.PtrToString((nint)versionPtr);

        Console.WriteLine($"OpenGL version: {version}");
    }
};

window.Run();

gl?.Dispose();