using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCutScene : MonoBehaviour
{
    [SerializeField] float _rotationSpriteSpeed = 5f;

    void Update()
    {
        transform.Rotate(0f, _rotationSpriteSpeed * Time.deltaTime, 0f);
    }
}
