using UnityEngine;

public class AndLogicGate : LogicOutput
{
    [SerializeField] private LogicOutput a, b;

    protected override bool GetOutput()
    {
        return GetOutput(a) && GetOutput(b);
    }
}
