using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    private Enemy enemy;
    private ItemDrop myDropSystem;

    [Header("Level details")]
    [SerializeField] private int level = 1;

    [Range(0f, 1f)]
    [SerializeField] private float percantageModifier = .4f;

    [Header("Drop Settings")]
    [SerializeField] private int expValue;    // 이 적을 죽였을 때 얻는 경험치
    [SerializeField] private int goldValue;   // 이 적을 죽였을 때 얻는 골드
    [SerializeField] private int bossExpValue;  // 추가: 보스 경험치 값

    protected override void Start()
    {
        ApplyLevelModifiers();

        base.Start();

        enemy = GetComponent<Enemy>();
        myDropSystem = GetComponent<ItemDrop>();
    }

    private void ApplyLevelModifiers()
    {
        Modify(strength);
        Modify(agility);
        Modify(intelligence);
        Modify(vitality);

        Modify(damage);
        Modify(critChance);
        Modify(critPower);

        Modify(maxHealth);
        Modify(armor);
        Modify(evasion);
        Modify(magicResistance);

        Modify(fireDamage);
        Modify(iceDamage);
        Modify(lightingDamage);
    }

    private void Modify(Stat _stat)
    {
        for (int i = 1; i < level; i++)
        {
            float modifier = _stat.GetValue() * percantageModifier;

            _stat.AddModifier(Mathf.RoundToInt(modifier));
        }
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
    }

    protected override void Die()
    {
        base.Die();
        
        // 플레이어에게 경험치와 골드 지급
        PlayerStats playerStats = FindObjectOfType<PlayerStats>();
        if(playerStats != null)
        {
            // 레벨에 따른 경험치와 골드 보너스 계산
            int expBonus = Mathf.RoundToInt(expValue * (1 + (level - 1) * 0.1f));
            int goldBonus = Mathf.RoundToInt(goldValue * (1 + (level - 1) * 0.1f));
            int bossExpBonus = Mathf.RoundToInt(bossExpValue * (1 + (level - 1) * 0.1f));
            
            playerStats.GainExperienceAndGold(expBonus, goldBonus);
            playerStats.GainBossExp(bossExpBonus);  // 보스 경험치 추가
        }

        enemy.Die();
        myDropSystem.GenerateDrop();
    }
}
