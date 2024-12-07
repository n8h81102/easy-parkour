using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public enum MoeskState
{
    Idle, Shoot1, Shoot2, Smoke0, Smoke1, Escape
}
public class MoeskFSM : MonoBehaviour
{
    public Parameter parameter;
    public IState currentState;
    [SerializeField] private GameObject bullet0;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private GameObject target;
    private Dictionary<MoeskState, IState> states = new Dictionary<MoeskState, IState>();
    
    void Start()
    {
        states.Add(MoeskState.Idle, new MoeskIdle(this));
        states.Add(MoeskState.Shoot1, new MoeskShoot1(this));

        TransitionState(MoeskState.Idle);
    }

    void Update()
    {
        currentState.OnUpdate();
    }

    public void TransitionState(MoeskState type)
    {
        if(currentState != null)
            currentState.OnExit();
        currentState = states[type];
        currentState.OnEnter();
    }
    public void Shoot0()
    {
        
        MoeskFire moeskfire = Instantiate(bullet0, transform.position, Quaternion.identity).GetComponent<MoeskFire>();
        moeskfire.InitializedBullet(target.transform, bulletSpeed);
    }
}
