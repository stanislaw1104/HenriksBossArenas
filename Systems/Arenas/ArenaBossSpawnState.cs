namespace HenriksBossArenas.Systems.Arenas;

public sealed class ArenaBossSpawnState
{
    private bool requested;

    public void Request() => requested = true;

    public bool TryConsume(bool playerIsActive)
    {
        if (!requested || !playerIsActive)
        {
            return false;
        }

        requested = false;
        return true;
    }
}
