using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public enum CharacterStates
{
    Idle,
    NotIdle
}

public class CustomCharacterController : MonoBehaviour
{
    public UnityEvent OnDestinationReach;

    float speed;
    float distance;
    float maxDistanceDelta;
    public float maxSpeed = 10.0f;
    public float acceleration = 5.0f;
    public float destroyRange = 5.0f;
    public float stopRange = 0.0f;
    //Assumes that particles have their own behaviors like deleting themselves after spawning
    public GameObject particleEffectPrefab;

    public Vector3 destination;

    public UnityEvent OnDestroy;

    [Header("Take Damage Animation Variables")]
    public float duration;

    [Header("Death Animation Variables")]
    public GameObject skullPrefab;
    public GameObject deathParticle;
    public float skullSpawnOffset;
    public float floatPointOffset;

    // Update is called once per frame
    public virtual void Update()
    {
        
    }

    public virtual void WhileCharacterMoving() { }

    public void MoveTo(Vector3 dest)
    {
        if (speed > maxSpeed) speed = maxSpeed;
        //Direction
        Vector3 direction;
        direction = dest - transform.position;
        direction.Normalize();

        //Velocity
        Vector3 velocity;

        //Acceleration
        velocity = direction * maxDistanceDelta;
        speed += acceleration;

        transform.position += velocity;
        //transform.position = Vector3.MoveTowards(transform.position, dest, Time.deltaTime * speed);
    }

    public IEnumerator TakeDamageAnimation()
    {
        Sequence s = DOTween.Sequence();

        s.Append(this.transform.DOPunchPosition(Vector3.forward, duration));
        s.Insert(0,this.transform.DOPunchScale(Vector3.up ,duration));

        yield return new WaitForSeconds(duration);
        this.transform.DOScale(new Vector3(6,6,6), 0);
    }

    public IEnumerator DeathAnimation()
    {
        GameObject particle = Instantiate(deathParticle, this.transform.position + (Vector3.up*floatPointOffset), deathParticle.transform.rotation);
        GameObject skull = Instantiate(skullPrefab, this.transform.position + (Vector3.up*skullSpawnOffset), skullPrefab.transform.rotation);
        skull.GetComponent<SkullAnimator>().point = this.transform.position + (Vector3.up*floatPointOffset);
        yield return null;
    }
}