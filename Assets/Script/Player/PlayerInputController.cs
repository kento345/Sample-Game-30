using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    private bool notMove = false;

    //Player操作反転
    public int playerID;
    private bool isInverted = false;

    //Script取得
    private PlayerStateManager stateManager;
    private MoveController move;
    private AtackController atack;


    private void Awake()
    {
        stateManager = GetComponent<PlayerStateManager>();
        move = GetComponent<MoveController>();
        atack = GetComponent<AtackController>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (notMove) {
            move.SetMoveInput(Vector2.zero);
            stateManager.UpdateMoveState(Vector2.zero);
            return; }
        Vector2 inputVer = context.ReadValue<Vector2>();

        if (isInverted)
        {
            //操作反転
            inputVer *= -1;
        }


        //ステート変更のための入力受け取り
        stateManager.UpdateMoveState(inputVer);
        //移動処理のための入力受け取り
        move.SetMoveInput(inputVer);
    }

    public void OnAtatck(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            atack.Shot(0);
        }
        if (context.canceled)
        {
            atack.Shot(1);
        }
    }

    public void SetReverse(bool value)
    {
        isInverted = value;
    }

    public void  DontMove(bool x)
    {
        notMove = x;
    }
}
