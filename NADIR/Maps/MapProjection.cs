using System.Numerics;

namespace NADIR.Maps;

// Преобразует мировые координаты в координаты карты,
// без зависимости от графики.
internal sealed class MapProjection
{
    private readonly MapConfig _config;

    public MapProjection(MapConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);

        // Ожидаем конфигурацию, проверенную MapConfigLoader.
        _config = config;
    }

    public Vector2 WorldToMap(Vector3 worldPosition)
    {
        // Вид сверху на плоскость XZ.
        // Мировую высоту Y не используем.
        float mapX =
            _config.OriginX +
            worldPosition.X * _config.WorldToMapScale;

        float mapY =
            _config.OriginY -
            worldPosition.Z * _config.WorldToMapScale;

        return new Vector2(mapX, mapY);
    }
}