using System.Collections;
using UnityEngine;

public class Reception : MonoBehaviour
{
    [Header("ノックバック,無敵設定")]
    private float knockbackTime = 0.3f;
    private float knockbackCounter;

    private Vector3 knockbackDir;

    Collider col;

    [SerializeField] private float StunInvincibleTime = 1.0f; //無敵時間
    bool isKonckback = false;
    private bool isHit = false;
    Rigidbody rb;
    //Animator animator;

    //-----Script参照-----
    private PlayerStateManager stateManager;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        //animator = GetComponent<Animator>();
        col = GetComponent<Collider>();
        stateManager = GetComponent<PlayerStateManager>();
    }

    private void Update()
    {
        if (isKonckback)
        {
            knockbackCounter -= Time.deltaTime;
            if (knockbackCounter <= 0)
            {
                //stateManager.SetActionState(ActionState.None);
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
        isKonckback = true;
        knockbackCounter = knockbackTime;
        Debug.Log("ノックバック");
        knockbackDir = pos.normalized * force;
        rb.linearVelocity = Vector3.zero;
        StartCoroutine(Hit());
    }

    IEnumerator Hit()
    {
        isHit = true;
        col.enabled = false;
        rb.useGravity = false;
        yield return new WaitForSeconds(StunInvincibleTime);

        rb.useGravity = true;
        col.enabled = true;
        Debug.Log("無敵終了");
       isHit = false;
    }
}
