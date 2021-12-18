using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeFixture : MonoBehaviour
{
    Rigidbody2D rigidbody2D;
    [SerializeField] Rope connected;

    bool playerIsAbove = false;

    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.IsPlayer())
        {
            if (connected != null || PlayerRopePuller.Instance.IsActive)
                PlayerControlPromptUI.Instance.Show(ControlType.Interact, transform.position + Vector3.up);

            playerIsAbove = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.IsPlayer())
        {
            PlayerControlPromptUI.Instance.Hide();
            playerIsAbove = false;
        }
    }

    private void Update()
    {
        if (playerIsAbove && PlayerInputHandler.PlayerInput.Interact)
        {
            if (PlayerRopePuller.Instance.IsActive && connected == null)
            {
                RopeConnectionInformation info = PlayerRopePuller.Instance.DeactivateAndFetchInfo();
                info.attached = rigidbody2D;
                connected = RopeHandler.Instance.CreateRope(info);
            }
            else if (connected != null && !PlayerRopePuller.Instance.IsActive)
            {
                PlayerRopePuller.Instance.ReplaceRope(connected);
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
