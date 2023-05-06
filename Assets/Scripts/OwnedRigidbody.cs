using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwnedRigidbody : MonoBehaviour
{
    [SerializeField] private List<Collider> colliders;

    [SerializeField] private float mass;
    [SerializeField] private float drag;
    [SerializeField] private float angularDrag;
    [SerializeField] private bool useGravity;
    [SerializeField] private RigidbodyInterpolation interpolate;
    [SerializeField] private CollisionDetectionMode collisionDetection;
    public Rigidbody Rigidbody { get; private set; }

    private void Awake()
    {
        Rigidbody = ConstructRigidbody();
    }

    private Rigidbody ConstructRigidbody()
    {
        Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();

        rigidbody.mass = mass;
        rigidbody.drag = drag;
        rigidbody.angularDrag = angularDrag;
        rigidbody.useGravity = useGravity;
        rigidbody.isKinematic = false;
        rigidbody.interpolation = interpolate;
        rigidbody.collisionDetectionMode = collisionDetection;

        return rigidbody;
    }

    public void EnableColliders() => colliders.ForEach(c => c.enabled = true);
    public void DisableColliders() => colliders.ForEach(c => c.enabled = false);

    public void Enable() => Rigidbody.isKinematic = false;
    public void Disable() => Rigidbody.isKinematic = true;
}
