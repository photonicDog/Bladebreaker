using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    void GoToMenu() {
        //TODO: Go to main menu scene
    }

    void Continue() {
        GameManager.Instance.LoadLevel();
    }
}
