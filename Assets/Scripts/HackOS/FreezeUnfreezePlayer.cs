using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class FreezeUnfreezePlayer : MonoBehaviour
{
    PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerController>();
        Unfreeze();
    }

    public void Freeze ()
    {
        player.Frozen = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Unfreeze()
    {
        player.Frozen = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
