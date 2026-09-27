using SkiaSharp;
using Svg.Skia;

namespace NADIR.Rendering;

// Владеет загруженным SVG и связанными ресурсами Skia.
internal sealed class SvgMap : IDisposable
{
    private readonly SKSvg _svg;
    private bool _disposed;

    private SvgMap(SKSvg svg)
    {
        _svg = svg;
    }

    // Picture заимствуется для рисования.
    // Отдельно освобождать её нельзя.
    public SKPicture Picture
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return _svg.Picture!;
        }
    }

    public SKRect Bounds => Picture.CullRect;

    public static SvgMap Load(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                "SVG map file was not found.",
                path);
        }

        var svg = new SKSvg();

        try
        {
            SKPicture picture = svg.Load(path)
                ?? throw new InvalidDataException(
                    $"Failed to load SVG map: {path}");

            SKRect bounds = picture.CullRect;

            if (!float.IsFinite(bounds.Left) ||
                !float.IsFinite(bounds.Top) ||
                !float.IsFinite(bounds.Right) ||
                !float.IsFinite(bounds.Bottom) ||
                !float.IsFinite(bounds.Width) ||
                !float.IsFinite(bounds.Height) ||
                bounds.Width <= 0 ||
                bounds.Height <= 0)
            {
                throw new InvalidDataException(
                    $"SVG map has invalid bounds: {path}");
            }

            // Владение svg передаётся новому объекту SvgMap.
            return new SvgMap(svg);
        }
        catch
        {
            // Если загрузка не завершилась,
            // освобождаем уже созданные ресурсы.
            svg.Dispose();
            throw;
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _svg.Dispose();
        _disposed = true;
    }
}