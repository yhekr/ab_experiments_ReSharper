// Pseudocode for A/B experiment controller
using System;
using System.Collections.Generic;

namespace App.Core {

    /// <summary>
    /// Controls A/B experiments by determining which users are in experimental vs control groups.
    /// </summary>
    [Component]
    public class ABExperimentController
    {
        private readonly ILogger logger;
        private readonly ISettingsStore settingsStore;
        private readonly IComponentContainer componentContainer;
        private readonly Lazy<ExperimentInfo> experimentInfo;

        public ABExperimentController(ILogger logger, ISettingsStore settingsStore, IComponentContainer componentContainer)
        {
            this.logger = logger;
            this.settingsStore = settingsStore;
            this.componentContainer = componentContainer;
            experimentInfo = new Lazy<ExperimentInfo>(InitializeExperimentInfo);
        }

        /// <summary>
        /// Checks if the current user is in the experimental group for the specified experiment.
        /// </summary>
        public bool IsInExperimentalGroupForExperiment<TExperiment>() where TExperiment : IABExperiment
        {
            return IsEnabled(typeof(TExperiment));
        }

        /// <summary>
        /// Checks if the current user is in the experimental group for the specified experiment type.
        /// </summary>
        public bool IsEnabled(Type experimentType)
        {
            // Pseudocode implementation:
            // 1. Check if there's an override in settings
            // 2. If yes, return that value
            // 3. If no, use the point per mille to determine the group
            // 4. Notify all IABExperimentNotifications components

            try {
                // Check settings override
                var experimentTypeName = experimentType.FullName;
                var settingsEntry = settingsStore.GetSettings<ABExperimentsSettings>();
                
                if (settingsEntry.ExperimentStatus.ContainsKey(experimentTypeName)) {
                    var isEnabled = settingsEntry.ExperimentStatus[experimentTypeName];
                    NotifyIsEnabledCalled(experimentType, isEnabled);
                    return isEnabled;
                }
                
                // Use point per mille
                var pointPerMille = experimentInfo.Value.PointPerMille;
                var groupLimits = experimentInfo.Value.GroupLimits;
                
                foreach (var (experiment, limit) in groupLimits) {
                    if (experiment.GetType() == experimentType) {
                        var isEnabled = pointPerMille < limit;
                        NotifyIsEnabledCalled(experimentType, isEnabled);
                        return isEnabled;
                    }
                }
                
                // Experiment not found
                logger.Warn($"Experiment {experimentTypeName} not found in group limits");
                NotifyIsEnabledCalled(experimentType, false);
                return false;
            }
            catch (Exception e) {
                logger.LogException(e);
                return false;
            }
        }

        private void NotifyIsEnabledCalled(Type experimentType, bool isEnabled)
        {
            foreach (var notification in componentContainer.GetComponents<IABExperimentNotifications>()) {
                notification.IsEnabledCalled(experimentType, isEnabled);
            }
        }

        private ExperimentInfo InitializeExperimentInfo()
        {
            // Pseudocode implementation:
            // 1. Get all IABExperiment components
            // 2. Generate a point per mille based on machine ID
            // 3. Calculate group limits for each experiment
            
            try {
                var experiments = componentContainer.GetComponents<IABExperiment>();
                var pointPerMille = GeneratePointPerMille();
                var groupLimits = CalculateGroupLimits(experiments);
                
                return new ExperimentInfo(pointPerMille, groupLimits);
            }
            catch (Exception e) {
                logger.LogException(e);
                return new ExperimentInfo(0, new List<(IABExperiment, int)>());
            }
        }

        private int GeneratePointPerMille()
        {
            // Generate a consistent point per mille (0-999) based on machine ID
            try {
                var machineId = Environment.MachineName;
                var hash = machineId.GetHashCode();
                return Math.Abs(hash) % 1000;
            }
            catch (Exception e) {
                logger.LogException(e);
                return 0;
            }
        }

        private List<(IABExperiment experiment, int limit)> CalculateGroupLimits(IEnumerable<IABExperiment> experiments)
        {
            // Pseudocode implementation:
            // 1. Handle experiments with explicit fractions
            // 2. Distribute remaining fraction evenly among experiments with null fractions
            
            var result = new List<(IABExperiment, int)>();
            var experimentsList = experiments.ToList();
            
            if (experimentsList.Count == 0)
                return result;
                
            var remainingExperiments = new List<IABExperiment>();
            var remainingFraction = 1.0;
            
            foreach (var experiment in experimentsList) {
                if (experiment.FractionOfUsers.HasValue) {
                    var fraction = Math.Min(experiment.FractionOfUsers.Value, 0.5);
                    var limit = (int)(fraction * 1000);
                    result.Add((experiment, limit));
                    remainingFraction -= fraction;
                }
                else {
                    remainingExperiments.Add(experiment);
                }
            }
            
            if (remainingExperiments.Count > 0) {
                var fractionPerExperiment = Math.Max(0, remainingFraction) / remainingExperiments.Count;
                var limitPerExperiment = (int)(fractionPerExperiment * 1000);
                
                foreach (var experiment in remainingExperiments) {
                    result.Add((experiment, limitPerExperiment));
                }
            }
            
            return result;
        }

        public (int pointPerMille, List<(Type experimentType, bool status)> experimentsStatus, List<(Type, int)> groupLimits)
            GetDataForStatistics()
        {
            // Get experiment status and group limits for statistics
            var experimentStatus = GetExperimentsStatus();
            var groupLimits = new List<(Type, int)>();
            
            foreach (var (experiment, limit) in experimentInfo.Value.GroupLimits)
                groupLimits.Add((experiment.GetType(), limit));

            return (experimentInfo.Value.PointPerMille, experimentStatus, groupLimits);
        }

        private List<(Type, bool)> GetExperimentsStatus()
        {
            // Get status of all experiments
            try {
                var status = new List<(Type, bool)>();
                
                foreach (var (experiment, _) in experimentInfo.Value.GroupLimits) {
                    var experimentId = experiment.GetType();
                    var isEnabled = IsEnabled(experiment.GetType());
                    status.Add((experimentId, isEnabled));
                }

                return status;
            }
            catch (Exception exception) {
                logger.Error(exception);
                return new List<(Type, bool)>();
            }
        }
    }

    // Record to store experiment information
    public record ExperimentInfo(int PointPerMille, List<(IABExperiment experiment, int limit)> GroupLimits);
}