using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableGameObject : MonoBehaviour
{
    [SerializeField] private Interactable parent;
    public Interactable Interactable => parent;
}
