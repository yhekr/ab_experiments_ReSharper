// Pseudocode for A/B experiment notifications interface
namespace App.Core {

    /// <summary>
    /// Provides notifications related to the state of A/B experiments.
    /// </summary>
    public interface IABExperimentNotifications
    {
        /// <summary>
        /// Invoked when the IsEnabled method is called for a specific experiment type.
        /// </summary>
        void IsEnabledCalled(Type experimentType, bool isEnabled);
    }
}