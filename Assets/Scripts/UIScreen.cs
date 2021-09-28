using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Assets.Scripts.Controllers;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIScreen : SerializedMonoBehaviour {
    public GameObject cursor;
    public PlayerInput input;

    public delegate void UIAction();
    [NonSerialized][OdinSerialize]public Dictionary<int, Tuple<Vector2, UIAction>> Actions;

    private int position;
    public bool control;

    private AudioController _audio;

    public AudioClip SelectAudio;
    public AudioClip StartAudio;

    public void Awake() {
        cursor.SetActive(false);

    }

    public void Start() {
        if (input == null) {
            input = GameManager.Instance.player.GetComponent<PlayerInput>();
        }
        _audio = AudioController.Instance;
        _audio.PlayMusic();
    }

    public void OneWayActivate() {
        if (control) return;
        Activate(1);
    }

    public void Activate(int active)
    {
        _audio.PlayStageSFX(StartAudio);
        StartCoroutine(Activate(control = active==1));
    } 
    
    public void Select(InputAction.CallbackContext context) {
        if (!control) return;
        if (context.started) {
            _audio.PlayStageSFX(SelectAudio);
            SetCursor(context.ReadValue<float>());
        }
    }

    private void SetCursor(float direction) {
        if (direction < 0) {
            position++;
        }

        if (direction > 0) {
            position--;
        }

        if (position >= Actions.Count) position = 0;
        if (position < 0) position = Actions.Count - 1;
        UpdatePosition();
    }

    private void UpdatePosition() {
        cursor.transform.localPosition = Actions[position].Item1;
    }

    public void Do(InputAction.CallbackContext context) {
        if (!control) return;
        if (context.started) {
            Actions[position].Item2.Invoke();
            Activate(0);
        }
    }

    IEnumerator Activate(bool set) {
        yield return new WaitForEndOfFrame();
        control = set;
        input.currentActionMap = input.actions.FindActionMap(control?"UI":"Gameplay");
        cursor.SetActive(control);
        position = 0;
        UpdatePosition();
    }
}
