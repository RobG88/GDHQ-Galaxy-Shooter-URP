using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static UI instance;

    [SerializeField] Text _scoreText;
    [SerializeField] Image _livesRemainingImage;
    [SerializeField] Sprite[] _livesSprites;
    [SerializeField] Text _shipWrap;
    [SerializeField] Text _playerLives;
    [SerializeField] Text _gameOverText;
    [SerializeField] float BlinkTime;
    [SerializeField] Text _enemiesRemaing;
    [SerializeField] Text _currentLevel;
    [SerializeField] Text _waveName;
    [SerializeField] Text _waveIncomingText;
    [SerializeField] Text _waveIncomingSecondsText;
    [SerializeField] Text _restartGameText;

    [SerializeField] GameObject _PowerUp_Tripleshot;
    [SerializeField] GameObject _PowerUp_SpeedBoost;
    [SerializeField] GameObject _PowerUp_Shields;

    [SerializeField] Timer Timer_PowerUp_Tripleshot;
    [SerializeField] Timer Timer_PowerUp_Speed;
    [SerializeField] Timer Timer_PowerUp_Shields;


    string textToBlink;

    bool CheatKeyEnabled;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        _scoreText.text = "---";
        _gameOverText.gameObject.SetActive(false);
        _restartGameText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ActiveTripleShotUI();
        }
    }

    public void UpdateScore(int playerScore)
    {
        if (_scoreText != null)
        {
            _scoreText.text = playerScore.ToString("#,#");
        }
    }

    public void DisplayLives(int _playerLivesRemaining)
    {
        _livesRemainingImage.sprite = _livesSprites[_playerLivesRemaining];
        _playerLives.text = _playerLivesRemaining.ToString();
    }

    public void DisplayEnemies(int _remainingEnemies, int _totalEnemies)
    {
        if (_enemiesRemaing != null)
        {
            _enemiesRemaing.text = _remainingEnemies + "/" + _totalEnemies;
        }
    }

    public void DisplayLevel(int _level)
    {
        _currentLevel.text = _level.ToString();
    }

    public void DisplayShipWrapStatus()
    {
        if (CheatKeyEnabled)
        {
            textToBlink = "Ship Wrap: Enabled";
            _shipWrap.text = textToBlink;
            StartCoroutine(Blink());
        }
        else
        {
            textToBlink = "Ship Wrap: Disabled";
            _shipWrap.text = textToBlink;
        }
    }

    IEnumerator Blink()
    {
        while (CheatKeyEnabled)
        {
            _shipWrap.text = textToBlink;
            yield return new WaitForSeconds(BlinkTime);
            _shipWrap.text = string.Empty;
            yield return new WaitForSeconds(BlinkTime);
        }

        _shipWrap.text = "Ship Wrap: Disabled";
    }

    public void SetCheatKey(bool status)
    {
        CheatKeyEnabled = status;
    }

    public void WaveCountdown(int Timer)
    {
        Timer++;
        _waveIncomingSecondsText.text = Timer.ToString();
    }

    public void WaveCountdownEnableUI(bool isEnabled, int countdownTime, string waveName)
    {
        _waveName.text = waveName;
        _waveName.gameObject.SetActive(isEnabled);
        _waveIncomingText.gameObject.SetActive(isEnabled);
        _waveIncomingSecondsText.gameObject.SetActive(isEnabled);
        _waveIncomingSecondsText.text = countdownTime.ToString();
    }

    public void ActiveTripleShotUI()
    {
        _PowerUp_Tripleshot.SetActive(true);
        Activate_Tripleshot(5);
    }

    public void ActiveSpeedBoostUI()
    {
        _PowerUp_SpeedBoost.SetActive(true);
        Activate_SpeedBoost(5);
    }

    public void ActiveShieldsUI()
    {
        _PowerUp_Shields.SetActive(true);
        Activate_Shields(5);
    }

    void Activate_Tripleshot(int duration)
    {
        Timer_PowerUp_Tripleshot
        .SetDuration(duration)
        .OnEnd(() => _PowerUp_Tripleshot.SetActive(false))
        .Begin();
        Timer_PowerUp_Tripleshot.enabled = false;
    }

    public void Activate_SpeedBoost(int duration)
    {
        Timer_PowerUp_Speed
        .SetDuration(duration)
        .OnEnd(() => _PowerUp_SpeedBoost.SetActive(false))
        .Begin();
    }

    public void Activate_Shields(int duration)
    {
        Timer_PowerUp_Shields
        .SetDuration(duration)
        .OnEnd(() => _PowerUp_Shields.SetActive(false))
        .Begin();
    }

    public void ActivatePowerUp()
    {
        /// Similar to Player PowerUp Case & PowerUp String
        /// Activating the PowerUp Counter should work the same
    }

    public void GameOver(bool isGameOVer)
    {
        _gameOverText.gameObject.SetActive(isGameOVer);
        //_gameOverText.gameObject.SetActive(true);
        _restartGameText.gameObject.SetActive(isGameOVer);
        StartCoroutine(GameOverColorChange());
    }
    public void DisplayGameOver()
    {
        _gameOverText.gameObject.SetActive(true);
        _restartGameText.gameObject.SetActive(true);
        StartCoroutine(GameOverColorChange());
    }

    IEnumerator GameOverColorChange()
    {
        var gameoverText = _gameOverText.GetComponent<Text>();

        for (int i = 60; i >= 0; i--)
        {
            gameoverText.color = Color.red;
            if (i % 2 == 0)
            {
                gameoverText.color = Color.blue;
            }
            if (i % 3 == 0)
            {
                gameoverText.color = Color.white;
            }
            yield return new WaitForSeconds(.1f);
        }
        gameoverText.color = Color.red;
    }
}
