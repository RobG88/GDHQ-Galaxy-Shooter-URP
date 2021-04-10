using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInvader : MonoBehaviour
{
    [SerializeField] GameObject _enemyInvaderExplosion;
    [SerializeField] GameObject _enemyChild;

    float _xScreenClampRight = 10.5f;
    float _xScreenClampLeft = -10.5f;

    [SerializeField] float _speed = 3;

    Vector3 direction = Vector3.right;

    void Update()
    {
        transform.Translate(direction * _speed * Time.deltaTime);

        if (transform.position.x > _xScreenClampRight || transform.position.x < _xScreenClampLeft)
        {
            direction = direction * -1;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Laser")
        {
            Instantiate(_enemyInvaderExplosion, transform.position, Quaternion.identity);
            _enemyChild.SetActive(false);
            Destroy(this.gameObject, 2f);
        }
    }
}
