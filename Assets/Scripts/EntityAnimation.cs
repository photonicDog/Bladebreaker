using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAnimation : MonoBehaviour {
    
    [SerializeField] private Animator _anim;
    [SerializeField] private EntityMovement _em;
    [SerializeField] private SpriteRenderer _spr;

    private bool _midair;
    
    private static readonly int WalkAnim = Animator.StringToHash("Walk");
    private static readonly int JumpAnim = Animator.StringToHash("Jump");
    private static readonly int DashAnim = Animator.StringToHash("Dash");
    private static readonly int SprintAnim = Animator.StringToHash("Sprint");
    private static readonly int FallAnim = Animator.StringToHash("Fall");
    private static readonly int SwingAnim = Animator.StringToHash("Swing");

    private void Update() {
        if (_em.walk) {
            _anim.SetBool(WalkAnim, true);
        }
        else {
            _anim.SetBool(WalkAnim, false);
        }

        if (_em.midair && _em.velocity.y >= 0) {
            _anim.SetBool(JumpAnim, true);
            _anim.SetBool(FallAnim, false);
        } else if (_em.midair) {
            _anim.SetBool(JumpAnim, false);
            _anim.SetBool(FallAnim, true);
        }
        else {
            _anim.SetBool(JumpAnim, false);
            _anim.SetBool(FallAnim, false);
        }
        
        _anim.SetBool(DashAnim, _em.dash);
        _anim.SetBool(SprintAnim, _em.sprint);
           
    }

    public void Walk(bool active) {
        
    }

    public void Dash(bool active) {
        
    }

    public void Air(bool active) {
        _midair = active;
    }

    public void SetFlip(bool flip) {
        _spr.flipX = flip;
    }
}
