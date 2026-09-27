namespace NADIR.Maps;

// Описание одной карты и её привязки к мировым координатам.
// Начальная схема: мировая X направлена вправо, мировая Z — вверх.
internal sealed record MapConfig
{
    // Уникальный идентификатор карты в приложении.
    public required string Id { get; init; }

    // Путь к SVG относительно папки конфигурации карты.
    public required string SvgFile { get; init; }

    // Координаты на SVG, соответствующие мировой точке X=0, Z=0.
    // Это не центр окна и не позиция локального игрока.
    public required float OriginX { get; init; }
    public required float OriginY { get; init; }

    // Число единиц координат SVG на одну мировую единицу.
    // Значение должно быть конечным и положительным;
    // проверку добавит загрузчик.
    public required float WorldToMapScale { get; init; }
}