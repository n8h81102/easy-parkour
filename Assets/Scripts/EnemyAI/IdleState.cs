using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    private float timer;//狀態機計時
    private EnemyFSM manager;
    private Parameter parameter;

    public IdleState(EnemyFSM manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;

    }
    public void OnEnter()
    {
        //parameter.animator.Play("Idle");
    }

    public void OnUpdate()
    {
        timer += Time.deltaTime;

        if(parameter.target != null &&
         manager.transform.position.x > parameter.chaseArea[0].position.x && 
         manager.transform.position.x < parameter.chaseArea[1].position.x)
        {
            manager.TransitionState(StateType.Chase);
        }
        if(timer >= parameter.idleTime)
        {
            manager.TransitionState(StateType.Wander);
        }
    }

    public void OnExit()
    {
        timer = 0;
    }
}

public class WanderState : IState
{
    private int wanderPosition = 0;
    private EnemyFSM manager;
    private Parameter parameter;

    public WanderState(EnemyFSM manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }
    public void OnEnter()
    {
        //parameter.animator.Play("Walk");
    }

    public void OnUpdate()
    {
        manager.FlipTo(parameter.wanderPoint[wanderPosition]);
        manager.transform.position = Vector2.MoveTowards(manager.transform.position,
         parameter.wanderPoint[wanderPosition].position, parameter.moveSpeed * Time.deltaTime);
        
        if(parameter.target != null &&
         manager.transform.position.x > parameter.chaseArea[0].position.x && 
         manager.transform.position.x < parameter.chaseArea[1].position.x)
        {
            manager.TransitionState(StateType.Chase);
        }
        if(Vector2.Distance(manager.transform.position, parameter.wanderPoint[wanderPosition].position) <= 0.1f)
        {
            manager.TransitionState(StateType.Idle);
        }
    }

    public void OnExit()
    {
        wanderPosition++;
        if(wanderPosition >= parameter.wanderPoint.Length)
        {
            wanderPosition = 0;
        }
    }
}

public class ReactState : IState
{
    private EnemyFSM manager;
    private Parameter parameter;

    public ReactState(EnemyFSM manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;

    }
    public void OnEnter()
    {

    }

    public void OnUpdate()
    {

    }

    public void OnExit()
    {

    }
}

public class ChaseState : IState
{
    private EnemyFSM manager;
    private Parameter parameter;

    public ChaseState(EnemyFSM manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;

    }
    public void OnEnter()
    {
        //parameter.animator.Play("Chase");
    }

    public void OnUpdate()
    {
        manager.FlipTo(parameter.target);
        if(parameter.target)
        {
            manager.transform.position = new Vector2(Mathf.MoveTowards(manager.transform.position.x, parameter.target.position.x, parameter.chaseSpeed * Time.deltaTime)
             , manager.transform.position.y);
        }
        if(parameter.target == null ||
         manager.transform.position.x < parameter.chaseArea[0].position.x || 
         manager.transform.position.x > parameter.chaseArea[1].position.x)
        {
            manager.TransitionState(StateType.Idle);
        }
    }

    public void OnExit()
    {

    }
}

public class AttackState : IState
{
    private EnemyFSM manager;
    private Parameter parameter;

    public AttackState(EnemyFSM manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;

    }
    public void OnEnter()
    {

    }

    public void OnUpdate()
    {

    }

    public void OnExit()
    {

    }
}