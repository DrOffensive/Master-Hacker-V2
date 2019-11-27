using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackSocket : MonoBehaviour
{
    HackJack_jack connectedJack;
    new public Light light;
    public Material onMaterial, offMaterial;
    public MeshRenderer lightMesh;

    public void SetOn (bool on, HackJack_jack jack)
    {
        if (jack != null)
            connectedJack = jack;
        else
            connectedJack = null;

        light.enabled = on;
        lightMesh.material = on ? onMaterial : offMaterial;
    }
}
