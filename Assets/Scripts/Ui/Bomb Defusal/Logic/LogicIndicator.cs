using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogicIndicator : MonoBehaviour
{
    [SerializeField] private LogicOutput output;
    [SerializeField] private Image image;

    private void Update()
    {
        image.color = LogicOutput.GetOutput(output) ? LogicConstants.PoweredColor : LogicConstants.UnpoweredColor;
    }
}
