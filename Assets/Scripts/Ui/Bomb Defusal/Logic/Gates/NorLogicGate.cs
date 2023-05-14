using UnityEngine;

public class NorLogicGate : LogicOutput
{
    [SerializeField] private LogicOutput a, b;

    protected override bool GetOutput()
    {
        return !(GetOutput(a) || GetOutput(b));
    }
}
