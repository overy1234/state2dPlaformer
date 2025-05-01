using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    GameObject enemy;
    public PlayerMoveState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        enemy = GameObject.FindGameObjectWithTag("Enemy");
    }

   

    public override void Update()
    {
        base.Update();

        if (enemy != null)
        {
            if (player.transform.position.x < enemy.transform.position.x)
            {
                xInput = 1;
            }
            else if (player.transform.position.x > enemy.transform.position.x)
            {

                xInput = -1;
            }

        }






        player.SetVelocity(xInput*player.moveSpeed, rb.linearVelocityY);

        if (xInput == 0 || player.IsWallDetected())
            stateMachine.ChangeState(player.idleState);



    }


    public override void Exit()
    {
        base.Exit();
    }
}
