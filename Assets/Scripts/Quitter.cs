using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Quitter : MonoBehaviour {
    public GameObject quitGuy;
    private Canvas _canvas;

    private static Quitter _instance;
    public static Quitter Instance => _instance;
    
    public void Awake() {
        DontDestroyOnLoad(gameObject);
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
        }
        else {
            _instance = this;
        }
        
        quitGuy.SetActive(false);
        _canvas = GetComponent<Canvas>();
    }

    private void FixedUpdate() {
        if (_canvas.worldCamera == null && Camera.main != null) {
            _canvas.worldCamera = Camera.current;
        }
    }

    public void StartQuit() {
        quitGuy.SetActive(true);
    }

    public void CancelQuit() {
        quitGuy.SetActive(false);
    }
    
    public void Quit(InputAction.CallbackContext context) {
        switch (context.phase) {
            case InputActionPhase.Started:
                StartQuit();
                break;
            case InputActionPhase.Disabled:
                break;
            case InputActionPhase.Waiting:
                break;
            case InputActionPhase.Performed:
                EndGame();
                break;
            case InputActionPhase.Canceled:
                CancelQuit();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void EndGame() {
        Application.Quit();
    }
}
