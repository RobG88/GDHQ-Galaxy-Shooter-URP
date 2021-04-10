using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance;

    [SerializeField] GameObject _enemyPrefab;
    [SerializeField] GameObject _enemyContainer;
    //[SerializeField] GameObject[] _powerUpPrefabs;
    //[SerializeField] GameObject _powerUpContainer;

    float _waitTimeBetweenEnemySpawns;
    //float _waitTimeBetweenPowerUpSpawns;
    float _delayAfterAsteroidDestroyed = 2.5f;
    bool _playerIsAlive = true;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //Spawn();
    }
    public void Spawn()
    {
        StartCoroutine(SpawnEnemyRoutine());
        //StartCoroutine(SpawnPowerUpRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(_delayAfterAsteroidDestroyed);

        while (_playerIsAlive)
        {
            _waitTimeBetweenEnemySpawns = Random.Range(0.5f, 3.0f);
            GameObject newEnemy = Instantiate(_enemyPrefab, new Vector3(0, 10, 0), Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(_waitTimeBetweenEnemySpawns);
        }
    }

    public void OnPlayerDeath()
    {
        _playerIsAlive = false;
    }
}