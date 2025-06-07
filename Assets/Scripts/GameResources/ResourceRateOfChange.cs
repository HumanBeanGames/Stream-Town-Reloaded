using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Managers;
using DataStructures;
using Sirenix.OdinInspector;
using Character;

namespace GameResources
{
    /// <summary>
    /// Used for calculating the rate of change of a resource.
    /// </summary>
    [CreateAssetMenu(menuName = "Game Resources/Resource Rate Of Change", fileName = "NewResourceRateOfChange")]
    public class ResourceRateOfChange : SerializedScriptableObject
    {
        [SerializeField] private Resource _resourceType;
        [SerializeField] private float _timePeriod = 60f;
        [SerializeField] private float _updateRate = 5f;

        private Queue<ChangeTimeStamp> _timestampData = new Queue<ChangeTimeStamp>();
        private float _updateTimer = 0f;
        private int _changeDuringPeriod = 0;

        [NonSerialized] private int _averageOverTime;
        public int AverageOverTime => _averageOverTime;

        /// <summary>
        /// Called by Unity when the ScriptableObject is enabled (e.g., loaded or instantiated).
        /// </summary>
        private void OnEnable()
        {
            if (Application.isPlaying)
            {
                TownResourceManager.OnAnyResourceChangeEvent.AddListener(OnResourceChange);
            }
        }

        /// <summary>
        /// Called by Unity when the ScriptableObject is disabled or destroyed.
        /// </summary>
        private void OnDisable()
        {
            if (Application.isPlaying)
            {
                TownResourceManager.OnAnyResourceChangeEvent.RemoveListener(OnResourceChange);
            }
        }

        /// <summary>
        /// Optional initialization if created at runtime.
        /// </summary>
        public void Init(Resource resourceType, float timePeriod, float updateRate)
        {
            _resourceType = resourceType;
            _timePeriod = timePeriod;
            _updateRate = updateRate;
        }

        /// <summary>
        /// Processes a queue and calculates rate of change for town resources.
        /// </summary>
        public void ProcessQueue()
        {
            _updateTimer += Time.deltaTime;

            if (_updateTimer < _updateRate)
                return;

            _updateTimer -= _updateRate;

            DateTime now = DateTime.UtcNow;
            _timestampData.Enqueue(new ChangeTimeStamp(now, _changeDuringPeriod));
            _changeDuringPeriod = 0;

            List<float> medianAmounts = new List<float>();

            int count = _timestampData.Count;
            for (int i = 0; i < count; i++)
            {
                ChangeTimeStamp cts = _timestampData.Dequeue();
                double timeDifference = (now - cts.TimeStamp).TotalSeconds;

                if (timeDifference < _timePeriod)
                    medianAmounts.Add(cts.Change);

                _timestampData.Enqueue(cts);
            }

            if (medianAmounts.Count < 10)
                return;

            List<float> plottedPoints = new List<float>();
            for (int i = 2; i < medianAmounts.Count - 2; i++)
            {
                float avg = (medianAmounts[i - 2] + medianAmounts[i - 1] + medianAmounts[i] + medianAmounts[i + 1] + medianAmounts[i + 2]) / 5f;
                plottedPoints.Add(avg);
            }

            float movingMean = 0;
            foreach (var point in plottedPoints)
                movingMean += point;

            movingMean /= plottedPoints.Count;

            _averageOverTime = Mathf.RoundToInt(movingMean * 60f * 60f);
        }

        /// <summary>
        /// Called when a town resource value changes.
        /// </summary>
        private void OnResourceChange(Resource resource, int amount, bool purchase)
        {
            if (resource != _resourceType || purchase)
                return;

            _changeDuringPeriod += amount;
        }

        /// <summary>
        /// Creates and initializes a new ResourceInventory ScriptableObject instance.
        /// </summary>
        public static ResourceRateOfChange CreateRROC(Resource resourceType, float timePeriod, float updateRate)
        {
            var rroc = ScriptableObject.CreateInstance<ResourceRateOfChange>();
            rroc.Init(resourceType, timePeriod, updateRate);
            return rroc;
        }
    }
}
