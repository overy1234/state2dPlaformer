using UnityEngine;

public class Blackhole_Skill : Skill
{

    [SerializeField] private int amountOfAttacks;       // 공격 횟수
    [SerializeField] private float cloneCooldown;       // 클론 쿨다운
    [SerializeField] private float blackholeDuration;   // 블랙홀 지속 시간
    [Space]
    [SerializeField] private GameObject blackHolePrefab; // 블랙홀 프리팹
    [SerializeField] private float maxSize;             // 최대 크기
    [SerializeField] private float growSpeed;           // 성장 속도
    [SerializeField] private float shrinkSpeed;         // 축소 속도

    Blackhole_Skill_Controller currentBalckhole;        // 현재 생성된 블랙홀 컨트롤러

    // 스킬 사용 가능 여부 체크
    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }

    // 스킬 사용 메서드
    public override void UseSkill()
    {
        base.UseSkill();

        // 새 블랙홀 생성
        GameObject newBlackHole = Instantiate(blackHolePrefab,player.transform.position,Quaternion.identity);

        // 블랙홀 컨트롤러 설정
        currentBalckhole = newBlackHole.GetComponent<Blackhole_Skill_Controller>();

        // 블랙홀 설정 (크기, 속도, 공격수, 쿨다운, 지속시간)
        currentBalckhole.SetupBlackhole(maxSize,growSpeed,shrinkSpeed,amountOfAttacks,cloneCooldown,blackholeDuration);

    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    // 블랙홀 종료 여부 확인 함수
    public bool BlackholeFinished()
    {
        if (!currentBalckhole)
            return false;

        // 플레이어가 상태를 나갈 수 있는지 확인
        if(currentBalckhole.playerCanExitState)
        {
            currentBalckhole = null;
            return true;
        }

        return false;
    }

    // 블랙홀 반지름 반환 함수
    public float GetBlackholeRadius()
    {
        return maxSize / 2;
    }
}
