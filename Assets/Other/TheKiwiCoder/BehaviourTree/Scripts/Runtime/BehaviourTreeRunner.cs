using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace TheKiwiCoder {
    public class BehaviourTreeRunner : EnemyModule<BehaviourTreeRunner> {

        // The main behaviour tree asset
        public BehaviourTree Tree;
        public bool Run = false;

        // Storage container object to hold game object subsystems
        Context context;

        public Context Context => context;

        [ReadOnly] public Node LastNode = null;
        [ShowNativeProperty] string LastNodeName => (LastNode == null ? "Null" : LastNode.ToString());

        // Start is called before the first frame update
        void Start() {
            context = CreateBehaviourTreeContext();
            context.runner = this;
            Tree = Tree.Clone();
            Tree.Bind(context);
        }

        // Update is called once per frame
        void Update() {
            if (Tree && Run) {
                Tree.Update();
            } else
            {
                if (Time.time > 0.5f)
                    Run = true;
            }
        }

        Context CreateBehaviourTreeContext() {
            return Context.CreateFromGameObject(gameObject);
        }

        private void OnDrawGizmosSelected() {
            if (!Tree) {
                return;
            }

            BehaviourTree.Traverse(Tree.rootNode, (n) => {
                if (n.drawGizmos) {
                    n.OnDrawGizmos();
                }
            });
        }
    }
}