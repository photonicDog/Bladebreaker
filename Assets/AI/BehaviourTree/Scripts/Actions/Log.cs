using System.Collections;
using System.Collections.Generic;
using AI.BehaviourTree.Scripts.Runtime;
using UnityEngine;

namespace TheKiwiCoder {
    public class Log : ActionNode
    {
        public string message;

        protected override void OnStart() {
        }

        protected override void OnStop() {
        }

        protected override State OnUpdate() {
            Debug.Log($"{message}");
            return State.Success;
        }
    }
}
