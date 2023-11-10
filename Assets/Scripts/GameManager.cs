using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public Transform[] spawnPoints;
    public GameObject[] enemyTypes;
    public float timeBetweenWaves = 10f;
    private int currentWave = 0;

    public delegate void textWave(int number);
    public static textWave wave;

    private void Start()
    {
        // Comienza el sistema de oleadas
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenWaves);

            currentWave++;
            wave.Invoke(currentWave);

            if (currentWave % 2 == 0)
            {
                SpawnEnemies(enemyTypes[0]);
            }
            else
            {
                SpawnEnemies(enemyTypes[1]);
            }
        }
    }

    void SpawnEnemies(GameObject enemyPrefab)
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        }
    }
}
