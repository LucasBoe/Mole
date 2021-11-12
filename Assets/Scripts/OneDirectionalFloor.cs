using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFloor
{
    void DeactivateUntilPlayerIsAboveAgain(PlayerController playerController);
}

public class OneDirectionalFloor : MonoBehaviour, IFloor
{
    PlayerController player;
    float playerYonDeactivation;
    const float upperFadeValue = 0.2f /* small buffer */, lowerFadeValue = 2f; //player hight + buffer

    [SerializeField] Collider2D collider;

    public void DeactivateUntilPlayerIsAboveAgain(PlayerController playerController)
    {
        player = playerController;
        playerYonDeactivation = player.transform.position.y;
        collider.enabled = false;
        StartCoroutine(CheckForGettingSolidAgainRoutine());
    }

    private IEnumerator CheckForGettingSolidAgainRoutine()
    {
        bool shouldBeSolidAgain = false;
        while (!shouldBeSolidAgain)
        {
            float playerY = player.transform.position.y;
            shouldBeSolidAgain = (playerY > playerYonDeactivation + upperFadeValue || playerY < (playerYonDeactivation - lowerFadeValue));
            yield return new WaitForSeconds(0.05f);
        }

        collider.enabled = true;
        StopAllCoroutines();
    }
}
