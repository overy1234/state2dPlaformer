using UnityEngine;
using UnityEngine.UI;

public class UI_CoolTime : MonoBehaviour
{
    [SerializeField]
    Image SkillFront;

    [SerializeField]
    float SkillCoolDown;
    float coolTimer;

    
    void Start()
    {
        coolTimer = SkillCoolDown;
    }

    
    void Update()
    {
        coolTimer -= Time.deltaTime;

        SkillFront.fillAmount = coolTimer/ SkillCoolDown;

        if (coolTimer < 0 )
        {
            coolTimer = SkillCoolDown;
        }
    }
}
