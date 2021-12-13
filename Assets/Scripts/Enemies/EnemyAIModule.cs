using UnityEngine;

public class EnemyAIModule : EnemyModule<EnemyAIModule>
{
    EnemyStatemachineModule statemachineModule;
    EnemyViewconeModule viewconeModule;
    EnemyMemoryModule memoryModule;

    [SerializeField] NoiseListener noiseListener;

    private void Start()
    {
        statemachineModule = GetModule<EnemyStatemachineModule>();
        viewconeModule = GetModule<EnemyViewconeModule>();
        memoryModule = GetModule<EnemyMemoryModule>();

        viewconeModule.OnPlayerEnter += OnPlayerEnteredViewcone;
        viewconeModule.OnPlayerExit += CheckOutLocation;
        noiseListener.OnNoise += CheckOutLocation;
    }

    private void CheckOutLocation(Vector2 location)
    {
        memoryModule.SetTarget(location);
        statemachineModule.OverrideState(new EnemyWaitState(1f));
        statemachineModule.OverrideState(new EnemyAlertState(onTargetReached: LookAround));
    }

    private void LookAround()
    {
        statemachineModule.OverrideState(new EnemyWaitState(0.5f));
        statemachineModule.OverrideState(new EnemyLookAroundState());

    }

    private void OnPlayerEnteredViewcone(Transform target)
    {
        memoryModule.SetTarget(target);
        statemachineModule.OverrideState(new EnemyWaitState(1f));
        statemachineModule.OverrideState(new EnemyAlertState(onTargetReached: LookAround));
    }
}
