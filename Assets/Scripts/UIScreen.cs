using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
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

    public void Awake() {
        cursor.SetActive(false);

    }

    public void Start() {
        if (input == null) {
            input = GameManager.Instance.player.GetComponent<PlayerInput>();
        }
    }

    public void OneWayActivate() {
        if (control) return;
        Activate(1);
    }

    public void Activate(int active) {
        StartCoroutine(Activate(control = active==1));
    } 
    
    public void Select(InputAction.CallbackContext context) {
        if (!control) return;
        if (context.started) {
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
