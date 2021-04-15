using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    public static bool _playerIsAlive;

    public static bool _disablePowerUp_Spawning = false;

    [SerializeField] bool _isGameOver = false;
    [SerializeField] bool _gamePaused = false;
    [SerializeField] GameObject _pausePanel;

    private void Awake()
    {
        instance = this;
        _playerIsAlive = true;
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.I)) && _isGameOver)
        {
            SceneManager.LoadScene(1); // Reload current game scene "GalaxyShooterDemo"
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }

        if (Input.GetKeyDown(KeyCode.P) && !_isGameOver)
        {
            PauseGame();
        }

        /*
        if (_isAsteroidDestroyed && !_EnemyInvasion)
        {
            _EnemyInvasion = true;
            _spawnManager.Spawn();
        }
        */
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void PauseGame()
    {

        //_pausePanel.SetActive(true);
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;

        if (!_gamePaused)
        {
            _gamePaused = true;
            _pausePanel.SetActive(true);
            //Time.timeScale = 0;
        }
        else
        {
            _gamePaused = false;
            _pausePanel.SetActive(false);
            //Time.timeScale = 1;
        }
    }

    public void GameOver()
    {
        _isGameOver = true;
    }
    public void OnPlayerDeath()
    {
        _playerIsAlive = false;
        _isGameOver = true;
    }
}
