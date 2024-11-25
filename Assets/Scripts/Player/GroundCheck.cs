using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    private PlayerStateMachine playerStateMachine;
    private PlayerParameter playerParameter;
    void Start()
    {
        playerStateMachine = GetComponentInParent<PlayerStateMachine>();
        playerParameter = playerStateMachine.playerParameter;
    }

    void OnCollisionStay2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Ground")) //判斷碰撞方向和爬牆
        {
            foreach(ContactPoint2D p in other.contacts)
            {
                if (p.normal == new Vector2(1f, 0f)) 
                {
                    playerParameter.wallCheck = "Left";
                }
                else if (p.normal == new Vector2(-1f, 0f))
                {
                    playerParameter.wallCheck = "Right";
                }
                else if (p.normal != new Vector2(0f, -1f))
                {
                    playerParameter.wallCheck = "Ground";
                    playerParameter.airJump = 1;
                }
            }
        }
        if(other.gameObject.CompareTag("Platform")) //判斷碰撞方向和爬牆
        {
            playerParameter.wallCheck = "Ground";
            playerParameter.airJump = 1;
        }
    }


}
