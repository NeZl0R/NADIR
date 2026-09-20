using Silk.NET.Maths;
using Silk.NET.Windowing;

WindowOptions options = WindowOptions.Default with
{
    Size = new Vector2D<int>(1280, 720),
    Title = "NADIR"
};

using IWindow window = Window.Create(options);

window.Run();