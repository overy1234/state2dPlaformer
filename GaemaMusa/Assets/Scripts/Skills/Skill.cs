using UnityEngine;

public class Skill : MonoBehaviour
{
    [SerializeField] protected float cooldown;    // 쿨다운 시간
    protected float cooldownTimer;                // 쿨다운 타이머


    protected Player player;                      // 플레이어 참조

    
    protected virtual void Start()
    {
        player = PlayerManager.instance.player;   // 플레이어 매니저에서 플레이어 참조 가져오기
    }
    
    
    protected virtual void Update()
    {
        cooldownTimer -= Time.deltaTime;          // 쿨다운 타이머 감소
    }


    public virtual bool CanUseSkill()
    {
        if (cooldownTimer < 0)                    // 쿨다운 시간이 지났는지 확인
        {
            UseSkill();                           // 스킬 사용
            cooldownTimer = cooldown;             // 쿨다운 타이머 재설정
            return true;        
        }


        Debug.Log("스킬이 쿨다운 중입니다");      // 쿨다운 중이라면 로그 출력

        return false;
    }


    public virtual void UseSkill()
    {
        //스킬사용
    }

    protected virtual Transform FindClosestEnemy(Transform _checkTransform)
    {
        // 주변 콜라이더 검색 (반경 25 이내)
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_checkTransform.position, 25);

        float closestDistance = Mathf.Infinity;   // 가장 가까운 거리 초기화
        Transform closestEnemy = null;            // 가장 가까운 적 초기화


        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)    // Enemy 컴포넌트를 가진 객체인지 확인
            {
                float distanceToEnemy = Vector2.Distance(_checkTransform.position, hit.transform.position);

                // 더 가까운 적을 찾았다면 갱신
                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }

            }
        }

        return closestEnemy;    // 가장 가까운 적 반환
    }

}
