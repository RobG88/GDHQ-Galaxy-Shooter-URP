using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShields : MonoBehaviour
{
    /// 
    /// SHIELD VARIABLES
    /// Inner Hex Color = 6200FF
    /// Outer Glow Color = 3B00FF
    /// 
    [SerializeField] bool _shieldActive = false;
    [SerializeField] GameObject _shield;
    [SerializeField] int _shieldPower;
    [SerializeField] int _shieldBonus;
    /// 
    /// SHIELD VARIABLES - END
    /// 

    private void OnEnable()
    {
        _shield.transform.localScale = new Vector3(1, 1, 1);
        _shieldPower = 3;
        _shieldActive = true;
    }

    public void Damage()
    {
        if (_shieldActive)
        {
            //_sound.clip = _explosionSFX;
            //_sound.PlayOneShot(_sound.clip);
            if (_shieldPower > 0)
            {
                _shieldPower--;
                _shield.transform.localScale -= new Vector3(0.20f, 0.20f, 0.20f);
            }
            if (_shieldPower == 0)
            {
                _shieldActive = false;
                Player.instance.ShieldsDestroyed();
                _shield.SetActive(false);
            }
        }
    }

    /*
    ///
    /// SHIELDS - SHIP DAMAGE ROUTINE
    /// 
    public void Damage() // Ship & Shield damage
    {
        if (_shieldBonus == 3 && !_bonusLifeOncePerLevel) // Enable 3x Shield Bonus 'hit'
        {
            _bonusLife = true;
            _shieldBonus = 0;
            _bonusLifeOncePerLevel = true;
        }

        if (_shieldActive)
        {
            //_sound.clip = _explosionSFX;
            //_sound.PlayOneShot(_sound.clip);
            if (_shieldPower > 0)
            {
                _shieldPower--;
                _shield.transform.localScale -= new Vector3(0.5f, 0.5f, 0.5f);
            }
            if (_shieldPower == 0)
            {
                _shieldActive = false;
                _shieldPower = 3; // reset Shield 'hits' remain value
                _shield.transform.localScale = _shieldOriginalSize;
                _shield.SetActive(false);
            }
        }
        else
        {
            if (_bonusLife)
            {
                UIManager.Instance.UpdateShieldBonusUI(_shieldBonus);
                _bonusLife = false;
            }
            else
            {
                _playerLives--;
                UIManager.Instance.UpdatePlayerLives(_playerLives);
                _bonusLifeOncePerLevel = true;
                _shieldBonus = 0;
                UIManager.Instance.UpdateShieldBonusUI(_shieldBonus);

                if (_playerLives < 1)
                {
                    _gameOver = true;
                    ///
                    /// CAMERA SHAKE done via CINEMACHINE
                    /// 
                    CinemachineShake.Instance.ShakeCamera(16f, 4f);
                    PlayerDeathSequence();
                    return;
                }

                ///
                /// CAMERA SHAKE done via CINEMACHINE
                /// 
                CinemachineShake.Instance.ShakeCamera(5f, 1f);
                SpaceshipDamaged();
            }
        }
    }
    ///
    /// SHIELDS - SHIP DAMAGE ROUTINE - END
    ///
    */


}
