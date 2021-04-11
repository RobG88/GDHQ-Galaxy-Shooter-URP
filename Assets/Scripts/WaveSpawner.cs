using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string name;
        public GameObject enemyPrefab;
        public int enemyCount;
        public float spawnRate;
        public float delayBetweenEnemySpawns; // seconds between enemy spawning
    }

    public static WaveSpawner instance;
    public enum SpawnState { SPAWNING, WAITING, COUNTING }
    [SerializeField] SpawnState _spawnState = SpawnState.COUNTING;

    public Wave[] waves;

    int _nextWave = 0; // current wave number
    int _currentEnemies; // track # of spawned enemies in wave

    [SerializeField] float _timeBetweenWaves = 15.0f;
    [SerializeField] float _waveCountdown;
    [SerializeField] GameObject _enemyContainer;

    [SerializeField] GameObject[] _powerUpPrefabs;
    [SerializeField] GameObject _powerUpContainer;
    float _waitTimeBetweenPowerUpSpawns;
    [SerializeField]float _spawnNextPowerup = -1.0f;  // game time value, tracking when a powerup can be spawned
    float _spawnPowerRate; // random time between min & max;


    float _delayAfterAsteroidDestroyed = 2.5f;

    bool _playerIsAlive = true;  // as long as playerIsAlive keep spawning current wave
    bool _beginCountdown = true;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        _waveCountdown = _timeBetweenWaves;
    }

    void Update()
    {
        if (_spawnState == SpawnState.WAITING)
        {
            if (_currentEnemies == 0)
            {
                WaveCompleted();
            }
            else
                return;
        }

        if (_waveCountdown <= 0)
        {
            if (_spawnState != SpawnState.SPAWNING)
            {
                UI.instance.WaveCountdownEnableUI(_beginCountdown, (int)_waveCountdown, waves[_nextWave].name);
                // Begin spawning the wave
                StartCoroutine(SpawnWave(waves[_nextWave]));
            }
        }
        else
        {
            // Countdown
            if (_beginCountdown)
            {
                UI.instance.DisplayLevel(_nextWave + 1);
                UI.instance.WaveCountdownEnableUI(_beginCountdown, (int)_waveCountdown, waves[_nextWave].name);
                _beginCountdown = false;
            }

            if (_waveCountdown > 0)
            {
                UI.instance.WaveCountdown((int)_waveCountdown);
            }

            _waveCountdown -= Time.deltaTime;
        }
    }

    IEnumerator SpawnWave(Wave _wave)
    {
        _spawnState = SpawnState.SPAWNING;

        //SpawnManager.instance.StopOldPowerUpSwawnRoutine(false);
        //SpawnManager.instance.SpawnPowerup(true);
        //StartCoroutine(SpawnManager.instance.SpawnPowerUpRoutine());
        StartCoroutine("SpawnPowerUpRoutine");
        /*
        if (Time.time > _spawnNextPowerup)
        {
            int _RNDPowerUp = Random.Range(0, _powerUpPrefabs.Length);
            GameObject newPowerUp = Instantiate(_powerUpPrefabs[_RNDPowerUp], new Vector3(Random.Range(-6, 6), Random.Range(12, 14), 0), Quaternion.identity);
            newPowerUp.transform.parent = _powerUpContainer.transform;
            _spawnPowerRate = Random.Range(4f, 9f);
            _spawnNextPowerup = Time.time + _spawnPowerRate;
        }
        */
        // Spawn enemies
        for (int i = 0; i < _wave.enemyCount; i++)
        {
            _currentEnemies++;
            SpawnEnemy(_wave.enemyPrefab);
            UI.instance.DisplayEnemies(_currentEnemies, _wave.enemyCount);
            // yield return new WaitForSeconds(1f / _wave.spawnRate);
            yield return new WaitForSeconds(_wave.delayBetweenEnemySpawns);

            if (!_playerIsAlive) { yield break; }
        }

        _spawnState = SpawnState.WAITING;

        yield break;
    }

    void SpawnEnemy(GameObject _enemyPrefab)
    {
        Debug.Log("Spawning Enemy: " + _enemyPrefab.name);
        GameObject newEnemy = Instantiate(_enemyPrefab, new Vector3(0, 10, 0), Quaternion.identity);
        newEnemy.transform.parent = _enemyContainer.transform;
    }

    void WaveCompleted()
    {
        // Begin a new Wave
        // Wave Over
        // Give Name of next wave
        // Start a wave countdown
        // Depending on performance a bonus
        Debug.Log("All Enemies Destroyed --- Wave Completed!");

        //SpawnManager.instance.SpawnPowerup(false);
        //SpawnManager.instance.StopOldPowerUpSwawnRoutine(true);
        //StopCoroutine(SpawnManager.instance.SpawnPowerUpRoutine());
        StopCoroutine("SpawnPowerUpRoutine");
        _spawnState = SpawnState.COUNTING;
        _waveCountdown = _timeBetweenWaves;
        _currentEnemies = 0;
        _beginCountdown = true;

        if (_nextWave + 1 > waves.Length - 1)
        {
            _nextWave = 0;

            // Because all waves are completed
            // Game difficulty could be increased by an enemy stat multiplier
            // Earn perks, bonus, shields, weapons, defense, bombs, nukes, specials

            Debug.Log("All WAVES Complete! ... Loopinng");

            // Game Completed rather then looping
            // Begin a new scene ... new level of the game
        }
        else
        {
            _nextWave++; // increment the NextWave Index
        }

        return;
    }


    public void EnemyDeath()
    {
        _currentEnemies--;
        UI.instance.DisplayEnemies(_currentEnemies, waves[_nextWave].enemyCount);
    }

    public void OnPlayerDeath()
    {
        _playerIsAlive = false;
    }

    public IEnumerator SpawnPowerUpRoutine()
    {
        //yield return new WaitForSeconds(_delayAfterAsteroidDestroyed + Random.Range(4.0f, 8.0f));
        //_SpawnPowerups = true;
        //Debug.Log("Spawning PowerUps: STARTED");
        //SpawnCount++;

        while (GameManager._playerIsAlive)
        {

            //Debug.Log("Spawning PowerUps: SPAWNING");
            _waitTimeBetweenPowerUpSpawns = Random.Range(9.0f, 15.0f);
            int _RNDPowerUp = Random.Range(0, _powerUpPrefabs.Length);

            //Debug.Log("WaitBetweenPowerUpSpawns = " + _waitTimeBetweenPowerUpSpawns);
            //Debug.Log("PowerUp = " + _powerUpPrefabs[_RNDPowerUp]);

            // TODO: if Shield is active do not spawn another

            GameObject newPowerUp = Instantiate(_powerUpPrefabs[_RNDPowerUp], new Vector3(Random.Range(-6, 6), Random.Range(7, 14), 0), Quaternion.identity);
            newPowerUp.transform.parent = _powerUpContainer.transform;
            yield return new WaitForSeconds(_waitTimeBetweenPowerUpSpawns);
            //if (_StopOldPowerUpSpawnRoutine) yield return null;
        }
        //Debug.Log("Spawning PowerUps: STOPPED");
    }
}
