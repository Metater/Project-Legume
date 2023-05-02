using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Manager : MonoBehaviour
{
    protected GameManager manager;

    public virtual void ManagerAwake() { }
    public virtual void ManagerStart() { }
    public virtual void ManagerUpdate() { }
    public virtual void ManagerLateUpdate() { }

    public void Init(GameManager manager)
    {
        this.manager = manager;

        ManagerAwake();
    }
}
