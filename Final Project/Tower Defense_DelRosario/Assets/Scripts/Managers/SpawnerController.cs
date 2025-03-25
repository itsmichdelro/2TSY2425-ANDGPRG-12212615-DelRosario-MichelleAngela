using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerController : MonoBehaviour // tracker and handles spawns, despawns
{
    public static SpawnerController Instance;
    public static int enemiesAlive = 0;
    [SerializeField] GameObject[] enemyPrefab; // create an array bc we have multiple prefabs
    [SerializeField] Transform spawnPoint;
    public List<GameObject> enemyList = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnEnemy(int index) // pass in index of enemyPrefab to know which one to spawn
    {
        GameObject enemyObj = (GameObject)Instantiate(enemyPrefab[index]);
        enemyObj.transform.position = spawnPoint.position;
        enemyObj.GetComponent<Enemy>().SetTarget(GameManager.Instance.CrystalCore.transform);
        enemyList.Add(enemyObj); //add to enemy spawn list
        enemiesAlive++;
    }

    public IEnumerator SpawnNormalEnemy(int index, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            SpawnEnemy(index);
            yield return new WaitForSeconds(1f);
        }
    }

    public void SpawnBossEnemy()
    {
        SpawnEnemy(3); //dragon
    }

    public void SpawnEasyWave(int variant)
    {
        if (variant == 1)
        {
            StartCoroutine(SpawnNormalEnemy(0, 5));
            StartCoroutine(SpawnNormalEnemy(1, 15));
            StartCoroutine(SpawnNormalEnemy(2, 5));
        }
        if (variant == 2)
        {
            StartCoroutine(SpawnNormalEnemy(0, 5));
            StartCoroutine(SpawnNormalEnemy(1, 10));
            StartCoroutine(SpawnNormalEnemy(2, 20));
        }
    }

    public void SpawnMediumWave(int variant)
    {
        if (variant == 1)
        {
            StartCoroutine(SpawnNormalEnemy(0, 25));
            StartCoroutine(SpawnNormalEnemy(1, 20));
        }
        if (variant == 2)
        {
            StartCoroutine(SpawnNormalEnemy(0, 25));
            StartCoroutine(SpawnNormalEnemy(1, 15));
            StartCoroutine(SpawnNormalEnemy(2, 15));
        }
    }

    public void SpawnBossWave()
    {
       
        StartCoroutine(SpawnMultipleBosses(5));

        StartCoroutine(SpawnNormalEnemy(0, 20));
        StartCoroutine(SpawnNormalEnemy(1, 20));
        StartCoroutine(SpawnNormalEnemy(2, 20));
    }

    private IEnumerator SpawnMultipleBosses(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnBossEnemy();
            yield return new WaitForSeconds(0.5f); // Half second delay between boss spawns
        }
    }
 

    public void RemoveEnemyFromList(GameObject obj)
    {
        enemyList.Remove(obj); // remove enemy game object from list
        enemiesAlive--;
    }

    public List<GameObject> GetEnemyList() { return enemyList; }
}
