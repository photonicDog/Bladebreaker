using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAnimation : MonoBehaviour {
    
    [SerializeField] private Animator _anim;
    [SerializeField] private EntityMovement _em;
    [SerializeField] private SpriteRenderer _spr;
    
    private GameManager _gm;

    [SerializeField] private GameObject _weaponry;
    [SerializeField] private GameObject _asplode;

    private static readonly int WalkAnim = Animator.StringToHash("Walk");
    private static readonly int JumpAnim = Animator.StringToHash("Jump");
    private static readonly int DashAnim = Animator.StringToHash("Dash");
    private static readonly int SprintAnim = Animator.StringToHash("Sprint");
    private static readonly int FallAnim = Animator.StringToHash("Fall");
    private static readonly int AttackAnim = Animator.StringToHash("Attack");
    private static readonly int UpAnim = Animator.StringToHash("UpHeld");
    private static readonly int DownAnim = Animator.StringToHash("DownHeld");
    private static readonly int GroundAnim = Animator.StringToHash("Grounded");
    private static readonly int GuardAnim = Animator.StringToHash("Guard");
    private static readonly int LungeAnim = Animator.StringToHash("Lunge");
    private static readonly int HurtAnim = Animator.StringToHash("Hurt");
    private static readonly int StunAnim = Animator.StringToHash("Stun");
    private static readonly int DieAnim = Animator.StringToHash("Die");

    [SerializeField] private bool canJump;
    [SerializeField] private bool canDash;
    [SerializeField] private bool canSprint;
    [SerializeField] private bool canDie;

    private void Start()
    {
        _gm = GameManager.Instance;
    }

    private void Update() {
        if (!_gm.IsPaused)
        {
            if (_em.walk)
            {
                _anim.SetBool(WalkAnim, true);
            }
            else
            {
                _anim.SetBool(WalkAnim, false);
            }

            if (canJump) JumpAnimations();
            if (canDash) _anim.SetBool(DashAnim, _em.dash);
            if (canSprint) _anim.SetBool(SprintAnim, _em.sprint);
        }
    }

    private void JumpAnimations() {
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
    }

    public void Attack() {
        _anim.SetTrigger(AttackAnim);
    }

    public void Look(bool up, bool press) {
        _anim.SetBool(up?UpAnim:DownAnim, press);
    }

    public void Guard(bool active) {
        _anim.SetBool(GuardAnim, active);
    }
    
    public void Lunge() {
        _anim.SetTrigger(LungeAnim);
    }
    
    public void Hurt() {
        _anim.SetTrigger(HurtAnim);
        Stun(true);
    }
    
    public void StopHurt() {
        Stun(false);
    }

    public void Stun(bool active) {
        _anim.SetBool(StunAnim, active);
    }

    public void SpoofDash(float time) {
        if (canDash) StartCoroutine(DashCoroutine(time));
    }

    public void Die() {
        if (canDie) _anim.SetTrigger(DieAnim);
        else Instantiate(_asplode, transform.position, Quaternion.identity);
    }

    IEnumerator DashCoroutine(float time) {
        _anim.SetBool(DashAnim, true);
        yield return new WaitForSeconds(time);
        _anim.SetBool(DashAnim, false);
    }

    public void SetFlip(bool flip) {
        _spr.flipX = flip;
        if (_weaponry) _weaponry.transform.localScale = new Vector3(flip?-1:1, 1, 1);
    }
}
