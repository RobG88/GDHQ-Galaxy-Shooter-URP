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

    void WaveCompleted()
    {
        // Begin a new Wave
        // Wave Over
        // Give Name of next wave
        // Start a wave countdown
        // Depending on performance a bonus
        Debug.Log("All Enemies Destroyed --- Wave Completed!");

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

    IEnumerator SpawnWave(Wave _wave)
    {
        _spawnState = SpawnState.SPAWNING;

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

    public void EnemyDeath()
    {
        _currentEnemies--;
        UI.instance.DisplayEnemies(_currentEnemies, waves[_nextWave].enemyCount);
    }

    public void OnPlayerDeath()
    {
        _playerIsAlive = false;
    }
}
