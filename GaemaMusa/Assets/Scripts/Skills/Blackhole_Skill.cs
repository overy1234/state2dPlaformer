using UnityEngine;

public class Blackhole_Skill : Skill
{

    [SerializeField] private int amountOfAttacks;
    [SerializeField] private float cloneCooldown;
    [SerializeField] private float blackholeDuration;
    [Space]
    [SerializeField] private GameObject blackHolePrefab;
    [SerializeField] private float maxSize;
    [SerializeField] private float growSpeed;
    [SerializeField] private float shrinkSpeed;

    Blackhole_Skill_Controller currentBalckhole;
    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }

    public override void UseSkill()
    {
        base.UseSkill();

        GameObject newBlackHole = Instantiate(blackHolePrefab,player.transform.position,Quaternion.identity);

        currentBalckhole = newBlackHole.GetComponent<Blackhole_Skill_Controller>();

        currentBalckhole.SetupBlackhole(maxSize,growSpeed,shrinkSpeed,amountOfAttacks,cloneCooldown,blackholeDuration);

    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }


    public bool BlackholeFinished()
    {
        if (!currentBalckhole)
            return false;


        if(currentBalckhole.playerCanExitState)
        {
            currentBalckhole = null;
            return true;
        }

        return false;
    }

    public float GetBlackholeRadius()
    {
        return maxSize / 2;
    }
}
