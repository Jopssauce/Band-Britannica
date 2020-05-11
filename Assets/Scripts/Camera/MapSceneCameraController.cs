using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSceneCameraController : MonoBehaviour
{
    public float cameraSpeed;
    public float variable;

    // Update is called once per frame
    void Update()
    {
		CameraMove();
    }

    void CameraMove ()
    {
        if (Input.mousePosition.x > Screen.width)
		{
			transform.position += Vector3.right * 1 * cameraSpeed * Time.deltaTime;
		}
		if (Input.mousePosition.x < 0)
		{
			transform.position += Vector3.right * -1 * cameraSpeed * Time.deltaTime;
		}
		if (Input.mousePosition.y > Screen.height)
		{
			transform.position += Vector3.forward * 1 * cameraSpeed * Time.deltaTime;
		}
		if (Input.mousePosition.y < 0)
		{
			transform.position += Vector3.forward * -1 * cameraSpeed * Time.deltaTime;
		}
        CameraClamp();
    }

    void CameraClamp()
    {
        Vector3 clampedPos = transform.position;
		clampedPos.z = Mathf.Clamp(transform.position.z, -13f, 3f);
		clampedPos.x = Mathf.Clamp(transform.position.x, -9f, 9f);
		transform.position = clampedPos;
    }
}