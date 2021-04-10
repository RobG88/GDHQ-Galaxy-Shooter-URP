using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    ////////////////////////////////
    /// Laser (Player)
    /// 
    /// Laser gameObject travels UP @ _speed
    /// 
    /// Laser triggers/collides with object &
    /// is destroyed or self-desrtucts when
    /// position.y > _yThreshold (8)
    /// 
    ///

    [SerializeField] float _speed = 13.0f;
    [SerializeField] float _yThreshold = 12.5f;

    void Update()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        if (transform.position.y > _yThreshold)
            Destroy(transform.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) // || other.CompareTag("Asteroid"))
        {
            Destroy(gameObject);
        }
    }
}
