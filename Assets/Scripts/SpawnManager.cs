using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance;

    [SerializeField] GameObject _enemyPrefab;
    [SerializeField] GameObject _enemyContainer;
    [SerializeField] GameObject[] _powerUpPrefabs;
    [SerializeField] GameObject _powerUpContainer;

    float _waitTimeBetweenEnemySpawns;
    float _waitTimeBetweenPowerUpSpawns;
    float _delayAfterAsteroidDestroyed = 2.5f;
    bool _SpawnPowerups = false;
    [SerializeField]bool _StopOldPowerUpSpawnRoutine = false;

    [SerializeField] int SpawnCount = 0;
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
        //StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerUpRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(_delayAfterAsteroidDestroyed);

        while (GameManager._playerIsAlive)
        {
            _waitTimeBetweenEnemySpawns = Random.Range(0.5f, 3.0f);
            GameObject newEnemy = Instantiate(_enemyPrefab, new Vector3(0, 10, 0), Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(_waitTimeBetweenEnemySpawns);
        }
    }

    public IEnumerator SpawnPowerUpRoutine()
    {
        //yield return new WaitForSeconds(_delayAfterAsteroidDestroyed + Random.Range(4.0f, 8.0f));
        //_SpawnPowerups = true;
        Debug.Log("Spawning PowerUps: STARTED");
        SpawnCount++;

        while (GameManager._playerIsAlive && _SpawnPowerups)
        {

            Debug.Log("Spawning PowerUps: SPAWNING");
            _waitTimeBetweenPowerUpSpawns = Random.Range(9.0f, 15.0f);
            int _RNDPowerUp = Random.Range(0, _powerUpPrefabs.Length);

            Debug.Log("WaitBetweenPowerUpSpawns = " + _waitTimeBetweenPowerUpSpawns);
            Debug.Log("PowerUp = " + _powerUpPrefabs[_RNDPowerUp]);
            // TODO: if Shield is active do not spawn another

            GameObject newPowerUp = Instantiate(_powerUpPrefabs[_RNDPowerUp], new Vector3(Random.Range(-6, 6), Random.Range(7, 14), 0), Quaternion.identity);
            newPowerUp.transform.parent = _powerUpContainer.transform;
            yield return new WaitForSeconds(_waitTimeBetweenPowerUpSpawns);
            if (_StopOldPowerUpSpawnRoutine) yield return null;
        }
        Debug.Log("Spawning PowerUps: STOPPED");
    }


    public void SpawnPowerup(bool SpawnPowerUpState)
    {
        _SpawnPowerups = SpawnPowerUpState;
    }

    public void StopOldPowerUpSwawnRoutine (bool StopRoutineState)
    {
        _StopOldPowerUpSpawnRoutine = StopRoutineState;
    }
}