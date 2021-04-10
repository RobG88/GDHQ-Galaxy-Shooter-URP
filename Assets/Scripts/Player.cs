using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;

    [SerializeField] int _lives = 3;

    bool isGameOver = false;
    [SerializeField] float _speed = 15f;
    [SerializeField] float _fireRate = 0.15f;  // delay (in Seconds) how quickly the laser will fire
    float _nextFire = -1.0f;  // game time value, tracking when player/ship can fire next laser

    [SerializeField] GameObject _laserPrefab;
    [SerializeField] Transform _gunLeft, _gunRight, _gunCenter;
    [SerializeField] Vector3 _laserOffset = new Vector3(0, 1.20f, 0); // distance offset when spawning laser Y-direction
    [SerializeField] Vector3 _cannonOffset; // in the event that two lasers strike the same collider, then give one cannon a slightly elevated offset.
    Animator anim;

    bool _wrapShip = false; // Q = toggle wrap
    float _xScreenClampRight = 18.75f;
    float _xScreenClampLeft = -18.75f;
    float _yScreenClampUpper = 0;
    float _yScreenClampLower = -7f; // offical game = -3.8f;

    bool _powerup_Tripleshot;
    [SerializeField] GameObject _tripleshotPrefab;

    // CHEAT KEYS
    //
    // G = GOD mode
    bool _cheat_GODMODE = false;
    // T = TRIPLESHOT
    bool _cheat_TRIPLE = false;

    private void Awake()
    {
        instance = this;
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        _lives = 3;
        isGameOver = false;
        transform.position = new Vector3(0, -7, 0); // offical game (0, -3.5f, 0);
        ////transform.position = new Vector3(0.3834784f, -5, 0); // exactly fire two lasers into one enemy
        //UI.instance.DisplayLives(_lives);
        //UI.instance.DisplayShipWrapStatus();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _nextFire)
        {
            FireLaser();
            //anim.SetBool("fire", true);
            ////anim.SetTrigger("isFiring");
        }

        // Cheat Keys:
        // 
        // Q = Enable ship wrapping left/right
        // Below is for testing purposes only
        //
        // G = Enable GOD mode
        //
        if (Input.GetKeyDown(KeyCode.Q)) { _wrapShip = !_wrapShip; }
        //if (Input.GetKeyDown(KeyCode.Q)) { _wrapShip = !_wrapShip; UI.instance.SetCheatKey(_wrapShip); UI.instance.DisplayShipWrapStatus(); }
        if (Input.GetKeyDown(KeyCode.G)) { _cheat_GODMODE = !_cheat_GODMODE; }
        if (Input.GetKeyDown(KeyCode.T)) { _cheat_TRIPLE = !_cheat_TRIPLE; _powerup_Tripleshot = _cheat_TRIPLE; }
        CalculateMovement();
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        transform.Translate(direction * _speed * Time.deltaTime);

        // Player Boundaries 
        // Clamp Ship's Y pos
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, _yScreenClampLower, _yScreenClampUpper), 0);

        // Clamp xPos
        if (_wrapShip && transform.position.x > _xScreenClampRight) // Wrap ship
        {
            transform.position = new Vector3(_xScreenClampLeft, transform.position.y, 0);
        }
        else if (!_wrapShip && transform.position.x > _xScreenClampRight) // Lock pos
        {
            transform.position = new Vector3(_xScreenClampRight, transform.position.y, 0);
        }

        // or Wrap Ship's X pos
        if (_wrapShip && transform.position.x < _xScreenClampLeft) // Wrap ship
        {
            transform.position = new Vector3(_xScreenClampRight, transform.position.y, 0);
        }
        else if (!_wrapShip && transform.position.x < _xScreenClampLeft) // Lock pos 
        {
            transform.position = new Vector3(_xScreenClampLeft, transform.position.y, 0);
        }
    }

    void FireLaser()
    {
        _nextFire = Time.time + _fireRate; // delay (in Seconds) how quickly the laser will fire
        if (!_powerup_Tripleshot)
        {
            GameObject laser1 = Instantiate(_laserPrefab, _gunLeft.position, Quaternion.identity);
            //laser1.transform.parent = transform;
            GameObject laser2 = Instantiate(_laserPrefab, _gunRight.position, Quaternion.identity);
            //laser2.transform.parent = transform;
        }
        else
        {
            GameObject tripleshot = Instantiate(_tripleshotPrefab, _gunCenter.position, Quaternion.identity);
        }
    }

    public void FireLaserCanon()
    {
        anim.SetBool("fire", false);
        FireLaser();
    }

    void Damage()
    {
        if (_cheat_GODMODE) return;

        _lives--;
        if (_lives == 0)
        {
            PlayerDeath();
        }
        UI.instance.DisplayLives(_lives);
    }

    void PlayerDeath()
    {
        isGameOver = true;
        UI.instance.GameOver(isGameOver);
        //SpawnManager.instance.OnPlayerDeath();
        WaveSpawner.instance.OnPlayerDeath();
        Destroy(this.gameObject, 0.25f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            Damage();
        }

        if (other.tag == "Powerup_Tripleshot")
        {
            //TripleshotActivate();
        }
    }

    public void TripleshotActivate()
    {
        _powerup_Tripleshot = !_powerup_Tripleshot;
        StartCoroutine(ActivatePowerupTripleshot());
    }

    IEnumerator ActivatePowerupTripleshot()
    {
        yield return new WaitForSeconds(5f);

        _powerup_Tripleshot = !_powerup_Tripleshot;
    }
}