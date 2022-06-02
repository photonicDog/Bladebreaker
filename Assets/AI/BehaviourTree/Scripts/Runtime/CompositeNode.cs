using System.Collections.Generic;
using UnityEngine;

namespace AI.BehaviourTree.Scripts.Runtime {
    public abstract class CompositeNode : Node {
        [HideInInspector] public List<Node> children = new List<Node>();

        public override Node Clone() {
            CompositeNode node = Instantiate(this);
            node.children = children.ConvertAll(c => c.Clone());
            return node;
        }
    }
}