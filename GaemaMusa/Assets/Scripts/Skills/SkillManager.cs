using UnityEngine;

public class SkillManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static SkillManager instance;


    public Dash_Skill dash { get; private set; }      // 대시 스킬
    public Clone_Skill clone { get; private set; }    // 분신 스킬
    public Sword_Skill sword { get; private set; }    // 검 스킬

    public Blackhole_Skill blackhole { get; private set; } // 블랙홀 스킬
    public Crystal_Skill crystal { get; private set; }     // 크리스탈 스킬

    // 게임 시작 시 싱글톤 설정
    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;


    }

    // 모든 스킬 컴포넌트 초기화
    private void Start()
    {
        dash = GetComponent<Dash_Skill>();
        clone = GetComponent<Clone_Skill>();
        sword = GetComponent<Sword_Skill>();
        blackhole = GetComponent<Blackhole_Skill>();
        crystal = GetComponent<Crystal_Skill>();
    }



}
