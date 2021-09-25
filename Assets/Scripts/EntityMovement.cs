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
    [SerializeField] private float _airResistance;

    [SerializeField] private float _gravity;

    [SerializeField] private float _maxVelocity;

    [SerializeField] private float _horizontalCollisionRange;
    [SerializeField] private float _verticalCollisionRange;
    [SerializeField] private float _colliderWidthOffset;
    [SerializeField] private float _fastfallFrameDelay;


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
    public bool downHeld = false;

    public bool attackFreeze = false;
    public bool antigravity = false;
    public bool hitstun;

    private LayerMask terrain;
    private LayerMask platform;

    public bool leftCollide;
    public bool rightCollide;

    private float _dashStartPos;
    private Vector3 _newPos;

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
        _frameVelocity = new Vector2(0, 0);

        if (!midair)
        {
            fastfall = false;
        }

        if (jump && _hasJump && !attackFreeze) {
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


        _newPos = (transform.position + (Vector3)velocity);

        if (_isDashing && Math.Abs(_dashStartPos - _newPos.x) > _dashDistance && !rightCollide && !leftCollide)
        {
            _newPos.x = _dashStartPos + _facing * (_dashDistance + 0.01f);
        }

        transform.position = _newPos;

        Vector3 groundComp = FloorRaycast();
        Vector3 wallComp = WallRaycast();

        transform.position += groundComp + wallComp;
    }

    private void MidairPhysics() {
        if (!antigravity) {
            _frameVelocity += (Vector2.down * _gravity);
        }

        if (!attackFreeze)
            _frameVelocity += new Vector2(_walkInput * _airSpeed, 0);

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

        if (_dashKill)
        {
            velocity = new Vector2(_facing * _sprintMod * _airResistance, velocity.y);
            _dashKill = false;
        }

        if (fastfall) {
            _frameVelocity += new Vector2(0, -_jumpHeight);
        }

        if (_walkInput == 0)
        {
            _frameVelocity.x *= _airResistance;
        }

        _frameVelocity = VelocityLimit(_frameVelocity, _maxVelocity);
    }

    private void GroundPhysics() {
        if (!attackFreeze) 
            _frameVelocity = new Vector2(_walkInput * _groundSpeed, 0);
        
        if (dash && !attackFreeze) {
            _frameVelocity += new Vector2(_dashSpeed * _facing, 0);
            if (!_isDashing) StartCoroutine(DashCoroutine());
        }

        if (attackFreeze && sprint)
        {
            StartCoroutine(SprintStopIfAttackingCoroutine());
        }

        if (_dashKill) {
            velocity = new Vector2(_facing * _sprintMod * _groundFriction, velocity.y);
            _dashKill = false;
        }

        if (dash) {
            _frameVelocity = VelocityLimit(_frameVelocity, _maxVelocity * 40);
        } else if (sprint) {
            _frameVelocity = VelocityLimit(_frameVelocity, _maxVelocity * _sprintMod);
        } else {
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
            if (Math.Abs(transform.position.x - _dashStartPos) < _dashDistance / 2)
            {
                _hasJump = false;
            }
            yield return null;
        }
        dash = false;
        _isDashing = false;
        _dashKill = true;
    }

    private IEnumerator FastfallCoroutine()
    {
        for(int i = 0; i < _fastfallFrameDelay; i++) {
            if (!downHeld) break;
            yield return new WaitForEndOfFrame();
        }
        if (midair && downHeld)
        {
            fastfall = true;
        }
    }

    // Stops sprint after one frame to allow dash attack that replies on sprint boolean
    private IEnumerator SprintStopIfAttackingCoroutine()
    {
        yield return new WaitForEndOfFrame();
        sprint = false;
    }

    private Vector3 FloorRaycast() {
        Bounds bounds = _coll.bounds;
        float leftXBound = bounds.min.x + _colliderWidthOffset + velocity.x;
        float rightXBound = bounds.max.x - _colliderWidthOffset + velocity.x;
        float bottomY = bounds.center.y + velocity.y;
        float rayDistance = _verticalCollisionRange;

        RaycastHit2D leftRay = Physics2D.Raycast(new Vector2(leftXBound, bottomY), Vector2.down, rayDistance, terrain);
        RaycastHit2D rightRay = Physics2D.Raycast(new Vector2(rightXBound, bottomY), Vector2.down, rayDistance, terrain);

        Debug.DrawRay(new Vector3(leftXBound, bottomY), Vector3.down*rayDistance, Color.red, 0f);
        Debug.DrawRay(new Vector3(rightXBound, bottomY), Vector3.down*rayDistance, Color.red, 0f);

        if (leftRay || rightRay) {
            _hasJump = true;
            midair = false;
            fastfall = false;
            velocity.y = 0;
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
        Bounds bounds = _coll.bounds;
        float bottomBound = bounds.min.y + 0.25f + velocity.y;
        float topBound = bounds.max.y - 0.25f + velocity.y;
        float leftX = bounds.min.x + velocity.x;
        float rightX = bounds.max.x + velocity.x;
        float rayDistance = Math.Max(bounds.size.x, _horizontalCollisionRange);
        
        Debug.DrawRay(new Vector3(rightX, bottomBound), Vector3.left*rayDistance, Color.blue, 0f);
        Debug.DrawRay(new Vector3(leftX, bottomBound), Vector3.right*rayDistance, Color.blue, 0f);
        Debug.DrawRay(new Vector3(rightX, topBound), Vector3.left*rayDistance, Color.blue, 0f);
        Debug.DrawRay(new Vector3(leftX, topBound), Vector3.right*rayDistance, Color.blue, 0f);

        RaycastHit2D topLeftRay = Physics2D.Raycast(new Vector2(rightX, topBound), Vector2.left, rayDistance, terrain);
        RaycastHit2D bottomLeftRay = Physics2D.Raycast(new Vector2(rightX, bottomBound), Vector2.left, rayDistance, terrain);
        RaycastHit2D topRightRay = Physics2D.Raycast(new Vector2(leftX, topBound), Vector2.right, rayDistance, terrain);
        RaycastHit2D bottomRightRay = Physics2D.Raycast(new Vector2(leftX, bottomBound), Vector2.right, rayDistance, terrain);

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
            if (topRightRay && topRightRay.point.x < _coll.bounds.max.x) {
                return new Vector2((topRightRay.point.x - _coll.bounds.max.x), 0);
            }
            if (bottomRightRay && bottomRightRay.point.x < _coll.bounds.max.x) {
                return new Vector2((bottomRightRay.point.x - _coll.bounds.max.x), 0);
            }
        }
        else {
            rightCollide = false;
        }
        
        return Vector2.zero;
    }

    public void PushEntity(Vector2 impulse)
    {
        if (impulse.y > 0)
        {
            midair = true;
            attackFreeze = false;
        }
        velocity += impulse;
    }

    public void FloatEntity(float lift)
    {
        velocity.y = lift;
    }

    public void Hitstun(float time) {
        velocity = Vector2.zero;
        hitstun = true;
        StartCoroutine(HitstunTimer(time));
    }

    IEnumerator HitstunTimer(float time) {
        yield return new WaitForSeconds(time);
        hitstun = false;
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
        if (hitstun) return;
        walk = true;
        _walkInput = input;
        int sign = Math.Sign(input);
        if (sign != 0) {
            _facing = sign;
        }
    }
    
    public void Sprint(float input) {
        if (hitstun) return;
        _walkInput = input*_sprintMod;
        int sign = Math.Sign(input);
        if (sign != 0) {
            _facing = sign;
        }
        sprint = true;
    }

    public void Dash() {
        if (hitstun) return;
        if (!midair) {
            dash = true; 
        }
    }

    public void Jump() {
        if (hitstun) return;
        if (_hasJump) {
            jump = true;
        }
    }

    public void JumpRelease() {
        jump = false;
    }

    public void FastFall() {
        if (hitstun) return;
        if (midair) {
            StartCoroutine(FastfallCoroutine());
        }
    }

    public void Stop() {
        walk = false;
        _walkInput = 0;
        sprint = false;
    }

    public void FullStop() {
        Stop();
        velocity = Vector2.zero;
    }
}
