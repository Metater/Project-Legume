using Mirror;

public abstract class PlayerComponent : NetworkBehaviour
{
    protected GameManager manager;
    protected Player player;

    public virtual void PlayerAwake() { }
    public virtual void PlayerStart() { }
    public virtual void PlayerUpdate() { }
    public virtual void PlayerLateUpdate() { }

    public void Init(GameManager manager, Player player)
    {
        this.manager = manager;
        this.player = player;

        PlayerAwake();
    }
}
