using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Rework Effect Pipeline to be scriptableObjectBased
public class EffectHandler : SingletonBehaviour<EffectHandler>
{
    public ParticleSystem waterSplashPrefab;
    public static void Spawn(Effect effect, Vector2 position)
    {
        Type type = effect.GetType();
        EffectSpawnResult result = null;

        if (type == typeof(WaterSplashEffect))
        {
            result = new EffectSpawnResult()
            {
                ResultType = EffectSpawnType.ParticleSystem,
                ParticleSystem = Instantiate(Instance.waterSplashPrefab, position, Quaternion.identity)
            };
        }


        if (type == typeof(CustomEffect))
        {
            CustomEffect customEffect = effect as CustomEffect;

            if (customEffect.CustomSpawnType == EffectSpawnType.ParticleSystem)
            {
                ParticleSystem instance = Instantiate(customEffect.PSPrefab, position, Quaternion.identity);

                result = new EffectSpawnResult()
                {
                    ResultType = EffectSpawnType.ParticleSystem,
                    ParticleSystem = instance,
                    GameObject = instance.gameObject
                };
            }
            else
            {
                result = new EffectSpawnResult()
                {
                    ResultType = EffectSpawnType.GameObject,
                    GameObject = Instantiate(customEffect.GOPrefab, position, Quaternion.identity)
                };
            }
        }

        if (result != null)
        {
            Destroy(result.GameObject, effect.Duration);

            if (result.ResultType == EffectSpawnType.ParticleSystem)
            {
                effect.Play(result.ParticleSystem);
            }
            else
                effect.Play();
        }
    }
}

public class Effect
{
    public float Duration = 0f;

    public virtual void Play() { }

    public virtual void Play(ParticleSystem particleSystem) { }
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
public enum EffectSpawnType
{
    ParticleSystem,
    GameObject
}

public class CustomEffect : Effect
{
    public EffectSpawnType CustomSpawnType;
    public GameObject GOPrefab;
    public ParticleSystem PSPrefab;
    public CustomEffect(ParticleSystem prefab, float duration = 10f)
    {
        CustomSpawnType = EffectSpawnType.ParticleSystem;
        PSPrefab = prefab;
        Duration = duration;
    }
    public CustomEffect(GameObject prefab, float duration = 10f)
    {
        CustomSpawnType = EffectSpawnType.GameObject;
        GOPrefab = prefab;
        Duration = duration;
    }
}

public class EffectSpawnResult
{
    public EffectSpawnType ResultType;
    public ParticleSystem ParticleSystem;
    public GameObject GameObject;
}