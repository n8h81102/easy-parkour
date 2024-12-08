using System.Runtime.CompilerServices;
using System.Threading;
using Mono.Cecil;
using Unity.VisualScripting;
using UnityEditor.Scripting;
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
        Debug.Log("MoeskIdle" + time);
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
    private MoeskFSM manager;
    private Parameter parameter;
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
        else if(fireTime > 0 && parameter.bullet > 0)
        {
            parameter.bullet--;
            manager.Shoot0();
            fireTime--;
            time = Random.Range(0.2f, 0.5f);
        }
        else if(parameter.bullet <= 0)
        {
            manager.TransitionState(MoeskState.Reload);
        }
        else
            manager.TransitionState(MoeskState.Idle);

    }

    public void OnExit()
    {
        
    }
}

public class MoeskReload : IState
{
    private float time;
    private MoeskFSM manager;
    private Parameter parameter;

    public MoeskReload(MoeskFSM manager)
    {
        this.manager = manager;
        parameter = manager.parameter;
    }
    public void OnEnter()
    {
        parameter.target = GameObject.FindGameObjectWithTag("Player").transform;
        Debug.Log("MoeskReload");
        time = 3;
        parameter.bullet = 6;
    }
    public void OnUpdate()
    {
        if(time >= 0)
            time -= Time.deltaTime;
        else if(Vector3.Distance(parameter.target.position, manager.transform.position) <= parameter.escapeDis) //To decide whether Moesk should jump to other side 
            manager.TransitionState(MoeskState.Escape);
        else
            manager.TransitionState(MoeskState.Idle);
    }
    public void OnExit()
    {

    }
}

public class MoeskEscape : IState
{   
    private float time;
    private Vector2 target; //Moesk would teleport to random place
    private MoeskFSM manager;
    private Parameter parameter;
    public MoeskEscape(MoeskFSM manager)
    {
        this.manager = manager;
        parameter = manager.parameter;
    }
    public void OnEnter()
    {
        Debug.Log("MoeskEscape");
        time = 1.5f;
        if(manager.transform.position.x >= 23)
            target = new Vector2(Random.Range(18, 13), -4);
        else
            target = new Vector2(Random.Range(28, 33), -4);

    }
    public void OnUpdate()
    {
        if(time >= 0)
            time -= Time.deltaTime;
        else
        {
            manager.transform.position = target;
            manager.TransitionState(MoeskState.Idle);
        }
            
    }
    public void OnExit()
    {

    }
}

public class MoeskCopy : IState //If we want to add new states we can just copy this so we don't have to do the same shit lol
{
    private MoeskFSM manager;
    private Parameter parameter;
    public MoeskCopy(MoeskFSM manager)
    {
        this.manager = manager;
        parameter = manager.parameter;
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
