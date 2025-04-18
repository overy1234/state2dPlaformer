
using System.Collections.Generic;
using UnityEngine;

public class Blackhole_Skill_Controller : MonoBehaviour
{
    [SerializeField] private GameObject hotKeyPrefab;
    [SerializeField] private List<KeyCode> keyCodeList;

    public float maxSize;
    public float growSpeed;
    public float shrinkSpeed;
    public bool canShrink;
    public bool canGrow;

    private bool canCreateHotKeys = true;
    private bool cloneAttackReleased;
    public int amountOfAttacks = 4;
    public float cloneAttackCooldown = 0.3f;
    private float cloneAttackTimer;



    private List<Transform> targets = new List<Transform>();
    private List<GameObject> createHotKey = new List<GameObject>();
    
    void Update()
    {
        cloneAttackTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.G))
        {
            DestroyHotKeys();
            cloneAttackReleased = true;
            canCreateHotKeys = false;
        }

        if (cloneAttackTimer < 0 && cloneAttackReleased)
        {
            cloneAttackTimer = cloneAttackCooldown;

            int randomIndex = Random.Range(0, targets.Count);


            float xOffset;
            if (Random.Range(0, 100) > 50)
                xOffset = 2;
            else
                xOffset = -2;


            SkillManager.instance.clone.CreateClone(targets[randomIndex],new Vector3( xOffset,0));

            amountOfAttacks--;

            if(amountOfAttacks <= 0)
            {
                canShrink = true;
                cloneAttackReleased = false;
            }

        }






        
        if(canGrow && !canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);
        }

        if(canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(-1, -1), shrinkSpeed * Time.deltaTime);

            if (transform.localScale.x < 0)
                Destroy(gameObject);
        }


    }



    private void DestroyHotKeys()
    {
        if (createHotKey.Count <= 0)
            return;

        for(int i=0; i< createHotKey.Count; i++)
        {
            Destroy(createHotKey[i]);
        }

    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Enemy>() != null)
        {
            collision.GetComponent<Enemy>().FreezeTime(true);

            CreateHotKey(collision);

        }
    }

    private void CreateHotKey(Collider2D collision)
    {

        if (keyCodeList.Count <= 0)
        {

            Debug.LogWarning("키등록 까먹었는지 체크");
            return;

        }

        if (!canCreateHotKeys)
            return;

        GameObject newHotKey = Instantiate(hotKeyPrefab, collision.transform.position + new Vector3(0, 2), Quaternion.identity);
        createHotKey.Add(newHotKey);



        KeyCode choosenKey = keyCodeList[Random.Range(0, keyCodeList.Count)];
        keyCodeList.Remove(choosenKey);

        Blackhole_HotKey_Controller newHotKeyScript = newHotKey.GetComponent<Blackhole_HotKey_Controller>();

        newHotKeyScript.SetupHotKey(choosenKey, collision.transform, this);
    }


    public void AddEnemyToList(Transform _enemyTransform) => targets.Add(_enemyTransform);


}
