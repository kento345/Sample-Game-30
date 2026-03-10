using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class BOTController : MonoBehaviour
{
   [SerializeField] GameObject target;
    GameObject near = null;
    float minDist;

    private float attackPrepareTime = 1f;
    bool preparingAttack = false;
    float prepareCounter = 0f;

    bool attackRest = false;
    private float restTime = 3f;

    private PlayerStateManager stateManager;
    private MoveController move;
    private AtackController atack;
    private Seencer sencer;

    Rigidbody rb;



    void Start()
    {
        stateManager = GetComponent<PlayerStateManager>();
        move = GetComponent<MoveController>();
        atack = GetComponent<AtackController>();
        sencer = GetComponentInChildren<Seencer>();
        rb = GetComponent<Rigidbody>();

        CreatePoint();
    }

    void Update()
    {
        Debug.Log(stateManager.ActionState);
        //false
        if (!sencer.CheckLayer())
        {
            StopAndCreate();

            Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + 180f, 0);

            // Slerp‚ÅŠŠ‚ç‚©‚É‰ñ“]
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, 5 * Time.deltaTime);

            // ‚Ù‚Ú–Ú•WŒü‚«‚É‚È‚Á‚½‚ç’âŽ~
            if (Quaternion.Angle(rb.rotation, targetRotation) < 0.5f)
            {
                rb.rotation = targetRotation;
            }
            return;
        }
        Serch();

        if (!attackRest && near != null && minDist < 10f)
        {
            target = near;

            Attack();
        }

        if (target == null)
        {
            CreatePoint();
            return;
        }

        MoVeToPoint();

        float dist = Vector3.Distance(transform.position, target.transform.position);
        if (target.name == "CheckPoint" && dist < 1f)
        {
            Destroy(target);
            CreatePoint();
        }
        if(!attackRest && target == near && stateManager.ActionState == ActionState.None)
        {
            StartCoroutine(RestTime());
        }
        if (stateManager.ActionState == ActionState.Attack)
        {
            preparingAttack = false;
        }
    }

    void MoVeToPoint()
    {
        Vector3 dir = target.transform.position - transform.position;
        Vector2 moveInput = new Vector2(dir.normalized.x, dir.normalized.z);

        OnMove(moveInput);
    }

    void CreatePoint()
    {
        Vector3 pos = transform.position;
        Vector2 random = Random.insideUnitCircle * 10f;
        Vector3 pointPos = new Vector3(pos.x + random.x, pos.y, pos.z + random.y);

        target = new GameObject("CheckPoint");
        target.transform.position = pointPos;
    }
    
    void StopAndCreate()
    {
        OnMove(Vector2.zero);

        if(target != null)
        {
            Destroy(target);
        }
        CreatePoint();
    }

  /*  void PrepareAttack()
    {
        if (target == null) return;

        // “G‚Ì•ûŒü‚ðŒü‚­
        Vector3 dir = target.transform.position - transform.position;
        dir.y = 0;
        transform.forward = dir.normalized;

        // ˆÚ“®’âŽ~
        OnMove(Vector2.zero);

        if (!preparingAttack)
        {
            preparingAttack = true;
            prepareCounter = attackPrepareTime;
        }

        // —­‚ßŽžŠÔ
        if (prepareCounter <= 0f)
        {
            prepareCounter -= Time.deltaTime;
        }

        // —­‚ßI‚í‚Á‚½‚çƒ`ƒƒ[ƒWUŒ‚ŠJŽn
        //atack.BOTAttack(target.transform.position);
    }*/

    void Attack()
    {
        if (target == null) return;

        Vector3 dir = target.transform.position - transform.position;
        dir.y = 0;
        transform.forward = dir.normalized;
        float dist = dir.magnitude;
        if (!preparingAttack && dist < 6)
        {
            preparingAttack = true;
            prepareCounter = attackPrepareTime;
            OnMove(Vector2.zero); //Ž~‚Ü‚é
        }
        // —­‚ßŽžŠÔ
        if (preparingAttack)
        {
            prepareCounter -= Time.deltaTime;

            // ƒ`ƒƒ[ƒWŠJŽn
            if (stateManager.ActionState == ActionState.None)
            {
                atack.Shot(0);
            }

            // —­‚ßŠ®—¹
            if (prepareCounter <= 0f)
            {
                preparingAttack = false;

                if (stateManager.ActionState == ActionState.Charge)
                {
                    atack.Shot(1);
                }
            }
        }
    }

    void Serch()
    {
        near = null;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");


        minDist = Mathf.Infinity;

        foreach (var player in players)
        {
            if (player == gameObject) continue;

            float dist = Vector3.Distance(transform.position, player.transform.position);
            if(dist < minDist)
            {
                minDist = dist;
                near = player;
            }
        }
    }

    void OnMove(Vector2 context)
    {
        stateManager.UpdateMoveState(context);
        move.SetMoveInput(context);
    }

    IEnumerator RestTime()
    {
        attackRest = true;

        target = null;
        CreatePoint();
        yield return new WaitForSeconds(restTime);

        attackRest = false;
    }
}
