using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheKiwiCoder
{
    public abstract class Node : ScriptableObject
    {
        public enum State
        {
            Running,
            Failure,
            Success
        }

        [HideInInspector] public State state = State.Running;
        [HideInInspector] public bool started = false;
        [HideInInspector] public string guid;
        [HideInInspector] public Vector2 position;
        [HideInInspector] public Context context;
        [HideInInspector] public Blackboard blackboard;
        [TextArea] public string description;
        public bool drawGizmos = false;
        public AnimationClip animationOverride;

        public State Update()
        {
            bool init = false;
            if (!started)
            {
                OnStart();
                started = true;

                init = true;
            }

            if (context != null) context.runner.LastNode = ToString();
            if (context != null) context.runner.SendWatch("node", name);
            state = OnUpdate();

            if (init && state == State.Running)
                context.EnteredState?.Invoke(this);

            if (state != State.Running)
            {
                OnStop();
                started = false;
            }

            return state;
        }

        public virtual Node Clone()
        {
            return Instantiate(this);
        }

        public void Abort()
        {
            BehaviourTree.Traverse(this, (node) =>
            {
                node.started = false;
                node.state = State.Running;
                node.OnStop();
            });
        }

        public virtual void OnDrawGizmos() { }

        protected abstract void OnStart();
        protected abstract void OnStop();
        protected abstract State OnUpdate();

        protected void Log(string message)
        {
            context.runner.SendLog(message);
        }

        public virtual void Bind(Context context, Blackboard blackboard)
        {
            this.context = context;
            this.blackboard = blackboard;
        }
    }
}