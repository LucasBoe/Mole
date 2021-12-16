using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpriteDamageVisualizer : MonoBehaviour
{
    [SerializeField] AnimationCurve lerpCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    Material playerMaterial;

    // Start is called before the first frame update
    void Start()
    {
        PlayerHealth.Instance.OnHealthChange += VisualizeDamage;
        playerMaterial = GetComponent<SpriteRenderer>().material;
    }

    private void VisualizeDamage(float newHealth)
    {
        StopAllCoroutines();
        StartCoroutine(AnimateDamageOverlay());

    }

    IEnumerator AnimateDamageOverlay()
    {
        float t = 0;
        float d = 0.5f;

        while (t < d)
        {
            t += Time.deltaTime;
            playerMaterial.SetFloat("_DamageOverlayTransparency", lerpCurve.Evaluate(t / d));
            yield return null;
        }
    }
}
