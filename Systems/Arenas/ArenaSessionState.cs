namespace HenriksBossArenas.Systems.Arenas;

public sealed class ArenaSessionState
{
    public bool IsActive { get; private set; }

    public int BossWhoAmI { get; private set; } = -1;

    public int BossType { get; private set; } = -1;

    public bool IsAwaitingArenaBoss { get; private set; }

    public bool TryBegin(int bossWhoAmI, int bossType)
    {
        if (IsActive)
        {
            return false;
        }

        IsActive = true;
        BossWhoAmI = bossWhoAmI;
        BossType = bossType;
        IsAwaitingArenaBoss = false;
        return true;
    }

    public void WaitForArenaBoss() => IsAwaitingArenaBoss = true;

    public bool IsTracking(int bossWhoAmI, int bossType) =>
        IsActive && BossWhoAmI == bossWhoAmI && BossType == bossType;

    public bool TryTrackArenaBoss(int bossWhoAmI, int bossType)
    {
        if (!IsActive || BossType != bossType)
        {
            return false;
        }

        BossWhoAmI = bossWhoAmI;
        IsAwaitingArenaBoss = false;
        return true;
    }

    public void Clear()
    {
        IsActive = false;
        BossWhoAmI = -1;
        BossType = -1;
        IsAwaitingArenaBoss = false;
    }
}
