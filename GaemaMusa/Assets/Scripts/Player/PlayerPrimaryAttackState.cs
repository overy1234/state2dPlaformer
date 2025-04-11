using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{

    private int comboCounter;

    private float lastTimeAttacked;
    private float comboWindow = 0.5f;


    public PlayerPrimaryAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        xInput = 0;

        if (comboCounter > 2 || Time.time >= lastTimeAttacked + comboWindow)
            comboCounter = 0;

        player.anim.SetInteger("ComboCounter", comboCounter);
        //player.anim.speed = 3;

        
        float attackDir = player.facingDir;

        if (xInput != 0)
        {
           // Debug.Log(xInput);
            attackDir = xInput;
        }
           
     

        player.SetVelocity(player.attackMovement[comboCounter].x * attackDir, player.attackMovement[comboCounter].y);


        stateTimer = 0.1f;
    }

    public override void Exit()
    {
        base.Exit();


        player.StartCoroutine("BusyFor", 0.1f);
       // player.anim.speed = 1;

        comboCounter++;
        lastTimeAttacked = Time.time;

       
    }

    public override void Update()
    {
        base.Update();


        if (stateTimer < 0)
            player.SetZeroVelocity();

        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }
}
