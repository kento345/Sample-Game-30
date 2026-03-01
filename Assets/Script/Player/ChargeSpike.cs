using UnityEditor.Profiling;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ChargeSpike : MonoBehaviour
{
    [SerializeField] private DecalProjector targetRender;
    [SerializeField] private float MaxChargeTime = 1.5f;
    private float charge;

    private Material mat;
    private AtackController ac;
    private PlayerStateManager stateManager;

    private void OnEnable()
    {
        mat = new Material(targetRender.material);
        targetRender.material = mat;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ac = GetComponent<AtackController>();
        stateManager = GetComponent<PlayerStateManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (stateManager.ActionState == ActionState.Charge)
        {
            charge += Time.deltaTime / MaxChargeTime;
        }
        else
        {
            //charge -= Time.deltaTime * 2f;
            charge = 0f;
        }

        charge = Mathf.Clamp01(charge);

        ac.SetCharge(charge);
        mat.SetFloat("_Charge", charge);
    }
}
