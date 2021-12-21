using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeFixture : RopeEnd
{
    protected override bool ShouldShowPrompt()
    {
        return base.ShouldShowPrompt() || PlayerRopeUser.Instance.IsActive;
    }

    protected override void PlayerTryInteract(PlayerRopeUser ropeUser)
    {
        Debug.LogWarning(name);

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
}
