using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomedCamAim : MonoBehaviour
{
    public Vector2 sensivity, clamp;
    bool on;

    public bool On { get => on;
        set
        {
            on = value;
            if(!value)
            {
                transform.localRotation = Quaternion.Euler(Vector3.zero);
            }
        }
    }

    private void Update()
    {
        if(on)
        {
            Vector2 mouseAxis = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            Aim(mouseAxis);
        }
    }

    void Aim (Vector2 aim)
    {
        Vector3 rotation = transform.localRotation.eulerAngles;
        rotation.x = rotation.x - (aim.y * sensivity.y * Time.deltaTime);
        rotation.y = rotation.y + (aim.x * sensivity.x * Time.deltaTime);
        float xClampX = -clamp.y + 360;
        if (rotation.x > 180 && rotation.x < xClampX)
            rotation.x = xClampX;

        if (rotation.x < 180 && rotation.x > clamp.y)
            rotation.x = clamp.y;

        float yClampX = -clamp.x + 360;
        if (rotation.y > 180 && rotation.y < yClampX)
            rotation.y = yClampX;

        if (rotation.y < 180 && rotation.y > clamp.x)
            rotation.y = clamp.x;


        transform.localRotation = Quaternion.Euler(rotation);
    }
}
