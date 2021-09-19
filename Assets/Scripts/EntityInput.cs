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
        if (context.canceled && (!_dashStart||_dashConfirm)) {
            _dashStart = false;
            _dashConfirm = false;
            _em.Stop();
        }
        
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

    public void Jump(InputAction.CallbackContext context) {
        if (context.started) _em.Jump();
    }

    public void FastFall(InputAction.CallbackContext context) {
        if (context.started) _em.FastFall();
    }

    IEnumerator DashCheck() {
        yield return new WaitForSeconds(0.3f);
        if (_dashConfirm) {
            _em.Dash();
        }
        _dashStart = false;
    }
}
