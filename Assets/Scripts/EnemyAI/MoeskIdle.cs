using System.Runtime.CompilerServices;
using System.Threading;
using Mono.Cecil;
using UnityEngine;

public class MoeskIdle : IState
{

    private float time;
    private int newState;
    private MoeskFSM manager;
    private Parameter parameter;
    public MoeskIdle(MoeskFSM manager)
    {   
        this.manager = manager;
        parameter = manager.parameter;
    }
    public void OnEnter()
    {
        time = Random.Range(0.5f, 1f);
    }

    public void OnUpdate()
    {
        time -= Time.deltaTime;
        if(time <= 0)
        {
            manager.TransitionState(MoeskState.Shoot1);
        }
    }

    public void OnExit()
    {

    }
}

public class MoeskShoot1 : IState
{
    private MoeskFire moeskFire;
    private MoeskFSM manager;
    private Parameter parameter;
    public GameObject bullet0;
    private float time;
    private int fireTime;
    public MoeskShoot1(MoeskFSM manager)
    {   
        this.manager = manager;
        parameter = manager.parameter;
    }
    public void OnEnter()
    {
        fireTime = 3;
        time = Random.Range(0.1f, 0.5f);
    }

    public void OnUpdate()
    {
        if(time > 0)
            time -= Time.deltaTime;
        else if(fireTime > 0)
        {
            manager.Shoot0();
            fireTime--;
            time = Random.Range(0.1f, 0.5f);
        }
        else
            manager.TransitionState(MoeskState.Idle);

    }

    public void OnExit()
    {
        
    } 
}
