using UnityEngine;

public abstract class LogicOutput : MonoBehaviour
{
    [SerializeField] private WireVisual outputWireVisual;

    protected abstract bool GetOutput();

    public static bool GetOutput(LogicOutput output)
    {
        if (output == null)
        {
            return false;
        }

        bool isPowered = output.GetOutput();
        if (output.outputWireVisual != null)
        {
            output.outputWireVisual.SetIsPowered(isPowered);
        }
        return isPowered;
    }
}
