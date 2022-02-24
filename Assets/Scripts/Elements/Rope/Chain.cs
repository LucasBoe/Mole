using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Chain : Cable
{
    private ChainElement chainElement1 => elements[0] as ChainElement;
    private ChainElement chainElement2 => elements[1] as ChainElement;

    private float distributionChangeBefore = 0;
    public bool TidalLock = false;

    //smoothing
    [SerializeField, Range(-5f, 5f)] protected float smoothForceDifference = 0;
    [SerializeField, Range(-100f, 100f)] protected float forceDifferenceDebug = 0;

    [SerializeField] protected float tidalLockDuration = 1f;
    [SerializeField] protected float forceSmoothDuration = 0.1f;
    [SerializeField] protected float durationChangeMultiplier = 0.5f;

    public Chain(Rigidbody2D start, List<CableAnchor> cableAnchors, Rigidbody2D end) : base(start, cableAnchors, end) { }

    protected override CableElement CreateElementBetween(Rigidbody2D start, Rigidbody2D end, float length)
    {
        ChainElement instance = CableHandler.Instance.SpawnChainElement(start, end);
        instance.Setup(start, end, length);
        return instance;
    }

    private float BalanceOperationn()
    {


        float forceDifference = (chainElement1.PullForce - chainElement2.PullForce);
        forceDifferenceDebug = forceDifference;
        smoothForceDifference = Mathf.Lerp(smoothForceDifference, forceDifference, Time.deltaTime / forceSmoothDuration);
        return smoothForceDifference;
    }

    public override void Update()
    {
        ////balance
        float newDistributionChange = (Mathf.Sign(BalanceOperationn()) * Time.deltaTime * durationChangeMultiplier) / totalLength;

        if (Mathf.Sign(newDistributionChange) != Mathf.Sign(distributionChangeBefore))
            CableHandler.Instance.LockChainFor(tidalLockDuration, this);

        if (!TidalLock)
        {
            distribution += newDistributionChange;
            distribution = Mathf.Clamp(distribution, 0, 1);
        }
        distributionChangeBefore = newDistributionChange;

        base.Update();
    }
}

public class CreateCableElementResult
{
    public float Length;
    public CableElement Instance;
}