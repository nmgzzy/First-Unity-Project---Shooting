using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Wave[] waves;
    public Enemy enemy;

    LivingEntity playerEntity;
    Transform playerT;

    Wave currentWave;
    int currentWaveNumber;

    int enemiesRemainingToSpawn;
    int enemiesRemainingAlive;
    float nextSpawnTime;
    MapGenerator map;
    public event System.Action<int> OnNewWave;

    void Start()
    {
		playerEntity = FindObjectOfType<Player> ();
        playerT = playerEntity.transform;

        map = FindObjectOfType<MapGenerator>();
        NextWave();
    }

    void Update()
    {

        if (enemiesRemainingToSpawn > 0 && Time.time > nextSpawnTime)
        {
            enemiesRemainingToSpawn--;
            nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;
            StartCoroutine(SpawnEnemy());
        }
    }

    IEnumerator SpawnEnemy() {
        float spawnDelay = 1;
        float tileFlashSpeed = 4;

        Transform randomTile =  map.GetRandomOpenTile();
        Material tileMat = randomTile.GetComponent<Renderer>().material;
        Color initialColor = tileMat.color;
        Color flashColor = Color.red;
        float spawnTimer = 0;

        while (spawnTimer < spawnDelay)
        {
            tileMat.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));
            spawnTimer += Time.deltaTime;
            yield return null;
        }
        tileMat.color = initialColor;
        Enemy spawnedEnemy = Instantiate(enemy, randomTile.position + Vector3.up, Quaternion.identity) as Enemy;
        spawnedEnemy.OnDeath += OnEnemyDeath;
    }

    void OnEnemyDeath()
    {
        enemiesRemainingAlive--;

        if (enemiesRemainingAlive == 0)
        {
            NextWave();
        }
    }

    void ResetPlayer() {
		playerT.position = map.GetTileFromPosition (Vector3.zero).position + Vector3.up * 3;
        playerEntity.Revive();
	}

    void NextWave()
    {
        if (currentWaveNumber < waves.Length)
        {
            currentWave = waves[currentWaveNumber];

            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesRemainingAlive = enemiesRemainingToSpawn;

            if (OnNewWave != null) {
				OnNewWave(currentWaveNumber);
			}
			ResetPlayer();
        }
        currentWaveNumber++;
    }

    [System.Serializable]
    public class Wave
    {
        public int enemyCount;
        public float timeBetweenSpawns;
    }
}
