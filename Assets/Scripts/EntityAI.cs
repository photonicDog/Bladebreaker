using System.Collections;
using System.Collections.Generic;
using AIModules;
using Assets.Scripts.Components;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

public class EntityAI : SerializedMonoBehaviour {
    
    public bool logicEnabled;
    public bool playerDetected;
    public EntityMovement em;
    public EntityAnimation ea;
    public Harmable harm;
    
    private Vector3 startPosition;
    public AIBehavior AI;
    
    [Header("AI Parameters")]
    [HideInInspector] public Vector3 wanderRadiusCenter;
    public Vector3 wanderRadiusCenterOffset;
    public float wanderRadiusBounds;
    
    [HideInInspector] public Vector3 leashRadiusCenter;
    public Vector3 leashRadiusCenterOffset;
    public float leashRadiusBounds;

    public float detectionRadius = 5;

    public bool currentlyWandering;

    private Vector2 moveDirection = Vector2.left;
    private bool leashBounded;

    private GameObject player;
    private Harmable playerHarm;

    public int _behaviorQueueIndex = 0;
    private AIModuleBase currentModule;
    private List<AIModuleBase> modules;

    private float entityRandomSpeed;

    private float entityRandomSpeed;

    // Start is called before the first frame update
    void Start()
    {
        leashRadiusCenter = leashRadiusCenterOffset + transform.position;
        wanderRadiusCenter = wanderRadiusCenterOffset + transform.position;
        player = GameObject.FindWithTag("Player");
        playerHarm = player.GetComponent<Harmable>();
        modules = new List<AIModuleBase>(AI.behaviorCycle);

        if (AI.canWander) {
            currentlyWandering = true;
        }

        entityRandomSpeed = Random.Range(0.90f, 1.1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerDetected && logicEnabled) {
            IdleBehavior();
        }

        if (playerDetected && logicEnabled) {
            PlayerDetectedBehavior();
        }

        if (Vector2.Distance(player.transform.position, transform.position) < detectionRadius) {
            playerDetected = true;
        }
    }

    void IdleBehavior() {
        if (AI.canWander && currentlyWandering) {
            if (AI.canFly) {
                
            }
            else {
                Wander();
                LeashCheck();
                em.Walk(moveDirection.x * entityRandomSpeed);
            }
        }
    }

    void LeashCheck() {
        if ((transform.position - leashRadiusCenter).magnitude > leashRadiusBounds/2) {
            leashBounded = true;
        }
        
        if ((transform.position - leashRadiusCenter).magnitude < leashRadiusBounds/2 - 0.5f) {
            leashBounded = false;
        }

        if (leashBounded) {
            float currentDistance = (transform.position - leashRadiusCenter).magnitude;
            float prospectiveDistance = ((transform.position + (Vector3) moveDirection) - leashRadiusCenter).magnitude;
            if (prospectiveDistance > currentDistance) moveDirection = Vector2.zero;
        }
    }
    
    void PlayerDetectedBehavior() {
        if (!logicEnabled) {
            moveDirection = Vector2.zero;
            return;
        }

        em._facing = 1 * (int)Mathf.Sign((player.transform.position - transform.position).x);
        if (currentModule == null) {
            currentModule = modules[_behaviorQueueIndex];
            currentModule.Start(this);
        }
        else {
            if (currentModule.ended) {
                if (currentModule.GetType().BaseType != typeof(AIModuleConditional)) 
                    _behaviorQueueIndex++;
                if (_behaviorQueueIndex >= AI.behaviorCycle.Count) {
                    _behaviorQueueIndex = 0;
                }
                
                Debug.Log(name + " AI set to " + modules[_behaviorQueueIndex].GetType());
                currentModule.ended = false;
                currentModule = null;
            }
            else {
                currentModule.Do();
            }

        }

        LeashCheck();
        if (moveDirection.magnitude > 0.05f) {
            em.Walk(moveDirection.x * entityRandomSpeed);
        }
    }

    public void Walk(Vector2 direction) {
        moveDirection = direction;
    }

    public void Stop() {
        moveDirection = Vector2.zero;
        em.velocity = Vector2.zero;
    }

    public void Attack() {
        ea.Attack();
    }

    void Wander() {
        if (transform.position.x > wanderRadiusCenter.x + wanderRadiusBounds/2) {
            moveDirection = new Vector2(Mathf.Abs(moveDirection.x) * -1, moveDirection.y);
        }
        if (transform.position.x < wanderRadiusCenter.x - wanderRadiusBounds/2) {
            moveDirection = new Vector2(Mathf.Abs(moveDirection.x), moveDirection.y);
        }
    }

    public void TryDamage(Harmable input, Hitbox hitbox) {
        if (currentModule.GetType() == typeof(AIGuard)) {
            //TODO: Guard effect
            ((AIGuard) currentModule).GuardHit();
            playerHarm.Damage(0, 1f, 0.5f, 0.5f, this.transform);
            
        } else
        {
            ea.Hurt();
            input.Damage(hitbox);
        }
    }
    
    void FlyWander() {
        
    }

    public void Hitstun(float time) {
        moveDirection = Vector2.zero;
        logicEnabled = false;
        StartCoroutine(HitstunTimer(time));
    }

    IEnumerator HitstunTimer(float time) {
        yield return new WaitForSeconds(time);
        logicEnabled = true;
    }
}
