using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //[SerializeField] int _scoreValue = 0;
    [SerializeField] float _enemySpeed = 4.0f;
    //[SerializeField] float _enemyLaser = 6.0f;  // speed of enemy's laser
    //[SerializeField] GameObject _enemyLaserPrefab;

    [SerializeField] GameObject _enemyInvaderExplosion;
    [SerializeField] bool CHEAT_LINE_THEM_UP = false;

    BoxCollider2D enemyCollider;

    //float _fireRate = 3.0f;
    //float _canFire = -1.0f;
    bool _isDestroyed = false; // if enemy is hit by player/ship Laser then isDestroyed = true, enemy is put back into pool

    float _enemyReSpawnThreshold = -10.0f; // Game Screen threshold, once enemy is beyond this point and has been destroyed it will respawn 
    Vector3 _enemyPos = new Vector3(0, 0, 0); // Random position of enemy once re-spawned X(-8,8) Y(12,9)

    float _respawnXmin = -9.0f;
    float _respawnXmax = 9.0f;
    float _respawnYmin = 14.0f;
    float _respawnYmax = 19.0f;

    void Awake()
    {
        enemyCollider = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        Spawn();
    }

    void Update()
    {
        CalculateMovement();
    }

    private void CalculateMovement()
    {
        transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);
        if (transform.position.y < _enemyReSpawnThreshold && !_isDestroyed)
        {
            transform.position = SpawnEnmeyAtRandomLocation();
        }
    }

    Vector3 SpawnEnmeyAtRandomLocation()
    {
        return (_enemyPos = new Vector3(Random.Range(_respawnXmin, _respawnXmax), Random.Range(_respawnYmin, _respawnYmax), 0));
    }

    void Spawn()
    {
        float respawnX = Random.Range(_respawnXmin, _respawnXmax);
        _enemyPos.x = respawnX;
        if (CHEAT_LINE_THEM_UP)
        {
            _enemyPos.x = 0;  // testing, line enemy for two lasers intersecting collider}
        } 
        _enemyPos.y = Random.Range(_respawnYmin, _respawnYmax);
        transform.position = _enemyPos;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Laser") || other.CompareTag("Player"))
        {
            enemyCollider.enabled = false; // disable collider so two lasers can not collider at the same time

            Instantiate(_enemyInvaderExplosion, transform.position, Quaternion.identity);

            Destroy(this.gameObject);
        }
    }

    void OnDestroy()
    {
        WaveSpawner.instance.EnemyDeath();
    }
}
