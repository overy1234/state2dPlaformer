using UnityEngine;

public class Crystal_Skill_Controller : MonoBehaviour
{
    // 컴포넌트 참조를 위한 프로퍼티
    private Animator anim => GetComponent<Animator>();
    private CircleCollider2D cd => GetComponent<CircleCollider2D>();

    // 크리스탈 관련 변수
    private float crystalExistTimer;              // 크리스탈 존재 시간

    // 폭발 및 이동 관련 변수
    private bool canExplode;                      // 폭발 가능 여부
    private bool canMove;                         // 이동 가능 여부
    private float moveSpeed;                      // 이동 속도

    // 성장 관련 변수
    private bool canGrow;                         // 성장 가능 여부
    [SerializeField] private float growSpeed;     // 성장 속도

    // 적 타겟 관련 변수
    private Transform closestTarget;              // 가장 가까운 적 타겟
    [SerializeField] private LayerMask whatIsEnemy; // 적 레이어 마스크

    // 크리스탈 설정 함수
    public void SetupCrystal(float _crystalDuration, bool _canExplode, bool _canMove, float _moveSpeed, Transform _closestTarget)
    {
        crystalExistTimer = _crystalDuration;     // 존재 시간 설정
        canExplode = _canExplode;                 // 폭발 가능 여부 설정
        canMove = _canMove;                       // 이동 가능 여부 설정
        moveSpeed = _moveSpeed;                   // 이동 속도 설정
        closestTarget = _closestTarget;           // 타겟 설정
    }

    // 무작위 적 선택 함수
    public void ChooseRandomEnemy()
    {
        // 블랙홀 반경 내의 모든 적 검색
        float radius = SkillManager.instance.blackhole.GetBlackholeRadius();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, whatIsEnemy);

        // 적이 있으면 무작위로 하나 선택
        if(colliders.Length > 0)
            closestTarget = colliders[Random.Range(0, colliders.Length)].transform;
    }

    // 매 프레임 업데이트
    private void Update()
    {
        // 크리스탈 존재 시간 감소
        crystalExistTimer -= Time.deltaTime;

        // 시간이 다 되면 크리스탈 종료
        if (crystalExistTimer < 0)
        {
            FinishCrystal();
        }

        // 이동 가능하면 타겟을 향해 이동
        if(canMove)
        {
            transform.position = Vector2.MoveTowards(transform.position, closestTarget.position, moveSpeed * Time.deltaTime);

            // 타겟에 접근하면 크리스탈 종료
            if(Vector2.Distance(transform.position, closestTarget.position) < 1)
            {
                FinishCrystal();
                canMove = false;
            }
        }

        // 성장 가능하면 크기 확대
        if (canGrow)
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(3, 3), growSpeed * Time.deltaTime);
    }

    // 애니메이션 폭발 이벤트 함수
    private void AnimationExplodeEvent()
    {
        // 주변의 모든 콜라이더 검색
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, cd.radius);

        // 적에게 데미지 효과 적용
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
                hit.GetComponent<Enemy>().DamageEffect();
        }
    }

    // 크리스탈 종료 함수
    public void FinishCrystal()
    {
        if (canExplode)
        {
            // 폭발 가능하면 성장 후 폭발 애니메이션 재생
            canGrow = true;
            anim.SetTrigger("Explode");
        }
        else
        {
            // 폭발 불가능하면 자기 파괴
            SelfDestroy();
        }
    }

    // 자기 파괴 함수
    public void SelfDestroy() => Destroy(gameObject);
}
