using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    void GoToMenu() {
        SceneManager.Instance.GoToMainMenu();
    }

    void Continue() {
        GameManager.Instance.LoadLevel();
    }
}
