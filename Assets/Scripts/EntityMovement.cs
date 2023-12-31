using System;
using System.Collections;
using Assets.Scripts.Controllers;
using Sirenix.OdinInspector;
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

    [SerializeField] private bool _bouncy;
    [ShowIf("_bouncy")][SerializeField] private int _bounces;
    private int bouncesLeft;
    private Vector3 saveBounceSpeed;

    [SerializeField] private bool _freeMove;

    private Coroutine _defer;
    private Coroutine _antigrav;
    
    private RaycastHit2D _leftplatRay;
    private RaycastHit2D _rightplatRay;

    private RaycastHit2D _leftRay;
    private RaycastHit2D _rightRay;
    
    public Vector2 velocity;
    public Vector2 _frameVelocity;

    private Rigidbody2D _rb;
    private BoxCollider2D _coll;
    [SerializeField] private EntityAnimation _animation;

    internal float _walkInput;
    public int _facing = 1;
    public bool midair = true;
    public bool _hasJump = true;
    public bool IsPaused = false;
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
    public bool inGround = false;
    public bool standingOnPlatform = false;
    public bool levelFinish;

    public bool attackFreeze = false;
    [ShowInInspector] private bool _attackFreezeEnd = false;
    public bool antigravity = false;
    public bool hitstun;

    private LayerMask terrain;
    private LayerMask platform;

    public bool leftCollide;
    public bool rightCollide;

    [Header("SFX")]
    public AudioClip JumpSFX;
    public AudioClip LandSFX;

    private float _dashStartPos;
    private Vector3 _newPos;
    private AudioController _ac;
    private bool _isPlayer;
    private bool _landedThisFrame;

    // Start is called before the first frame update
    void Awake() {
        _rb = GetComponent<Rigidbody2D>();
        _coll = GetComponent<BoxCollider2D>();
        _hasAnimation = _animation != null;
        velocity = new Vector2(0, 0);
        
        terrain = LayerMask.GetMask("Terrain");
        platform = LayerMask.GetMask("Platform");
        levelFinish = false;
        _isPlayer = gameObject.CompareTag("Player");
        if (_bouncy) bouncesLeft = _bounces;
    }

    void Start()
    {
        _ac = AudioController.Instance;
    }

    private void FixedUpdate() {
        if (!IsPaused) {
            _frameVelocity = new Vector2(0, 0);

            if (levelFinish) {
                walk = true;
                _walkInput = 1;
            }

            if (!midair) {
                fastfall = false;
            }

            if (jump && _hasJump && !attackFreeze) {
                if (_isPlayer) {
                    _ac.PlayPlayerSFX(JumpSFX);
                }
                else {
                    _ac.PlayEnemySFX(JumpSFX);
                }

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

            if (midair && _bouncy && (Mathf.Abs(velocity.y) > 0.001 || Mathf.Abs(velocity.x) > 0.001)) {
                Debug.Log("BOUNCESAVE");
                saveBounceSpeed = velocity;
            }

            velocity = (velocity * ((_bouncy) ? 1f : 0.95f)) + _frameVelocity;
            if (!midair) velocity *= new Vector2(1, 0);

            if ((leftCollide && velocity.x < 0) || (rightCollide && velocity.x > 0))
                velocity *= new Vector2(0, 1);


            _newPos = (transform.position + (Vector3) velocity);

            if (_isDashing && Math.Abs(_dashStartPos - _newPos.x) > _dashDistance && !rightCollide && !leftCollide) {
                _newPos.x = _dashStartPos + _facing * (_dashDistance + 0.01f);
            }

            if (attackFreeze) {
                _attackFreezeEnd = true;
            }

            transform.position = _newPos;

            Vector3 groundComp = FloorRaycast();
            Vector3 wallComp = WallRaycast();

            if (allowPlatformNoclip && standingOnPlatform) {
                _hasJump = false;
            }

            if (_landedThisFrame) {
                if (_isPlayer) {
                    _ac.PlayPlayerSFX(LandSFX);
                }
                else {
                    _ac.PlayEnemySFX(LandSFX);
                }
            }

            transform.position += groundComp + wallComp;
            inGround = false;
        }
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

            if (_dashKill) {
                velocity = new Vector2(_facing * _sprintMod * _airResistance, velocity.y);
                _dashKill = false;
            }

            if (fastfall) {
                _frameVelocity += new Vector2(0, -_jumpHeight);
            }

            if (_walkInput == 0) {
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

            if (attackFreeze && sprint) {
                StartCoroutine(SprintStopIfAttackingCoroutine());
            }

            if (_dashKill) {
                velocity = new Vector2(_facing * _sprintMod * _groundFriction, velocity.y);
                _dashKill = false;
            }

            if (dash) {
                _frameVelocity = VelocityLimit(_frameVelocity, _maxVelocity * 40);
            }
            else if (sprint) {
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

    public void Antigravity() {
        if (_antigrav == null) _antigrav = StartCoroutine(AntigravityCoroutine());
        else {
            StopCoroutine(_antigrav);
            antigravity = false;
            _antigrav = StartCoroutine(AntigravityCoroutine());
        }
    }

    public void StopAntigravity() {
        if (_antigrav != null) StopCoroutine(_antigrav);
        antigravity = false;
    }

    public void BounceCheck(bool y) {
        if (_bouncy && bouncesLeft > 0) {
            midair = true;
            if (y) BounceY();
            else BounceX();
            bouncesLeft--;
        }
    }

    public void BounceY() {
        Debug.Log("BOUCNEY" + saveBounceSpeed);
        velocity = saveBounceSpeed * new Vector2(1, -1);
    }

    public void BounceX() {
        Debug.Log("BOUCNEX");
        velocity = saveBounceSpeed * new Vector2(-1, 1);
    }

    IEnumerator AntigravityCoroutine() {
        if (velocity.y < 0) velocity = new Vector2(velocity.x, 0);
        antigravity = true;
        yield return new WaitForSeconds(1f);
        antigravity = false;
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
        bool midairThisFrame = midair;

        Physics2D.queriesStartInColliders = true;
        RaycastHit2D platformClipRightRay = Physics2D.Raycast(new Vector2(rightXBound, bottomY + 0.25f), Vector2.up, sizeY, platform);
        RaycastHit2D platformClipLeftRay = Physics2D.Raycast(new Vector2(leftXBound, bottomY + 0.25f), Vector2.up, sizeY, platform);
        Physics2D.queriesStartInColliders = false;

        RaycastHit2D leftRay = Physics2D.Raycast(new Vector2(leftXBound, centerY), Vector2.down, _verticalCollisionRange, terrain);
        RaycastHit2D rightRay = Physics2D.Raycast(new Vector2(rightXBound, centerY), Vector2.down, _verticalCollisionRange, terrain);
        RaycastHit2D leftPlatRay = Physics2D.Raycast(new Vector2(leftXBound, centerY), Vector2.down, _verticalCollisionRange, platform);
        RaycastHit2D rightPlatRay = Physics2D.Raycast(new Vector2(rightXBound, centerY), Vector2.down, _verticalCollisionRange, platform);

        RaycastHit2D leftHeadRay = Physics2D.Raycast(new Vector2(leftXBound, centerY - 0.25f), Vector2.up, sizeY/2, terrain);
        RaycastHit2D rightHeadRay = Physics2D.Raycast(new Vector2(rightXBound, centerY - 0.25f), Vector2.up, sizeY/2, terrain);

        _leftplatRay = leftPlatRay;
        _rightplatRay = rightPlatRay;
        _leftRay = leftRay;
        _rightRay = rightRay;

        Debug.DrawRay(new Vector3(leftXBound, centerY), Vector3.down * _verticalCollisionRange, Color.red, 0f);
        Debug.DrawRay(new Vector3(rightXBound, centerY), Vector3.down * _verticalCollisionRange, Color.red, 0f);
        Debug.DrawRay(new Vector3(leftXBound + 0.25f, bottomY + 0.25f), Vector2.up * sizeY/2, Color.green, 0f);
        Debug.DrawRay(new Vector3(rightXBound - 0.25f, bottomY + 0.25f), Vector2.up * sizeY/2, Color.green, 0f);
        Debug.DrawRay(new Vector3(leftXBound + 0.25f, centerY - 0.25f), Vector2.up * sizeY/2, Color.blue, 0f);
        Debug.DrawRay(new Vector3(rightXBound - 0.25f, centerY - 0.25f), Vector2.up * sizeY/2, Color.blue, 0f);

        allowPlatformNoclip = (platformClipLeftRay || platformClipRightRay) && !standingOnPlatform;

        if ((leftRay || rightRay) && velocity.y <= 0) {
            standingOnPlatform = false;
            inGround = true;
            _hasJump = true;
            midair = false;
            fastfall = false;
            velocity.y = 0;
            _landedThisFrame = midairThisFrame && !midair;
            inGround = (_leftRay.point.y > _coll.bounds.min.y + 0.125f || _rightRay.point.y > _coll.bounds.min.y + 0.125f);
            if (leftRay) {
                BounceCheck(true);
                return new Vector2(0, (leftRay.point.y - _coll.bounds.min.y));
            }
            if (rightRay)
            {
                BounceCheck(true);
                return new Vector2(0, (rightRay.point.y - _coll.bounds.min.y));
            }

        }
        else if (((_leftplatRay && !platformDrop) || (_rightplatRay && !platformDrop)) && !allowPlatformNoclip && velocity.y <= 0)
        {
            standingOnPlatform = true;
            inGround = true;
            _hasJump = true;
            midair = false;
            fastfall = false;
            velocity.y = 0;
            _landedThisFrame = midairThisFrame && !midair;
            inGround = (_leftplatRay.point.y > _coll.bounds.min.y + 0.125f || _rightplatRay.point.y > _coll.bounds.min.y + 0.125f);
            if (leftPlatRay && leftPlatRay.point.y > _coll.bounds.min.y)
            {
                BounceCheck(true);
                return new Vector2(0, (leftPlatRay.point.y - _coll.bounds.min.y));
            }
            if (rightPlatRay && rightPlatRay.point.y > _coll.bounds.min.y)
            {
                BounceCheck(true);
                return new Vector2(0, (rightPlatRay.point.y - _coll.bounds.min.y));
            }
        } else if ((leftHeadRay || rightHeadRay) && velocity.y > 0 && midair)
        {
            standingOnPlatform = false;
            midair = true;
            velocity.y = 0;
            return Vector2.zero;
        }
        else if (allowPlatformNoclip)
        {
            standingOnPlatform = true;
            midair = true;
            return Vector2.zero;
        }
        else if (!leftRay && !rightRay)
        {
            standingOnPlatform = false;
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
        
        //Debug.DrawRay(new Vector3(rightX, bottomBound), Vector3.left*rayDistance, Color.blue, 0f);
        //Debug.DrawRay(new Vector3(leftX, bottomBound), Vector3.right*rayDistance, Color.blue, 0f);
        //Debug.DrawRay(new Vector3(rightX, topBound), Vector3.left*rayDistance, Color.blue, 0f);
        //Debug.DrawRay(new Vector3(leftX, topBound), Vector3.right*rayDistance, Color.blue, 0f);

        RaycastHit2D topLeftRay = Physics2D.Raycast(new Vector2(rightX, topBound), Vector2.left, rayDistance, terrain);
        RaycastHit2D bottomLeftRay = Physics2D.Raycast(new Vector2(rightX, bottomBound), Vector2.left, rayDistance, terrain);
        RaycastHit2D topRightRay = Physics2D.Raycast(new Vector2(leftX, topBound), Vector2.right, rayDistance, terrain);
        RaycastHit2D bottomRightRay = Physics2D.Raycast(new Vector2(leftX, bottomBound), Vector2.right, rayDistance, terrain);

        if ((topLeftRay || bottomLeftRay) && !(inGround)) {
            leftCollide = true;
            if (topLeftRay && topLeftRay.point.x > _coll.bounds.min.x) {
                BounceCheck(false);
                return new Vector2((topLeftRay.point.x - _coll.bounds.min.x), 0);
            }
            if (bottomLeftRay && bottomLeftRay.point.x > _coll.bounds.min.x) {
                BounceCheck(false);
                return new Vector2((bottomLeftRay.point.x - _coll.bounds.min.x), 0);
            }

        }
        else {
            leftCollide = false;
        }
        
        if ((topRightRay || bottomRightRay) && !(inGround)) {
            rightCollide = true;
            if (topRightRay && topRightRay.point.x < _coll.bounds.max.x) {
                BounceCheck(false);
                return new Vector2((topRightRay.point.x - _coll.bounds.max.x), 0);
            }
            if (bottomRightRay && bottomRightRay.point.x < _coll.bounds.max.x) {
                BounceCheck(false);
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

        while (midair) {
            yield return new WaitForEndOfFrame();
        }
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
        if (levelFinish) return;
        if (hitstun) {
            if (_defer != null) {
                StopCoroutine(_defer);
                _defer = null;
            }
            _defer = StartCoroutine(DeferInput(input));
            return;
        }
        walk = true;
        _walkInput = input;
        int sign = Math.Sign(input);
        if (sign != 0) {
            _facing = sign;
        }
    }

    IEnumerator DeferInput(float input) {
        while (hitstun) {
            yield return new WaitForEndOfFrame();
        }

        Walk(input);
    }
    
    public void Sprint(float input)
    {
        if (levelFinish) return;
        if (hitstun) return;
        _walkInput = input*_sprintMod;
        int sign = Math.Sign(input);
        if (sign != 0) {
            _facing = sign;
        }
        sprint = true;
    }

    public void Dash()
    {
        if (levelFinish) return;
        if (hitstun) return;
        if (!midair) {
            dash = true; 
        }
    }

    public void Jump()
    {
        if (hitstun) return;
        if (_hasJump) {
            jump = true;
        }
    }

    public void JumpRelease() {
        jump = false;
    }

    public void FastFall()
    {
        if (hitstun) return;
        StartCoroutine(KeepPlatformDrop());
        if (midair) {
            StartCoroutine(FastfallCoroutine());
        }
    }

    public void Stop() {
        if (_defer != null) {
            StopCoroutine(_defer);
            _defer = null;
        }
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
