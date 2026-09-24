using System.Numerics;
using NADIR.Domain;

namespace NADIR.Data;

// Источник фиксированных данных для разработки и проверки приложения.
internal sealed class FakeSceneSource
{
    // Объект создаётся один раз при создании источника.
    public PlayerState LocalPlayer { get; } = new()
    {
        Id = "local-player",
        IsLocalPlayer = true,
        Position = new Vector3(100f, 0f, 200f)
    };
}