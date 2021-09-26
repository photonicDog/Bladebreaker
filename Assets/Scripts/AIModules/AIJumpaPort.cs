using System;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Unity.Mathematics;
using UnityEngine;

namespace AIModules {
    public class AIJumpaPort : AIModuleBase {
        
        [NonSerialized, OdinSerialize][ShowInInspector] private float teleportDistance;
        [NonSerialized, OdinSerialize][ShowInInspector] private GameObject puff;
        
        private EntityMovement _em;

        private Transform playerTransform;
        private EntityMovement pem;
        private bool midairChecking = false;
        public override void Start(EntityAI entityAI) {
            ended = false;
            this._entityAI = entityAI;
            
            playerTransform = GameObject.FindWithTag("Player").transform;
            pem = playerTransform.GetComponent<EntityMovement>();
            _em = entityAI.GetComponent<EntityMovement>();

            JumpaportAction();
        }

        async void JumpaportAction() {
            float ctime = Time.realtimeSinceStartup;
            _em.FullStop();
            _em.Jump();

            
            while (ctime + time > Time.realtimeSinceStartup) {
                await Task.Yield();
            }
            
            midairChecking = true;

            GameObject.Instantiate(puff, _entityAI.transform.position, quaternion.identity);

            ctime = Time.realtimeSinceStartup;
            Vector3 guyPos = new Vector2(999, 999);
            
            while (ctime + 1f > Time.realtimeSinceStartup) {
                _entityAI.transform.position = guyPos;
                await Task.Yield();
            }
            
            Vector2 teleportPosition = new Vector2((playerTransform.position.x - pem._facing * teleportDistance), playerTransform.position.y + 6);
            _entityAI.transform.position = teleportPosition;
            _em.velocity *= Vector2.right;
        }

        public override void Do() {
            if (!_em.midair && midairChecking) {
                End();
            }
        }

        public override void End() {
            ended = true;
        }
    }
}