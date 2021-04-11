using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    public static bool _playerIsAlive;

    private void Awake()
    {
        instance = this;
        _playerIsAlive = true;
    }

    public void OnPlayerDeath()
    {
        _playerIsAlive = false;
    }
}
