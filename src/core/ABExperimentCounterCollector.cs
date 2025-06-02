// Pseudocode for A/B experiment counter collector
using System;

namespace App.Core {

    /// <summary>
    /// Collects counter statistics for A/B experiments.
    /// </summary>
    [Component]
    public class ABExperimentCounterCollector : CounterCollector, IABExperimentNotifications
    {
        private readonly EventLogger eventLogger;

        public ABExperimentCounterCollector(EventLogger eventLogger)
        {
            this.eventLogger = eventLogger;
            
            // Register event for tracking when IsEnabled is called
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            // Register event for tracking when IsEnabled is called for an experiment
        }

        /// <summary>
        /// Called when IsEnabled is called for an experiment.
        /// </summary>
        public void IsEnabledCalled(Type experimentType, bool isEnabled)
        {
            // Log that IsEnabled was called for this experiment with this result
            eventLogger.LogEvent("abExperiments.isEnabledCalled", experimentType, isEnabled);
        }
    }
}