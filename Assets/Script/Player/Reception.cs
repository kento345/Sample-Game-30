using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Reception : MonoBehaviour
{
    [Header("ノックバック,無敵設定")]
    private float knockbackTime = 0.3f;
    private float knockbackCounter;

    private Vector3 knockbackDir;

    [HideInInspector] public float smallKnockback = 1;

    [SerializeField] private float StunInvincibleTime = 1.0f; //無敵時間
    bool isKonckback = false;
    private bool isHit = false;
    Collider col;
    Rigidbody rb;

    //-----Script参照-----
    private PlayerStateManager stateManager;
    private ChargeSpike cs;

    private AtackController ac;
    private PlayerInputController playerCon;
    private BOTController botCon;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        stateManager = GetComponent<PlayerStateManager>();
        cs = GetComponent<ChargeSpike>();;
        ac = GetComponent<AtackController>();
        playerCon = GetComponent<PlayerInputController>();
        botCon = GetComponent<BOTController>();
    }

    private void Update()
    {
        if (isKonckback)
        {
            knockbackCounter -= Time.deltaTime;
            if (knockbackCounter <= 0)
            {
                isKonckback = false;
                //stateManager.SetState(State.None);
                rb.linearVelocity = Vector3.zero;
            }
        }
    }
    private void FixedUpdate()
    {
        if(isKonckback)
        {
            rb.linearVelocity = knockbackDir;
        }
    }

    public void KnockBack(Vector3 pos,float force)
    {
        if (isHit) return;

        isKonckback = true;
        if (ac != null)
        {
            ac.SetCharge(0);
        }
        if (stateManager != null)
        {
            stateManager.SetActionState(ActionState.None);
        }
        //stateManager.SetState(State.Hit);
        knockbackCounter = knockbackTime;
        Debug.Log("ノックバック");
        knockbackDir = pos.normalized * force * smallKnockback;
        rb.linearVelocity = Vector3.zero;

        if (botCon != null)
        {
            botCon.OnMove(Vector2.zero);
        }
        if (playerCon != null)
        {
            playerCon.DontMove(true);
        }
        StartCoroutine(Hit());
    }

    IEnumerator Hit()
    {
        isHit = true;
        //stateManager.SetState(State.Knockback);
        yield return new WaitForSeconds(0.05f);
        col.enabled = false;
        rb.useGravity = false;
        yield return new WaitForSeconds(StunInvincibleTime);

        rb.useGravity = true;
        col.enabled = true;
        isHit = false;
        if (playerCon != null)
        {
            playerCon.DontMove(false);
        }
    }
}
