using System.Numerics;
using NADIR.Domain;

namespace NADIR.Data;

// Источник фиксированных данных для разработки и проверки приложения.
internal sealed class FakeSceneSource
{
    // Локальный игрок создаётся один раз вместе с источником.
    public PlayerState LocalPlayer { get; } = new()
    {
        Id = "local-player",
        IsLocalPlayer = true,
        Position = new Vector3(100f, 0f, 200f)
    };

    // Коллекция создаётся один раз. Снаружи её нельзя изменять.
    public IReadOnlyList<PlayerState> RemotePlayers { get; } =
        Array.AsReadOnly(new PlayerState[]
        {
            new()
            {
                Id = "remote-1",
                IsLocalPlayer = false,
                Position = new Vector3(120f, 0f, 210f)
            },
            new()
            {
                Id = "remote-2",
                IsLocalPlayer = false,
                Position = new Vector3(80f, 0f, 230f)
            },
            new()
            {
                Id = "remote-3",
                IsLocalPlayer = false,
                Position = new Vector3(110f, 0f, 170f)
            }
        });
}