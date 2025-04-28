
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("기본적인 스탯")]
    public Stat strength;     // 힘 - 물리 데미지 증가
    public Stat agility;      // 민첩성 - 회피율과 치명타 확률 증가
    public Stat intelligence; // 지능 - 마법 데미지 증가
    public Stat vitality;     // 체력 - 최대 체력 증가

    [Header("공격관련 스탯")]
    public Stat damage;       // 기본 데미지
    public Stat critChance;   // 치명타 확률
    public Stat critPower;    // 치명타 피해량 증가율


    [Header("방어관련 스탯")]
    public Stat maxHealth;    // 최대 체력
    public Stat armor;        // 물리 방어력
    public Stat evasion;      // 회피율
    public Stat magicResistance; // 마법 저항


    [Header("마법 스탯")]
    public Stat fireDamage;    // 화염 데미지
    public Stat iceDamage;     // 빙결 데미지
    public Stat lightingDamage; // 번개 데미지


    public bool isIgnited;    // 화염 상태 여부
    public bool isChilled;    // 빙결 상태 여부
    public bool isShocked;    // 감전 상태 여부


    private float ignitedTimer; // 화염 상태 지속 시간
    private float chilledTimer; // 빙결 상태 지속 시간
    private float shockedTimer; // 감전 상태 지속 시간

   

    private float igniteDamageCooldown = 0.3f; // 화염 상태 데미지 쿨타임
    private float igniteDamageTimer; // 화염 상태 데미지 타이머

    private int igniteDamage; // 화염 상태 데미지



    
    public int currentHealth; // 현재 체력

    public System.Action onHealthChanged; // 체력 변경 시 호출되는 델리게이트


    protected virtual void Start()
    {
        // 기본 치명타 피해량 설정 (150%)
        critPower.SetDefaultValue(150);
        // 현재 체력을 최대 체력으로 초기화
        currentHealth = GetMaxHealthValue();                     
        Debug.Log("캐릭터스탯 부르고있다.");
    }

    protected virtual void Update()
    {
       ignitedTimer -= Time.deltaTime; // 화염 상태 지속 시간 감소
       chilledTimer -= Time.deltaTime; // 빙결 상태 지속 시간 감소
       shockedTimer -= Time.deltaTime; // 감전 상태 지속 시간 감소


       igniteDamageTimer -= Time.deltaTime; // 화염 상태 데미지 쿨타임 감소
        if(ignitedTimer <0)
          isIgnited = false; // 화염 상태 해제

        if(chilledTimer <0)
          isChilled = false; // 빙결 상태 해제

        if(shockedTimer <0)
          isShocked = false; // 감전 상태 해제


        if(igniteDamageTimer <0 && isIgnited) // 화염 상태 데미지 쿨타임이 끝나고 화염 상태일 때
        {
         
            Debug.Log("화염 상태 데미지 적용됨"+igniteDamage);
           
            DecreaeaseHealth(igniteDamage); // 체력 감소 처리

            if(currentHealth <0)
              Die(); // 체력이 0 이하가 되면 사망 처리


            igniteDamageTimer = igniteDamageCooldown; // 쿨타임 초기화          
        }
    }




    public virtual void DoDamage(CharacterStats _targetStats)
    {
        // 대상이 공격을 회피했는지 확인
        if (TargetCanAvoidAttack(_targetStats))
            return;

        // 물리 데미지 계산 (기본 데미지 + 힘)
        int totalDamage = damage.GetValue() + strength.GetValue();

        // 치명타 확인 및 계산
        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
            
        }

        // 대상의 방어력 적용
        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        // 데미지 적용
        _targetStats.TakeDamage(totalDamage);
        // 마법 데미지 적용
        DoMagicalDamage(_targetStats);

    }

    public virtual void DoMagicalDamage(CharacterStats _targetStats)
    {
        // 각 속성별 마법 데미지 계산
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightingDamage = lightingDamage.GetValue();

        // 전체 마법 데미지 계산 (각 속성 데미지 + 지능)
        int totalMagicalDamage = _fireDamage + _iceDamage + _lightingDamage + intelligence.GetValue();
        // 대상의 마법 저항력 적용
        totalMagicalDamage = CheckTargetResistance(_targetStats, totalMagicalDamage);

        // 데미지 적용
        _targetStats.TakeDamage(totalMagicalDamage);


        if(Mathf.Max(_fireDamage, _iceDamage, _lightingDamage) <= 0)
        {
            return; // 모든 속성 데미지가 0 이하일 경우 상태이상 적용하지 않음
        }


        // 상태이상 적용 조건 확인
        bool canApplyIgnite = _fireDamage >_iceDamage && _fireDamage > _lightingDamage;  // 화염 데미지가 다른 데미지보다 높을 때 점화 가능
        bool canApplyChill = _iceDamage>_fireDamage && _iceDamage > _lightingDamage;     // 빙결 데미지가 다른 데미지보다 높을 때 냉각 가능
        bool canApplyShock = _lightingDamage>_fireDamage && _lightingDamage > _iceDamage; // 번개 데미지가 다른 데미지보다 높을 때 감전 가능

        // 모든 상태이상 조건이 충족되지 않았을 경우 실행되는 루프
        while(!canApplyIgnite && !canApplyChill && !canApplyShock)
        {
            // 50% 확률로 화염 데미지가 0보다 크면 점화 상태를 적용
            if(Random.value <0.35f && _fireDamage >0)
            {
                canApplyIgnite = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock); // 상태이상 적용
                Debug.Log("점화 상태 적용됨");
                return;
            }

            // 50% 확률로 빙결 데미지가 0보다 크면 빙결 상태를 적용
            if(Random.value <0.25f && _iceDamage >0)
            {
                canApplyChill = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock); // 상태이상 적용
                Debug.Log("빙결 상태 적용됨");
                return;
            }
            
            // 50% 확률로 번개 데미지가 0보다 크면 감전 상태를 적용
            if(Random.value <0.15f && _lightingDamage >0)
            {
                canApplyShock = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock); // 상태이상 적용
                Debug.Log("감전 상태 적용됨");
                return;
            }

            // 위 세 조건 중 하나도 충족되지 않으면 루프가 계속 실행됨
        }


        if(canApplyIgnite)
          _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage *0.2f)); // 점화 상태 데미지 설정

        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock); // 상태이상 적용


    }

    private  int CheckTargetResistance(CharacterStats _targetStats, int totalMagicalDamage)
    {
        // 마법 데미지에서 대상의 마법 저항력과 지능 보너스를 차감
        totalMagicalDamage -= _targetStats.magicResistance.GetValue() + (_targetStats.intelligence.GetValue() * 3);
        // 0 이하로 내려가지 않도록 제한
        totalMagicalDamage = Mathf.Clamp(totalMagicalDamage, 0, int.MaxValue);
        return totalMagicalDamage;
    }

    public void ApplyAilments(bool _ignite, bool _chill, bool _shock)
    {
        // 이미 상태이상이 적용되어 있으면 무시
        if (isIgnited || isChilled || isShocked)
        {
            return;
        }

        if(_ignite)
        {
            isIgnited = _ignite;  // 점화 상태 적용
            ignitedTimer = 2; // 점화 상태 지속 시간 설정
        }
  
        if(_chill)
        {
            isChilled = _chill; // 빙결 상태 적용
            chilledTimer = 2; // 빙결 상태 지속 시간 설정
        }
        if(_shock)
        {
            isShocked = _shock; // 감전 상태 적용
            shockedTimer = 2; // 감전 상태 지속 시간 설정
        }

       
       
    }

  
    public void SetupIgniteDamage(int _damage) => igniteDamage = _damage; // 화염 상태 데미지 설정




    private  int CheckTargetArmor(CharacterStats _targetStats, int totalDamage)
    {

        if(_targetStats.isChilled)
           totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() *0.8f); // 빙결 상태일 경우 방어력 20% 감소
        else 
            totalDamage -= _targetStats.armor.GetValue(); // 방어력 차감

       
        // 0 이하로 내려가지 않도록 제한
        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
    }

    private bool TargetCanAvoidAttack(CharacterStats _targetStats)
    {



        // 총 회피율 계산 (회피 + 민첩성)
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();


        if(isShocked)
        {
            totalEvasion +=20; // 감전 상태일 경우 회피율 20% 증가
        }



        // 회피 확률 체크 (0~100 사이 난수가 총 회피율보다 작으면 회피 성공)
        if (Random.Range(0, 100) < totalEvasion)
        {
            return true;

        }
        return false;
    }



    



    public virtual void TakeDamage(int _damage)
    {
       
        DecreaeaseHealth(_damage); // 체력 감소 처리
        Debug.Log(_damage);

        // 체력이 0 이하면 사망 처리
        if (currentHealth < 0)
            Die();


       
    }

  protected virtual void DecreaeaseHealth(int _damage)
  {
     currentHealth -= _damage; // 현재 체력에서 데미지 차감
      
      if(onHealthChanged != null)
        onHealthChanged(); // 체력 변경 시 델리게이트 호출
  }





    protected virtual void Die()
    {
        // 자식 클래스에서 오버라이드하여 사망 처리 구현
    }



    private bool CanCrit()
    {
        // 총 치명타 확률 계산 (치명타 확률 + 민첩성)
        int totalCriticalChance = critChance.GetValue() + agility.GetValue();

        // 치명타 확률 체크 (0~100 사이 난수가 총 치명타 확률보다 작거나 같으면 치명타 적중)
        if(Random.Range(0,100) <= totalCriticalChance)
        {
            return true;
        }

        return false;
    }

    private int CalculateCriticalDamage(int _damage)
    {
        // 총 치명타 배율 계산 ((치명타 파워 + 힘) / 100)
        float totalCritPower = (critPower.GetValue() + strength.GetValue()) * 0.01f;  
        // 치명타 데미지 계산 (기본 데미지 * 치명타 배율)
        float critDamage = _damage * totalCritPower;
        
        // 정수로 반올림하여 반환
        return Mathf.RoundToInt(critDamage);
    }

    public int GetMaxHealthValue()
    {
        return maxHealth.GetValue() + vitality.GetValue() *5; // 최대 체력 계산 (기본 최대 체력 + 민첩성 * 5)
    }

}
