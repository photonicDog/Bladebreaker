using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BladeBreaker.Core
{
    public class Preload : MonoBehaviour
    {
        void Start()
        {
            SceneManager.Instance.GoToMainMenu();
        }
    }
}

