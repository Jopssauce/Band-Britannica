using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSceneCamera : MonoBehaviour
{
    public float horizontalSpeed;
    public float verticalSpeed;

    public float minPitchView;
    public float maxPitchView;
    public float minYawView;
    public float maxYawView;

    protected float yaw;
    protected float pitch;

    // Update is called once per frame
    void LateUpdate()
    {
        yaw += horizontalSpeed*Input.GetAxis("Mouse X");
        pitch -= verticalSpeed*Input.GetAxis("Mouse Y");

        LimitRange();

        transform.eulerAngles = new Vector3(pitch,yaw,transform.rotation.eulerAngles.z);
    }

    void LimitRange()
    {
        if(pitch < minPitchView) pitch = minPitchView;
        if(pitch > maxPitchView) pitch = maxPitchView;
        if(yaw < minYawView) yaw = minYawView;
        if(yaw > maxYawView) yaw = maxYawView;
    }
}
