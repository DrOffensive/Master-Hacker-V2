using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public bool usable, collectable;

    public abstract void Use(PlayerController player);
}
