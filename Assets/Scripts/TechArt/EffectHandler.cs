using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectHandler : SingletonBehaviour<EffectHandler>
{
    public ParticleSystem waterSplashPrefab;
    public static void Spawn(Effect effect, Vector2 position)
    {
        ParticleSystem spawned = null;

        if (effect.GetType() == typeof(WaterSplashEffect))
            spawned = Instantiate(Instance.waterSplashPrefab, position, Quaternion.identity);

        if (spawned != null)
        {
            effect.Play(spawned);
            Destroy(spawned.gameObject, effect.Duration);
        }
        else
            effect.Play();
    }
}

public class Effect
{
    public float Duration = 0f;

    public virtual void Play()
    {

    }

    public virtual void Play(ParticleSystem particleSystem)
    {

    }
}

public class WaterSplashEffect : Effect
{
    private Vector2 dir;

    public WaterSplashEffect(Vector2 impact)
    {
        Duration = 2f;
        dir = Vector2.Reflect(impact, Vector2.up);
    }

    public override void Play(ParticleSystem particleSystem)
    {
        particleSystem.transform.forward = dir.normalized;
        var main = particleSystem.main;
        main.startSpeed = dir.magnitude;
    }
}