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

    int _score = 0;

    [SerializeField] GameObject _thruster_left;
    [SerializeField] GameObject _thruster_right;
    [SerializeField] GameObject _thrusters;

    Vector3 _originalThrustersLocalScale;
    Vector3 _leftThrusterOriginalPos;
    Vector3 _rightThrusterOriginalPos;
    Vector3 ThrusterOffset = new Vector3(0, -0.5f, 0);
    Vector3 _newLeft;
    Vector3 _newRight;

    [SerializeField] GameObject _shipDamageLeft;
    [SerializeField] GameObject _shipDamageRight;
    bool _damagedLeft = false;
    bool _damagedRight = false;
    Animator _animShipDamageLeft;
    Animator _animShipDamageRight;

    bool _enableMainThrusters;

    bool isGameOver = false;

    [SerializeField] float _fireRate = 0.15f;  // delay (in Seconds) how quickly the laser will fire
    float _nextFire = -1.0f;  // game time value, tracking when player/ship can fire next laser

    [SerializeField] GameObject _laserPrefab;
    [SerializeField] Transform _gunLeft, _gunRight, _gunCenter;

    Animator _anim;
    SpriteRenderer _spriteRenderer;
    BoxCollider2D _boxCollider2D;

    bool _wrapShip = false; // Q = toggle wrap
    float _xScreenClampRight = 18.75f;
    float _xScreenClampLeft = -18.75f;
    float _yScreenClampUpper = 0;
    float _yScreenClampLower = -7f; // offical game = -3.8f;

    /// 
    /// POWERUP VARIABLES
    /// 
    /// TRIPLESHOT VARIABLES - BEGIN
    /// 
    [SerializeField] bool _powerUp_Tripleshot;
    [SerializeField] GameObject _tripleshotPrefab;
    /// 
    /// TRIPLESHOT VARIABLES - END
    /// 

    /// 
    /// SPEED VARIABLES
    /// 
    bool _speedActive = false;
    /// 
    /// SPEED VARIABLES - END
    /// 

    /// 
    /// SHIELD VARIABLES
    /// 
    [SerializeField] bool _shieldActive = false;
    [SerializeField] GameObject _shield;
    //[SerializeField] int _shieldPower;
    //[SerializeField] int _shieldBonus;
    //Vector3 _shieldOriginalSize;
    PlayerShields _playerShield;
    /// 
    /// SHIELD VARIABLES - END
    /// 
    /// 
    /// 
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
        _anim = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        _playerLives = 3;
        isGameOver = false;
        transform.position = new Vector3(0, -5, 0); // offical game (0, -3.5f, 0);
        _originalThrustersLocalScale = _thruster_left.transform.localScale;
        ////transform.position = new Vector3(0.3834784f, -5, 0); // exactly fire two lasers into one enemy
        UI.instance.DisplayLives(_playerLives);
        UI.instance.DisplayShipWrapStatus();

        _speed = _spaceshipSpeed; // initialize Ship/Player speed

        /// Thrusters Left & Right
        /// 
        _leftThrusterOriginalPos = _thruster_left.transform.localPosition;
        _rightThrusterOriginalPos = _thruster_right.transform.localPosition;
        _newLeft = _leftThrusterOriginalPos + ThrusterOffset;
        _newRight = _rightThrusterOriginalPos + ThrusterOffset;

        ///
        /// SHIELDS VARIABLES INITIALIZE
        ///
        //_shieldOriginalSize = _shield.transform.localScale;
        ///
        /// SHIELDS VARIABLES INITIALIZE - END
        ///
    }

    IEnumerator RepeatLerp(Vector3 a, Vector3 b, float time)
    {

        yield return null;
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

        if (Input.GetKeyDown(KeyCode.X) && _shieldActive) { DropShield(); }
        CalculateMovement();
    }

    void CalculateMovement()
    {
        _speed = CalculateShipSpeed();

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        transform.Translate(direction * _speed * Time.deltaTime);


        ////////////////////////////////////////////////
        /// Thrusters Left & Right (not MAIN THRUSTER)
        // Use the verticalInput 'W' or UpArrow * 1.75 as Thruster localScale multiplier
        float thruster_y = verticalInput * .75f;

        if (verticalInput > 0.20f)
        {
            Debug.Log("Verticle Input = " + verticalInput);
            // Reset Thrusters/Afterburners to originalLocalScale so when Afterburners 
            // do not over extend graphically under Spaceship
            _thruster_left.transform.localScale = _originalThrustersLocalScale;
            _thruster_right.transform.localScale = _originalThrustersLocalScale;

            Vector3 thrusters = new Vector3(_thruster_left.transform.localScale.x,
                                            thruster_y,
                                            _thruster_left.transform.localScale.z);

            float _thrustersAnimation = Random.Range(1.25f, 1.50f);

            _thruster_left.transform.localScale = thrusters * _thrustersAnimation;
            _thruster_right.transform.localScale = thrusters * _thrustersAnimation;
        }
        else if (verticalInput < 0.20f) // if the verticalInput < .20 then 'flicker' thrusters
        {
            float _thrustersAnimation = Random.Range(1.25f, 1.50f);
            _thruster_left.transform.localScale = _originalThrustersLocalScale * _thrustersAnimation;
            _thruster_right.transform.localScale = _originalThrustersLocalScale * _thrustersAnimation;
        }

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
        _anim.SetBool("fire", false);
        FireLaser();
    }

    void Damage()
    {
        if (_cheat_GODMODE) return;

        if (_shieldActive)
        {
            _playerShield.Damage();
        }
        else
        {
            _playerLives--;

            if (_playerLives == 0)
            {
                PlayerDeath();
            }

            UI.instance.DisplayLives(_playerLives);
        }
    }

    void PlayerDeath()
    {
        _spriteRenderer.enabled = false;
        _boxCollider2D.enabled = false;

        GameManager.instance.OnPlayerDeath();
        isGameOver = true;
        UI.instance.GameOver(isGameOver);
        WaveSpawner.instance.OnPlayerDeath();
        Destroy(this.gameObject, 0.25f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Damage();
        }

        if (other.CompareTag("PowerUp"))
        {
            if (_shieldActive) return;

            string PowerUpToActivate;
            PowerUpToActivate = other.transform.GetComponent<PowerUp>().PowerType().ToString();

            // TODO: Need to Handle Multiple PowerUps?
            // TODO: Do not allow collecting Power-Ups when shield is enabled

            ActivatePowerUp(PowerUpToActivate);
        }
    }

    public void Activate_PowerUp_Tripleshot()
    {
        StartCoroutine(ActivatePowerupTripleshot());
    }

    IEnumerator ActivatePowerupTripleshot()
    {
        UI.instance.ActiveTripleShotUI();
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
        UI.instance.ActiveSpeedBoostUI();
        _speedActive = true;
        yield return new WaitForSeconds(5f);
        _speedActive = false;
    }

    public void Activate_PowerUp_Shields()
    {
        _shieldActive = true;
        //_shieldPower = 3;                                   // # of hits before shield is destroyed
        //_shield.transform.localScale = _shieldOriginalSize; // reset shield graphic to initial size
        _shield.SetActive(_shieldActive);                            // enable the Shield gameObject
        _playerShield = gameObject.GetComponentInChildren<PlayerShields>();
    }

    public void ShieldsDestroyed()
    {
        _shieldActive = false;
    }

    IEnumerator ActivatePowerupShields()
    {
        UI.instance.ActiveShieldsUI();
        _shieldActive = true;
        yield return new WaitForSeconds(5f);
        _shieldActive = false;
        _shield.SetActive(false);
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
            /// 
            /// SHIELD POWERUP
            /// 
            case "Shield":
                Activate_PowerUp_Shields();
                /*
                _shieldActive = true;
                _shieldPower = 3; // # of hits before shield is destroyed
                _shield.transform.localScale = _shieldOriginalSize; // reset shield graphic to initial size

                _shield.SetActive(true); // enable the Shield gameObject
                /*
                   Activate_PowerUp_Shields();
                */
                break;
                /// 
                /// SHIELD POWERUP - END
                /// 
                /*
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


    public void AddScore(int scoreAmount) // Update score, send to UI
    {
        _score += scoreAmount;
        UI.instance.UpdateScore(_score);
    }

    void DropShield()
    {
        _shieldActive = false;
        _shield.SetActive(false);
    }
}