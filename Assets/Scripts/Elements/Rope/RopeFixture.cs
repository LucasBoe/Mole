using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeFixture : MonoBehaviour
{
    private Rope rope;
    [SerializeField] private RopeElement connected;
    [SerializeField] private Rigidbody2D rigidbody2D;
    [SerializeField] private bool playerIsAbove = false;

    PlayerControlPromptUI prompt;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.IsPlayer())
            return;

        if (rope != null || PlayerRopeUser.Instance.IsActive)
            prompt = PlayerControlPromptUI.Show(ControlType.Interact, transform.position + Vector3.up);

        playerIsAbove = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.IsPlayer())
            return;

        if (prompt != null) prompt.Hide();
        playerIsAbove = false;

    }

    private void Update()
    {
        if (playerIsAbove && PlayerInputHandler.PlayerInput.Interact)
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
    }
}

public class RopeConnectionInformation
{
    public RopeAnchor Anchor;
    public float Length;
    public float Buffer;
    public Rigidbody2D attached;
}
