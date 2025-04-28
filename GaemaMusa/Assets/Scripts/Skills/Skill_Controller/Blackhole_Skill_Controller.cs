using System.Collections.Generic;
using UnityEngine;

public class Blackhole_Skill_Controller : MonoBehaviour
{
    [SerializeField] private GameObject hotKeyPrefab;        // 단축키 프리팹
    [SerializeField] private List<KeyCode> keyCodeList;      // 단축키 목록

    public float maxSize;                                    // 블랙홀 최대 크기
    public float growSpeed;                                  // 성장 속도
    public float shrinkSpeed;                                // 축소 속도
    private float blackholeTimer;                            // 블랙홀 지속 시간 타이머

    private bool canGrow = true;                             // 성장 가능 여부
    private bool canShrink;                                  // 축소 가능 여부
    

    private bool canCreateHotKeys = true;                    // 단축키 생성 가능 여부
    private bool cloneAttackReleased;                        // 클론 공격 해제 여부
    private bool playerCanDisapear = true;                   // 플레이어 사라짐 가능 여부

    private int amountOfAttacks = 4;                         // 공격 횟수
    private float cloneAttackCooldown = 0.3f;                // 클론 공격 쿨다운
    private float cloneAttackTimer;                          // 클론 공격 타이머

    private List<Transform> targets = new List<Transform>(); // 타겟 목록
    private List<GameObject> createHotKey = new List<GameObject>(); // 생성된 단축키 목록

    public bool playerCanExitState {  get; private set; }    // 플레이어 상태 종료 가능 여부

    // 블랙홀 설정 함수
    public void SetupBlackhole(float _maxSize,float _growSpeed, float _shrinkSpeed , int _amountOfAttacks, float _cloneAttackCooldown , float _balckholeDuration)
    {
        maxSize = _maxSize;
        growSpeed = _growSpeed;
        shrinkSpeed = _shrinkSpeed;
        amountOfAttacks = _amountOfAttacks;
        cloneAttackCooldown = _cloneAttackCooldown;

        blackholeTimer = _balckholeDuration;

        // 크리스탈 대신 클론을 사용하는 경우 플레이어가 사라지지 않음
        if (SkillManager.instance.clone.crystalInseadOfClone)
            playerCanDisapear = false;
    }

    // 매 프레임 업데이트
    void Update()
    {
        cloneAttackTimer -= Time.deltaTime;
        blackholeTimer -= Time.deltaTime;

        // 블랙홀 타이머 종료 시
        if(blackholeTimer < 0)
        {
            blackholeTimer = Mathf.Infinity;

            if (targets.Count > 0)
                ReleaseCloneAttack();
            else
                FinishBlackHoleAbillity();
        }

        // G키를 눌러 클론 공격 해제
        if (Input.GetKeyDown(KeyCode.G))
        {
            ReleaseCloneAttack();
        }

        // 클론 공격 로직 실행
        CloneAttackLogic();

        // 블랙홀 성장 로직
        if (canGrow && !canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);
        }

        // 블랙홀 축소 로직
        if (canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(-1, -1), shrinkSpeed * Time.deltaTime);

            if (transform.localScale.x < 0)
                Destroy(gameObject);
        }
    }

    // 클론 공격 해제 함수
    private void ReleaseCloneAttack()
    {
        if (targets.Count <= 0)
            return;

        // 단축키 제거
        DestroyHotKeys();
        cloneAttackReleased = true;
        canCreateHotKeys = false;

        // 플레이어 투명화 처리
        if(playerCanDisapear)
        {
            playerCanDisapear = false;
            PlayerManager.instance.player.MakeTransparent(true);
        }
    }

    // 클론 공격 로직 함수
    private void CloneAttackLogic()
    {
        if (cloneAttackTimer < 0 && cloneAttackReleased && amountOfAttacks > 0)
        {
            cloneAttackTimer = cloneAttackCooldown;

            // 무작위 타겟 선택
            int randomIndex = Random.Range(0, targets.Count);

            // 무작위 x 오프셋 설정
            float xOffset;
            if (Random.Range(0, 100) > 50)
                xOffset = 2;
            else
                xOffset = -2;

            // 크리스탈 또는 클론 생성
            if(SkillManager.instance.clone.crystalInseadOfClone)
            {
                SkillManager.instance.crystal.CreateCrystal();
                SkillManager.instance.crystal.CurrentCrystalChooseRandomTarget();
            }
            else
            {
                SkillManager.instance.clone.CreateClone(targets[randomIndex], new Vector3(xOffset, 0));
            }

            // 공격 횟수 감소
            amountOfAttacks--;

            // 모든 공격 완료 시 블랙홀 종료
            if (amountOfAttacks <= 0)
            {
                Invoke("FinishBlackHoleAbillity",1f);
            }
        }
    }

    // 블랙홀 능력 종료 함수
    private void FinishBlackHoleAbillity()
    {
        DestroyHotKeys();
        playerCanExitState = true;
      
        canShrink = true;
        cloneAttackReleased = false;
    }

    // 단축키 제거 함수
    private void DestroyHotKeys()
    {
        if (createHotKey.Count <= 0)
            return;

        for(int i=0; i< createHotKey.Count; i++)
        {
            Destroy(createHotKey[i]);
        }
    }

    // 충돌 감지 시 적에게 단축키 생성
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Enemy>() != null)
        {
            collision.GetComponent<Enemy>().FreezeTime(true);
            CreateHotKey(collision);
        }
    }

    // 충돌 종료 시 적의 시간 정지 해제
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            collision.GetComponent<Enemy>().FreezeTime(false);
        }
    }

    // 단축키 생성 함수
    private void CreateHotKey(Collider2D collision)
    {
        if (keyCodeList.Count <= 0)
        {
            Debug.LogWarning("키등록 까먹었는지 체크");
            return;
        }

        if (!canCreateHotKeys)
            return;

        // 새 단축키 생성
        GameObject newHotKey = Instantiate(hotKeyPrefab, collision.transform.position + new Vector3(0, 2), Quaternion.identity);
        createHotKey.Add(newHotKey);

        // 무작위 키 선택
        KeyCode choosenKey = keyCodeList[Random.Range(0, keyCodeList.Count)];
        keyCodeList.Remove(choosenKey);

        // 단축키 설정
        Blackhole_HotKey_Controller newHotKeyScript = newHotKey.GetComponent<Blackhole_HotKey_Controller>();
        newHotKeyScript.SetupHotKey(choosenKey, collision.transform, this);
    }

    // 적을 타겟 리스트에 추가하는 함수
    public void AddEnemyToList(Transform _enemyTransform) => targets.Add(_enemyTransform);
}
