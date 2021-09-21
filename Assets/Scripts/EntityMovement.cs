using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EntityMovement : MonoBehaviour {
    [SerializeField] private float _groundSpeed;
    [SerializeField] private float _airSpeed;
    [SerializeField] private float _dashSpeed;
    [SerializeField] private float _dashDistance;
    [SerializeField] private float _jumpHeight;
    [SerializeField] private float _sprintMod;
    [SerializeField] private float _jumpDecay;
    [SerializeField] private float _groundFriction;

    [SerializeField] private float _gravity;

    [SerializeField] private float _maxVelocity;


    public Vector2 velocity;
    public Vector2 _frameVelocity;

    private Rigidbody2D _rb;
    private BoxCollider2D _coll;
    [SerializeField] private EntityAnimation _animation;

    private float _walkInput;
    public int _facing = 1;
    public bool midair = true;
    public bool _hasJump = true;
    private float _jumpDecayCurrent;

    private bool _hasAnimation;
    
    //Input state triggers
    public bool walk = false;
    public bool dash = false;
    private bool _isDashing = false;
    private bool _dashKill;
    public bool jump = false;
    public bool fastfall = false;
    public bool sprint = false;

    private LayerMask terrain;
    private LayerMask platform;

    public bool leftCollide;
    public bool rightCollide;

    private float _dashStartPos;


    // Start is called before the first frame update
    void Awake() {
        _rb = GetComponent<Rigidbody2D>();
        _coll = GetComponent<BoxCollider2D>();
        _hasAnimation = _animation != null;
        velocity = new Vector2(0, 0);
        
        terrain = LayerMask.GetMask("Terrain");
        platform = LayerMask.GetMask("Platform");
    }

    private void FixedUpdate() {
        Vector3 groundComp = Vector3.zero;
        Vector3 wallComp = Vector3.zero;
        _frameVelocity = new Vector2(0, 0);
        groundComp = FloorRaycast();
        wallComp = WallRaycast();

        if(!midair)
        {
            fastfall = false;
        }

        if (jump && _hasJump) {
            _frameVelocity += new Vector2(0, _jumpHeight);
            _jumpDecayCurrent = _jumpDecay;
            midair = true;
            _hasJump = false;
        }
        
        if (midair) MidairPhysics();
        else GroundPhysics();

        if (_hasAnimation) {
            if (_facing > 0) {
                _animation.SetFlip(false);
            }
            else {
                _animation.SetFlip(true);
            }
        }

        velocity = (velocity * 0.95f) + _frameVelocity;
        if (!midair) velocity *= new Vector2(1, 0);

        if ((leftCollide && velocity.x < 0) || (rightCollide && velocity.x > 0)) 
            velocity *= new Vector2(0, 1);

        Vector3 newPos = (transform.position + (Vector3)velocity + groundComp + wallComp);

        if (_isDashing && Math.Abs(_dashStartPos - newPos.x) > _dashDistance)
        {
            newPos.x = _dashStartPos + _facing * (_dashDistance + 0.01f);
        }

        transform.position = newPos;
    }

    private void MidairPhysics() {
        _frameVelocity += new Vector2(_walkInput * _airSpeed, 0) + (Vector2.down * _gravity);

        if (jump) {
            if (_jumpDecayCurrent > 0) {
                _frameVelocity += new Vector2(0, _jumpDecayCurrent);
                _jumpDecayCurrent -= 0.003f;
            }
            else {
                jump = false;
            }
            _frameVelocity += new Vector2(0, _jumpDecayCurrent);
        }
        
        if (fastfall) {
            _frameVelocity += new Vector2(0, -_jumpHeight);
        }

        _frameVelocity = VelocityLimit(_frameVelocity, _maxVelocity);
    }

    private void GroundPhysics() {
        _frameVelocity = new Vector2(_walkInput * _groundSpeed, 0);
        if (dash) {
            _frameVelocity += new Vector2(_dashSpeed * _facing, 0);
            if (!_isDashing) StartCoroutine(DashCoroutine());
        }

        if (_dashKill) {
            velocity = new Vector2(velocity.x * 0.05f, velocity.y);
            _dashKill = false;
        }

        if (dash) {
            _frameVelocity = VelocityLimit(_frameVelocity, _maxVelocity * 40);
        } else if (sprint) {
            _frameVelocity = VelocityLimit(_frameVelocity, _maxVelocity * _sprintMod);
        }
        else {
            _frameVelocity = VelocityLimit(_frameVelocity, _maxVelocity);
        }

        if (!walk && !sprint && !dash) {
            velocity += new Vector2(-velocity.x * _groundFriction, velocity.y);
        }
    }

    private IEnumerator DashCoroutine() {
        _isDashing = true;
        _dashStartPos = transform.position.x;
        while(Math.Abs(transform.position.x - _dashStartPos) < _dashDistance && !leftCollide && !rightCollide && !midair)
        {
            yield return null;
        }
        dash = false;
        _isDashing = false;
        _dashKill = true;
    }

    private Vector3 FloorRaycast() {
        float leftXBound = _coll.bounds.min.x;
        float rightXBound = _coll.bounds.max.x;
        float bottomY = _coll.bounds.center.y;
        float rayDistance = 2f;

        RaycastHit2D leftRay = Physics2D.Raycast(new Vector2(leftXBound, bottomY), Vector2.down, rayDistance, terrain);
        RaycastHit2D rightRay = Physics2D.Raycast(new Vector2(rightXBound, bottomY), Vector2.down, rayDistance, terrain);

        //Debug.DrawRay(new Vector3(bottomY, leftXBound), Vector3.left*rayDistance, Color.green, 1f);
        //Debug.DrawRay(new Vector3(bottomY, rightXBound), Vector3.right*rayDistance, Color.green, 1f);

        if (leftRay || rightRay) {
            _hasJump = true;
            midair = false;
            if (leftRay && leftRay.point.y > _coll.bounds.min.y) {
                return new Vector2(0, (leftRay.point.y - _coll.bounds.min.y));
            }
            if (rightRay && rightRay.point.y > _coll.bounds.min.y) {
                return new Vector2(0, (rightRay.point.y - _coll.bounds.min.y));
            }
        }
        else {
            midair = true;
            return Vector2.zero;
        }
        return Vector2.zero;
    }

    private Vector2 WallRaycast() {
        float bottomBound = _coll.bounds.min.y + 0.25f;
        float topBound = _coll.bounds.max.y - 0.25f;
        float centerX = _coll.bounds.center.x;
        float rayDistance = 1.51f;
        
        //Debug.DrawRay(new Vector3(centerX, bottomBound), Vector3.left*rayDistance, Color.green, 1f);
        //Debug.DrawRay(new Vector3(centerX, bottomBound), Vector3.right*rayDistance, Color.green, 1f);
        //Debug.DrawRay(new Vector3(centerX, topBound), Vector3.left*rayDistance, Color.green, 1f);
        //Debug.DrawRay(new Vector3(centerX, topBound), Vector3.right*rayDistance, Color.green, 1f);

        RaycastHit2D topLeftRay = Physics2D.Raycast(new Vector2(centerX, topBound), Vector2.left, rayDistance, terrain);
        RaycastHit2D bottomLeftRay = Physics2D.Raycast(new Vector2(centerX, bottomBound), Vector2.left, rayDistance, terrain);
        RaycastHit2D topRightRay = Physics2D.Raycast(new Vector2(centerX, topBound), Vector2.right, rayDistance, terrain);
        RaycastHit2D bottomRightRay = Physics2D.Raycast(new Vector2(centerX, bottomBound), Vector2.right, rayDistance, terrain);

        if (topLeftRay || bottomLeftRay) {
            leftCollide = true;
            if (topLeftRay && topLeftRay.point.x > _coll.bounds.min.x) {
                return new Vector2((topLeftRay.point.x - _coll.bounds.min.x), 0);
            }
            if (bottomLeftRay && bottomLeftRay.point.x > _coll.bounds.min.x) {
                return new Vector2((bottomLeftRay.point.x - _coll.bounds.min.x), 0);
            }
        }
        else {
            leftCollide = false;
        }
        
        if (topRightRay || bottomRightRay) {
            rightCollide = true;
            //Debug.Log("Colliding right!");
            if (topRightRay && topRightRay.point.x > _coll.bounds.max.x) {
                return new Vector2((_coll.bounds.max.x - topRightRay.point.x), 0);
            }
            if (bottomRightRay && bottomRightRay.point.x > _coll.bounds.max.x) {
                return new Vector2((_coll.bounds.max.x - bottomRightRay.point.x), 0);
            }
        }
        else {
            rightCollide = false;
        }
        
        return Vector2.zero;
    }

    private Vector2 VelocityLimit(Vector2 input, float limit) {
        if (Mathf.Abs(input.x) > limit) {
            return new Vector2(Mathf.Sign(input.x) * limit, input.y);
        }
        else {
            return input;
        }
    }

    public void Walk(float input) {
        walk = true;
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
        sprint = true;
    }

    public void Dash() {
        if (!midair) {
            dash = true; 
        }
    }

    public void Jump() {
        if (_hasJump) {
            jump = true;
        }
    }

    public void JumpRelease() {
        jump = false;
    }

    public void FastFall() {
        if (midair) {
            fastfall = true;
        }
    }

    public void Stop() {
        walk = false;
        _walkInput = 0;
        sprint = false;
    }
}
