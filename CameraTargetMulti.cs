using System.Collections.Generic;
using UnityEngine;

namespace UModules
{
    /// <summary>
    /// Camera targeting component that takes a weighted average of a set of influences.
    /// Used by the CameraMotion component to provide smooth tracking of multiple targets
    /// </summary>
    public class CameraTargetMulti : CameraTarget
    {
        /// <summary>
        /// Target item that is always included in average calculation
        /// </summary>
        [SerializeField]
        TargetItem baseTarget = new TargetItem(null, 100, 8, 0);

        /// <summary>
        /// Set the values of the base target item
        /// </summary>
        /// <param name="target">Transform to always track</param>
        /// <param name="weight">Base weight</param>
        /// <param name="speed">Base movement speed</param>
        /// <returns>The base target item</returns>
        public TargetItem SetBaseTarget(Transform target, float weight, float speed)
        {
            baseTarget.transform = target;
            baseTarget.weight = weight;
            baseTarget.speed = speed;
            return baseTarget;
        }
        /// <summary>
        /// Set the base target item transform
        /// </summary>
        /// <param name="target">Transform to always track</param>
        /// <returns>The base target item</returns>
        public TargetItem SetBaseTarget(Transform target) { return SetBaseTarget(target); }
        
        /// <summary>
        /// List of all targets to include in average calculation
        /// </summary>
        //private SortedList<TargetItem> foo;
    }
}