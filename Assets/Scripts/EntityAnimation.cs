using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAnimation : MonoBehaviour {
    
    [SerializeField] private Animator _anim;
    [SerializeField] private EntityMovement _em;
    [SerializeField] private SpriteRenderer _spr;

    [SerializeField] private GameObject _weaponry;

    private static readonly int WalkAnim = Animator.StringToHash("Walk");
    private static readonly int JumpAnim = Animator.StringToHash("Jump");
    private static readonly int DashAnim = Animator.StringToHash("Dash");
    private static readonly int SprintAnim = Animator.StringToHash("Sprint");
    private static readonly int FallAnim = Animator.StringToHash("Fall");
    private static readonly int AttackAnim = Animator.StringToHash("Attack");
    private static readonly int UpAnim = Animator.StringToHash("UpHeld");
    private static readonly int DownAnim = Animator.StringToHash("DownHeld");
    private static readonly int GroundAnim = Animator.StringToHash("Grounded");

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
            _anim.SetBool(GroundAnim, false);
        } else if (_em.midair) {
            _anim.SetBool(JumpAnim, false);
            _anim.SetBool(FallAnim, true);
            _anim.SetBool(GroundAnim, false);
        }
        else {
            _anim.SetBool(JumpAnim, false);
            _anim.SetBool(FallAnim, false);
            _anim.SetBool(GroundAnim, true);
        }
        
        _anim.SetBool(DashAnim, _em.dash);
        _anim.SetBool(SprintAnim, _em.sprint);
        
        

    }

    public void Attack() {
        _anim.SetTrigger(AttackAnim);
    }

    public void Look(bool up, bool press) {
        _anim.SetBool(up?UpAnim:DownAnim, press);
    }

    public void SetFlip(bool flip) {
        _spr.flipX = flip;
        if (_weaponry) _weaponry.transform.localScale = new Vector3(flip?-1:1, 1, 1);
    }
}
