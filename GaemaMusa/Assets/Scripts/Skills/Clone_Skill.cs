using System.Collections;
using UnityEngine;

public class Clone_Skill : Skill
{

    [Header("클론정보")]
    [SerializeField] private GameObject clonePrefab;    // 클론 프리팹
    [SerializeField] private float cloneDuration;       // 클론 지속 시간
    [Space]
    [SerializeField] private bool canAttack;            // 공격 가능 여부

    [SerializeField] private bool createCloneOnDashStart;    // 대시 시작 시 클론 생성 여부
    [SerializeField] private bool createCloneOnDashOver;     // 대시 종료 시 클론 생성 여부
    [SerializeField] private bool canCreateCloneOnCounterAttack; // 카운터 어택 시 클론 생성 여부

    [Header("필살기")]
    [SerializeField] private bool canDuplicateClone;    // 클론 복제 가능 여부
    [SerializeField] private float chanceToDuplicate;   // 복제 확률
    [Header("Crystal 필살기")]
    public bool crystalInseadOfClone;                  // 클론 대신 크리스탈 사용 여부

    // 클론 생성 함수
    public void CreateClone(Transform _clonePosition , Vector3 _offset)
    {
        // 크리스탈 대신 클론 사용 설정이 되어있다면 크리스탈 생성
        if(crystalInseadOfClone)
        {
            SkillManager.instance.crystal.CreateCrystal();
         
            return;
        }

        // 새 클론 생성 및 설정
        GameObject newClone = Instantiate(clonePrefab);

        newClone.GetComponent<Clone_Skill_Controller>().SetupClone(_clonePosition,
            cloneDuration,canAttack,_offset,FindClosestEnemy(newClone.transform),canDuplicateClone, chanceToDuplicate);
    }

    // 대시 시작 시 클론 생성 함수
    public void CreateCloneOnDashStart()
    {
        if (createCloneOnDashStart)
            CreateClone(player.transform, Vector3.zero);
    }

    // 대시 종료 시 클론 생성 함수
    public void CreateCloneOnDashOver()
    {
        if (createCloneOnDashOver)
            CreateClone(player.transform, Vector3.zero);
    }

    // 카운터 어택 시 클론 생성 함수
    public void CreateCloneOnCounterAttack(Transform _enemyTransform)
    {
        if (canCreateCloneOnCounterAttack)
            StartCoroutine(CreateCLoneWithDelay(_enemyTransform, new Vector3(2 * player.facingDir, 0)));
    }

    // 지연 후 클론 생성 코루틴
    private IEnumerator CreateCLoneWithDelay(Transform _transform, Vector3 _offset)
    {
        yield return new WaitForSeconds(0.4f);
        CreateClone(_transform, _offset);
    }
}
