using System.Collections.Generic;
using UnityEngine;

namespace UModules
{
    // Thoughts:
    //  3D positional scale, Z affects zoom?
    public class CameraShake : ExtendedBehaviour
    {
        /// <summary>The amount of trauma required to reach maximum shake values</summary>
        /// <access>private float</access>
        [SerializeField]
        private float maxTrauma = 1000;

        /// <summary>Intensity scale from 0 to max trauma (at t=1)</summary>
        /// <access>private CurveAsset</access>
        [SerializeField]
        private CurveAsset intensityCurve;

        /// <summary>
        /// Speed at which noise field traveral happens. 
        /// Higher values result in faster shaking, lower values in a handheld camera feel.
        /// </summary>
        /// <access>private float</access>
        [Header("Speeds")]
        [SerializeField]
        private float shakeSpeed = 10;

        /// <summary>
        /// Speed at which added traumas decay (units per second)
        /// </summary>
        /// <aceess>private float</access>
        [SerializeField]
        private float decaySpeed = 1000;

        /// <summary>Maximum offset (+/-) from original position that can occur as a result of camera shake</summary>
        /// <access>private float</access>
        [Header("Extremes")]
        [SerializeField]
        private float maxOffset = 0.2f;

        /// <summary>Maximum rotation (+/-) that occur as a result of camera shake (degrees)</summary>
        /// <access>private float</access>
        [SerializeField]
        private float maxRotation = 10;

        /// <summary>All trauma sources, sorted with a name and value (with X and Y components)</summary>
        /// <access>private Dictionary&lt;string, Vector2&gt;</access>
        private Dictionary<string, Vector2> traumaSources = new Dictionary<string, Vector2>();

        /// <summary>Current accumulated time value, used to sample noise</summary>
        /// <access>private float</access>
        private float t;

        /// <summary>
        /// Modes for calling AddTrauma. 
        /// KeepMax always uses the higher of the existing and new values (for each component). 
        /// Add always adds the new value to the existing value. 
        /// Replace always uses the new value. 
        /// </summary>
        /// <access>public enum</access>
        public enum TraumaMode { KeepMax, Add, Replace }

        /// <summary>Add trauma with a given source name.</summary>
        /// <param name="sourceName" type="string">Name to use to compare with existing trauma sources</param>
        /// <param name="amount" type="Vector2">Intensity of trauma in two dimensions (always positive)</param>
        /// <param name="mode" type="TraumaMode">How should the new value interact with any existing values?</param>
        public void AddTrauma(string sourceName, Vector2 amount, TraumaMode mode = TraumaMode.KeepMax)
        {
            if (!traumaSources.ContainsKey(sourceName)) traumaSources.Add(sourceName, Vector2.zero);
            switch (mode)
            {
                case TraumaMode.KeepMax:
                    Vector2 v = traumaSources[sourceName];
                    if (v.x < amount.x) v.x = amount.x; // choose max
                    if (v.y < amount.y) v.y = amount.y;
                    traumaSources[sourceName] = v;
                    break;
                case TraumaMode.Add:
                    v = traumaSources[sourceName];
                    v += amount;
                    if (v.x > maxTrauma) v.x = maxTrauma; // clamp values
                    if (v.y > maxTrauma) v.y = maxTrauma;
                    traumaSources[sourceName] = v;
                    break;
                case TraumaMode.Replace:
                    traumaSources[sourceName] = amount;
                    break;
            }
        }
        /// <summary>Add trauma to a "default" trauma source</summary>
        /// <param name="amount" type="Vector2">Intensity of trauma in two dimensions (always positive)</param>
        /// <param name="mode" type="TraumaMode">How should the new value interact with any existing values?</param>
        public void AddTrauma(Vector2 amount, TraumaMode mode = TraumaMode.Add) { AddTrauma("default", amount, mode); }
        /// <summary>Add trauma with a given source name.</summary>
        /// <param name="sourceName" type="string">Name to use to compare with existing trauma sources</param>
        /// <param name="amount" type="float">Intensity of trauma in both dimensions (always positive)</param>
        /// <param name="mode" type="TraumaMode">How should the new value interact with any existing values?</param>
        public void AddTrauma(string sourceName, float amount, TraumaMode mode = TraumaMode.KeepMax) { AddTrauma(sourceName, Vector2.one * amount, mode); }
        /// <summary>Add trauma to a "default" trauma source</summary>
        /// <param name="amount" type="float">Intensity of trauma in both dimensions (always positive)</param>
        /// <param name="mode" type="TraumaMode">How should the new value interact with any existing values?</param>
        public void AddTrauma(float amount, TraumaMode mode = TraumaMode.Add) { AddTrauma("default", amount, mode); }

        /// <summary>Accumulate time according to shakeSpeed</summary>
        /// <access>private void</access>
        private void Update()
        {
            t += Time.deltaTime * shakeSpeed;
            DecayTraumaSources();
        }

        /// <summary>Structure to store result of camera shake for use by CameraMovement component</summary>
        /// <access>public struct</access>
        public struct ShakeValue
        {
            /// <summary>Translational component of shake</summary>
            /// <access>public readonly Vector2</access>
            public readonly Vector2 offset;
            /// <summary>Rotational component of shake</summary>
            /// <access>public readonly float</access>
            public readonly float rotation;

            public ShakeValue(Vector2 offset, float rotation)
            {
                this.offset = offset;
                this.rotation = rotation;
            }
        }

        private void DecayTraumaSources()
        {
            string[] sourceNames = new string[traumaSources.Count];
            traumaSources.Keys.CopyTo(sourceNames, 0);
            for(int i = 0; i < sourceNames.Length; i++)
            {
                Vector2 traumaValue = traumaSources[sourceNames[i]];
                if (traumaValue.x == 0 && traumaValue.y == 0) continue;

                traumaValue.x -= Time.deltaTime * decaySpeed; // linear decay
                traumaValue.y -= Time.deltaTime * decaySpeed;

                if (traumaValue.x < 0) traumaValue.x = 0; // clamp values
                if (traumaValue.y < 0) traumaValue.y = 0;

                traumaSources[sourceNames[i]] = traumaValue;
            }
        }

        /// <summary>Get the current translational and rotational shake values taking all sources into account</summary>
        /// <returns>Offset and rotation as a structure</returns>
        public ShakeValue GetShake()
        {
            Vector2 totalTrauma = Vector2.zero;
            foreach (var pair in traumaSources)
            {
                Vector2 traumaValue = pair.Value;
                totalTrauma += traumaValue;
            }
            totalTrauma /= maxTrauma;
            if (totalTrauma.x > 1) totalTrauma.x = 1;
            if (totalTrauma.y > 1) totalTrauma.y = 1;
            
            Vector2 translationalIntensity = new Vector2(intensityCurve.Evaluate(totalTrauma.x), intensityCurve.Evaluate(totalTrauma.y));
            float rotationalIntensity = intensityCurve.Evaluate((totalTrauma.x + totalTrauma.y) / 2);

            if (rotationalIntensity < 0.001f) return new ShakeValue(Vector2.zero, 0);

            float x = (Mathf.PerlinNoise(t, 0) * 2 - 1) * translationalIntensity.x * maxOffset;
            float y = (Mathf.PerlinNoise(0, t) * 2 - 1) * translationalIntensity.y * maxOffset;
            float r = (Mathf.PerlinNoise(t, t) * 2 - 1) * rotationalIntensity * maxRotation;

            return new ShakeValue(new Vector2(x, y), r);
        }
    }
}