using UnityEngine;

public class Enemy : Entity
{
    [Header("이동 정보")]
    public float moveSpeed;
    public float idleTime;


    public EnemyStateMachine stateMachine { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        stateMachine = new EnemyStateMachine();
    }


    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();
    }



}
