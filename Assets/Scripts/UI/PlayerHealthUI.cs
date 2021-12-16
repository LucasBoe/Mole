using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] AnimationCurve lerpCurve = AnimationCurve.EaseInOut(0,0,1,1); 
    [SerializeField] Slider healthSliderAccurate, healSliderSmooth;
    float relativeHealthBefore = 1;

    // Start is called before the first frame update
    void Start()
    {
        PlayerHealth.Instance.OnHealthChange += AnimateHealthbarTo;
    }

    private void AnimateHealthbarTo(float relativeHealth)
    {
        StopAllCoroutines();
        StartCoroutine(AnimateHealthbarRoutine(relativeHealthBefore, relativeHealth));
        relativeHealthBefore = relativeHealth;
        healthSliderAccurate.value = relativeHealth;
    }

    IEnumerator AnimateHealthbarRoutine(float relativeHealthBefore, float relativeHealth)
    {
        float t = 0;
        float d = 0.75f;

        while (t < d)
        {
            t += Time.deltaTime;
            float lerp = lerpCurve.Evaluate(t / d);
            healSliderSmooth.value = Mathf.Lerp(relativeHealthBefore, relativeHealth, lerp);
            yield return null;
        }

        healSliderSmooth.value = relativeHealth;
    }
}
