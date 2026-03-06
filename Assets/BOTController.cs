using UnityEngine;

public class BOTController : MonoBehaviour
{
    private PlayerStateManager stateManager;
    private MoveController move;
    private AtackController atack;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stateManager = GetComponent<PlayerStateManager>();
        move = GetComponent<MoveController>();
        atack = GetComponent<AtackController>();
    }

    void Update()
    {
        
    }


}
