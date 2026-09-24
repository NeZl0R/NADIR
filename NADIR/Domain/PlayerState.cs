using System.Numerics;

namespace NADIR.Domain;

// Данные одного игрока на определённый момент времени.
internal sealed record PlayerState
{
    // Стабильный идентификатор игрока в пределах текущей сцены.
    public required string Id { get; init; }

    // Является ли этот игрок локальным.
    public required bool IsLocalPlayer { get; init; }

    // Мировые координаты X, Y, Z. Это не пиксели экрана.
    public required Vector3 Position { get; init; }
}