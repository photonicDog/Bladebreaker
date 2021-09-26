using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
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

    private RaycastHit2D _leftplatRay;
    private RaycastHit2D _rightplatRay;


    [SerializeField] private bool _freeMove;
    
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
    public bool allowPlatformNoclip = false;
    public bool platformDrop = false;
    public bool standingOnPlatform = false;

    public bool attackFreeze = false;
    [ShowInInspector] private bool _attackFreezeEnd = false;
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
        Vector3 groundComp = Vector3.zero;
        Vector3 wallComp = Vector3.zero;
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

        if (attackFreeze) {
            _attackFreezeEnd = true;
        }

        transform.position = _newPos;

        groundComp = FloorRaycast();
        wallComp = WallRaycast();

        if(allowPlatformNoclip && standingOnPlatform)
        {
            _hasJump = false;
        }

        transform.position += groundComp + wallComp;
    }

    private void MidairPhysics() {
        if (!antigravity) {
            _frameVelocity += (Vector2.down * _gravity);
        }

        if (!attackFreeze || !_attackFreezeEnd)
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
        walk = true;
    }

    private IEnumerator KeepPlatformDrop()
    {
        platformDrop = true;

        RaycastHit2D platformDetectRay = Physics2D.Raycast(new Vector2(_coll.bounds.center.x, _coll.bounds.min.y), Vector2.down, _coll.size.y, platform);

        while ((_leftplatRay || _rightplatRay) || platformDetectRay)
        {
            platformDetectRay = Physics2D.Raycast(new Vector2(_coll.bounds.center.x, _coll.bounds.min.y), Vector2.down, _coll.size.y, platform);
            yield return new WaitForEndOfFrame();
        }

        platformDrop = false;
    }

    private Vector3 FloorRaycast() {
        float leftXBound = _coll.bounds.min.x + _colliderWidthOffset + velocity.x;
        float rightXBound = _coll.bounds.max.x - _colliderWidthOffset + velocity.x;
        float midXBound = _coll.bounds.center.x + velocity.x;
        float bottomY = _coll.bounds.min.y + velocity.y;
        float centerY = _coll.bounds.center.y + velocity.y;
        float topY = _coll.bounds.max.y + velocity.y;
        float sizeX = _coll.bounds.size.x;
        float sizeY = _coll.bounds.size.y;
        float rayDistance = 2f;

        Physics2D.queriesStartInColliders = true;
        RaycastHit2D platformClipRightRay = Physics2D.Raycast(new Vector2(rightXBound, bottomY + 0.25f), Vector2.up, sizeY, platform);
        RaycastHit2D platformChipLeftRay = Physics2D.Raycast(new Vector2(leftXBound, bottomY + 0.25f), Vector2.up, sizeY, platform);
        Physics2D.queriesStartInColliders = false;

        RaycastHit2D leftRay = Physics2D.Raycast(new Vector2(leftXBound, centerY), Vector2.down, rayDistance, terrain);
        RaycastHit2D rightRay = Physics2D.Raycast(new Vector2(rightXBound, centerY), Vector2.down, rayDistance, terrain);
        RaycastHit2D leftPlatRay = Physics2D.Raycast(new Vector2(leftXBound, centerY), Vector2.down, rayDistance, platform);
        RaycastHit2D rightPlatRay = Physics2D.Raycast(new Vector2(rightXBound, centerY), Vector2.down, rayDistance, platform);

        RaycastHit2D leftHeadRay = Physics2D.Raycast(new Vector2(leftXBound, centerY - 0.25f), Vector2.up, sizeY/2, terrain);
        RaycastHit2D rightHeadRay = Physics2D.Raycast(new Vector2(rightXBound, centerY - 0.25f), Vector2.up, sizeY/2, terrain);

        _leftplatRay = leftPlatRay;
        _rightplatRay = rightPlatRay;

        Debug.DrawRay(new Vector3(leftXBound, centerY), Vector3.down * rayDistance, Color.red, 0f);
        Debug.DrawRay(new Vector3(rightXBound, centerY), Vector3.down * rayDistance, Color.red, 0f);
        Debug.DrawRay(new Vector3(leftXBound + 0.25f, bottomY + 0.25f), Vector2.up * sizeY/2, Color.green, 0f);
        Debug.DrawRay(new Vector3(rightXBound - 0.25f, bottomY + 0.25f), Vector2.up * sizeY/2, Color.green, 0f);
        Debug.DrawRay(new Vector3(leftXBound + 0.25f, centerY - 0.25f), Vector2.up * sizeY/2, Color.blue, 0f);
        Debug.DrawRay(new Vector3(rightXBound - 0.25f, centerY - 0.25f), Vector2.up * sizeY/2, Color.blue, 0f);

        allowPlatformNoclip = (platformChipLeftRay || platformClipRightRay);

        if ((leftRay || rightRay) && velocity.y <= 0) {
            standingOnPlatform = false;
            _hasJump = true;
            midair = false;
            fastfall = false;
            velocity.y = 0;
            if (leftRay && leftRay.point.y > _coll.bounds.min.y) {
                return new Vector2(0, (leftRay.point.y - _coll.bounds.min.y));
            }
            if (rightRay && rightRay.point.y > _coll.bounds.min.y)
            {
                return new Vector2(0, (rightRay.point.y - _coll.bounds.min.y));
            }
        }
        else if (((_leftplatRay && !platformDrop) || (_rightplatRay && !platformDrop)) && !allowPlatformNoclip && velocity.y <= 0)
        {
            standingOnPlatform = true;
            _hasJump = true;
            midair = false;
            fastfall = false;
            velocity.y = 0;
            if (leftPlatRay && leftPlatRay.point.y > _coll.bounds.min.y)
            {
                return new Vector2(0, (leftPlatRay.point.y - _coll.bounds.min.y));
            }
            if (rightPlatRay && rightPlatRay.point.y > _coll.bounds.min.y)
            {
                return new Vector2(0, (rightPlatRay.point.y - _coll.bounds.min.y));
            }
        } else if ((leftHeadRay || rightHeadRay) && velocity.y > 0 && midair) {
            midair = true;
            velocity.y = 0;
        }
        else if (allowPlatformNoclip)
        {
            standingOnPlatform = true;
            midair = true;
            return Vector2.zero;
        }
        else if (!leftRay && !rightRay) {
            midair = true;
            return Vector2.zero;
        }
        return Vector2.zero;
    }

    private Vector2 WallRaycast() {
        float bottomBound = _coll.bounds.min.y + 0.25f + velocity.y;
        float topBound = _coll.bounds.max.y - 0.25f + velocity.y;
        float leftX = _coll.bounds.min.x + velocity.x;
        float rightX = _coll.bounds.max.x + velocity.x;
        float rayDistance = Math.Max(_coll.bounds.size.x, _horizontalCollisionRange);
        
        //Debug.DrawRay(new Vector3(rightX, bottomBound), Vector3.left*rayDistance, Color.blue, 0f);
        //Debug.DrawRay(new Vector3(leftX, bottomBound), Vector3.right*rayDistance, Color.blue, 0f);
        //Debug.DrawRay(new Vector3(rightX, topBound), Vector3.left*rayDistance, Color.blue, 0f);
        //Debug.DrawRay(new Vector3(leftX, topBound), Vector3.right*rayDistance, Color.blue, 0f);

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
        _animation.StopHurt();
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
        StartCoroutine(KeepPlatformDrop());
        if (midair) {
            StartCoroutine(FastfallCoroutine());
        }
    }

    public void Stop() {
        _attackFreezeEnd = false;
        walk = false;
        _walkInput = 0;
        sprint = false;
    }

    public void FullStop() {
        Stop();
        velocity = Vector2.zero;
    }
}
