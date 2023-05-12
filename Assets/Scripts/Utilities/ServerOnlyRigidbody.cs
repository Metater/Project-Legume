using Mirror;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ServerOnlyRigidbody : NetworkBehaviour
{
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnStartClient()
    {
        if (!isServer)
        {
            rb.isKinematic = true;
        }
    }
}
