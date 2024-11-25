using System.Runtime.CompilerServices;
using System.Diagnostics.Contracts;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;

using UnityEngine.UIElements;
using Unity.Mathematics;
using System;

public class PlayerMove : MonoBehaviour
{   
    private static int normalGravity = 5;
    public string PState;
    public float jumpSpeed;
    public float moveSpeed;
    private float wallJumpDir; //蹬牆跳方向，右牆跳為-1，左牆跳為1
    private float  moveDir;
    public float dashSpeed;
    public int dashDir;
    public float dashCD = 0.5f;
    private bool canDash = true;
    public float wallJumpFlex = 0.2f;
    public char isWall = 'N';
    public int canJump;
    private int knockBackDir;
    private Rigidbody2D myRigidbody;
    private BoxCollider2D myBoxCollider2D;
    private Animator myAnimator;
    public Transform respawnPoint;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myBoxCollider2D = GetComponent<BoxCollider2D>();
        myAnimator = GetComponent<Animator>();
        PState = "Run";
        dashSpeed = 25f;
        dashDir = 0;
        knockBackDir = 0;
        isWall = 'N';
    }

    void Update()
    {
        Flip();
        Move();
        Jump();
        MaxFallSpeed();
    }

    void Flip()
    {   
        if(moveDir == 1)
            transform.localScale = new Vector3(1, 1, 1);
        else if(moveDir == -1)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    void Move() //判定移動方向與速度
    {
        if(Input.GetKeyDown(KeyCode.A) || (Input.GetKeyUp(KeyCode.D) && Input.GetKey(KeyCode.A)))
            moveDir = -1;
        if(Input.GetKeyDown(KeyCode.D) || (Input.GetKeyUp(KeyCode.A) && Input.GetKey(KeyCode.D)))
            moveDir = 1;
        if(!(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)))
            moveDir = 0;

        if(PState == "Dash")// 衝刺
        {
            myRigidbody.gravityScale = 0;
            if(isWall != 'N')
                PState = "Climb";
            Vector2 playerVel = new Vector2(dashDir * dashSpeed, 0);
            myRigidbody.linearVelocity = playerVel;
        }

        if(PState == "Run" || PState == "Jump" || PState == "ClimbBackground")//地面和跳躍中移動
        {
            Vector2 playerVel = new Vector2(moveDir * moveSpeed, myRigidbody.linearVelocity.y);
            myRigidbody.linearVelocity = playerVel;
            bool isRun = Mathf.Abs(myRigidbody.linearVelocity.x) > Mathf.Epsilon;
            myAnimator.SetBool("isRun", isRun);
        }

        if(PState == "ClimbBackground")
        {
            if(Input.GetKey(KeyCode.W))
                myRigidbody.linearVelocity = new Vector2(myRigidbody.linearVelocity.x * 0.5f, 5);
            else if(Input.GetKey(KeyCode.S))
                myRigidbody.linearVelocity = new Vector2(myRigidbody.linearVelocity.x * 0.5f, -5);
            else
                myRigidbody.linearVelocity = new Vector2(myRigidbody.linearVelocity.x * 0.5f, 0);
        }

        if(PState == "Climb")//爬牆
        {
            Vector2 playerVel = new Vector2(moveDir * moveSpeed, myRigidbody.linearVelocity.y);
            myRigidbody.linearVelocity = playerVel;
            if(Input.GetKey(KeyCode.W))
                myRigidbody.linearVelocity = Vector2.up * 5;
            else if(Input.GetKey(KeyCode.S))
                myRigidbody.linearVelocity = Vector2.down * 5;
            else
                myRigidbody.linearVelocity = Vector2.up * 0;
        }

        if(PState == "Run" || PState == "Jump" || PState == "WallJump" || PState == "ClimbBackground")
        {
            if(Input.GetKey(KeyCode.LeftControl) && canDash)
            {
                StartCoroutine(Dash());
            }
        }
    
        if(PState == "GetKnockBack")//受擊退
        {
            Vector2 playerVel = new Vector2(knockBackDir * 5, myRigidbody.linearVelocity.y);
            myRigidbody.linearVelocity = playerVel;
        }
        if(PState == "WallJump")//蹬牆跳
        {
            Vector2 playerVel = new Vector2(wallJumpDir * moveSpeed, myRigidbody.linearVelocity.y);
            myRigidbody.linearVelocity = playerVel;
        }
        if(PState == "ClimbBackground")//攀爬背景牆
            myRigidbody.gravityScale = 0;

    }
    IEnumerator Dash() //衝刺計時器協程
    {   
        if(transform.localScale == new Vector3(1,1,1))
            dashDir = 1;
        else
            dashDir = -1;
        PState = "Dash";
        myRigidbody.linearVelocity = Vector2.up * 0;
        canDash = false;
        yield return new WaitForSeconds(0.1f);
        if(isWall == 'N')
            myRigidbody.gravityScale = normalGravity;
        else
            myRigidbody.gravityScale = 0;
        if(PState == "Dash")
            PState = "Run";
        yield return new WaitForSeconds(dashCD);
        canDash = true;
    }
    public void KnockBack(int KnockBackDir)
    {
        knockBackDir = KnockBackDir;
        StartCoroutine(GetKnockBack());
    }
    public IEnumerator GetKnockBack() //擊退
    {
        myAnimator.SetTrigger("getHurt");
        myRigidbody.gravityScale = normalGravity;
        myRigidbody.linearVelocity = Vector2.up * 15;
        PState = "GetKnockBack";
        if(knockBackDir == 0)
            myBoxCollider2D.enabled = false;
        yield return new WaitForSeconds(0.5f);
        myAnimator.ResetTrigger("getHurt");
        if(knockBackDir == 0)
        {
            transform.position = respawnPoint.position;
            myBoxCollider2D.enabled = true;
            yield return new WaitForSeconds(1f);
            PState = "Run";
        }   
        else
        {
            PState = "Run";
        }
    }

    void Jump()
    {
        if(PState == "Run" || PState == "Jump")
        {
            if(Input.GetKeyDown(KeyCode.Space) && canJump > 0 && isWall == 'N') //能否一般跳躍或多段跳
            {
                Vector2 jumpVel = new Vector2(0.0f, jumpSpeed);
                myRigidbody.linearVelocity = Vector2.up * jumpVel;
                canJump -= 1;
                PState = "Jump";
            }
            if(Input.GetKeyUp(KeyCode.Space) && canJump >= 0 && myRigidbody.linearVelocity.y > 0 && PState == "Jump")
                myRigidbody.linearVelocity = myRigidbody.linearVelocity * 0.5f * Vector2.up;
        }
        else if(Input.GetKeyDown(KeyCode.Space) && PState == "Climb") //蹬牆跳
        {
            if(isWall == 'L')
                wallJumpDir = 1;
            else if(isWall == 'R')
                wallJumpDir = -1;
            StartCoroutine(WallJump());
        }
    }

    IEnumerator WallJump()
    {   
        transform.Translate(wallJumpDir * moveSpeed * Time.deltaTime, 0, 0);
        PState = "WallJump";
        myRigidbody.linearVelocity = Vector2.up * jumpSpeed;
        yield return new WaitForSeconds(wallJumpFlex);
        myRigidbody.gravityScale = normalGravity;
        if(PState != "Dash")
            wallJumpDir = 0;
        if(PState == "WallJump")
            PState = "Jump";
    }

    void MaxFallSpeed()
    {
        if(myRigidbody.linearVelocity.y < -30)
        {
            myRigidbody.gravityScale = 0;
            myRigidbody.linearVelocity = Vector2.down * 30;
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Ground") //判斷碰撞方向和爬牆
        {
            foreach (ContactPoint2D p in other.contacts) 
            {
                if (p.normal == new Vector2(1f, 0f)) 
                {
                    PState = "Climb";
                    isWall = 'L';
                    myRigidbody.gravityScale = 0;
                    myRigidbody.linearVelocity = Vector2.up * 0;
                    break;
                }
                else if (p.normal == new Vector2(-1f, 0f))
                {
                    PState = "Climb";
                    isWall = 'R';
                    myRigidbody.gravityScale = 0;
                    myRigidbody.linearVelocity = Vector2.up * 0;
                    break;
                }
                else if (p.normal != new Vector2(0f, -1f))
                {
                    if(PState != "GetKnockBack")
                        PState = "Run";
                    canJump = 2;
                }
            }
        }
    }
    void OnCollisionStay2D(Collision2D other)
    {
        if(other.gameObject.tag == "Ground") //判斷碰撞方向和爬牆
        {
            /*foreach (ContactPoint2D p in other.contacts) 
            {
                if (p.normal == new Vector2(1f, 0f)) 
                {
                    PState = "Climb";
                    isWall = 'L';
                    myRigidbody.gravityScale = 0;
                    myRigidbody.velocity = Vector2.up * 0;
                    break;
                }
                else if (p.normal == new Vector2(-1f, 0f))
                {
                    PState = "Climb";
                    isWall = 'R';
                    myRigidbody.gravityScale = 0;
                    myRigidbody.velocity = Vector2.up * 0;
                    break;
                }
                else if (p.normal != new Vector2(0f, -1f))
                {
                    if(PState != "GetKnockBack")
                        PState = "Run";
                    canJump = 2;
                }
            }*/
            if(other.contacts[0].normal == new Vector2(1f, 0f))
            {
                PState = "Climb";
                isWall = 'L';
                myRigidbody.gravityScale = 0;
                myRigidbody.linearVelocity = Vector2.up * 0;
            }
            else if(other.contacts[0].normal == new Vector2(-1f, 0f))
            {
                PState = "Climb";
                isWall = 'R';
                myRigidbody.gravityScale = 0;
                myRigidbody.linearVelocity = Vector2.up * 0;
            }
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if(other.gameObject.tag == "Ground")
        {
            myRigidbody.gravityScale = normalGravity;
            if(isWall == 'L' || isWall == 'R')
            {
                if(PState != "WallJump" && PState != "GetKnockBack")
                    PState = "Run";
                isWall = 'N';
            }
            if(isWall == 'N')
                StartCoroutine(leaveGround());
        }
    }
    IEnumerator leaveGround()
    {
        yield return new WaitForSeconds(0.1f);
        PState = "Jump";
        if(canJump > 1)
            canJump = 1;
    }
    

    void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.tag == "Background" && PState != "GetKnockBack" && PState != "ClimbBackground")
        {
            myRigidbody.linearVelocity = Vector2.up * 0;
            PState = "ClimbBackground";
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.tag == "Background")
        {
            myRigidbody.gravityScale = normalGravity;
            myRigidbody.linearVelocity = Vector2.up * 0;
            PState = "Jump";
        }
    }

}
