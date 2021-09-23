using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class EntityAttacker : MonoBehaviour {
    public EntityAnimation entityAnimation;
    public EntityMovement entityMovement;

    private bool _up;
    private bool _down;

    public void Awake() {
        _up = false;
        _down = false;
    }

    public void Attack(InputAction.CallbackContext context) {
        if (context.started) {
            entityAnimation.Attack();
            entityMovement.attackFreeze = true;
        }
    }

    public void LookUp(InputAction.CallbackContext context) {
        if (context.started) {
            entityAnimation.Look(true, true);
        } else if (context.canceled) {
            entityAnimation.Look(true, false);
        }
    }

    public void LookDown(InputAction.CallbackContext context) {
        if (context.started) {
            entityAnimation.Look(false, true);
        } else if (context.canceled) {
            entityAnimation.Look(false, false);
        }
    }
}
