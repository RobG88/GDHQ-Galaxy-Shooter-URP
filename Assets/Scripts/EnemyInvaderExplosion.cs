using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInvaderExplosion : MonoBehaviour
{
    void Start()
    {
        Destroy(this.gameObject, 2f);
    }
}
