using UnityEngine;

// 검 유형 열거형
public enum SwordType
{
    Regular, // 기본 검
    Bounce,  // 바운스 검
    Pierce,  // 관통 검
    Spin     // 회전 검
}

public class Sword_Skill : Skill
{
    // 검 유형 선택 
    public SwordType swordType = SwordType.Regular;

    [Header("바운스 정보")]
    [SerializeField] private int bounceAmount;      // 바운스 횟수
    [SerializeField] private float bounceGravity;   // 바운스 중력
    [SerializeField] private float bounceSpeed;     // 바운스 속도

    [Header("관통 정보")]
    [SerializeField] private int pierceAmount;      // 관통 횟수
    [SerializeField] private float pierceGravity;   // 관통 중력

    [Header("스핀 정보")]
    [SerializeField] private float hitCooldown = 0.35f;         // 타격 쿨다운
    [SerializeField] private float maxTravelDistance = 7;       // 최대 이동 거리
    [SerializeField] private float spinDuration = 2;            // 회전 지속 시간
    [SerializeField] private float spinGravity = 1;             // 회전 중력

    [Header("스킬 정보")]
    [SerializeField] private GameObject swordPrefab;            // 검 프리팹
    [SerializeField] private Vector2 launchForce;               // 발사 힘
    [SerializeField] private float swordGravity;                // 검 중력
    [SerializeField] private float freezeTimeDuration;          // 시간 정지 지속 시간
    [SerializeField] private float returnSpeed;                 // 귀환 속도

    private Vector2 finalDir;                                   // 최종 방향

    [Header("에임 보여주기")]
    [SerializeField] private int numberOfDots;                  // 점의 개수
    [SerializeField] private float spaceBeetwenDots;            // 점 사이의 간격
    [SerializeField] private GameObject dotPrefab;              // 점 프리팹
    [SerializeField] private Transform dotsParent;              // 점 상위 객체

    private GameObject[] dots;                                  // 점 배열

    protected override void Start()
    {
        base.Start();
        GenereateDots();
        SetupGravity();
    }

    // 중력 설정 함수
    private void SetupGravity()
    {
        if (swordType == SwordType.Bounce)
            swordGravity = bounceGravity;
        else if (swordType == SwordType.Pierce)
            swordGravity = pierceGravity;
        else if (swordType == SwordType.Spin)
            swordGravity = spinGravity;
    }

    protected override void Update()
    {
        if(Input.GetKeyUp(KeyCode.Mouse1))
        {
            finalDir = new Vector2(AimDirection().normalized.x * launchForce.x, AimDirection().normalized.y * launchForce.y);
        }

        if(Input.GetKey(KeyCode.Mouse1))
        {
            for(int i =0; i<dots.Length; i++)
            {
                dots[i].transform.position = DotsPosition(i * spaceBeetwenDots);
            }
        }
    }

    // 검 생성 함수
    public void CreateSword()
    {
        GameObject newSword = Instantiate(swordPrefab, player.transform.position, transform.rotation);
        Sword_Skill_Controller newSwordScript = newSword.GetComponent<Sword_Skill_Controller>();

        if (swordType == SwordType.Bounce)
            newSwordScript.SetupBounce(true, bounceAmount,bounceSpeed);
        else if (swordType == SwordType.Pierce)
            newSwordScript.SetupPierce(pierceAmount);
        else if (swordType == SwordType.Spin)
            newSwordScript.SetupSpin(true, maxTravelDistance, spinDuration,hitCooldown);

        newSwordScript.SetupSword(finalDir, swordGravity, player,freezeTimeDuration,returnSpeed);

        player.AssignNewSword(newSword);

        DotsActive(false);
    }

    #region Aim
    // 조준 방향 계산
    public Vector2 AimDirection()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - playerPosition;

        return direction;
    }

    // 점(에임) 활성화 함수
    public void DotsActive(bool _isActive)
    {
        for(int i =0; i<dots.Length; i++)
        {
            dots[i].SetActive(_isActive);
        }
    }

    // 점(에임) 생성 함수
    private void GenereateDots()
    {
        dots = new GameObject[numberOfDots];
        for(int i =0; i<numberOfDots; i++)
        {
            dots[i] = Instantiate(dotPrefab, player.transform.position, Quaternion.identity, dotsParent);
            dots[i].SetActive(false);
        }
    }

    // 점(에임) 위치 계산 함수
    private Vector2 DotsPosition(float t)
    {
        Vector2 position = (Vector2)player.transform.position + new Vector2(
            AimDirection().normalized.x * launchForce.x,
            AimDirection().normalized.y * launchForce.y) * t + 0.5f * (Physics2D.gravity * swordGravity) * (t * t);

        return position;
    }
    #endregion
}
