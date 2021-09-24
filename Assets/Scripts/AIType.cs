<<<<<<<< HEAD:Assets/Scripts/AIType.cs
public enum AIType {
    WAIT,
    WALK,
    ATTACK,
    CHARGE,
    RETREAT,
    CONDITIONAL
}
========
using System;
using AIModules;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class AIAction {
    public int index;
    public AIType action;
    public float time;

    [ShowIf("action", AIType.WAIT)] 
    public bool roamWhileWaiting;
    [ShowIf("action", AIType.GUARD)] 
    public bool plusOnBlock;
    [ShowIf("action", AIType.ATTACK)] 
    public bool airAttack;
    [ShowIf("action", AIType.CHARGE)] 
    public float chargeDistance;
    [ShowIf("action", AIType.RETREAT)] 
    public float retreatDistance;
    [ShowIf("action", AIType.WALK)] 
    public float runSpeed;
    [ShowIf("action", AIType.WALK)] 
    public float stopDistance;
    public AIModuleBase Build() {
        switch (action) {
            case AIType.WAIT:
                return new AIWait(time, roamWhileWaiting);
                break;
            case AIType.GUARD:
                return new AIGuard(time, plusOnBlock);
                break;
            case AIType.ATTACK:
                return new AIAttack(time, airAttack);
                break;
            case AIType.CHARGE:
                return new AICharge(time, chargeDistance);
                break;
            case AIType.RETREAT:
                return new AIRetreat(time, retreatDistance);
                break;
            case AIType.WALK:
                return new AIWalk(time, runSpeed, stopDistance);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
>>>>>>>> 0d17ee7 (feat: ziplet AI):Assets/Scripts/AIAction.cs
