using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    [Header("Aim Settings")]
    public Camera cam;
    public Vector2 mouseSensivity = new Vector2(50, 40);
    public float yClamp = 60;

    [Header("Move Settings")]
    public Transform body;
    public float walkSpeed;
    public float runSpeed;
    [Range(0,1)]
    public float reverseMod = .5f, strafeMod = .75f;
    public bool allowMoveInAir, allowCrouchInAir;
    [Range(.001f,1)]
    public float crouchLength;
    float startCamHeight;

    public float jumpForce;
    public Transform groundScan;
    public LayerMask groundMask;

    bool crouched = false;

    [Header("Key Settings")]
    public string moveXAxis;
    public string moveYAxis;
    public KeyCode jumpKey, runKey, crouchKey;


    [HideInInspector]
    public bool freezed = false;


    // Start is called before the first frame update
    void Start()
    {
        startCamHeight = cam.transform.localPosition.y;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!freezed)
        {
            Vector2 mouseAxis = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            Aim(mouseAxis);

            bool grounded = CheckGround();
            if(grounded || allowMoveInAir)
            {
                Vector2 move = new Vector2(Input.GetAxis(moveXAxis), Input.GetAxis(moveYAxis));
                Move(move);
            }
            if(grounded || allowCrouchInAir)
            {

                if (Input.GetKeyDown(crouchKey))
                {
                    body.localScale = new Vector3(1f, crouchLength, 1f);
                    cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, startCamHeight * crouchLength, cam.transform.localPosition.z);
                    crouched = true;
                }
            }

            if (grounded)
            {
                if (Input.GetKeyDown(jumpKey))
                {
                    rb.AddForce(Vector3.up * jumpForce);
                    if(crouched && !allowCrouchInAir)
                    {
                        cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, startCamHeight, cam.transform.localPosition.z);
                        body.localScale = new Vector3(1f, 1f, 1f);
                        crouched = false;
                    }
                }
            }
            if (Input.GetKeyUp(crouchKey))
            {
                cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, startCamHeight, cam.transform.localPosition.z);
                body.localScale = new Vector3(1f, 1f, 1f);
                crouched = false;
            }
        }
    }
    
    bool CheckGround ()
    {
        Ray ray = new Ray(groundScan.position, Vector3.down);
        if (Physics.Raycast(ray, .05f, groundMask))
            return true;

        return false;
    }

    void Aim (Vector2 aim)
    {
        Vector3 camRot = cam.transform.rotation.eulerAngles;
        Vector3 rot = transform.rotation.eulerAngles;

        camRot.x -= aim.y * mouseSensivity.y * Time.deltaTime;

        float yCy = -yClamp + 360;

        if (camRot.x > 180 && camRot.x < yCy)
            camRot.x = yCy;

        if (camRot.x < 180 && camRot.x > yClamp)
            camRot.x = yClamp;

        rot.y += aim.x * mouseSensivity.x * Time.deltaTime;

        cam.transform.rotation = Quaternion.Euler(camRot);
        transform.rotation = Quaternion.Euler(rot);
    }

    void Move (Vector2 dir)
    {
        float spd = (Input.GetKey(runKey) ? runSpeed : walkSpeed) * Time.deltaTime;
        rb.position += (transform.forward * (dir.y * (dir.y < 0 ? spd * reverseMod: spd))) + (transform.right * (dir.x * spd * strafeMod));
    }

    public bool Frozen { set => freezed = value; }

}
