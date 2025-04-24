using UnityEngine;

public class WaterCamera : MonoBehaviour
{
    
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        Transform ptr =  PlayerManager.instance.player.transform;

        transform.position = new Vector3(ptr.position.x, 1.64f, -10);
    }
}
