using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float ShakeValueAsDefault = 3.0f;
    public float ShakeTimerAsDefault = 0.5f;

    Vector3 origPosition;

    private void Awake()
    {
        SingletonManager.Register<CameraShake>(this,SingletonType.Occasional);
    }

    // Start is called before the first frame update
    void Start()
    {
        origPosition = transform.position;
    }

    /// <summary>
    /// ShakeValue = 3.0f
    /// ShakeTimer = 0.5f
    /// </summary>
    /// <param name="shakeValue"></param>
    /// <param name="shakeTimer"></param>
    public void CameraShakeByInput(float shakeValue, float shakeTimer)
    {
        StartCoroutine(ShakeCamera(shakeValue, shakeTimer));
    }

    public void CameraShakeByDefault()
    {
        StartCoroutine(ShakeCamera(ShakeValueAsDefault, ShakeTimerAsDefault));
    }

    IEnumerator ShakeCamera(float shakeValue, float shakeTimer)
    {
        float timer = shakeTimer;

        while (true)
        {
            if (timer > 0)
            {
                transform.position = origPosition + Random.insideUnitSphere * shakeValue * Time.deltaTime;
            }
            else
            {
                transform.position = origPosition;
                yield break;
            }

            timer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
