using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Sword_Skill_Controller : MonoBehaviour
{
     
    private Animator anim;                         // 애니메이터 컴포넌트
    private Rigidbody2D rb;                        // 리지드바디 컴포넌트
    private CircleCollider2D cd;                   // 원형 콜라이더 컴포넌트
    private Player player;                         // 플레이어 참조


    private bool canRotate = true;                 // 회전 가능 여부
    private bool isReturning;                      // 귀환 중인지 여부


    private float freezeTimeDuration;              // 시간 정지 지속 시간
    private float returnSpeed = 12;                // 귀환 속도

    [Header("관통 정보")]
    [SerializeField] private float pierceAmount;   // 관통 횟수



    [Header("바운스 정보")]
    private float bounceSpeed;                     // 바운스 속도
    private bool isBouncing;                       // 바운스 중인지 여부
    private int bounceAmount;                      // 바운스 횟수
    private List<Transform> enemyTarget;           // 적 타겟 목록
    private int targetIndex;                       // 현재 타겟 인덱스


    [Header("회전 정보")]
    private float maxTravelDistance;               // 최대 이동 거리
    private float spinDuration;                    // 회전 지속 시간
    private float spinTimer;                       // 회전 타이머
    private bool wasStopped;                       // 멈춤 여부
    private bool isSpinning;                       // 회전 중인지 여부

    private float hitTimer;                        // 타격 타이머
    private float hitCooldown;                     // 타격 쿨다운

    private float spinDirection;                   // 회전 방향


    private void Awake()
    {
        // 컴포넌트 초기화
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CircleCollider2D>();
    }

    // 자기 자신 파괴 함수
    private void DestroyMe()
    {
        Destroy(gameObject);
    }


    // 검 설정 함수
    public void SetupSword(Vector2 _dir, float _gravityScale, Player _player, float _freezeTimeDuration, float _returnSpeed)
    {
        player = _player;
        freezeTimeDuration = _freezeTimeDuration;
        returnSpeed = _returnSpeed;


        // 리지드바디 속도 및 중력 설정
        rb.linearVelocity = _dir;
        rb.gravityScale = _gravityScale;

        // 관통량이 0 이하일 때만 회전 애니메이션 활성화
        if(pierceAmount <= 0)
        anim.SetBool("Rotation", true);

        // 회전 방향 설정 (-1 또는 1)
        spinDirection = Mathf.Clamp(rb.linearVelocity.x, -1, 1);

        // 7초 후 자동 파괴
        Invoke("DestroyMe", 7);
    }

    // 바운스 설정 함수
    public void SetupBounce(bool _isBouncing, int _amountOfBounces, float _bounceSpeed)
    {
        isBouncing = _isBouncing;
        bounceAmount = _amountOfBounces;
        bounceSpeed = _bounceSpeed;

        enemyTarget = new List<Transform>();
    }


    // 관통 설정 함수
    public void SetupPierce(int _pierceAmount)
    {
        pierceAmount = _pierceAmount;
    }

    // 회전 설정 함수
    public void SetupSpin(bool _isSpinning, float _maxTravelDistance, float _spinDuration, float _hitCooldown)
    {
        isSpinning = _isSpinning;
        maxTravelDistance = _maxTravelDistance;
        spinDuration = _spinDuration;
        hitCooldown = _hitCooldown;
    }



    // 검 귀환 함수
    public void ReturnSword()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        transform.parent = null;
        isReturning = true;
    }


    private void Update()
    {
        // 회전 가능하면 검이 진행 방향을 바라보도록 설정
        if (canRotate)
            transform.right = rb.linearVelocity;

        // 귀환 중이면 플레이어 위치로 이동
        if (isReturning)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, returnSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, player.transform.position) < 1)
                player.ClearTheSword();
        }

        // 바운스 로직 실행
        BounceLogic();

        // 회전 로직 실행
        SpinLogic();
    }

    // 회전 로직 함수
    private void SpinLogic()
    {
        if (isSpinning)
        {
            // 최대 이동거리 도달 시 멈춤
            if (Vector2.Distance(player.transform.position, transform.position) > maxTravelDistance && !wasStopped)
            {
                StopWhenSpinning();
            }

            if (wasStopped)
            {
                spinTimer -= Time.deltaTime;

                // 회전 중 위치 이동
                transform.position = Vector2.MoveTowards(transform.position,
                    new Vector2(transform.position.x + spinDirection, transform.position.y), 1.5f * Time.deltaTime);

                // 타이머 종료 시 귀환
                if (spinTimer < 0)
                {
                    isReturning = true;
                    isSpinning = false;
                }

                hitTimer -= Time.deltaTime;

                // 주변 적 타격
                if (hitTimer < 0)
                {
                    hitTimer = hitCooldown;

                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);

                    foreach (var hit in colliders)
                    {
                        if (hit.GetComponent<Enemy>() != null)
                            SwordSkillDamage(hit.GetComponent<Enemy>());

                    }
                }
            }
        }
    }

    // 회전 시 멈춤 함수
    private void StopWhenSpinning()
    {
        wasStopped = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        spinTimer = spinDuration;
    }

    // 바운스 로직 함수
    private void BounceLogic()
    {
        if (isBouncing && enemyTarget.Count > 0)
        {
            // 현재 타겟을 향해 이동
            transform.position = Vector2.MoveTowards(transform.position, enemyTarget[targetIndex].position, bounceSpeed * Time.deltaTime);

            // 타겟에 도달하면 데미지 처리 및 다음 타겟으로 이동
            if (Vector2.Distance(transform.position, enemyTarget[targetIndex].position) < 0.1f)
            {
                SwordSkillDamage(enemyTarget[targetIndex].GetComponent<Enemy>());
               
                targetIndex++;
                bounceAmount--;

                // 바운스 횟수 소진 시 귀환
                if (bounceAmount <= 0)
                {
                    isBouncing = false;
                    isReturning = true;
                }

                // 인덱스가 범위를 넘어가면 초기화
                if (targetIndex >= enemyTarget.Count)
                    targetIndex = 0;
            }

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 귀환 중에는 충돌 처리 무시
        if (isReturning)
            return;

        // 적과 충돌 시 데미지 처리
        if(collision.GetComponent<Enemy>() !=null)
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            SwordSkillDamage(enemy);

        }

        // 바운스를 위한 타겟 설정
        SetupTargetsForBounce(collision);

        // 물체에 박히기
        StuckInto(collision);
    }

    // 검 스킬 데미지 함수
    private void SwordSkillDamage(Enemy enemy)
    {
        enemy.DamageEffect();
        enemy.StartCoroutine("FreezeTimerFor", freezeTimeDuration);
    }

    // 바운스를 위한 타겟 설정 함수
    private void SetupTargetsForBounce(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            if (isBouncing && enemyTarget.Count <= 0)
            {
                // 반경 10 내의 모든 적 탐색
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);

                foreach (var hit in colliders)
                {
                    if (hit.GetComponent<Enemy>() != null)
                        enemyTarget.Add(hit.transform);
                }
            }
        }
    }

    // 물체에 박히는 함수
    private void StuckInto(Collider2D collision)
    {
        // 관통 처리
        if (pierceAmount > 0 && collision.GetComponent<Enemy>() != null)
        {
            pierceAmount--;
            return;
        }

        // 회전 검일 경우 멈춤 처리
        if (isSpinning)
        {
            StopWhenSpinning();
            return;
        }  

        // 검 회전 및 움직임 정지
        canRotate = false;
        cd.enabled = false;

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        // 바운스 중이면 처리 스킵
        if (isBouncing && enemyTarget.Count > 0)
            return;

        // 회전 애니메이션 중지 및 부모 설정
        anim.SetBool("Rotation", false);
        transform.parent = collision.transform;
    }
}
