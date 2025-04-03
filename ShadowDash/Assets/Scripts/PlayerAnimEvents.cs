using UnityEngine;

public class PlayerAnimEvents : MonoBehaviour
{
    private Player player;
    void Start()
    {
        player = GetComponentInParent<Player>();
    }

    
    public void AnimationTrigger()
    {
        player.AttackOver();
    }
}
