using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;

    [SerializeField] int _playerLives = 3;
    [SerializeField] float _speed; // player/ship movement speed
    [SerializeField] float _spaceshipSpeed = 15.0f;  // player/ship BASE CONSTANT speed
                                                     // speed lost for damage

    bool _enableMainThrusters;

    bool isGameOver = false;

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

    /// <summary>
    /// PowerUp Variables
    /// Tripleshot
    [SerializeField] bool _powerUp_Tripleshot;
    [SerializeField] GameObject _tripleshotPrefab;

    /// Speed
    bool _speedActive = false;
    /// Shield
    /// LaserEnergy
    /// Repair
    /// Ultimate
    /// </summary>


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
        _playerLives = 3;
        isGameOver = false;
        transform.position = new Vector3(0, -7, 0); // offical game (0, -3.5f, 0);
        ////transform.position = new Vector3(0.3834784f, -5, 0); // exactly fire two lasers into one enemy
        UI.instance.DisplayLives(_playerLives);
        UI.instance.DisplayShipWrapStatus();

        _speed = _spaceshipSpeed; // initialize Ship/Player speed
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _nextFire)
        {
            FireLaser();
        }

        // Cheat Keys:
        // 
        // Q = Enable ship wrapping left/right
        // Below is for testing purposes only
        //
        // G = Enable GOD mode
        //
        //if (Input.GetKeyDown(KeyCode.Q)) { _wrapShip = !_wrapShip; }
        if (Input.GetKeyDown(KeyCode.Q)) { _wrapShip = !_wrapShip; UI.instance.SetCheatKey(_wrapShip); UI.instance.DisplayShipWrapStatus(); }
        if (Input.GetKeyDown(KeyCode.G)) { _cheat_GODMODE = !_cheat_GODMODE; }
        if (Input.GetKeyDown(KeyCode.T)) { _cheat_TRIPLE = !_cheat_TRIPLE; _powerUp_Tripleshot = _cheat_TRIPLE; }
        CalculateMovement();
    }

    void CalculateMovement()
    {
        _speed = CalculateShipSpeed();

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
        if (!_powerUp_Tripleshot)
        {
            GameObject laser1 = Instantiate(_laserPrefab, _gunLeft.position, Quaternion.identity);
            GameObject laser2 = Instantiate(_laserPrefab, _gunRight.position, Quaternion.identity);
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

        _playerLives--;
        if (_playerLives == 0)
        {
            PlayerDeath();
        }

        UI.instance.DisplayLives(_playerLives);
    }

    void PlayerDeath()
    {
        GameManager.instance.OnPlayerDeath();
        isGameOver = true;
        UI.instance.GameOver(isGameOver);
        WaveSpawner.instance.OnPlayerDeath();
        Destroy(this.gameObject, 0.25f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            Damage();
        }

        if (other.tag == "PowerUp")
        {
            string PowerUpToActivate;
            PowerUpToActivate = other.transform.GetComponent<PowerUp>().PowerType().ToString();
            // TODO: Need to Handle Multiple PowerUps?
            ActivatePowerUp(PowerUpToActivate);
        }
    }

    public void Activate_PowerUp_Tripleshot()
    {
        StartCoroutine(ActivatePowerupTripleshot());
    }

    IEnumerator ActivatePowerupTripleshot()
    {
        _powerUp_Tripleshot = true;
        yield return new WaitForSeconds(5f);
        _powerUp_Tripleshot = false;
    }

    public void Activate_PowerUp_SpeedBoost()
    {
        StartCoroutine(ActivatePowerupSpeedBoost());
    }

    IEnumerator ActivatePowerupSpeedBoost()
    {
        _speedActive = true;
        yield return new WaitForSeconds(5f);
        _speedActive = false;
    }

    IEnumerator ActivePowerUp(int timer)
    {
        yield return new WaitForSeconds(timer);
    }

    void ActivatePowerUp(string _powerUpType) // PowerUp activations
    {
        //_timesUpText.text = _powerUpType;

        switch (_powerUpType)
        {
            case "TripleShot":
                Activate_PowerUp_Tripleshot();
                /*
                _tripleShotActive = true;
                LaserCannonsRefill(15);
                //_timesUpText.text = _powerUpType;
                // Enable PowerUpCountDownBar
                _powerUpCountDownBar.SetActive(true);
                StartCoroutine(PowerUpCoolDownRoutine(_tripleShotCoolDown));
                */
                break;
            case "Speed":
                Activate_PowerUp_SpeedBoost();
                /*
                // Enable PowerUpCountDownBar
                _powerUpCountDownBar.SetActive(true);
                StartCoroutine(PowerUpCoolDownRoutine(_speedCoolDown));
                */
                break;
                /*
                    /// 
                    /// SHIELD POWERUP
                    /// 
                    case "Shield":
                        _shieldPower = 3; // # of hits before shield is destroyed
                        _shield.transform.localScale = _shieldOriginalSize; // reset shield graphic to initial size
                        _shieldActive = true;
                        _shield.SetActive(true); // enable the Shield gameObject
                        break;
                    /// 
                    /// SHIELD POWERUP - END
                    /// 
                    case "EnergyCell":
                        LaserCannonsRefill(5);
                        break;
                    ///
                    /// REPAIR 'Health' POWERUP
                    /// 
                    case "Repair":
                        RepairShip();
                        break;
                    ///
                    /// REPAIR 'Health' POWERUP - END
                    /// 

                    ///
                    /// FREEZE/EMP TORPEDO POWERUP
                    /// 
                    case "FreezeTorpedo":
                        _freezeTorpedoLoaded = true;
                        _freezeTorpedoSprite.SetActive(true);
                        break;
                        ///
                        /// FREEZE/EMP TORPEDO POWERUP - END
                        ///
                */
        }
    }

    float CalculateShipSpeed() // Ship's speed = _spaceshipSpeed, calc PowerUp, damage 
    {
        var _newSpeed = _spaceshipSpeed;

        if (_playerLives == 2)
        {
            _newSpeed = _spaceshipSpeed - 1;
        }

        if (_playerLives == 1)
        {
            _newSpeed = _spaceshipSpeed - 2;
        }

        if (_speedActive) // PowerUp = speed * 175%
        {
            _newSpeed = _spaceshipSpeed * 1.75f;
        }
        ///
        /// THRUSTERS - SPEED CALC
        /// 
        if (_enableMainThrusters) // Thrusters = speed * 250%
        {
            _newSpeed = _spaceshipSpeed * 2.50f;
        }
        ///
        /// THRUSTERS - SPEED CALC - END
        /// 
        return _newSpeed;
    }
}