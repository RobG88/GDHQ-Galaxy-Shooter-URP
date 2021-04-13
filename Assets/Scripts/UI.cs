using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static UI instance;

    [SerializeField] Text _shipWrap;
    [SerializeField] Text _playerLives;
    [SerializeField] Text _gameOverText;
    [SerializeField] float BlinkTime;
    [SerializeField] Text _enemiesRemaing;
    [SerializeField] Text _currentLevel;
    [SerializeField] Text _waveName;
    [SerializeField] Text _waveIncomingText;
    [SerializeField] Text _waveIncomingSecondsText;

    [SerializeField] GameObject _PowerUp_Tripleshot;
    [SerializeField] GameObject _PowerUp_SpeedBoost;

    [SerializeField] Timer Timer_PowerUp_Tripleshot;
    [SerializeField] Timer Timer_PowerUp_Speed;


    string textToBlink;

    bool CheatKeyEnabled;

    void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ActiveTripleShotUI();
        }
    }

    public void DisplayLives(int _playerLivesRemaining)
    {
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

    public void GameOver(bool isGameOVer)
    {
        _gameOverText.gameObject.SetActive(isGameOVer);
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

    public void ActivatePowerUp()
    {
        /// Similar to Player PowerUp Case & PowerUp String
        /// Activating the PowerUp Counter should work the same
    }
}
