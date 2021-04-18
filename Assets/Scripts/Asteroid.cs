using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public enum AsteroidBehaviorType { Giant, Sin, Cos, Tan, Sinh, Cosh, Tanh };
    public AsteroidBehaviorType AsteroidBehavior;

    //[SerializeField] float _rotateSpeed = 3.0f;
    [SerializeField] int _rotateSpeed = 3;
    [SerializeField] float _speed;
    [SerializeField] GameObject _asteroid;
    [SerializeField] GameObject _explosionFX;

    int _minAsteroids = 2;
    int _maxAsteroids = 6;
    float _xScreenClampRight = 18.75f;
    float _xScreenClampLeft = -18.75f;
    float _yScreenClampUpper = 12.25f;
    float _yScreenClampLower = -10.50f; // offical game = -3.8f;
    Animator _anim;
    GameManager _gameManager;
    AudioSource _audioSource;
    //AudioManager _audioManager;
    Player _player;

    CircleCollider2D _asteroidCollider2D;
    SpriteRenderer _asteroidSprite;

    [SerializeField] bool _initialAsteroid = false;

    void Start()
    {
        _asteroidCollider2D = GetComponent<CircleCollider2D>();
        _asteroidSprite = GetComponent<SpriteRenderer>();

        if (AsteroidBehavior == AsteroidBehaviorType.Giant)
        {
            ////SetAsteroidAttributes();
            //transform.localScale = new Vector3(Random.Range(2.0f, 3f), Random.Range(1.75f, 2.65f));
            transform.localScale = new Vector3(Random.Range(3.5f, 4f), Random.Range(2.75f, 4.65f));
        }
        else
        {
            _rotateSpeed = 0;// Random.Range(-4, 4);
            _speed = 0.10f;
            transform.localScale = new Vector3(Random.Range(0.50f, 0.80f), Random.Range(0.50f, 0.80f));
            SetAsteroidAttributes();

        }
        /*
        if (!_initialAsteroid) // Common asteroids have random attributes
        {
            SetAsteroidAttributes();
        }
        else
        {
            _rotateSpeed = 0;// Random.Range(-4, 4);
            _speed = 0.10f;
        }
        */
        _anim = GetComponent<Animator>();
        //_audioSource = GetComponent<AudioSource>();
        //_audioManager = FindObjectOfType<AudioManager>().GetComponent<AudioManager>();
        //_gameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();

        //
        //_player = Player.Instance;



        //_player = FindObjectOfType<Player>().GetComponent<Player>();
        /*
        if (_anim == null)
        {
            Debug.Log("ASTEROID::Start *** ERROR: Animator is Null!");
        }
        */
        /*
        if (_audioManager == null)
        {
            Debug.Log("ASTEROID::Start *** ERROR: AudioSource is Null!");
        }
        */
        /*
        if (_gameManager == null)
        {
            Debug.Log("ASTEROID::Start *** ERROR: GameManager is Null");
        }
        */
        /*
        if (_player != null)
        {
            Debug.Log("ASTEROID::Start *** ERROR: Player is Null");
        }
        */
    }

    void Update()
    {
        //transform.Translate(Vector3.down * _speed * Time.deltaTime);
        RotateAsteroid();
        EuclideanTorus();
    }

    void SetAsteroidAttributes()
    {
        _rotateSpeed = Random.Range(-17, 18);
        _speed = Random.Range(0.75f, 1.75f);//1,3);

    }

    void RotateAsteroid()
    {
        //transform.Translate(Vector3.down * _speed * Time.deltaTime);
        //transform.Rotate(Vector3.forward * _rotateSpeed * Time.deltaTime);
        int x = 0;
        int y = 0;
        //int z = 1;
        int z = _rotateSpeed;
        transform.Rotate(x, y, z);

        Vector2 pos = transform.position;
        float _anotherspeed = 1.0f;
        //pos.y = pos.y - Time.deltaTime * _anotherspeed;
        pos.y = pos.y - Time.deltaTime * _speed;

        //transform.Translate(Vector3.down * _speed * Time.deltaTime);

        switch (AsteroidBehavior)
        {
            case AsteroidBehaviorType.Sin:
                pos.x = Mathf.Sin(pos.y) * 4;
                break;
            case AsteroidBehaviorType.Tan:
                pos.x = Mathf.Tan(pos.y) * 5;
                break;
            case AsteroidBehaviorType.Cos:
                pos.x = Mathf.Cos(pos.y) * 3;
                break;
            case AsteroidBehaviorType.Sinh:
                pos.x = Mathf.Sin(pos.y) * 2;
                break;
            case AsteroidBehaviorType.Tanh:
                pos.x = Mathf.Tan(pos.y) * 1;
                break;
            case AsteroidBehaviorType.Cosh:
                pos.x = Mathf.Cos(pos.y) * 7;
                break;
            case AsteroidBehaviorType.Giant:
                pos.x = 0;
                break;
        }

        transform.position = pos;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Laser") && AsteroidBehavior == AsteroidBehaviorType.Giant)
        {
            _asteroidCollider2D.enabled = false;
            _asteroidSprite.enabled = false;
            SpawnAsteroids();
            Destroyed();
            return;
        }
        if (other.CompareTag("Laser") || other.CompareTag("Player"))
        {
            _asteroidCollider2D.enabled = false;
            _asteroidSprite.enabled = false;
            Destroyed();
            //_audioSource.Play();
        }
    }

    void SpawnAsteroids()
    {
        // variable based on difficultity level
        int spawns = Random.Range(_minAsteroids, _maxAsteroids);
        spawns = 6;
        Debug.Log("Asteroids to Spawn = " + spawns);
        for (int i = 0; i < spawns; i++)
        {
            GameObject asteroid = Instantiate(_asteroid, transform.position, Quaternion.identity);
            Vector2 force = new Vector2(Random.Range(5f, 10f), Random.Range(0.5f, 2f));
            Rigidbody2D a_rb = asteroid.GetComponent<Rigidbody2D>();
            //asteroid.GetComponent<Rigidbody2D>().AddTorque(Random.Range(5, 50));
            int torque = Random.Range(0, 10);
            a_rb.AddTorque(torque);

            // Push the asteroid in the direction it is facing
            GetComponent<Rigidbody2D>()
                .AddForce(transform.up * Random.Range(-50.0f, 150.0f));

            // Give a random angular velocity/rotation
            GetComponent<Rigidbody2D>()
                .angularVelocity = Random.Range(-0.0f, 90.0f);

            Debug.Log("Counter I = " + i);
            switch (i)
            {
                case 1:
                    Debug.Log("Asteroid set to type: " + AsteroidBehavior);
                    asteroid.GetComponent<Asteroid>().AsteroidBehavior = AsteroidBehaviorType.Sin;
                    break;
                case 2:
                    asteroid.GetComponent<Asteroid>().AsteroidBehavior = AsteroidBehaviorType.Cos;
                    break;
                case 3:
                    asteroid.GetComponent<Asteroid>().AsteroidBehavior = AsteroidBehaviorType.Sin;
                    break;
                case 4:
                    asteroid.GetComponent<Asteroid>().AsteroidBehavior = AsteroidBehaviorType.Cos;
                    break;
                case 5:
                    asteroid.GetComponent<Asteroid>().AsteroidBehavior = AsteroidBehaviorType.Sin;
                    break;
                case 0:
                    asteroid.GetComponent<Asteroid>().AsteroidBehavior = AsteroidBehaviorType.Cos;
                    break;
            }

            //asteroid.GetComponent<Asteroid>()._rotateSpeed = Random.Range(25f, 100f);
            //asteroid.GetComponent<Asteroid>()._speed = Random.Range(.15f, .75f);
        }

    }

    void Destroyed()
    {
        //CinemachineShake.Instance.ShakeCamera(12.0f, 3.0f);
        // Animate Explosion
        //_anim.SetTrigger("AsteroidDestroyed");

        // Play Audio Explosion SFX
        //_audioManager.PlayExplosion();

        // Let GameManager know an object was destroyed
        //_gameManager.DestroyedAsteroid();

        // FOR THE TIME BEING BYPASS CinemachineShake and just destroy the asteroid
        // When using CinemachineShake the animation will play/kick off the DestroyAsteroid method 
        // via an animation event
        GameObject expFX = Instantiate(_explosionFX, transform.position, Quaternion.identity);
        expFX.transform.localScale = new Vector3(2f, 2f, 2f);
        DestoryAsteroid();
    }
    public void DestoryAsteroid()
    {
        // Destory Asteroid after Animation via animation event
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (AsteroidBehavior == AsteroidBehaviorType.Giant)
        {
            WaveSpawner.instance.EnemyDeath();
        }
    }

    void EuclideanTorus()
    {
        // Teleport the astreroid from botton to top and
        // left to right and right to left

        if (transform.position.x > _xScreenClampRight)
        {
            transform.position = new Vector3(_xScreenClampLeft, transform.position.y, 0);
        }
        else if (transform.position.x < _xScreenClampLeft)
        {
            transform.position = new Vector3(_xScreenClampRight, transform.position.y, 0);
        }
        else if (transform.position.y < _yScreenClampLower)
        {
            transform.position = new Vector3(transform.position.x, _yScreenClampUpper, 0);
        }
    }
}