using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class DestroyComplexParticleSystem : MonoBehaviour
{
    void Update()
    {
        if (transform.childCount == 0) {
            //She took the kids in the divorce
            Destroy(gameObject);
        }
    }
}
