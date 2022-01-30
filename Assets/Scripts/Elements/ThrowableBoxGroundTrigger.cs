using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IThrowListener
{
    void OnThrow();
}

public class ThrowableBoxGroundTrigger : MonoBehaviour, IThrowListener
{
    [SerializeField] BoxCollider2D boxCollider2D;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger" + collision.gameObject.layer + " / " + 0);

        if (collision.gameObject.layer == 0)
            boxCollider2D.enabled = true;
    }

    public void OnThrow()
    {
        boxCollider2D.enabled = false;
    }
}
