using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
    float speed;
    public float maxSpeed = 10.0f;
    public float acceleration = 5.0f;
    public float destroyRange = 5.0f;
    public float stopRange = 5.0f;

    public float BaseValue = 20.0f;

    //Assumes that particles have their own behaviors like deleting themselves after spawning
    public GameObject particleEffectPrefab;

    public Vector3 destination;

    public UnityEvent OnDestroy;
    public UnityEvent OnHit;

    protected AudioManager audioManager;

    protected virtual void Start()
    {
        audioManager = SingletonManager.Get<AudioManager>();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        /*
        if (Vector3.Distance(this.transform.position, destination) <= destroyRange)
        {
            OnProjectileHit();
            DestroyProjectile();
        }
        if (Vector3.Distance(this.transform.position, destination) <= stopRange)
        {
            return;
        }
        if (destination != null)
        {
            MoveTo(destination);
            WhileProjectileMoving();
        } */  
    }

    public virtual void OnProjectileHit() { OnHit.Invoke(); }
    public virtual void OnProjectileStop() { }
    public virtual void WhileProjectileMoving() { }

    public void MoveTo(Vector3 dest)
    {
        //Direction
        Vector3 direction;
        direction = dest - transform.position;
        direction.Normalize();

        //Velocity
        Vector3 velocity;

        //Acceleration
        velocity = speed * direction * Time.deltaTime;
        speed += acceleration;

        transform.position += velocity;
    }

    public void DestroyProjectile()
    {
        if (particleEffectPrefab != null)
        {
            Instantiate(particleEffectPrefab, this.transform.position, particleEffectPrefab.transform.rotation);
        }

        OnDestroy.Invoke();
        Destroy(this.gameObject);
    }

    public float Damage
    {
        get
        {
            return BaseValue;
        }
    }
}
