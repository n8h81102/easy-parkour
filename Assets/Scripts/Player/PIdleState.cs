using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class PIdleState : IPlayerState
{
    private PlayerStateMachine playerStateMachine;
    private PlayerParameter playerParameter;

    public PIdleState(PlayerStateMachine playerStateMachine)
    {
        this.playerStateMachine = playerStateMachine;
        playerParameter = playerStateMachine.playerParameter;
    }
    public void OnEnter()
    {
        playerParameter.myAnim.Play("Idle");
        Debug.Log("Idle");
    }
    public void OnUpdate()
    {
        flip();
        playerParameter.myRigidBody2D.linearVelocity = new Vector2(0, 0);
        WallCheck();
        if(Input.GetKey(KeyCode.A) ^ Input.GetKey(KeyCode.D) && playerParameter.wallCheck == "Ground")
            playerStateMachine.TransitionState(PStateType.Run);
        if(Input.GetKeyDown(KeyCode.Space))
            playerStateMachine.TransitionState(PStateType.Jump);
        if(Input.GetKey(KeyCode.LeftControl) && playerParameter.dashCD <= 0)
            playerStateMachine.TransitionState(PStateType.Dash);
    }

    void flip()
    {
        if(playerParameter.moveDir != 0)
            playerStateMachine.transform.localScale = new Vector3(playerParameter.moveDir, 1, 1);
    }
    void WallCheck()
    {
        if(playerParameter.wallCheck == "Left" || playerParameter.wallCheck == "Right")
            playerStateMachine.TransitionState(PStateType.Climb);
        if(playerParameter.wallCheck == "Air")
            playerStateMachine.TransitionState(PStateType.Fall);
        if(playerParameter.wallCheck == "Background")
            playerStateMachine.TransitionState(PStateType.BackgroundClimb);
    }
    public void OnExit()
    {

    }
}

public class PRunState : IPlayerState
{
    public PlayerStateMachine playerStateMachine;
    public PlayerParameter playerParameter;

    public PRunState(PlayerStateMachine playerStateMachine)
    {
        this.playerStateMachine = playerStateMachine;
        playerParameter = playerStateMachine.playerParameter;
    }
    public void OnEnter()
    {
        Debug.Log("Run");
        playerParameter.myAnim.Play("Run");
    }

    public void OnUpdate()
    {
        flip();
        if(!(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)))
            playerStateMachine.TransitionState(PStateType.Idle);
        if(Input.GetKeyDown(KeyCode.Space))
            playerStateMachine.TransitionState(PStateType.Jump);
        if(Input.GetKeyDown(KeyCode.LeftControl) && playerParameter.dashCD <= 0)
            playerStateMachine.TransitionState(PStateType.Dash);
        move();
        WallCheck();
        
    }

    void move()
    {
        Vector2 playerVel = new Vector2(playerParameter.moveDir * playerParameter.runSpeed, playerParameter.myRigidBody2D.linearVelocity.y);
        playerParameter.myRigidBody2D.linearVelocity = playerVel;
    }
    void flip()
    {
        if(Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            playerStateMachine.transform.localScale = new Vector3(-1, 1, 1);
            playerParameter.moveDir = -1;
            playerParameter.dashDir = -1;
        }
        else if(Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            playerStateMachine.transform.localScale = new Vector3(1, 1, 1);
            playerParameter.moveDir = 1;
            playerParameter.dashDir = 1;
        }
        else
            playerStateMachine.TransitionState(PStateType.Idle);
    }

    void WallCheck()
    {
        if(playerParameter.wallCheck == "Left" || playerParameter.wallCheck == "Right")
            playerStateMachine.TransitionState(PStateType.Climb);
        if(playerParameter.wallCheck == "Background")
            playerStateMachine.TransitionState(PStateType.BackgroundClimb);
    }

    public void OnExit()
    {
        //playerParameter.myAnim.ResetTrigger("isRun");
    }
}

public class PDashState : IPlayerState
{
    private float timer;
    public PlayerStateMachine playerStateMachine;
    public PlayerParameter playerParameter;
    public Transform startPoint;

    public PDashState(PlayerStateMachine playerStateMachine)
    {
        this.playerStateMachine = playerStateMachine;
        playerParameter = playerStateMachine.playerParameter;
    }
    public void OnEnter()
    {
        playerParameter.myRigidBody2D.gravityScale = 0;
        Debug.Log("Dash");
        playerParameter.myAnim.Play("Dash");
        timer = 0;
        startPoint = GameObject.FindGameObjectWithTag("DashStartPoint").GetComponent<Transform>();
        startPoint.position = playerStateMachine.transform.position;
    }

    public void OnUpdate()
    {
        if(MathF.Abs(playerStateMachine.transform.position.x - startPoint.transform.position.x) >= playerParameter.dashSpeed * 0.2f)
        {
            if(playerParameter.wallCheck == "Background")
                playerStateMachine.TransitionState(PStateType.BackgroundClimb);
            else
                playerStateMachine.TransitionState(PStateType.Idle);   
        }
        move();
        if(timer > 0.1f)
            WallCheck();
        timer += Time.deltaTime;
    }

    void move()
    {
        playerParameter.myRigidBody2D.linearVelocity = new Vector2(playerParameter.dashSpeed * playerParameter.dashDir, 0);
        timer += Time.deltaTime;
    }

    void WallCheck()
    {
        if(playerParameter.wallCheck == "Left" || playerParameter.wallCheck == "Right")
        {
            playerStateMachine.TransitionState(PStateType.Climb);
        }
    }

    public void OnExit()
    {
        playerParameter.moveDir = playerParameter.dashDir;
        playerParameter.myRigidBody2D.linearVelocity = new Vector2(0,0);
        playerParameter.myRigidBody2D.gravityScale = playerParameter.normalGravity;
        playerParameter.dashCD = 1.5f;
        Debug.Log(MathF.Abs(playerStateMachine.transform.position.x - startPoint.transform.position.x));
    }
}

public class PJumpState : IPlayerState
{
    public PlayerStateMachine playerStateMachine;
    public PlayerParameter playerParameter;

    public PJumpState(PlayerStateMachine playerStateMachine)
    {
        this.playerStateMachine = playerStateMachine;
        playerParameter = playerStateMachine.playerParameter;
    }
    public void OnEnter()
    {
        
        Vector2 jumpVel = new Vector2(playerParameter.myRigidBody2D.linearVelocity.x, playerParameter.jumpSpeed);
        playerParameter.myRigidBody2D.linearVelocity = jumpVel;
        Debug.Log("Jump");
    }

    public void OnUpdate()
    {
        flip();
        move();
        if(Input.GetKeyUp(KeyCode.Space) || playerParameter.myRigidBody2D.linearVelocity.y < 0)
            playerStateMachine.TransitionState(PStateType.Fall);
        if(Input.GetKey(KeyCode.LeftControl) && playerParameter.dashCD <= 0)
            playerStateMachine.TransitionState(PStateType.Dash);
        WallCheck();   
    }
    void WallCheck()
    {
        if(playerParameter.wallCheck == "Left" || playerParameter.wallCheck == "Right")
            playerStateMachine.TransitionState(PStateType.Climb);
        if(playerParameter.wallCheck == "Background")
            playerStateMachine.TransitionState(PStateType.BackgroundClimb);
    }
    void move()
    {
        Vector2 playerVel = new Vector2(playerParameter.moveDir * playerParameter.runSpeed, playerParameter.myRigidBody2D.linearVelocity.y);
        playerParameter.myRigidBody2D.linearVelocity = playerVel;
    }
        void flip()
    {
        if(Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            playerStateMachine.transform.localScale = new Vector3(-1, 1, 1);
            playerParameter.moveDir = -1;
            playerParameter.dashDir = -1;
        }
        else if(Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            playerStateMachine.transform.localScale = new Vector3(1, 1, 1);
            playerParameter.moveDir = 1;
            playerParameter.dashDir = 1;
        }
        else
            playerParameter.moveDir = 0;
        /*if(Input.GetKeyDown(KeyCode.A) || (Input.GetKeyUp(KeyCode.D) && Input.GetKey(KeyCode.A)))優化版移動
        {
            playerStateMachine.transform.localScale = new Vector3(-1, 1, 1);
            playerParameter.moveDir = -1;
            playerParameter.dashDir = -1;
        }
        if(Input.GetKeyDown(KeyCode.D) || (Input.GetKeyUp(KeyCode.A) && Input.GetKey(KeyCode.D)))
        {
            playerStateMachine.transform.localScale = new Vector3(1, 1, 1);
            playerParameter.moveDir = 1;
            playerParameter.dashDir = 1;
        }
        if(!(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)))
            playerParameter.moveDir = 0;*/
    }

    public void OnExit()
    {
        
    }
}

public class PFallState : IPlayerState
{
    public PlayerStateMachine playerStateMachine;
    public PlayerParameter playerParameter;

    public PFallState(PlayerStateMachine playerStateMachine)
    {
        this.playerStateMachine = playerStateMachine;
        playerParameter = playerStateMachine.playerParameter;
    }
    public void OnEnter()
    {
        playerParameter.myRigidBody2D.gravityScale = playerParameter.normalGravity;
        Debug.Log("Fall");
        if(playerParameter.jumpSpeed >= 0)
            playerParameter.myRigidBody2D.linearVelocity = new Vector2(playerParameter.myRigidBody2D.linearVelocity.x, playerParameter.myRigidBody2D.linearVelocity.y / 2); 
    }

    public void OnUpdate()
    {
        flip();
        WallCheck();
        move();
        if(Input.GetKey(KeyCode.LeftControl) && playerParameter.dashCD <= 0)
            playerStateMachine.TransitionState(PStateType.Dash);
        if(Input.GetKeyDown(KeyCode.Space) && playerParameter.airJump > 0)
        {
            playerStateMachine.TransitionState(PStateType.Jump);
            playerParameter.airJump -= 1;
        }
    }
    void WallCheck()
    {
        switch(playerParameter.wallCheck)
        {
            case "Left" : case "Right" :
                playerStateMachine.TransitionState(PStateType.Climb);
                break;
            case "Ground" :
                playerStateMachine.TransitionState(PStateType.Idle);
                break;
            case "Background" :
                playerStateMachine.TransitionState(PStateType.BackgroundClimb);
                break;
        }
    }
    void move()
    {
        Vector2 playerVel = new Vector2(playerParameter.moveDir * playerParameter.runSpeed, playerParameter.myRigidBody2D.linearVelocity.y);
        playerParameter.myRigidBody2D.linearVelocity = playerVel;
    }
    void flip()
    {
        if(Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            playerStateMachine.transform.localScale = new Vector3(-1, 1, 1);
            playerParameter.moveDir = -1;
            playerParameter.dashDir = -1;
        }
        else if(Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            playerStateMachine.transform.localScale = new Vector3(1, 1, 1);
            playerParameter.moveDir = 1;
            playerParameter.dashDir = 1;
        }
        else 
            playerParameter.moveDir = 0;
    }
    public void OnExit()
    {
        
    }
}

public class PClimbState : IPlayerState
{
    private PlayerStateMachine playerStateMachine;
    private PlayerParameter playerParameter;

    public PClimbState(PlayerStateMachine playerStateMachine)
    {
        this.playerStateMachine = playerStateMachine;
        playerParameter = playerStateMachine.playerParameter;
    }
    public void OnEnter()
    {
        //playerParameter.myAnim.Play("");
        Debug.Log("Climb");
        playerParameter.myRigidBody2D.gravityScale = 0;
        playerParameter.airJump = 1;
    }

    public void OnUpdate()
    {
        Move();
        WallCheck();
        if(Input.GetKey(KeyCode.LeftControl) && playerParameter.dashCD <= 0)
            playerStateMachine.TransitionState(PStateType.Dash);
    }

    
    void Move()
    {
        if(playerParameter.wallCheck == "Left" && Input.GetKey(KeyCode.D))
            playerParameter.myRigidBody2D.linearVelocity = Vector2.right * playerParameter.runSpeed;
        else if(playerParameter.wallCheck == "Right" && Input.GetKey(KeyCode.A))
            playerParameter.myRigidBody2D.linearVelocity = Vector2.left * playerParameter.runSpeed;
        else if(Input.GetKey(KeyCode.W))
            playerParameter.myRigidBody2D.linearVelocity = Vector2.up * 5;
        else if(Input.GetKey(KeyCode.S))
            playerParameter.myRigidBody2D.linearVelocity = Vector2.down * 5;
        else 
            playerParameter.myRigidBody2D.linearVelocity = Vector2.down * 0;

    }
    void WallCheck()
    {
        switch(playerParameter.wallCheck)
        {
            case "Air":
                playerStateMachine.TransitionState(PStateType.Fall);
                break;
            case "Ground":
                playerStateMachine.TransitionState(PStateType.Idle);
                break;
            case "Left":
                playerStateMachine.transform.localScale = new Vector3(-1, 1, 1);
                playerParameter.dashDir = 1;
                break;
            case "Right":
                playerStateMachine.transform.localScale = new Vector3(1, 1, 1);
                playerParameter.dashDir = -1;
                break;
        }

    }
    public void OnExit()
    {
        playerParameter.myRigidBody2D.gravityScale = playerParameter.normalGravity;
    }
}

public class PBackgroundClimbState : IPlayerState
{
    private PlayerStateMachine playerStateMachine;
    private PlayerParameter playerParameter;

    public PBackgroundClimbState(PlayerStateMachine playerStateMachine)
    {
        this.playerStateMachine = playerStateMachine;
        playerParameter = playerStateMachine.playerParameter;
    }
    public void OnEnter()
    {
        //playerParameter.myAnim.Play("");
        Debug.Log("Background");
        playerParameter.myRigidBody2D.gravityScale = 0;
        playerParameter.myRigidBody2D.linearVelocity = new Vector2(0, 0);
    }

    public void OnUpdate()
    {
        flip();
        MoveVertical();
        MoveHorizontal();
        WallCheck();
        if(Input.GetKey(KeyCode.LeftControl) && playerParameter.dashCD <= 0)
            playerStateMachine.TransitionState(PStateType.Dash);
    }

    void flip()
    {
        if(Input.GetKey(KeyCode.A))
        {
            playerStateMachine.transform.localScale = new Vector3(-1, 1, 1);
            playerParameter.moveDir = -1;
            playerParameter.dashDir = -1;
        }
        else if(Input.GetKey(KeyCode.D))
        {
            playerStateMachine.transform.localScale = new Vector3(1, 1, 1);
            playerParameter.moveDir = 1;
            playerParameter.dashDir = 1;
        }
        else 
            playerParameter.moveDir = 0;
    }
    void MoveHorizontal()
    {
        playerParameter.myRigidBody2D.linearVelocity = new Vector2(playerParameter.runSpeed * playerParameter.moveDir, playerParameter.myRigidBody2D.linearVelocity.y);
    }
    void MoveVertical()
    {
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space))
            playerParameter.myRigidBody2D.linearVelocity = new Vector2(playerParameter.myRigidBody2D.linearVelocity.x, 5);
        else if(Input.GetKey(KeyCode.S))
            playerParameter.myRigidBody2D.linearVelocity = new Vector2(playerParameter.myRigidBody2D.linearVelocity.x, -5);
        else 
            playerParameter.myRigidBody2D.linearVelocity = new Vector2(playerParameter.myRigidBody2D.linearVelocity.x, 0);
    }
    void WallCheck()
    {
        switch(playerParameter.wallCheck)
        {
            case "Air":
                playerStateMachine.TransitionState(PStateType.Fall);
                break;
            case "Ground":
                playerStateMachine.TransitionState(PStateType.Idle);
                break;
            case "Left": case "Right":
                playerStateMachine.TransitionState(PStateType.Climb);
                break;
        }

    }
    public void OnExit()
    {
        playerParameter.myRigidBody2D.gravityScale = playerParameter.normalGravity;
    }
}

public class PGetHitState : IPlayerState
{
    private PlayerStateMachine playerStateMachine;
    private PlayerParameter playerParameter;
    private Parameter parameter;
    public PGetHitState(PlayerStateMachine playerStateMachine)
    {
        this.playerStateMachine = playerStateMachine;
        playerParameter = playerStateMachine.playerParameter;
    }
    public void OnEnter()
    {
        playerParameter.myRigidBody2D.gravityScale = playerParameter.normalGravity;
        playerParameter.myRigidBody2D.linearVelocity = Vector2.up * 15;
        Debug.Log("GetHit");
    }
    public void OnUpdate()
    {
        flip();
        playerParameter.myRigidBody2D.linearVelocity = new Vector2(playerParameter.knockBackSpeed * playerParameter.moveDir, playerParameter.myRigidBody2D.linearVelocity.y);
    }

    void flip()
    {
        playerStateMachine.transform.localScale = new Vector3(playerParameter.moveDir, 1, 1);
    }
    public void OnExit()
    {
        
    }


}

