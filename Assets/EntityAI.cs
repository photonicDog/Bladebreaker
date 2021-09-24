using System.Collections;
using System.Collections.Generic;
using AIModules;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

public class EntityAI : SerializedMonoBehaviour {
    
    public bool logicEnabled;
    public bool playerDetected;
    public EntityMovement em;
    public EntityAnimation ea;
    
    private Vector3 startPosition;
    public AIBehavior AI;
    
    [Header("AI Parameters")]
    public Vector3 wanderRadiusCenter;
    public float wanderRadiusBounds;
    
    public Vector3 leashRadiusCenter;
    public float leashRadiusBounds;

    public float detectionRadius;

    public bool currentlyWandering;


    private Vector2 moveDirection = Vector2.left;
    private bool leashBounded;

    private GameObject player;

    public int _behaviorQueueIndex = 0;
    private AIModuleBase currentModule;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");

        if (AI.canWander) {
            currentlyWandering = true;
        }
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
                em.Walk(moveDirection.x);
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
        if (currentModule == null) {
            currentModule = AI.behaviorCycle[_behaviorQueueIndex].Build();
            currentModule.Start(this);
        }
        else {
            currentModule.Do();
            if (currentModule.ended) {
                if (currentModule.GetType() != typeof(AIModuleConditional)) 
                    _behaviorQueueIndex++;
                if (_behaviorQueueIndex >= AI.behaviorCycle.Count) {
                    _behaviorQueueIndex = 0;
                }
                
                currentModule = null;
            }
        }

        LeashCheck();
        em.Walk(moveDirection.x);
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
    
    void FlyWander() {
        
    }
}
