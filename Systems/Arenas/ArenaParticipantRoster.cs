using System.Collections.Generic;

namespace HenriksBossArenas.Systems.Arenas;

public sealed class ArenaParticipantRoster
{
    private readonly HashSet<string> names = new();

    public void Add(string playerName) => names.Add(playerName);

    public bool Contains(string playerName) => names.Contains(playerName);

    public void Clear() => names.Clear();
}
