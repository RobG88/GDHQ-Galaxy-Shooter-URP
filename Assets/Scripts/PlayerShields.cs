using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShields : MonoBehaviour
{
    [SerializeField] SpriteRenderer _shieldOuterGlow;

    /// 
    /// SHIELD VARIABLES
    /// Inner Hex Color = 6200FF
    /// Outer Glow Color = 3B00FF
    /// 
    [SerializeField] bool _shieldActive = false;
    [SerializeField] GameObject _shield;
    [SerializeField] int _shieldPower;
    [SerializeField] int _shieldBonus;
    Vector3 _shieldOriginalSize;
    [SerializeField] bool _glow = false;
    /// 
    /// SHIELD VARIABLES - END
    /// 


    void Start()
    {
        ///
        /// SHIELDS VARIABLES INITIALIZE
        ///
        _shieldOriginalSize = _shield.transform.localScale;
        _shieldPower = 3;
        ///
        /// SHIELDS VARIABLES INITIALIZE - END
        ///
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
                _shieldPower = 3; // reset Shield 'hits' remain value
                _shield.transform.localScale = _shieldOriginalSize;
                _shield.SetActive(false);
            }
        }
    }

    IEnumerator FadeTo(float aValue, float aTime)
    {
        Debug.Log("FadeTo: Starting");
        //float alpha = transform.renderer.material.color.a;
        float alpha = _shieldOuterGlow.material.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(1f, 1f, 1f, Mathf.Lerp(alpha, aValue, t));
            //_shieldOuterGlow.color = new Color(1f, 1f, 1f, Mathf.Lerp(min, max, amount));
            //transform.renderer.material.color = newColor;
            _shieldOuterGlow.material.color = newColor;

            yield return null;
            //yield return null;
        }
        _glow = !_glow;
        Debug.Log("FadeTo: Ended");
    }

    IEnumerator ShieldsActivate()
    {
        while (_shieldActive)
        {
            StartCoroutine(FadeTo(0.75f, 0.50f));
            _shield.transform.localScale -= new Vector3(0.5f, 0.5f, 0.5f);
            yield return new WaitForSeconds(1f);
            StartCoroutine(FadeTo(1.0f, 0.05f));
            _shield.transform.localScale = _shieldOriginalSize;
            yield return new WaitForSeconds(1f);

        }
        yield return null;
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
