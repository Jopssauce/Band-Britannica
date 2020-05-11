using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleHandler : MonoBehaviour
{
    public GameObject particleSystem;
    public void Destroy()
    {
        Instantiate(particleSystem, this.transform.position, particleSystem.transform.rotation);
        Destroy(this.gameObject);
    }
}
