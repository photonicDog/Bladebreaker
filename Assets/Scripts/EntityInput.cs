using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EntityInput : MonoBehaviour {
    private EntityMovement _em;

    public bool _dashStart;
    public bool _dashConfirm;
    
    // Start is called before the first frame update
    void Awake() {
        _em = GetComponent<EntityMovement>();
    }
    
    public void Walk(InputAction.CallbackContext context) {
        if (context.canceled) {
            _em.Stop();
            if ((!_dashStart||_dashConfirm)) {
                _dashStart = false;
                _dashConfirm = false;
            }
        }
        else {
            if (context.started && !_dashStart) {
                _dashStart = true;
                StartCoroutine(DashCheck());
            } else if (context.started && _dashStart) {
                _dashConfirm = true;
            }

            if (_dashConfirm) {
                _em.Sprint(context.ReadValue<float>());
            }
            else {
                _em.Walk(context.ReadValue<float>());
            }
        }
    }

    public void Jump(InputAction.CallbackContext context) {
        if (context.started) _em.Jump();
        else if (context.canceled) _em.JumpRelease();
    }

    public void FastFall(InputAction.CallbackContext context) {
        if (context.started) _em.FastFall();
    }

    IEnumerator DashCheck() {
        int counter = 20;
        while (counter > 0) {
            if (_dashConfirm) {
                _em.Dash();
                break;
            }
            else {
                counter--;
                yield return new WaitForFixedUpdate();
            }
        }
        _dashStart = false;
    }
}
