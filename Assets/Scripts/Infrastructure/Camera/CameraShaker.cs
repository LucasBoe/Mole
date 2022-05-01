using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using NaughtyAttributes;
using System;

public class CameraShaker : SingletonBehaviour<CameraShaker>
{
    [SerializeField] CinemachineVirtualCamera cinemachineVirtualCamera;
    CinemachineBasicMultiChannelPerlin noise;
    [SerializeField] CameraShakeCurve[] shakeCurves;

    [SerializeField, Range(0, 10)] float amplitude = 1;
    [SerializeField, Range(0, 1)] float frequency = 0.1f;

    List<CameraShake> activeShakes = new List<CameraShake>();

    // Start is called before the first frame update
    void Start()
    {
        noise = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    // Update is called once per frame
    void Update()
    {
        float amplitude = 0;
        float frequency = 0;

        List<CameraShake> finished = new List<CameraShake>();

        foreach (CameraShake shake in activeShakes)
        {
            float distanceEffector = 1f - (Mathf.Min(Vector2.Distance(transform.position, shake.Origin), shake.Range) / shake.Range);
            amplitude += shake.GetAmplitude(distanceEffector);
            frequency += shake.Frequency;

            if (shake.Done)
                finished.Add(shake);
        }

        if (finished.Count > 0)
        {
            for (int i = activeShakes.Count - 1; i >= 0; i--)
            {
                activeShakes.Remove(activeShakes[i]);
            }
        }

        frequency /= shakeCurves.Length;

        noise.m_AmplitudeGain = amplitude;
        noise.m_FrequencyGain = frequency;
    }

    public void Shake(Vector2 origin, CameraShakeType type = CameraShakeType.Explosion, float strength = 1f, float frequency = 0.25f, float duration = 1f, float range = 15f)
    {
        AnimationCurve curve = CurveFromType(type);
        if (curve == null) return;

        CameraShake shake = new CameraShake(origin, curve, strength, frequency, duration, range);
        activeShakes.Add(shake);
    }

    private AnimationCurve CurveFromType(CameraShakeType type)
    {
        foreach (CameraShakeCurve shake in shakeCurves)
        {
            if (shake.Type == type)
                return shake.Curve;
        }

        return null;
    }

    private class CameraShake
    {
        private Vector2 origin;
        public Vector2 Origin => origin;
        private float strength;
        public float Strength => strength;
        private float frequency;
        public float Frequency => frequency;


        private float startTimeStamp;
        private float duration;
        private AnimationCurve curve;
        private float range;
        public float Range => range;
        public bool Done = false;

        public CameraShake(Vector2 origin, AnimationCurve curve, float strength, float freqeuncy, float duration, float range)
        {
            this.origin = origin;
            this.curve = curve;
            this.strength = strength;
            this.frequency = freqeuncy;
            this.duration = duration;
            this.range = range;
            this.startTimeStamp = Time.time;
        }

        internal float GetAmplitude(float distanceEffector)
        {
            float t = (Time.time - startTimeStamp) / duration;
            Done = t >= 1f;
            return curve.Evaluate(t) * distanceEffector * strength;
        }
    }

    [System.Serializable]
    private class CameraShakeCurve
    {
        [SerializeField] public CameraShakeType Type;
        [SerializeField, CurveRange(0, 0, 1, 1, EColor.Red)] public AnimationCurve Curve;
    }

    private void OnDrawGizmos()
    {
        foreach (CameraShake shake in activeShakes)
        {
            Gizmos.DrawWireSphere(shake.Origin, shake.Range);
        }
    }
}

public enum CameraShakeType
{
    Explosion,
}
