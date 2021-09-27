using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainMenuScreen : MonoBehaviour {
    public Animator anim;
    private UIScreen ui;

    private void Awake() {
        ui = GetComponent<UIScreen>();
    }

    public void NewGame() {
        
    }

    public void Continue() {
        
    }

    public void Options() {
        
    }

    public void FadeIn(float time) {
        StartCoroutine(CameraFader.Instance.FadeCoroutine(1f, time));
    }
    
    public void FadeBlack(float time) {
        StartCoroutine(CameraFader.Instance.FadeCoroutine(0f, time));
    }
    
    public void FadeWhite(float time) {
        StartCoroutine(CameraFader.Instance.FadeCoroutine(20f, time));
    }

    public void StartMenu(InputAction.CallbackContext context) {
        if (!context.started) return;
        anim.SetTrigger("Started");
        ui.OneWayActivate();
    }
}
