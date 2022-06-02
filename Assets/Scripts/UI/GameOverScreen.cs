using BladeBreaker.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    void GoToMenu() {
        SceneManager.Instance.GoToMainMenu();
    }

    void Continue() {
        //TODO: Set up continue.
        SceneManager.Instance.GoToMainMenu();
        //GameManager.Instance.LoadLevel();
    }
}
