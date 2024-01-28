using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public bool HasInteraction { get; set; }

    [field: SerializeField]
    public int Id { get; private set; }

    public bool CanInteract { get; set; } = true;
}
