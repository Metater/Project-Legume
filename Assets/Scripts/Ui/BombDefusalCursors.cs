using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombDefusalCursors : NetworkManager
{


    [Command(channel = Channels.Unreliable, requiresAuthority = false)]
    public void CmdUpdateCursorPosition(NetworkConnectionToClient sender, Vector2 cursorPosition)
    {
        // Vector3.SmoothDamp   
    }

/*    private class BombDefusalCursor
    {
        public readonly uint netId;
        public readonly
    }*/
}
