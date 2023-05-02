using System;

public class PlayerManager : Manager
{
    public Player LocalPlayer { get; private set; }
    public bool IsLocalPlayerNull => LocalPlayer == null;
    public NetRefs<Player> Players { get; private set; } = new();

    public event Action<Player> OnStartLocalPlayer;
    public event Action<Player> OnStopLocalPlayer;

    public event Action<Player> OnStartPlayer;
    public event Action<Player> OnStopPlayer;

    public void SetLocalPlayer(Player player)
    {
        if (player == null)
        {
            OnStopLocalPlayer?.Invoke(LocalPlayer);
            LocalPlayer = null;
        }
        else
        {
            OnStartLocalPlayer?.Invoke(player);
            LocalPlayer = player;
        }
    }

    public void StartPlayer(Player player)
    {
        OnStartPlayer?.Invoke(player);
        Players.Add(player);
    }
    public void StopPlayer(Player player)
    {
        OnStopPlayer?.Invoke(player);
        Players.Remove(player);
    }
}
