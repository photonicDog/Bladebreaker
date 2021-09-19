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

    [SerializeField] private float _gravity;

    [SerializeField] private float _maxVelocity;


    public Vector2 _velocity;
    private Vector2 _frameVelocity;

    private Rigidbody2D _rb;
    private BoxCollider2D _coll;
    [SerializeField] private EntityAnimation _animation;

    private float _walkInput;
    private int _facing = 1;
    private bool _midair = true;
    private bool _hasJump = false;

    private bool _hasAnimation;
    
    //Input state triggers
    private bool _dash = false;
    private bool _jump = false;
    private bool _fastfall = false;
    private bool _sprint = false;
    
    // Start is called before the first frame update
    void Awake() {
        _rb = GetComponent<Rigidbody2D>();
        _coll = GetComponent<BoxCollider2D>();
        _hasAnimation = _animation != null;
        _velocity = new Vector2(0, 0);
    }

    private void FixedUpdate() {
        _frameVelocity = new Vector2(0, 0);
        GroundRaycast();
        if (_jump && _hasJump) {
            _frameVelocity += new Vector2(0, _jumpHeight);
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

        _velocity = (_velocity * 0.95f) + _frameVelocity;
        if (!_midair) _velocity *= new Vector2(1, 0);
        _rb.MovePosition(_rb.position + _velocity);
    }

    private void MidairPhysics() {
        _frameVelocity += new Vector2(_walkInput * _airSpeed, 0) + (Vector2.down * _gravity);

        if (_fastfall) {
            _frameVelocity += new Vector2(0, -_jumpHeight);
            _fastfall = false;
        }

        _frameVelocity = VelocityLimit(_frameVelocity, _maxVelocity);
    }

    private void GroundPhysics() {
        _frameVelocity = new Vector2(_walkInput * _groundSpeed, 0);
        if (_dash) {
            _frameVelocity += new Vector2(_dashDistance * _facing, 0);
            _dash = false;
        }

        if (_dash) {
            _frameVelocity = VelocityLimit(_frameVelocity, _maxVelocity * 2);
        } else if (_sprint) {
            _frameVelocity = VelocityLimit(_frameVelocity, _maxVelocity * _sprintMod);
        }
        else {
            _frameVelocity = VelocityLimit(_frameVelocity, _maxVelocity);
        }
    }

    private void GroundRaycast() {
        float leftXBound = _coll.bounds.min.x;
        float rightXBound = _coll.bounds.max.x;
        float bottomY = _coll.bounds.center.y;
        float rayDistance = 2f;

        LayerMask t = LayerMask.GetMask("Terrain");
        
        Debug.DrawRay(new Vector3(leftXBound, bottomY), Vector3.down*rayDistance, Color.green, 1f);
        Debug.DrawRay(new Vector3(rightXBound, bottomY), Vector3.down*rayDistance, Color.green, 1f);

        RaycastHit2D leftRay = Physics2D.Raycast(new Vector2(leftXBound, bottomY), Vector2.down, rayDistance, t);
        RaycastHit2D rightRay = Physics2D.Raycast(new Vector2(rightXBound, bottomY), Vector2.down, rayDistance, t);

        if (leftRay || rightRay) {
            if (leftRay.point.y > _coll.bounds.min.y) {
                _rb.MovePosition(new Vector2(_rb.position.x, leftRay.point.y + (transform.position.y - _coll.bounds.center.y) + _coll.bounds.extents.y));
            }
            _hasJump = true;
            _midair = false;
        }
        else {
            _midair = true;
        }
    }

    private Vector2 VelocityLimit(Vector2 input, float limit) {
        if (Mathf.Abs(input.x) > limit) {
            return new Vector2(Mathf.Sign(input.x) * limit, input.y);
        }
        else {
            return input;
        }
    }
/*
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
*/
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

        _sprint = true;
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

    public void Stop() {
        _walkInput = 0;
        _sprint = false;
    }
}
