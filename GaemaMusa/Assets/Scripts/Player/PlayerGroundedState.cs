using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();



        if (player.IsEnemyDetected())
        {
            
            player.CheckForDashInput();
            if (player.IsEnemyDetected().distance < player.attackDistance)
            {

                stateMachine.ChangeState(player.primaryAttack);
            }
            else
            {
                stateMachine.ChangeState(player.moveState);
            }
        }
        else if (player.IsEnemyDetectedLeft())
        {
            player.CheckForDashInput();
            if (player.IsEnemyDetectedLeft().distance < player.attackDistance)
            {

                stateMachine.ChangeState(player.primaryAttack);
            }
            else
            {
                stateMachine.ChangeState(player.moveState);
            }
        }
       






        if (Input.GetKeyDown(KeyCode.R))
            stateMachine.ChangeState(player.blackHole);

        if (Input.GetKeyDown(KeyCode.Mouse1) && HasNoSword())
            stateMachine.ChangeState(player.aimSowrd);

        if (Input.GetKeyDown(KeyCode.Q))
            stateMachine.ChangeState(player.counterAttack);

        if (Input.GetKeyDown(KeyCode.Mouse0))
            stateMachine.ChangeState(player.primaryAttack);

        if (!player.IsGroundDetected())
            stateMachine.ChangeState(player.airState);

        if (Input.GetKeyDown(KeyCode.Space) && player.IsGroundDetected())
            stateMachine.ChangeState(player.jumpState);
    }

    private bool HasNoSword()
    {
        if (!player.sword)
        {
            return true;
        }

        player.sword.GetComponent<Sword_Skill_Controller>().ReturnSword();
        return false;
    }
}
