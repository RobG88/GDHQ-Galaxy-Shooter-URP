using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static UI instance;

    [SerializeField] Text _shipWrap;
    [SerializeField] Text _livesText;
    [SerializeField] Text _gameOverText;
    [SerializeField] float BlinkTime;
    [SerializeField] Text _enemiesText;
    [SerializeField] Text _levelText;
    [SerializeField] Text _waveName;
    [SerializeField] Text _waveIncomingText;
    [SerializeField] Text _waveIncomingSecondsText;

    string textToBlink;

    bool CheatKeyEnabled;

    void Awake()
    {
        instance = this;
    }

    public void DisplayLives(int _lives)
    {
        _livesText.text = _lives.ToString();
    }

    public void DisplayEnemies(int _remainingEnemies, int _totalEnemies)
    {
        _enemiesText.text = _remainingEnemies.ToString() + "/" + _totalEnemies.ToString();
    }

    public void DisplayLevel(int _level)
    {
        _levelText.text = _level.ToString();
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
}
