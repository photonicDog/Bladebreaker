using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Controllers;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainMenuScreen : MonoBehaviour {
    public Animator anim;
    private UIScreen ui;

    private AudioController _audio;

    private void Awake() {
        ui = GetComponent<UIScreen>();
    }

    public void Start()
    {
        _audio = AudioController.Instance;
        _audio.PlayMusic();
    }

    public void NewGame()
    {
        if (_audio) _audio.StopMusic();
        SaveDataManager.Instance.CreateNewSave();
        SceneManager.Instance.SwitchLevel(0);
    }

    public void Continue()
    {
        if (_audio) _audio.StopMusic();
        SaveDataManager.Instance.LoadData();
        SceneManager.Instance.SwitchLevel(SaveDataManager.Instance.saveData.LastClearedLevel);
    }

    public void Options() {
        
    }

    public void FadeIn(float time) {
        if (CameraFader.Instance.FindCamera())
        StartCoroutine(CameraFader.Instance.FadeCoroutine(1f, time));
    }
    
    public void FadeBlack(float time) {
        if (CameraFader.Instance.FindCamera())
        StartCoroutine(CameraFader.Instance.FadeCoroutine(0f, time));
    }
    
    public void FadeWhite(float time) {
        if (CameraFader.Instance.FindCamera())
        StartCoroutine(CameraFader.Instance.FadeCoroutine(20f, time));
    }

    public void StartMenu(InputAction.CallbackContext context) {
        if (!context.started) return;
        if (!ui.control) {
            anim.SetTrigger("Started");
            ui.OneWayActivate();
            ui.control = true;
        }
    }
}
