using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EntityInput : MonoBehaviour {
    private EntityMovement _em;
    private Inventory _inventory;

    private bool _dashStart;
    private bool _dashConfirm;

    private Vector2 currentMoveInput;
    
    void Awake() {
        _em = GetComponent<EntityMovement>();
        _inventory = GetComponent<Inventory>();
    }
    
    public void Walk(InputAction.CallbackContext context) {
        currentMoveInput = new Vector2(context.ReadValue<float>(), currentMoveInput.y);
        if (context.canceled) {
            currentMoveInput = new Vector2(0, currentMoveInput.y);
            _em.Stop();
            if (!_dashStart||_dashConfirm) {
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
    
    public void LookUp(InputAction.CallbackContext context) {
        if (context.started) {
            currentMoveInput = new Vector2(currentMoveInput.x, 1);
        } else if (context.canceled) {
            currentMoveInput = new Vector2(currentMoveInput.y, 0);
        }
    }

    public void LookDown(InputAction.CallbackContext context) {
        if (context.started) {
            currentMoveInput = new Vector2(currentMoveInput.x, -1);
        } else if (context.canceled) {
            currentMoveInput = new Vector2(currentMoveInput.y, 0);
        }
    }

    public void Jump(InputAction.CallbackContext context) {
        if (context.started) _em.Jump();
        else if (context.canceled) _em.JumpRelease();
    }

    public void FastFall(InputAction.CallbackContext context) {
        if (context.started) {
            _em.FastFall();
            _em.downHeld = true;
        } else if (context.canceled) {
            _em.downHeld = false;
        }
    }

    public void Select(InputAction.CallbackContext context) {
        if (context.started) _inventory.SelectLogic(currentMoveInput);
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
