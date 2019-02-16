using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundControl : MonoBehaviour
{
    public GameControl gameControl;

    public Vector3 maxABSRotationChange = new Vector3(30, 0, 30);

    public float rotationSpeed = 15;
    Rigidbody rb;

    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    public void RandomiseOrientation(float maxAngle)
    {
        Vector3 newRot = new Vector3
            (
                Random.Range(-maxAngle, maxAngle),
                0,
                Random.Range(-maxAngle, maxAngle)
            );


        transform.rotation = Quaternion.Euler(newRot);
    }

    GameControl.DataOutputs controlInput;
    void Update()
    {
        if (!gameControl.playing)
            return;
        Vector3 curRot = transform.rotation.eulerAngles;
        if (curRot.x > 180)
            curRot.x -= 360;
        if (curRot.z > 180)
            curRot.z -= 360;

        controlInput = gameControl.GetOutput();

        Vector3 newRot = new Vector3
            (
                Mathf.Clamp(curRot.x + controlInput.outputX * Time.deltaTime * rotationSpeed, -maxABSRotationChange.x, maxABSRotationChange.x),
                0,
                Mathf.Clamp(curRot.z - controlInput.outputY * Time.deltaTime * rotationSpeed, -maxABSRotationChange.z, maxABSRotationChange.z)
            );


        transform.rotation = Quaternion.Euler(newRot);
    }


}
