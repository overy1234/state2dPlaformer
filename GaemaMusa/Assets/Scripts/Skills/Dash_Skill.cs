using UnityEngine;

public class Dash_Skill : Skill
{
    public GameObject explosion;

    // 대시 스킬 사용 메서드
    public override void UseSkill()
    {
        base.UseSkill();

        // 대시 스킬 구현 (나중에 기능 추가)
        GameObject go= Instantiate(explosion, player.transform.position, Quaternion.identity);
        Destroy(go,1);
    }

}
