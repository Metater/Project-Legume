using UnityEngine;

public class NotLogicGate : LogicOutput
{
    [SerializeField] private LogicOutput a;

    protected override bool GetOutput()
    {
        return !GetOutput(a);
    }
}
