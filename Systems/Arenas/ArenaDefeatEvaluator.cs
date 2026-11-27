namespace HenriksBossArenas.Systems.Arenas;

public static class ArenaDefeatEvaluator
{
    public static bool IsDefeat(bool bossIsActive, bool hasLivingParticipants) =>
        !bossIsActive && !hasLivingParticipants;

    public static bool IsVictory(bool bossIsActive, bool hasLivingParticipants) =>
        !bossIsActive && hasLivingParticipants;
}
