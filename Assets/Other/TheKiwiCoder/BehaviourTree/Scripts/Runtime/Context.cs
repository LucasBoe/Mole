using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TheKiwiCoder
{

    // The context is a shared object every node has access to.
    // Commonly used components and subsytems should be stored here
    // It will be somewhat specfic to your game exactly what to add here.
    // Feel free to extend this class 
    public class Context
    {
        public GameObject gameObject;
        public Transform transform;
        public BehaviourTreeRunner runner;
        public EnemyMoveModule moveModule;
        public EnemyVariableModule variable;
        public EnemyPlayerDetectionModule playerDetectionModule;
        public EnemyShootModule shootModule;
        public EnemyMemoryModule memory;
        public EnemyDamageModule damageModule;
        public EnemyGroundCheckModule groundCheck;
        public EnemyRigidbodyControllerModule rigigbodyController;
        public Vector2 PlayerPos => memory.PlayerPos;
        public System.Action<Node> EnteredState;

        // Add other game specific systems here

        public static Context CreateFromGameObject(GameObject gameObject)
        {
            // Fetch all commonly used components
            Context context = new Context();
            context.gameObject = gameObject;
            context.transform = gameObject.transform;
            context.moveModule = gameObject.GetComponent<EnemyMoveModule>();
            context.variable = gameObject.GetComponent<EnemyVariableModule>();
            context.shootModule = gameObject.GetComponent<EnemyShootModule>();
            context.playerDetectionModule = gameObject.GetComponent<EnemyPlayerDetectionModule>();
            context.memory = gameObject.GetComponent<EnemyMemoryModule>();
            context.damageModule = gameObject.GetComponent<EnemyDamageModule>();
            context.groundCheck = gameObject.GetComponentInChildren<EnemyGroundCheckModule>();
            context.rigigbodyController = gameObject.GetComponent<EnemyRigidbodyControllerModule>();

            // Add whatever else you need here...

            return context;
        }
    }
}