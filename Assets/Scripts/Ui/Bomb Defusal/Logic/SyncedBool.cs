using Mirror;
using System;

public class SyncedBool : NetworkBehaviour
{
    [SyncVar] private bool value = false;
    public bool Value => value;

    [Command(requiresAuthority = false)]
    public void CmdSetValue(bool value)
    {
        this.value = value;
    }
}
