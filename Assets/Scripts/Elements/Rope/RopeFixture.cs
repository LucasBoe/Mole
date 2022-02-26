using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeFixture : RopeEnd
{
    protected override bool ShouldShowPrompt()
    {
        return base.ShouldShowPrompt() || PlayerRopeUser.Instance.IsActive;
    }

    protected override void PlayerTryInteract()
    {
        PlayerRopeUser ropeUser = PlayerRopeUser.Instance;

        if (ropeUser.IsActive && rope == null)
        {
            rope = ropeUser.HandoverRopeTo(rigidbody2D);
        }
        else if (rope != null && !ropeUser.IsActive)
        {
            ropeUser.TakeRopeFrom(rope, rigidbody2D);
            rope = null;
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<CrossbowBoltRopeCreator>() != null)
        {
            PlayerRopeUser.Instance.TryConnectCrossbowRopeBolt(this);
            Destroy(collision.gameObject);
        }

        base.OnTriggerEnter2D(collision);
    }
}
