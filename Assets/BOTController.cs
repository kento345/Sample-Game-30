using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class BOTController : MonoBehaviour
{
    GameObject target;

    GameObject near = null;

    private PlayerStateManager stateManager;
    private MoveController move;
    private AtackController atack;
    private Seencer sencer;

    Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
                //isRotating = false;
            }
            return;
        }
        if (target == null)
        {
            CreatePoint();
            return;
        }

        MoVeToPoint();
        float dist = Vector3.Distance(transform.position, target.transform.position);
        if (dist < 1f)
        {
            Destroy(target);
            CreatePoint();
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

    void Serch()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        
        float minDist = Mathf.Infinity;

        foreach(var player in players)
        {
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

    void OnAtack(int i)
    {
        atack.Shot(i);
    }
}
