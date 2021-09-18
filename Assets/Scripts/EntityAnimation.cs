using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAnimation : MonoBehaviour {
    
    [SerializeField] private Animator _anim;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private SpriteRenderer _spr;

    private bool _midair;
    
    private static readonly int WalkAnim = Animator.StringToHash("Walk");
    private static readonly int JumpAnim = Animator.StringToHash("Jump");
    private static readonly int DashAnim = Animator.StringToHash("Dash");
    private static readonly int SprintAnim = Animator.StringToHash("Sprint");
    private static readonly int FallAnim = Animator.StringToHash("Fall");
    private static readonly int SwingAnim = Animator.StringToHash("Swing");

    private void Update() {
        if (_rb.velocity.x > 0.1f && _rb.velocity.y < 0.1f) {
            _anim.SetBool(WalkAnim, true);
        }
        else {
            _anim.SetBool(WalkAnim, false);
        }

        if (_midair && _rb.velocity.y >= 0) {
            _anim.SetBool(JumpAnim, true);
            _anim.SetBool(FallAnim, false);
        } else if (_midair) {
            _anim.SetBool(JumpAnim, false);
            _anim.SetBool(FallAnim, true);
        }
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
