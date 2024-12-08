using System.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public enum StateType
{
    Idle, Wander, React, Chase, Attack
}

[Serializable] 
public class Parameter
{
    public int health;
    public int damage;
    public int strength;
    public float moveSpeed;
    public float chaseSpeed;
    public float recoveryTime;
    public float idleTime;
    public int bullet;
    public float escapeDis;
    public Transform[] wanderPoint;
    public Transform[] chaseArea;
    public Transform target;
    public Animator animator;
    public int cnt;
}

public class EnemyFSM : MonoBehaviour
{   
    public Parameter parameter;
    private PlayerMove PlayerMove;
    private PlayerHealth PlayerHealth;
    private IState currentState;
    private PlayerStateMachine playerStateMachine;
    private PlayerParameter playerParameter;
    private PGetHitState pGetHitState;
    private Dictionary<StateType, IState> states = new Dictionary<StateType, IState>();
    void Start()
    {
        states.Add(StateType.Idle, new IdleState(this));
        states.Add(StateType.Wander, new WanderState(this));
        states.Add(StateType.React, new ReactState(this));
        states.Add(StateType.Chase, new ChaseState(this));
        states.Add(StateType.Attack, new AttackState(this));

        TransitionState(StateType.Idle);
        
        PlayerMove = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMove>();
        PlayerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        playerStateMachine = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateMachine>();
        playerParameter = playerStateMachine.playerParameter; 

        parameter.animator = GetComponent<Animator>();
    }

    void Update()
    {
        currentState.OnUpdate();
        parameter.recoveryTime -= Time.deltaTime;
    }

    public void TransitionState(StateType type)
    {
        if(currentState != null)
            currentState.OnExit();
        currentState = states[type];
        currentState.OnEnter();
    }

    public void FlipTo(Transform target)
    {
        if(target != null)
        {
            if(target.position.x > transform.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }
    

    /*void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player" && !PlayerHealth.invincibleToEnemy)
        {
            if(parameter.damage != 0)
            {
                StartCoroutine(PlayerHealth.InvincibleToEnemyTime(parameter.strength * 2));
                PlayerHealth.DamageToPlayer(parameter.damage);
            }
            if(transform.position.x > other.transform.position.x)
                PlayerMove.KnockBack(-parameter.strength);
            else
                PlayerMove.KnockBack(parameter.strength);
            parameter.recoveryTime = 2;
            parameter.target = null;
        }
    }*/
}
