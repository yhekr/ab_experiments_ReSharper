// Pseudocode for A/B experiment usage statistics collector
using System;
using System.Threading.Tasks;

namespace App.Core {

    /// <summary>
    /// Collects usage statistics for A/B experiments.
    /// </summary>
    [Component]
    public class ABExperimentApplicationUsagesCollector : UsageCollector
    {
        private readonly Lazy<ABExperimentController> experimentController;
        private readonly EventLogger eventLogger;

        public ABExperimentApplicationUsagesCollector(
            EventLogger eventLogger,
            Lazy<ABExperimentController> experimentController)
        {
            this.eventLogger = eventLogger;
            this.experimentController = experimentController;
            
            // Register event groups and types
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            // Register events for:
            // 1. User point per mille
            // 2. Experiment status (on/off)
            // 3. Group limits for experiments
        }

        public override async Task CollectAsync()
        {
            // Get data from experiment controller
            var (pointPerMille, experimentsStatus, groupLimits) = 
                experimentController.Value.GetDataForStatistics();

            // Log user's point per mille
            LogUserPointPerMille(pointPerMille);

            // Log status of each experiment
            foreach (var (experimentType, status) in experimentsStatus)
                LogExperimentStatus(experimentType, status);

            // Log group limits for each experiment
            foreach (var (experimentType, limit) in groupLimits)
                LogGroupLimit(experimentType, limit);
        }

        private void LogUserPointPerMille(int pointPerMille)
        {
            // Log the user's point per mille value
            eventLogger.LogEvent("abExperiments.userInfo", pointPerMille);
        }

        private void LogExperimentStatus(Type experimentType, bool status)
        {
            // Log whether an experiment is enabled for this user
            eventLogger.LogEvent("abExperiments.experimentStatus", experimentType, status);
        }

        private void LogGroupLimit(Type experimentType, int limit)
        {
            // Log the group limit for an experiment
            eventLogger.LogEvent("abExperiments.groupLimit", experimentType, limit);
        }
    }
}