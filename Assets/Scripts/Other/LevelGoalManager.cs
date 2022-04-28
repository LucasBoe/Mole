using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Level.Goals
{
    public class LevelGoalManager : MonoBehaviour
    {
        [SerializeField]
        NeverDetectedGoal neverDetectedGoal;
        [SerializeField]
        NobodyKilledGoal nobodyKilledGoal;
        [SerializeField]
        NoBodysGoal noBodysGoal;

        private void OnEnable()
        {
            neverDetectedGoal.OnEnable();
            nobodyKilledGoal.OnEnable();
            noBodysGoal.OnEnable();
        }

        private void Start()
        {
            neverDetectedGoal.Update();
            nobodyKilledGoal.Update();
            noBodysGoal.Update();
        }

        private void OnDisable()
        {
            neverDetectedGoal.OnDisable();
            nobodyKilledGoal.OnDisable();
            noBodysGoal.OnDisable();
        }
    }

    public abstract class LevelGoal
    {
        public LevelGoalUIElement ui;
        public GoalState State;
        public abstract void OnEnable();
        public abstract void OnDisable();
        public abstract GoalState UpdateState();

        public void Update()
        {
            State = UpdateState();
            ui.UpdateUI(State);
        }

        public enum GoalState
        {
            Open,
            Success,
            Failed
        }
    }

    [System.Serializable]
    public class NeverDetectedGoal : LevelGoal
    {
        bool hasBeenDetected = false;
        public override void OnEnable()
        {
            EnemyPlayerDetectionModule.PlayerFound += OnPlayerFound;
        }

        public override void OnDisable()
        {
            EnemyPlayerDetectionModule.PlayerFound -= OnPlayerFound;
        }

        private void OnPlayerFound()
        {
            hasBeenDetected = true;
            Update();
        }

        public override GoalState UpdateState()
        {
            if (!hasBeenDetected)
                return GoalState.Success;

            return GoalState.Failed;
        }
    }

    [System.Serializable]
    public class NobodyKilledGoal : LevelGoal
    {
        [SerializeField] List<EnemyDamageModule> enemys;
        public override void OnEnable()
        {
            enemys = new List<EnemyDamageModule>(UnityEngine.Object.FindObjectsOfType<EnemyDamageModule>());
            foreach (EnemyDamageModule enemy in enemys)
            {
                enemy.OutOfHealth += Update;
            }
        }
        public override void OnDisable()
        {
            foreach (EnemyDamageModule enemy in enemys)
            {
                if (enemy != null)
                    enemy.OutOfHealth -= Update;
            }
        }

        public override GoalState UpdateState()
        {
            Debug.Log("UpdateState: " + enemys.Count);

            foreach (EnemyDamageModule enemy in enemys)
            {
                Debug.Log(enemy.Dead);

                if (enemy == null || enemy.Dead)
                    return GoalState.Failed;
            }

            return GoalState.Success;
        }
    }

    [System.Serializable]
    public class NoBodysGoal : LevelGoal
    {
        [SerializeField] List<EnemyRagdoll> unconciousRagdolls;
        public override void OnEnable()
        {
            EnemyRagdoll.ChangedUnconcious += OnChangedUnconcious;
        }
        public override void OnDisable()
        {
            EnemyRagdoll.ChangedUnconcious -= OnChangedUnconcious;
        }

        private void OnChangedUnconcious(EnemyRagdoll enemyRagdoll, bool unconcious)
        {
            if (unconcious)
                unconciousRagdolls.AddUnique(enemyRagdoll);
            else
                unconciousRagdolls.Remove(enemyRagdoll);

            Update();
        }

        public override GoalState UpdateState()
        {
            return (unconciousRagdolls.Count > 0) ? GoalState.Open : GoalState.Success;
        }
    }
}