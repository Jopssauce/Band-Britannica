using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPan : MonoBehaviour
{
    public float maxDistance = 1;
    public float speed = 2;
    Vector3 startPos;
    float currentDistance;


    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        currentDistance = Vector3.Distance(startPos, transform.position);
        if (currentDistance < maxDistance)
        {
            transform.position += Vector3.right * speed * Time.deltaTime;
        }
    }
}
