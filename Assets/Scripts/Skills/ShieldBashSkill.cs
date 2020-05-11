using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShieldBashSkill : MeleeSkill
{
    [Header("Debuff Variables")]
    public Buff debuffToAdd;
    public float chanceToAddBuff;

    [Header("Animation Variables")]
    public float targetOffSet;
    public float travelDuration;
    public float spawnDuration;
    public float  windUp;
    public float windUpDuration;
    public float slam;
    public float slamDuration;
    public float returnDuration;

    [Header("Shield Variables")]
    public GameObject shieldObject;
    public float spawnOffSet;
    public float spawnSize;
    protected GameObject objectInstance;
    Vector3 shieldTarget;

    public float InitDur = 2;

    public void RollForBuff()
    {
        float rng = Random.value;

        chanceToAddBuff = _Intensity.Levels[IntensityLevel - 1].IDatas.First();

        if(rng <= chanceToAddBuff)
        {
            if(!enemy.GetComponent<BuffReciever>().FindBuff(debuffToAdd))
            {
                enemy.GetComponent<BuffReciever>().AddBuff(debuffToAdd);
            }
            else
            {
                enemy.GetComponent<BuffReciever>().FindBuff(debuffToAdd).duration = debuffToAdd.duration;
            }
        }
    }

    public override void SkillEffect()
    {
        OnActivated.AddListener(RollForBuff);
        
        Vector3 origin = actor.transform.position;
        StartCoroutine(SkillRoutine(actor.GetComponent<CustomCharacterController>(), origin));
    }

    public override IEnumerator SkillRoutine(CustomCharacterController cc, Vector3 origin)
    {
        yield return new WaitForSeconds(0.5f);
        
        Sequence s = DOTween.Sequence();

        Vector3 movePoint = new Vector3(enemy.transform.position.x - targetOffSet, enemy.transform.position.y, enemy.transform.position.z);
        s.Append(actor.transform.DOMove(movePoint, travelDuration));

        yield return new WaitForSeconds(travelDuration);

        SpawnShield();
        yield return new WaitForSeconds(spawnDuration);

        Vector3 rotate = new Vector3(windUp - actor.transform.rotation.x, 90, 0);
        s.Append(actor.transform.DORotate(rotate, windUpDuration));
        s.Append(actor.transform.DOPunchScale(new Vector3(2,2,2),windUpDuration, 5, 0.5f));
        yield return new WaitForSeconds(windUpDuration);

        //Slam
        rotate = new Vector3(slam + actor.transform.rotation.x, 90, 0);
        shieldTarget = enemy.transform.position;
        s.Append(actor.transform.DORotate(rotate, slamDuration).OnStart(MoveShield));

        DealDamage(enemy, Damage);
        OnActivated.Invoke();
        Quaternion rotation = Quaternion.Euler(-90,0, 0);
        yield return new WaitForSeconds(slamDuration);
        
        s.Append(actor.transform.DORotate(new Vector3(0,90,0), 0.1f));

        yield return new WaitForSeconds(0.1f);
        //DestroyShield();
        movePoint = origin;
        s.Append(actor.transform.DOMove(movePoint, returnDuration).OnComplete(SkillEffectEnd));
        yield return new WaitForSeconds(returnDuration);
        actor.GetComponent<CustomCharacterController>().OnDestinationReach.Invoke();
    }

    public void SpawnShield()
    {
        objectInstance = Instantiate(shieldObject, enemy.transform.position + (Vector3.up*spawnOffSet), shieldObject.transform.rotation);
        objectInstance.transform.localScale = new Vector3(0,0,0);
        objectInstance.transform.DOScale(new Vector3(spawnSize,spawnSize,spawnSize),spawnDuration);
    }

    public void MoveShield()
    {
        objectInstance.transform.DOMove(shieldTarget, slamDuration).OnComplete(DestroyShield);
    }

    public void DestroyShield()
    {
        if(audioManager) audioManager.PlaySFX("Hit_Shield_Bash");
        Destroy(objectInstance);
    }

    public override float Damage
    {
        get
        {
            return _Intensity.Levels[IntensityLevel - 1].IDatas.Last();
        }
    }

    protected override void UpdateSkillDescription()
    {
        Descriptions[InsertIndexes.First()] = "<color=blue>";

        int levelCount = _Intensity.Levels.Count;

        for (int a = 0; a < levelCount; a++)
        {
            List<float> IDatas = _Intensity.Levels[a].IDatas;
            int count = IDatas.Count;

            Descriptions[InsertIndexes.First()] += IDatas.Last().ToString();

            if (a < levelCount - 1)
            {
                Descriptions[InsertIndexes.First()] += "/";
            }
        }

        Descriptions[InsertIndexes.First()] += " damage</color>";
        Descriptions[InsertIndexes[1]] = "<color=blue>";

        for (int a = 0; a < levelCount; a++)
        {
            List<float> IDatas = _Intensity.Levels[a].IDatas;
            int count = IDatas.Count;

            Descriptions[InsertIndexes[1]] += (IDatas.First() * 100).ToString();

            if (a < levelCount - 1)
            {
                Descriptions[InsertIndexes[1]] += "/";
            }
        }

        Descriptions[InsertIndexes[1]] += "% chance</color>";

        Descriptions[InsertIndexes.Last()] = "<color=blue> (" + InitDur + " turns)</color>";
    }
}
