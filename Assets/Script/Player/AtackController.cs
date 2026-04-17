using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.ShaderGraph.Internal;
using UnityEditorInternal;
using UnityEngine;

public class AtackController : MonoBehaviour
{
    [SerializeField] private float curentForce = 15f;
    private float duration = 0.5f;
    private float cooldown = 1.0f; //攻撃クールダウン
    private bool lisCooldown = false;

    [HideInInspector] public float chargeMax = 5.0f; //チャージ上限
    private float t = 0f;
    private bool isMax = false;

    //-----硬直-----
    [SerializeField] private float StrongRecoveryTime = 1.0f; //硬直時間
    private float curentRecoveryTime;
    [HideInInspector] public bool isRigid = false;

    [Header("ノックバック,無敵設定")]
    public float WeakKnockbackForce = 15.0f; //弱ノックバック
    public float StrongKnockbackForce = 30.0f;//強ノックバック
    private float curentknockbackForce = 0f;//現在のノックバック力

    [Header("当たり判定設定")]
    [SerializeField] private SphereCollider searchArea;
    [SerializeField] private float angle = 45f;

    bool hasHit = false;

    Rigidbody rb;
    PlayerStateManager stateManager;
   
    public void SetCharge(float value)
    {
        t = value;
    }

    private void Start()
    {
        curentRecoveryTime = StrongRecoveryTime;

        rb = GetComponent<Rigidbody>();
        stateManager = GetComponent<PlayerStateManager>();
    }

    void Update()
    {
        if (stateManager.ActionState == ActionState.Charge && stateManager.State != State.Knockback)
        {
            if (t < chargeMax)
            {
                t += Time.deltaTime;
                //stateManager.SetAttackPower(AttackPower.Weak);
            }
            if(t >= chargeMax)
            {
                isMax = true;
                //stateManager.SetAttackPower(AttackPower.Strong);
            }
        }
        if(stateManager.State == State.Knockback)
        {
            SetCharge(0);
            CancelInvoke("EndAttack");
            EndAttack();
        }
        else
        {
            t = 0f;
            //stateManager.SetAttackPower(AttackPower.None);
        }
        if (isRigid)
        {
            if(curentRecoveryTime > 0f)
            {
                curentRecoveryTime -= Time.deltaTime;
            }
            if(curentRecoveryTime <= 0f)
            {
                isRigid = false;
                curentRecoveryTime = StrongRecoveryTime;
            }
        }
        //Debug.Log(stateManager.ActionState);
    }

    public void Shot(int x)
    {
        if (x == 0)
        {
            if(lisCooldown) { return; }
            if (stateManager.ActionState == ActionState.Charge) {return; }
            isRigid = false;

            //チャージ開始,ステート変更
            stateManager.SetActionState(ActionState.Charge);
        }
        if (x == 1)
        {
            if(lisCooldown) { return; }
            if (isRigid) { return; }

            if (stateManager.ActionState == ActionState.Charge)
            {
                //チャージを止め攻撃,ステート変更
                stateManager.SetActionState(ActionState.Attack);

                //? = true , : = false
                curentknockbackForce = isMax ? StrongKnockbackForce : WeakKnockbackForce;

                rb.AddForce(transform.forward * curentForce, ForceMode.Impulse);

                Invoke("EndAttack", duration);
            } 
          
        }  
    }

    void EndAttack()
    {
        rb.linearVelocity = Vector3.zero;
        //ステートをNoneに
        stateManager.SetActionState(ActionState.None);

        hasHit = false;

        if (isMax)
        {
            Debug .Log("強攻撃");
            isRigid = true;
        }
    
        isMax = false;
        //stateManager.SetAttackPower(AttackPower.None);
        t = 0f;

        StartCoroutine(CooldownCount());
    }

    IEnumerator CooldownCount()
    {
        lisCooldown = true;
        yield return new WaitForSeconds(cooldown);
        lisCooldown = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if(hasHit) { return; }
        if (stateManager == null || rb == null) return;

        if (stateManager.ActionState != ActionState.Attack) return;
        if (other.gameObject.CompareTag("Player"))
        {
            Vector3 posDir = other.transform.position - this.transform.position;
            float target_angle = Vector3.Angle(this.transform.forward, posDir);

            var dist = Vector3.Distance(other.transform.position, transform.position);

            if (target_angle > angle) { return; }
            float radius = searchArea.radius * transform.lossyScale.x;
            if (target_angle <= angle &&
                Vector3.Distance(transform.position, other.transform.position) <= radius)
            {
                hasHit = true;

                Reception p = other.GetComponent<Reception>();
                if (p == null) return;

                p.KnockBack(rb.linearVelocity.normalized, curentknockbackForce);

                CancelInvoke("EndAttack");
                EndAttack();
            }
        }
      
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        var pos = transform.position;
        pos.y = 1.0f;
        Handles.color = Color.red;
        Handles.DrawSolidArc(pos, Vector3.up, Quaternion.Euler(0.0f, -angle, 0f) * transform.forward, angle * 2f, searchArea.radius);
    }
#endif
}