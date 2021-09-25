using System;
using AIModules;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class AIAction {
    public AIType action;
    public float time;

    [ShowIf("action", AIType.WAIT)] 
    public bool roamWhileWaiting;
    

    
    [ShowIf("action", AIType.ATTACK)] 
    public float attackRange;
    
    [ShowIf("action", AIType.CHARGE)] 
    public float chargeDistanceX;
    [ShowIf("action", AIType.CHARGE)] 
    public float chargeDistanceY;
    
    [ShowIf("action", AIType.RETREAT)] 
    public float retreatDistance;
    
    [ShowIf("action", AIType.WALK)] 
    public float runSpeed;
    [ShowIf("action", AIType.WALK)] 
    public float stopDistance;


    
    [ShowIf("action", AIType.CONDITIONAL)][BoxGroup("Conditionals")] public AIConditionalType conditionalType;
    [ShowIf("action", AIType.CONDITIONAL)][BoxGroup("Conditionals")] public int target;
    [ShowIf("action", AIType.CONDITIONAL)][BoxGroup("Conditionals")] public int failure;
    [ShowIf("conditionalType", AIConditionalType.IFCHANCE)][BoxGroup("Conditionals")] public float chance;
    [ShowIf("conditionalType", AIConditionalType.IFDISTANCE)][BoxGroup("Conditionals")] public float distance;
    [ShowIf("conditionalType", AIConditionalType.GUARD)][BoxGroup("Conditionals")] public bool plusOnBlock;
    public AIModuleBase Build() {
        switch (action) {
            case AIType.WAIT:
                return new AIWait(time, roamWhileWaiting);
            case AIType.ATTACK:
                return new AIAttack(time, attackRange);
            case AIType.CHARGE:
                return new AICharge(time, chargeDistanceX, chargeDistanceY);
            case AIType.RETREAT:
                return new AIRetreat(time, retreatDistance);
            case AIType.WALK:
                return new AIWalk(time, runSpeed, stopDistance);
            case AIType.CONDITIONAL:
                switch (conditionalType) {
                    case AIConditionalType.GUARD:
                        return new AIGuard(time, target, plusOnBlock);
                    case AIConditionalType.IFDISTANCE:
                        return new AIIfDistance(time, target, failure, distance);
                    case AIConditionalType.IFCHANCE:
                        return new AIIfChance(time, target, failure, chance);
                    case AIConditionalType.GOTO:
                        return new AIGoto(time, target);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}