using System.Text.Json;

namespace NADIR.Maps;

internal static class MapConfigLoader
{
    public static MapConfig Load(string path)
    {
        // Ошибки доступа и отсутствия файла
        // передаются вызывающему коду.
        string json = File.ReadAllText(path);

        MapConfig config;

        try
        {
            config = JsonSerializer.Deserialize<MapConfig>(json)
                ?? throw new InvalidDataException(
                    $"Map configuration is null: {path}");
        }
        catch (JsonException exception)
        {
            // Сохраняем исходную причину ошибки.
            throw new InvalidDataException(
                $"Invalid map JSON: {path}",
                exception);
        }

        Validate(config);

        return config;
    }

    private static void Validate(MapConfig config)
    {
        if (string.IsNullOrWhiteSpace(config.Id))
            throw new InvalidDataException(
                "Map Id must not be empty.");

        if (string.IsNullOrWhiteSpace(config.SvgFile))
            throw new InvalidDataException(
                "SvgFile must not be empty.");

        if (Path.IsPathRooted(config.SvgFile))
            throw new InvalidDataException(
                "SvgFile must be a relative path.");

        if (!float.IsFinite(config.OriginX) ||
            !float.IsFinite(config.OriginY))
        {
            throw new InvalidDataException(
                "Map origin must contain finite numbers.");
        }

        if (!float.IsFinite(config.WorldToMapScale) ||
            config.WorldToMapScale <= 0)
        {
            throw new InvalidDataException(
                "WorldToMapScale must be finite and greater than zero.");
        }
    }
}