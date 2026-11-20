using System;
using HenriksBossArenas.Systems.Arenas;

var tests = new (string Name, Action Run)[]
{
    ("Allows a rectangle inside the required world margin", () =>
    {
        Assert.True(ArenaBounds.IsInsideWorld(left: 100, top: 100, width: 80, height: 60, maxTilesX: 500, maxTilesY: 400, margin: 20));
    }),
    ("Rejects a rectangle that reaches the left safety margin", () =>
    {
        Assert.False(ArenaBounds.IsInsideWorld(left: 20, top: 100, width: 80, height: 60, maxTilesX: 500, maxTilesY: 400, margin: 20));
    }),
    ("Rejects a rectangle that reaches the right safety margin", () =>
    {
        Assert.False(ArenaBounds.IsInsideWorld(left: 401, top: 100, width: 80, height: 60, maxTilesX: 500, maxTilesY: 400, margin: 20));
    }),
    ("Rejects a second arena while one boss is tracked", () =>
    {
        var session = new ArenaSessionState();

        Assert.True(session.TryBegin(bossWhoAmI: 12, bossType: 300));
        Assert.False(session.TryBegin(bossWhoAmI: 13, bossType: 300));
        Assert.True(session.IsTracking(bossWhoAmI: 12, bossType: 300));
    }),
    ("Does not treat a reused NPC slot as the tracked boss", () =>
    {
        var session = new ArenaSessionState();
        session.TryBegin(bossWhoAmI: 12, bossType: 300);

        Assert.False(session.IsTracking(bossWhoAmI: 12, bossType: 301));
    }),
    ("Keeps only the arena boundary as solid structure", () =>
    {
        Assert.True(ArenaLayout.IsBoundary(relativeX: 0, relativeY: 10, width: 180, height: 90, floorOffset: 8));
        Assert.True(ArenaLayout.IsBoundary(relativeX: 90, relativeY: 81, width: 180, height: 90, floorOffset: 8));
        Assert.False(ArenaLayout.IsBoundary(relativeX: 90, relativeY: 10, width: 180, height: 90, floorOffset: 8));
    }),
    ("Places the subworld arena spawn inside its boundaries", () =>
    {
        Assert.True(ArenaSubworldLayout.IsSpawnInsideWorld());
    }),
    ("Uses the full subworld identifier required by Subworld Library", () =>
    {
        Assert.Equal("HenriksBossArenas/DesertScourgeArena", ArenaSubworldId.Create("HenriksBossArenas", "DesertScourgeArena"));
    }),
    ("Clears arena participants when a session ends", () =>
    {
        var roster = new ArenaParticipantRoster();
        roster.Add("Host");
        roster.Add("Guest");

        roster.Clear();

        Assert.False(roster.Contains("Host"));
        Assert.False(roster.Contains("Guest"));
    })
};

var failures = 0;
foreach (var test in tests)
{
    try
    {
        test.Run();
        Console.WriteLine($"PASS: {test.Name}");
    }
    catch (Exception exception)
    {
        failures++;
        Console.Error.WriteLine($"FAIL: {test.Name}{Environment.NewLine}{exception.Message}");
    }
}

return failures == 0 ? 0 : 1;

static class Assert
{
    public static void True(bool value)
    {
        if (!value)
        {
            throw new InvalidOperationException("Expected true but was false.");
        }
    }

    public static void False(bool value)
    {
        if (value)
        {
            throw new InvalidOperationException("Expected false but was true.");
        }
    }

    public static void Equal(string expected, string actual)
    {
        if (expected != actual)
        {
            throw new InvalidOperationException($"Expected '{expected}' but was '{actual}'.");
        }
    }
}
