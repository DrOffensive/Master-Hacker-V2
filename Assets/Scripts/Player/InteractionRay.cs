using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class InteractionRay : MonoBehaviour
{
    public float reach;
    public PlayerController player;
    public LayerMask interactionLayers;
    public KeyCode interactKey = KeyCode.F, collectKey = KeyCode.E; 
    Interactable currentInteractable;

    private void Update()
    {
        currentInteractable = CastRay();

        if(currentInteractable!=null)
        {
            if (Input.GetKeyDown(interactKey))
                currentInteractable.Use(player);
        }
    }

    Interactable CastRay ()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, reach, interactionLayers))
        {
            if(hit.collider.GetComponent<Interactable>())
            {
                return hit.collider.GetComponent<Interactable>();
            }
        }

        return null;
    }
}
