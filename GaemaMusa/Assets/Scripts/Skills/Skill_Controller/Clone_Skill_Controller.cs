using UnityEngine;

public class Clone_Skill_Controller : MonoBehaviour
{

    private SpriteRenderer sr;                            // 스프라이트 렌더러
    private Animator anim;                                // 애니메이터
    [SerializeField] private float colorLoosingSpeed;     // 색상 투명화 속도
    
    private float cloneTimer;                             // 분신 지속 시간
    [SerializeField] private Transform attackCheck;       // 공격 범위 체크 위치
    [SerializeField] private float attackCheckRadius = 0.8f; // 공격 범위 반경
    private Transform closestEnemy;                       // 가장 가까운 적
    private bool canDuplicateClone;                       // 분신 복제 가능 여부
    private int facingDir = 1;                            // 바라보는 방향

    private float chanceToDuplicate;                      // 복제 확률

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }


    private void Update()
    {
        cloneTimer -= Time.deltaTime;     // 분신 지속 시간 감소

        if(cloneTimer < 0)
        {
            // 시간이 다 되면 투명해지며 사라짐
            sr.color = new Color(1, 1, 1, sr.color.a - (Time.deltaTime * colorLoosingSpeed));

            if (sr.color.a <= 0)
                Destroy(gameObject);
        }
    }




    public void SetupClone(Transform _newTransform,float _cloneDuration,bool _canAttack,Vector3 _offset,Transform _closestEnemy,
        bool _canDuplicate,float _chanceToDuplicate)
    {
        if (_canAttack)
            anim.SetInteger("AttackNumber", Random.Range(1, 4));   // 무작위 공격 애니메이션 설정


        transform.position = _newTransform.position + _offset;    // 위치 설정
        cloneTimer = _cloneDuration;                              // 지속 시간 설정


        closestEnemy = _closestEnemy;                             // 가장 가까운 적 설정
        canDuplicateClone = _canDuplicate;                        // 복제 가능 여부 설정
        chanceToDuplicate = _chanceToDuplicate;                   // 복제 확률 설정
        FaceClosestTarget();                                      // 적을 향해 방향 전환
    }


    private void AnimationTrigger()
    {
        cloneTimer = -0.1f;    // 애니메이션 종료 트리거
    }

    private void AttackTrigger()
    {
        // 공격 범위 내 모든 콜라이더 검색
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);


        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                EnemyStats _target = hit.GetComponent<EnemyStats>();

                // 플레이어 데미지 적용
                PlayerManager.instance.player.stats.DoDamage(_target);

                // 적 데미지 효과 표시
                hit.GetComponent<Enemy>().DamageEffect();

                // 분신 복제 시도
                if(canDuplicateClone)
                {
                    if(Random.Range(0,100) < chanceToDuplicate)
                    {
                        SkillManager.instance.clone.CreateClone(hit.transform, new Vector3(0.5f*facingDir, 0));
                    }
                }
            }
                
        }


    }


   

    private void FaceClosestTarget()
    {
      
        // 가장 가까운 적을 향해 방향 전환
        if(closestEnemy != null)
        {
            if (transform.position.x > closestEnemy.position.x)
            {
                facingDir = -1;
                transform.Rotate(0, 180, 0);
            }
                
        }



    }








}
