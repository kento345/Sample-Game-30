using JetBrains.Annotations;
using System.Collections;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerItemEffect : MonoBehaviour
{
    [SerializeField] private DecalProjector circle;
    [SerializeField] private DecalProjector arraw;

    Vector3 defaultScale;
    float defaultSpeed;
    float defaultWeakKnockbackForce;
    float defaultStrongKnockbackForce;
    Vector3 defaltCircleSize;
    Vector3 defaltArrawSize;
    Vector3 circleSize;
    Vector3 arrawSize;
    
    public bool isEffectActive {get; private set;} = false;

    AtackController ac;
    MoveController mc;
    Reception reception;

    private void Start()
    {
        ac = GetComponent<AtackController>();
        mc = GetComponent<MoveController>();
        reception = GetComponent<Reception>();

        defaultScale = transform.localScale;
        defaultWeakKnockbackForce = ac.WeakKnockbackForce;
        defaultSpeed = mc.Speed;
        defaultStrongKnockbackForce = ac.StrongKnockbackForce;
        defaltCircleSize = circle.size;
        defaltArrawSize = arraw.size;
    }

    public void ApplyItem(Item item)
    {
        switch (item.type)
        {
            case Item.Type.RandomBox:
                RandomEfect(item);
                break;
            case Item.Type.SmallBox:
                StartCoroutine(SmallEfect(item));
                break;
            case Item.Type.BigBox:
                StartCoroutine(BigEfect(item));
                break;
        }
    }

    void RandomEfect(Item item)
    {
        int r = Random.Range(0, 2);

        if (r == 0)
        {
            StartCoroutine(BigEfect(item));
        }
        else
        {
            StartCoroutine(SmallEfect(item));

        }
    }
    IEnumerator BigEfect(Item item)
    {
        transform.localScale = defaultScale * item.effectValue;
        DecalScale(0,item);
        isEffectActive = true;
        yield return new WaitForSeconds(item.duration);
        transform.localScale = defaultScale;
        DecalScale(1, item);
        isEffectActive = false;
    }

    IEnumerator SmallEfect(Item item)
    {
        transform.localScale = defaultScale / item.effectValue;
        DecalScale(2, item);
        mc.Speed = defaultSpeed * item.effectValue;
        reception.smallKnockback = 20;
        isEffectActive = true;
        yield return new WaitForSeconds(item.duration);
        transform.position += new Vector3(0,1,0);
        transform.localScale = defaultScale;
        DecalScale(1, item);
        mc.Speed = defaultSpeed;
        reception.smallKnockback = 1;
        isEffectActive = false;
    }

    void DecalScale(int i,Item item)
    { 
        if (i == 0)
        {
            //デカール拡大;
            circleSize = defaltCircleSize * item.effectValue;
            circleSize.z = defaltCircleSize.z;
            circle.size = circleSize;
            arrawSize = defaltArrawSize * item.effectValue;
            arrawSize.z = defaltArrawSize.z;
            arraw.size = arrawSize;
            //攻撃力アップ
            ac.WeakKnockbackForce = defaultWeakKnockbackForce * item.effectValue;
            ac.StrongKnockbackForce = defaultStrongKnockbackForce * item.effectValue;
        }
        else if (i == 1)
        {
            //デカールノーマル
            circle.size = defaltCircleSize;
            arraw.size  = defaltArrawSize;
            //攻撃力ノーマル
            ac.WeakKnockbackForce = defaultWeakKnockbackForce;
            ac.StrongKnockbackForce = defaultStrongKnockbackForce;
        }
        else
        {
            //デカール縮小
            circleSize = defaltCircleSize / item.effectValue;
            circleSize.z = defaltCircleSize.z;
            circle.size = circleSize;
            arrawSize = defaltArrawSize / item.effectValue;
            arrawSize.z = defaltArrawSize.z;
            arraw.size = arrawSize;
            //攻撃力ダウン
            ac.WeakKnockbackForce = defaultWeakKnockbackForce / item.effectValue;
            ac.StrongKnockbackForce = defaultStrongKnockbackForce / item.effectValue;
        }
    }
}
