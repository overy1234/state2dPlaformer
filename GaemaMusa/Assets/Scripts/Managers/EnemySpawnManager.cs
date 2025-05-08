using UnityEngine;
using System.Collections.Generic;

public class EnemySpawnManager : MonoBehaviour
{
    public static EnemySpawnManager instance;

    [SerializeField] private GameObject enemyPrefab;        // 생성할 적 프리팹
    [SerializeField] private Transform[] spawnPoints;       // 스폰 위치들
    [SerializeField] private float spawnInterval = 3f;      // 스폰 간격 (초)
    [SerializeField] private int maxEnemies = 5;           // 최대 적 수

    private float spawnTimer;
    private List<GameObject> activeEnemies = new List<GameObject>();

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    private void Update()
    {
        // 죽은 적들을 리스트에서 제거
        activeEnemies.RemoveAll(enemy => enemy == null);

        // 타이머 업데이트 및 스폰 체크
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0 && activeEnemies.Count < maxEnemies)
        {
            SpawnEnemy();
            spawnTimer = spawnInterval;
        }
    }

    private void SpawnEnemy()
    {
        if (spawnPoints.Length == 0) return;

        // 랜덤 스폰 포인트 선택
        int randomPoint = Random.Range(0, spawnPoints.Length);
        Vector3 spawnPosition = spawnPoints[randomPoint].position;

        // 적 생성
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        activeEnemies.Add(newEnemy);
    }

    // 현재 활성화된 적의 수 반환
    public int GetCurrentEnemyCount()
    {
        return activeEnemies.Count;
    }
}