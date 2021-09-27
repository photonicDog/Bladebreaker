using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Preload : MonoBehaviour
{
    void Start()
    {
        SceneManager.Instance.GoToMainMenu();
    }
}
