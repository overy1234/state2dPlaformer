using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Crystal_Skill : Skill
{
    [SerializeField] private float crystalDuration;      // 크리스탈 지속 시간
    [SerializeField] private GameObject crystalPrefab;    // 크리스탈 프리팹
    private GameObject currentCrystal;                    // 현재 생성된 크리스탈


    [SerializeField] private bool cloneInsteadOfCrystal;  // 크리스탈 대신 클론 생성 여부



    [Header("폭발성 크리스탈")]
    [SerializeField] private bool canExplode;             // 폭발 가능 여부


    [Header("이동 가능 크리스탈")]
    [SerializeField] private bool canMoveToEnemy;         // 적에게 이동 가능 여부
    [SerializeField] private float moveSpeed;             // 이동 속도


    [Header("다중 크리스탈 스택")]
    [SerializeField] private bool canUseMultiStacks;      // 다중 스택 사용 가능 여부
    [SerializeField] private int amountOfStacks;          // 스택 수량
    [SerializeField] private float multiStackCooldown;    // 다중 스택 쿨다운
    [SerializeField] private float useTimeWindow;         // 사용 시간 창
    [SerializeField] private List<GameObject> crystalLeft = new List<GameObject>(); // 남은 크리스탈 목록


    public override void UseSkill()
    {
        base.UseSkill();


        // 다중 크리스탈 사용 가능하면 해당 로직 실행
        if (CanUseMultiCrystal())
            return;

       
        // 현재 크리스탈이 없으면 새로 생성
        if (currentCrystal == null)
        {
            CreateCrystal();
        }
        // 현재 크리스탈이 있으면 플레이어와 위치 교체
        else
        {
            // 적에게 이동 가능한 크리스탈이면 위치 교체 안함
            if (canMoveToEnemy)
                return;

            // 플레이어와 크리스탈 위치 교체
            Vector2 playerPos = player.transform.position;
            player.transform.position = currentCrystal.transform.position;
            currentCrystal.transform.position = playerPos;

            // 클론 생성 옵션이 활성화된 경우
            if(cloneInsteadOfCrystal)
            {
                // 클론 생성 후 크리스탈 파괴
                SkillManager.instance.clone.CreateClone(currentCrystal.transform, Vector3.zero);
                Destroy(currentCrystal);
            }
            else
            {
                // 크리스탈 종료 처리
                currentCrystal.GetComponent<Crystal_Skill_Controller>()?.FinishCrystal();
            }


             
        }

    }

    // 크리스탈 생성 함수
    public void CreateCrystal()
    {
        // 크리스탈 생성 및 설정
        currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);
        Crystal_Skill_Controller currentCystalScript = currentCrystal.GetComponent<Crystal_Skill_Controller>();

        // 크리스탈 속성 설정 (지속시간, 폭발, 이동, 속도, 타겟)
        currentCystalScript.SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(currentCrystal.transform));

      
    }


    // 현재 크리스탈에 무작위 타겟 설정 함수
    public void CurrentCrystalChooseRandomTarget() => currentCrystal.GetComponent<Crystal_Skill_Controller>().ChooseRandomEnemy();





    // 다중 크리스탈 사용 가능 여부 확인 함수
    private bool CanUseMultiCrystal()
    {
        if(canUseMultiStacks)
        {
            if(crystalLeft.Count > 0)
            {
                // 모든 스택을 사용할 수 있는 시간 창 설정
                if (crystalLeft.Count == amountOfStacks)
                    Invoke("ResetAbility", useTimeWindow);


                cooldown = 0;
                // 남은 크리스탈 목록에서 하나 가져와 생성
                GameObject crystalToSwpan = crystalLeft[crystalLeft.Count - 1];
                GameObject newCrystal = Instantiate(crystalToSwpan, player.transform.position, Quaternion.identity);

                crystalLeft.Remove(crystalToSwpan);

                // 새 크리스탈 설정
                newCrystal.GetComponent<Crystal_Skill_Controller>().SetupCrystal(crystalDuration, 
                    canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(newCrystal.transform));

                // 모든 크리스탈을 사용했으면 리필 및 쿨다운 설정
                if(crystalLeft.Count <= 0)
                {
                    // 리필
                    cooldown = multiStackCooldown;
                    RefilCrystal();
                }

            }

            return true;
        }

        return false;
    }




    // 크리스탈 리필 함수
    private void RefilCrystal()
    {
        // 필요한 크리스탈 수 계산 및 추가
        int amountToAdd = amountOfStacks - crystalLeft.Count;

        for(int i = 0; i < amountToAdd; i++)
        {
            crystalLeft.Add(crystalPrefab);
        }
    }

    // 능력 재설정 함수
    private void ResetAbility()
    {
        // 쿨다운 중이면 무시
        if (cooldownTimer > 0)
            return;

        // 쿨다운 및 크리스탈 리필 설정
        cooldownTimer = multiStackCooldown;
        RefilCrystal();
    }
    


}
