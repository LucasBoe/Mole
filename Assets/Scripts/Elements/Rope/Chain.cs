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

    public Chain(Rigidbody2D start, List<CableAnchor> cableAnchors, Rigidbody2D end)
    {
        anchors = cableAnchors;
        totalLength = Util.GetDistance(Util.Merge(new Vector2[] { start.position }, cableAnchors.Select(c => (Vector2)c.transform.position).ToArray(), new Vector2[] { end.position }).ToArray());

        if (IsShortCable)
        {
            CreateChainElementResult newElement = CreateElementBetween(start, end);
            DefineElementsShort(newElement);
        }
        else
        {
            CreateChainElementResult newElement1 = CreateElementBetween(start, cableAnchors[0].Rigidbody2D);
            CreateChainElementResult newElement2 = CreateElementBetween(end, cableAnchors.Last().Rigidbody2D);
            DefineElementsLong(newElement1, newElement2);
        }
    }

    private CreateChainElementResult CreateElementBetween(Rigidbody2D start, Rigidbody2D end)
    {
        CreateChainElementResult result = new CreateChainElementResult()
        {
            Instance = CableHandler.Instance.CreateChainElement(start, end),
            Length = Vector2.Distance(start.position, end.position)
        };

        result.Instance.Setup(start, end, result.Length);
        return result;
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
            CableHandler.Instance.LockFor(tidalLockDuration, this);

        if (!TidalLock)
        {
            distribution += newDistributionChange;
            distribution = Mathf.Clamp(distribution, 0, 1);
        }
        distributionChangeBefore = newDistributionChange;

        base.Update();
    }
}

public class CreateChainElementResult
{
    public float Length;
    public ChainElement Instance;
}