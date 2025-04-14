using UnityEngine;

public class Dash_Skill : Skill
{

    public override void UseSkill()
    {
        base.UseSkill();

        Debug.Log("오버라이드 함수 UseSkill");
    }

}
