using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CableHandler : SingletonBehaviour<CableHandler>
{
    [SerializeField] private ChainElement chainElementPrefab;
    [SerializeField] private RopeElement ropeElementPrefab;
    [SerializeField] private RopeEnd ropeEndPrefab;

    [ReadOnly, SerializeField] private List<Cable> cables = new List<Cable>();
    private Dictionary<Chain, Coroutine> chainLockRegister = new Dictionary<Chain, Coroutine>();


    [ReadOnly, SerializeField] public Chain[] chainDebug;

    public Chain CreateChain(Rigidbody2D start, Rigidbody2D end, List<CableAnchor> anchors)
    {
        Chain newChain = new Chain(start, anchors, end);
        cables.Add(newChain);

        chainDebug = new Chain[1];
        chainDebug[0] = cables[0] as Chain;

        return newChain;
    }

    public Rope CreateRope(Rigidbody2D start, Rigidbody2D end, List<CableAnchor> anchors = null, Vector2[] pathPoints = null)
    {
        Rope newRope = (pathPoints == null) ?  new Rope(start, anchors, end) : new Rope(start, end, pathPoints);
        CheckAndPotentiallyConnectPlayer(newRope, start, end);
        cables.Add(newRope);
        return newRope;
    }

    private void CheckAndPotentiallyConnectPlayer(Cable newRope, Rigidbody2D start, Rigidbody2D end)
    {
        PlayerRopeUser playerStart = start.GetComponentInChildren<PlayerRopeUser>();
        PlayerRopeUser playerEnd = end.GetComponentInChildren<PlayerRopeUser>();

        if (playerStart != null)
            playerStart.ConnectToRope(newRope as Rope, playerIsAtStart: true);
        else if (playerEnd != null)
            playerStart.ConnectToRope(newRope as Rope, playerIsAtStart: false);
    }

    internal void DestroyCable(Rigidbody2D rigidbody2D)
    {
        Cable toDestroy = null;

        foreach (Cable rope in cables)
        {
            List<Rigidbody2D> rigidbody2Ds = new List<Rigidbody2D>();
            foreach (CableElement element in rope.Elements)
            {
                if (element != null)
                {
                    if (element.Rigidbody2DAttachedTo != null) rigidbody2Ds.Add(element.Rigidbody2DAttachedTo);
                    if (element.Rigidbody2DOther != null) rigidbody2Ds.Add(element.Rigidbody2DOther);
                }
            }
            if (rigidbody2Ds.Contains(rigidbody2D))
                toDestroy = rope;
        }

        if (toDestroy != null)
            DestroyCable(toDestroy);
    }

    internal void DestroyCable(Cable cable)
    {
        foreach (CableElement element in cable.Elements)
        {

            if (element != null)
                element.Destroy();
        }
        cables.Remove(cable);
    }
    public ChainElement SpawnChainElement(Rigidbody2D start, Rigidbody2D end)
    {
        return Instantiate(chainElementPrefab, start.position, Quaternion.identity, LayerHandler.Parent);
    }
    public RopeElement SpawnRopeElement(Rigidbody2D start, Rigidbody2D end)
    {
        Util.DebugDrawCircle(start.position, Color.yellow, 1, lifetime: 5);
        return Instantiate(ropeElementPrefab, start.position, Quaternion.identity, LayerHandler.Parent);
    }

    internal RopeEnd CreateRopeEnd(Vector2 position)
    {
        return Instantiate(ropeEndPrefab, position, Quaternion.identity, LayerHandler.Parent);
    }

    private void Update()
    {
        foreach (Cable rope in cables)
            rope.Update();
    }
    internal void LockChainFor(float duration, Chain toLock)
    {
        if (chainLockRegister.ContainsKey(toLock))
        {
            StopCoroutine(chainLockRegister[toLock]);
            chainLockRegister.Remove(toLock);
        }

        toLock.TidalLock = true;
        chainLockRegister.Add(toLock, StartCoroutine(LockChainRoutine(duration, () =>
        {
            chainLockRegister.Remove(toLock);
            toLock.TidalLock = false;
        })));
    }

    private IEnumerator LockChainRoutine(float duration, System.Action callback)
    {
        yield return new WaitForSeconds(duration);
        callback?.Invoke();
    }
}
