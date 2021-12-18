using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeFixture : MonoBehaviour
{
    [SerializeField] private Rope connected;
    [SerializeField] private Rigidbody2D rigidbody2D;
    private bool playerIsAbove = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.IsPlayer())
            return;

        if (connected != null || PlayerRopePuller.Instance.IsActive)
            PlayerControlPromptUI.Instance.Show(ControlType.Interact, transform.position + Vector3.up);

        playerIsAbove = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.IsPlayer())
            return;

        PlayerControlPromptUI.Instance.Hide();
        playerIsAbove = false;

    }

    private void Update()
    {
        if (playerIsAbove && PlayerInputHandler.PlayerInput.Interact)
        {
            if (PlayerRopePuller.Instance.IsActive && connected == null)
            {
                connected = RopeHandler.Instance.AttachPlayerRope(rigidbody2D);
            }
            else if (connected != null && !PlayerRopePuller.Instance.IsActive)
            {
                PlayerRopePuller.Instance.Activate(connected);
                connected = null;
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
