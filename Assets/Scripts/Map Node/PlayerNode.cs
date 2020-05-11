using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerNode : MonoBehaviour
{
    public float speed;

    public UnityEvent OnDestinationReached;

    public bool AtDestination(Vector3 destination)
    {
        if (Vector3.Distance(this.transform.position, destination) <= 0)
        {
            return true;
        }
        else return false;
        
    }

    public void MoveToDestination(Vector3 destination)
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * speed);
    }
}
