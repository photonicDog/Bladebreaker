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
    public float attackRange;
    
    [ShowIf("action", AIType.CHARGE)] 
    public float chargeDistance;
    
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

    public AIModuleBase Build() {
        switch (action) {
            case AIType.WAIT:
                return new AIWait(time, roamWhileWaiting);
                break;
            case AIType.GUARD:
                return new AIGuard(time, plusOnBlock);
                break;
            case AIType.ATTACK:
                return new AIAttack(time, attackRange);
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
            case AIType.CONDITIONAL:
                switch (conditionalType) {
                    case AIConditionalType.IFDISTANCE:
                        return new AIIfDistance(time, target, failure, distance);
                        break;
                    case AIConditionalType.IFCHANCE:
                        return new AIIfChance(time, target, failure, chance);
                        break;
                    case AIConditionalType.GOTO:
                        return new AIGoto(time, target);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}