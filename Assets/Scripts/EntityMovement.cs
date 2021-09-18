using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMovement : MonoBehaviour {
    [SerializeField] private float _groundSpeed;
    [SerializeField] private float _airSpeed;
    [SerializeField] private float _dashDistance;
    [SerializeField] private float _jumpHeight;
    [SerializeField] private float _sprintMod;

    private Rigidbody2D _rb;
    private BoxCollider2D _coll;
    [SerializeField] private EntityAnimation _animation;

    private float _walkInput;
    private int _facing = 1;
    private bool _midair = false;
    private bool _hasJump = false;

    private bool _hasAnimation;
    
    //Input state triggers
    private bool _dash = false;
    private bool _jump = false;
    private bool _fastfall = false;
    
    // Start is called before the first frame update
    void Awake() {
        _rb = GetComponent<Rigidbody2D>();
        _coll = GetComponent<BoxCollider2D>();
        _hasAnimation = _animation != null;
    }
    
    private void FixedUpdate() {
        if (_jump && _hasJump) {
            _rb.AddForce(new Vector2(0, _jumpHeight), ForceMode2D.Impulse);
            _jump = false;
            _midair = true;
            _hasJump = false;
        }
        
        if (_midair) MidairPhysics();
        else GroundPhysics();

        if (_hasAnimation) {
            if (_facing > 0) {
                _animation.SetFlip(false);
            }
            else {
                _animation.SetFlip(true);
            }
        }

    }

    private void MidairPhysics() {
        _rb.AddForce(new Vector2(_walkInput * _airSpeed, 0));

        if (_fastfall) {
            _rb.AddForce(new Vector2(0, -_jumpHeight), ForceMode2D.Impulse);
            _fastfall = false;
        }
    }

    private void GroundPhysics() {
        _rb.AddForce(new Vector2(_walkInput * _groundSpeed, 0));
        if (_dash) {
            _rb.AddForce(new Vector2(_dashDistance * _facing, 0), ForceMode2D.Impulse);
            _dash = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.collider.CompareTag("Terrain")) {
            if (transform.position.y < other.transform.position.y) {
                Debug.Log("From the bottom");
            }
            else {
                _hasJump = true;
                _midair = false;
                Debug.Log("From the top");
            }
        }
    }

    public void Walk(float input) {
        _walkInput = input;
        int sign = Math.Sign(input);
        if (sign != 0) {
            _facing = sign;
        }
    }
    
    public void Sprint(float input) {
        _walkInput = input*_sprintMod;
        int sign = Math.Sign(input);
        if (sign != 0) {
            _facing = sign;
        }
    }

    public void Dash() {
        if (!_midair) {
            _dash = true; 
        }
    }

    public void Jump() {
        if (_hasJump) {
            _jump = true;
        }
    }

    public void FastFall() {
        if (_midair) {
            _fastfall = true;
        }
    }
}
