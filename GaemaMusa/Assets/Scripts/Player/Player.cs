using System.Collections;
using UnityEngine;


public class Player : Entity
{

    [Header("���� ������")]
    public Vector2[] attackMovement;


    public bool isBusy { get; private set; }
    [Header("�̵� ����")]
    public float moveSpeed = 12f;
    public float jumpForce;

    [Header("��� ����")]
    [SerializeField] private float dashCooldown;
    private float dashUsageTimer;
    public float dashSpeed;
    public float dashDuration;
    public float dashDir { get; private set; }

  



    #region States
    // �÷��̾��� ���¸� �����ϴ� ���� �ӽ�
    public PlayerStateMachine stateMachine { get; private set; }

    // �÷��̾��� ���� (��� ����, �̵� ����)
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerDashState dashState { get; private set; }

    public PlayerWallSlideState wallSlide { get;  private set;}
    public PlayerWallJumpState wallJump { get; private set; }
    public PlayerPrimaryAttackState primaryAttack { get; private set; }

    #endregion
    

    protected override void Awake()
    {
        base.Awake();

        // ���� �ӽ� �ν��Ͻ� ����
        stateMachine = new PlayerStateMachine();

        // �� ���� �ν��Ͻ� ���� (this: �÷��̾� ��ü, stateMachine: ���� �ӽ�, "Idle"/"Move": ���� �̸�)
        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState  = new PlayerAirState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallSlide = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJump = new PlayerWallJumpState(this, stateMachine, "Jump");

        primaryAttack = new PlayerPrimaryAttackState(this, stateMachine, "Attack");

    }

    protected override void Start()
    {

        base.Start();

        // ���� ���� �� �ʱ� ���¸� ��� ����(idleState)�� ����
        stateMachine.Initialize(idleState);


      

    }




    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
        CheckForDashInput();
    }


    public IEnumerator BusyFor(float _seconds)
    {
        isBusy = true;

       
        yield return new WaitForSeconds(_seconds);
      
        isBusy = false;
    }






    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();

    private void CheckForDashInput()
    {

        //if (IsWallDetected())
        //    return;

        dashUsageTimer -= Time.deltaTime;



        if (Input.GetKeyDown(KeyCode.LeftShift) && dashUsageTimer<0)
        {

            dashUsageTimer = dashCooldown;


            dashDir = Input.GetAxisRaw("Horizontal");

            if (dashDir == 0)
                dashDir = facingDir;

            stateMachine.ChangeState(dashState);
        }
           
    }



   

  

}
