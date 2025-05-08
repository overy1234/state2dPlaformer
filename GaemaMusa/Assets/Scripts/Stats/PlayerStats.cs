using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : CharacterStats 
{
    [Header("Level & Currency")]
    public int currentExp;
    public int expToNextLevel = 100;
    public int currentGold;
    public int playerLevel = 1;

    [Header("Boss Stage Progress")]
    [SerializeField] Slider bossExpSlider;  // Inspector에서 연결
    public int currentBossExp;
    public int bossExpToNextLevel = 200;  // 보스 스테이지 입장을 위한 경험치
    public int bossLevel = 1;
    
    public bool bossclear = false;

    [Header("Boss Stage UI")]
    [SerializeField] private Button bossStagButton;  // Inspector에서 연결

    private Player player;

    protected override void Start()
    {
        base.Start();
        player = GetComponent<Player>();
        LoadPlayerData();
        UpdateBossExpSlider();
        
        // 버튼 초기 상태 설정
        if(bossStagButton != null)
            bossStagButton.interactable = false;
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
    }

    protected override void Die()
    {
        base.Die();
        player.Die();
        GetComponent<PlayerItemDrop>()?.GenerateDrop();
        SavePlayerData();
    }

    protected override void DecreaseHealthBy(int _damage)
    {
        base.DecreaseHealthBy(_damage);

        ItemData_Equipment currentArmor = Inventory.instance.GetEquipment(EquipmentType.Armor);

        if (currentArmor != null)
            currentArmor.Effect(player.transform);
    }

    public void GainExperienceAndGold(int exp, int gold)
    {
        currentExp += exp;
        currentGold += gold;
        
        // 레벨업 체크
        while(currentExp >= expToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        playerLevel++;
        currentExp -= expToNextLevel;
        expToNextLevel = Mathf.RoundToInt(expToNextLevel * 1.2f); // 다음 레벨업에 필요한 경험치 증가
        
        // 레벨업 시 스탯 증가 등 추가 효과
        maxHealth.AddModifier(10);
        strength.AddModifier(1);
        agility.AddModifier(1);
        
        // 체력 회복
        currentHealth = GetMaxHealthValue();
        
        if(onHealthChanged != null)
            onHealthChanged();
    }

   
    
    public void GainBossExp(int bossExp)
    {
        currentBossExp += bossExp;
        
        // 경험치가 꽉 찼는지 체크하고 버튼 상태 업데이트
        CheckBossExpFull();
        
        UpdateBossExpSlider();
        SavePlayerData();
    }

    private void CheckBossExpFull()
    {
        if(bossStagButton != null)
        {
            // 경험치가 가득 찼을 때 버튼 활성화
            bossStagButton.interactable = (currentBossExp >= bossExpToNextLevel);
        }
    }

    // 보스 스테이지 입장 버튼 클릭 이벤트에 연결할 메서드
    public void OnBossStageButtonClick()
    {
        if(currentBossExp >= bossExpToNextLevel)
        {
            // 보스 스테이지로 이동하는 로직
            // 예: SceneManager.LoadScene("BossStage");
        }
    }

    private void BossLevelUp()
    {
        bossLevel++;
        currentBossExp -= bossExpToNextLevel;
        bossExpToNextLevel = Mathf.RoundToInt(bossExpToNextLevel * 1.5f);
        
        // 보스 레벨업 시 추가 효과
        // 예: 새로운 보스 스테이지 해금
    }

    private void UpdateBossExpSlider()
    {
        if(bossExpSlider != null)
        {
            bossExpSlider.maxValue = bossExpToNextLevel;
            bossExpSlider.value = currentBossExp;
        }
    }

    public  void SavePlayerData()
    {
          // 기본 정보 저장
        PlayerPrefs.SetInt("PlayerLevel", playerLevel);
        PlayerPrefs.SetInt("CurrentExp", currentExp);
        PlayerPrefs.SetInt("CurrentGold", currentGold);
        
        // 주요 스탯 저장
        PlayerPrefs.SetInt("Strength", strength.GetValue());
        PlayerPrefs.SetInt("Agility", agility.GetValue());
        PlayerPrefs.SetInt("Intelligence", intelligence.GetValue());
        PlayerPrefs.SetInt("Vitality", vitality.GetValue());
        
        // 현재 체력 저장
        PlayerPrefs.SetInt("CurrentHealth", currentHealth);
        
        // 보스 관련 데이터 저장 추가
        PlayerPrefs.SetInt("BossLevel", bossLevel);
        PlayerPrefs.SetInt("CurrentBossExp", currentBossExp);
        PlayerPrefs.SetInt("BossExpToNextLevel", bossExpToNextLevel);
        PlayerPrefs.Save();
    }

    private void LoadPlayerData()
    {
        // 기본 정보 불러오기
        playerLevel = PlayerPrefs.GetInt("PlayerLevel", 1);
        currentExp = PlayerPrefs.GetInt("CurrentExp", 0);
        currentGold = PlayerPrefs.GetInt("CurrentGold", 0);
        
        // 스탯 불러오기 및 적용
        int savedStrength = PlayerPrefs.GetInt("Strength", strength.GetValue());
        int savedAgility = PlayerPrefs.GetInt("Agility", agility.GetValue());
        int savedIntelligence = PlayerPrefs.GetInt("Intelligence", intelligence.GetValue());
        int savedVitality = PlayerPrefs.GetInt("Vitality", vitality.GetValue());
        
        // 스탯 차이만큼 modifier 추가
        strength.AddModifier(savedStrength - strength.GetValue());
        agility.AddModifier(savedAgility - agility.GetValue());
        intelligence.AddModifier(savedIntelligence - intelligence.GetValue());
        vitality.AddModifier(savedVitality - vitality.GetValue());
        
        // 현재 체력 불러오기
        currentHealth = GetMaxHealthValue();
        
        if(onHealthChanged != null)
            onHealthChanged();
        
        // 보스 관련 데이터 로드 추가
        bossLevel = PlayerPrefs.GetInt("BossLevel", 1);
        currentBossExp = PlayerPrefs.GetInt("CurrentBossExp", 0);
        bossExpToNextLevel = PlayerPrefs.GetInt("BossExpToNextLevel", 200);
        
        UpdateBossExpSlider();
    }

    // 게임 초기화 기능 (새 게임 시작할 때 사용)
    public  void ResetPlayerData()
    {
         PlayerPrefs.DeleteAll();
        // 기본값으로 리셋
        playerLevel = 1;
        currentExp = 0;
        currentGold = 0;
        currentHealth = GetMaxHealthValue();
        
        // 기본 스탯으로 리셋 (필요한 경우)
        strength.RemoveAllModifiers();
        agility.RemoveAllModifiers();
        intelligence.RemoveAllModifiers();
        vitality.RemoveAllModifiers();
        
        if(onHealthChanged != null)
            onHealthChanged();
        // 보스 관련 데이터 리셋 추가
        bossLevel = 1;
        currentBossExp = 0;
        bossExpToNextLevel = 200;
        
        if(bossStagButton != null)
            bossStagButton.interactable = false;

        UpdateBossExpSlider();
    }

    public void BossStage()
    {
        bossclear = true;

        currentBossExp = 0;
    }

}
