using System.Threading.Tasks;
using AI.BehaviourTree.Scripts.Runtime;
using Unity.Mathematics;
using UnityEngine;

namespace AI.BehaviourTree.Scripts.Actions {
    public class Jumpaport : ActionNode {

        public float time = 1;
        public float teleportDistance = 4;

        private float cTime;
        private bool midairChecking = false;
        private EntityMovement pem;
        private int phase = 0;
        
        protected override void OnStart() {
            cTime = Time.realtimeSinceStartup;
            context.entityMovement.FullStop();
            context.entityMovement.Jump();
            pem = blackboard.player.GetComponent<EntityMovement>();
            phase = 0;
        }

        protected override void OnStop() {
            
        }

        protected override State OnUpdate() {
            switch (phase) {
                case 0:
                    while (cTime + time > Time.realtimeSinceStartup) {
                        return State.Running;
                    }
            
                    midairChecking = true;
                    context.enemyModel.EmitPuff();
                    cTime = Time.realtimeSinceStartup;
                    phase = 1;
                    return State.Running;
                case 1:
                    while (cTime + 1f > Time.realtimeSinceStartup) {
                        Vector3 guyPos = new Vector2(999, 999);
                        context.transform.position = guyPos;
                        return State.Running;
                    }
                    //HEH NOTHING PERSONELL KID
                    Vector2 teleportPosition = new Vector2((blackboard.player.transform.position.x - pem._facing * teleportDistance), blackboard.player.transform.position.y + 6);
                    context.transform.position = teleportPosition;
                    context.entityMovement.velocity *= Vector2.right;
                    return State.Success;
                default:
                    return State.Failure;
            }
        }
    }
}