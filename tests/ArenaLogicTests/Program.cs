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
    ("Replaces the source boss with the arena boss", () =>
    {
        var session = new ArenaSessionState();
        session.TryBegin(bossWhoAmI: 12, bossType: 300);

        Assert.True(session.TryTrackArenaBoss(bossWhoAmI: 0, bossType: 300));
        Assert.False(session.IsTracking(bossWhoAmI: 12, bossType: 300));
        Assert.True(session.IsTracking(bossWhoAmI: 0, bossType: 300));
    }),
    ("Waits for an active player before spawning the arena boss", () =>
    {
        var spawn = new ArenaBossSpawnState();
        spawn.Request();

        Assert.False(spawn.TryConsume(playerIsActive: false));
        Assert.True(spawn.TryConsume(playerIsActive: true));
        Assert.False(spawn.TryConsume(playerIsActive: true));
    }),
    ("Keeps the session alive while the arena boss is pending", () =>
    {
        var session = new ArenaSessionState();
        session.TryBegin(bossWhoAmI: 12, bossType: 300);

        session.WaitForArenaBoss();

        Assert.True(session.IsAwaitingArenaBoss);
    }),
    ("Treats an inactive boss with no living participants as defeat", () =>
    {
        Assert.True(ArenaDefeatEvaluator.IsDefeat(bossIsActive: false, hasLivingParticipants: false));
        Assert.False(ArenaDefeatEvaluator.IsDefeat(bossIsActive: true, hasLivingParticipants: false));
        Assert.False(ArenaDefeatEvaluator.IsDefeat(bossIsActive: false, hasLivingParticipants: true));
    }),
    ("Treats an inactive boss with a living participant as victory", () =>
    {
        Assert.True(ArenaDefeatEvaluator.IsVictory(bossIsActive: false, hasLivingParticipants: true));
        Assert.False(ArenaDefeatEvaluator.IsVictory(bossIsActive: true, hasLivingParticipants: true));
        Assert.False(ArenaDefeatEvaluator.IsVictory(bossIsActive: false, hasLivingParticipants: false));
    }),
    ("Clamps a player hitbox inside the arena box", () =>
    {
        Assert.Equal(ArenaSubworldLayout.SpawnTileX * 16f, ArenaPlayerBounds.ClampCenterX(ArenaSubworldLayout.SpawnTileX * 16f, width: 20));
        Assert.Equal((ArenaSubworldLayout.BoxLeftTileX + 1) * 16f + 10f, ArenaPlayerBounds.ClampCenterX(0f, width: 20));
        Assert.Equal(ArenaSubworldLayout.BoxRightTileX * 16f - 10f, ArenaPlayerBounds.ClampCenterX(ArenaSubworldLayout.Width * 16f, width: 20));
    }),
    ("Clamps item hitboxes inside every arena boundary", () =>
    {
        var inside = ArenaItemBounds.ClampCenter(ArenaSubworldLayout.SpawnTileX * 16f, ArenaSubworldLayout.SpawnTileY * 16f, width: 12, height: 14);
        Assert.Equal(ArenaSubworldLayout.SpawnTileX * 16f, inside.CenterX);
        Assert.Equal(ArenaSubworldLayout.SpawnTileY * 16f, inside.CenterY);
        Assert.False(inside.WasClampedX);
        Assert.False(inside.WasClampedY);

        var left = ArenaItemBounds.ClampCenter(0f, ArenaSubworldLayout.SpawnTileY * 16f, width: 12, height: 14);
        Assert.Equal((ArenaSubworldLayout.BoxLeftTileX + 1) * 16f + 6f, left.CenterX);
        Assert.True(left.WasClampedX);
        Assert.False(left.WasClampedY);

        var right = ArenaItemBounds.ClampCenter(ArenaSubworldLayout.Width * 16f, ArenaSubworldLayout.SpawnTileY * 16f, width: 12, height: 14);
        Assert.Equal(ArenaSubworldLayout.BoxRightTileX * 16f - 6f, right.CenterX);
        Assert.True(right.WasClampedX);

        var ceiling = ArenaItemBounds.ClampCenter(ArenaSubworldLayout.SpawnTileX * 16f, 0f, width: 12, height: 30);
        Assert.Equal((ArenaSubworldLayout.BoxCeilingTileY + 1) * 16f + 15f, ceiling.CenterY);
        Assert.False(ceiling.WasClampedX);
        Assert.True(ceiling.WasClampedY);

        var floor = ArenaItemBounds.ClampCenter(ArenaSubworldLayout.SpawnTileX * 16f, ArenaSubworldLayout.Height * 16f, width: 12, height: 30);
        Assert.Equal(ArenaSubworldLayout.FloorTileY * 16f - 15f, floor.CenterY);
        Assert.True(floor.WasClampedY);
    }),
    ("Keeps the arena background within one seamless panorama", () =>
    {
        Assert.Equal(-501f, ArenaBackgroundTiling.GetSingleBackgroundX(relativeCameraX: 0f, screenWidth: 1366, backgroundWidth: 2368, parallax: 0.1f));
        Assert.Equal(-725f, ArenaBackgroundTiling.GetSingleBackgroundX(relativeCameraX: 2240f, screenWidth: 1366, backgroundWidth: 2368, parallax: 0.1f));
        Assert.Equal(-1002f, ArenaBackgroundTiling.GetSingleBackgroundX(relativeCameraX: 10000f, screenWidth: 1366, backgroundWidth: 2368, parallax: 0.1f));
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
    ("Places the spawn four tiles above the arena floor", () =>
    {
        Assert.Equal(ArenaSubworldLayout.Height - 124, ArenaSubworldLayout.SpawnTileY);
    }),
    ("Keeps the arena spawn outside Terraria beach zones", () =>
    {
        Assert.True(ArenaSubworldLayout.SpawnTileX >= 380);
        Assert.True(ArenaSubworldLayout.SpawnTileX <= ArenaSubworldLayout.Width - 380);
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

    public static void Equal(int expected, int actual)
    {
        if (expected != actual)
        {
            throw new InvalidOperationException($"Expected {expected} but was {actual}.");
        }
    }

    public static void Equal(float expected, float actual)
    {
        if (expected != actual)
        {
            throw new InvalidOperationException($"Expected {expected} but was {actual}.");
        }
    }
}
