using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RopeRenderer : MonoBehaviour
{
    LineRenderer lineRenderer;
    public float length;
    public Transform A, B;
    public int verts;
    public AnimationCurve slope;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = verts + 1;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLine();
    }

    void UpdateLine ()
    {
        Vector3[] positions = new Vector3[verts + 1];

        Vector3 line = B.position - A.position;
        if(line.magnitude <= length)
        {
            if (!lineRenderer.enabled)
                lineRenderer.enabled = true;

            float maxDrop = length - line.magnitude;
            for (int i = 0; i < verts; i++)
            {
                float p = (1f / (float)verts) * i;
                float drop = slope.Evaluate(p) * maxDrop;
                Vector3 position = A.position + (line.normalized * (p * line.magnitude));

                Ray ray = new Ray(position, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, drop))
                {
                    positions[i] = hit.point + (Vector3.up * lineRenderer.widthMultiplier);
                } else
                {
                    positions[i] = position + new Vector3(0, -drop, 0);
                }
            }
            positions[positions.Length - 1] = B.position;
            lineRenderer.SetPositions(positions);
        } else
        {
            lineRenderer.enabled = false;
        }
    }
}
