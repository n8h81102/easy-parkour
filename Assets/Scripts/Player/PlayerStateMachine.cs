using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PStateType
{
    Idle, Run, Dash, Jump, Fall, Climb, BackgroundClimb, GetHit, Die
}

[Serializable]
public class PlayerParameter
{
    public float normalGravity;
    public float runSpeed;
    public float jumpSpeed;
    public float dashSpeed;
    public float dashCD;
    public int moveDir;
    public int dashDir;
    public int airJump;
    public string wallCheck;
    public float knockBackSpeed;
    public Rigidbody2D myRigidBody2D;
    public Animator myAnim;
    public Transform respawnPoint;
}

public class PlayerStateMachine : MonoBehaviour
{
    private IPlayerState currentState;
    public PlayerParameter playerParameter;
    private Parameter enemyParameter;
    private EnemyFSM enemyFSM;
    private PlayerHealth playerHealth;
    
    private Dictionary<PStateType, IPlayerState> states = new Dictionary<PStateType, IPlayerState>();
    void Start()
    {
        states.Add(PStateType.Idle, new PIdleState(this));
        states.Add(PStateType.Run, new PRunState(this));
        states.Add(PStateType.Dash, new PDashState(this));
        states.Add(PStateType.Jump, new PJumpState(this));
        states.Add(PStateType.Fall, new PFallState(this));
        states.Add(PStateType.Climb, new PClimbState(this));
        states.Add(PStateType.BackgroundClimb, new PBackgroundClimbState(this));
        states.Add(PStateType.GetHit, new PGetHitState(this));

        playerParameter.moveDir = 1;
        playerParameter.normalGravity = 5;
        playerParameter.myRigidBody2D.gravityScale = playerParameter.normalGravity;
        playerParameter.dashCD = 0;
        playerParameter.dashDir = 1;
        
        playerHealth = GetComponent<PlayerHealth>();
        TransitionState(PStateType.Idle);   
    }

    void Update()
    {
        currentState.OnUpdate();
        if(playerParameter.dashCD >= 0)
            playerParameter.dashCD -= Time.deltaTime;
    }

    void flip()
    {
        if(Input.GetKeyDown(KeyCode.A) || (Input.GetKeyUp(KeyCode.D) && Input.GetKey(KeyCode.A)))
        {
            transform.localScale = new Vector3(-1, 1, 1);
            playerParameter.moveDir = -1;
        }
        if(Input.GetKeyDown(KeyCode.D) || (Input.GetKeyUp(KeyCode.A) && Input.GetKey(KeyCode.D)))
        {
            transform.localScale = new Vector3(1, 1, 1);
            playerParameter.moveDir = 1;
        }
    }

    public void TransitionState(PStateType type)
    {
        if(currentState != null)
            currentState.OnExit();
        currentState = states[type];

        //if (currentState == states[PStateType.Climb])
           
        currentState.OnEnter();
    }

    void OnCollisionStay2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Ground")) //判斷碰撞方向和爬牆
        {
            foreach(ContactPoint2D p in other.contacts)
            {
                if (p.normal.y <= Mathf.Abs(Mathf.Epsilon) && p.normal.x == 1) 
                {
                    playerParameter.wallCheck = "Left";
                }
                else if (p.normal.y <= Mathf.Abs(Mathf.Epsilon) && p.normal.x == -1) 
                {
                    playerParameter.wallCheck = "Right";
                }
            }
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Platform"))
        {
            playerParameter.wallCheck = "Air";
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Enemy") && !playerHealth.invincibleToEnemy)
        {
            enemyFSM = other.GetComponent<EnemyFSM>();
            enemyParameter = enemyFSM.parameter;
            if(transform.position.x > other.transform.position.x)
                playerParameter.moveDir = 1;
            else
                playerParameter.moveDir = -1;
            if(enemyParameter.damage != 0)
            {
                StartCoroutine(playerHealth.InvincibleToEnemyTime(enemyParameter.strength * 2));
                playerHealth.DamageToPlayer(enemyParameter.damage);
                StartCoroutine(GetHit(enemyParameter.strength));
                TransitionState(PStateType.GetHit);
            }
        }
        else if(other.gameObject.CompareTag("Background"))
        {
            playerParameter.wallCheck = "Background";
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Background"))
            playerParameter.wallCheck = "Air";
    }

    IEnumerator GetHit(float power)
    {   
        playerParameter.knockBackSpeed = power * 5;
        yield return new WaitForSeconds(power);
        TransitionState(PStateType.Fall);
    }
}
