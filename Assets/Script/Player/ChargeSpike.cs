using UnityEditor.Profiling;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ChargeSpike : MonoBehaviour
{
    //[SerializeField] private Renderer targetRender;
    [SerializeField] private DecalProjector targetRender;
    [SerializeField] private float chargeSpeed = 1.5f;

    private Material mat;
    private float charge;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mat = targetRender.material;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            charge += Time.deltaTime * chargeSpeed;
        }
        else
        {
            charge -= Time.deltaTime * chargeSpeed;
        }

        charge = Mathf.Clamp01(charge);

        mat.SetFloat("_Charge", charge);
    }
}
