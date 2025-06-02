// Pseudocode for A/B experiment settings
namespace App.Core {

    /// <summary>
    /// Settings for A/B Experiments.
    /// </summary>
    [SettingsKey(typeof(ApplicationSettings))]
    public class ABExperimentsSettings
    {
        /// <summary>
        /// Dictionary of experiment status overrides.
        /// Key is the experiment type name, value is whether it's enabled.
        /// </summary>
        [SettingsEntry]
        public IDictionary<string, bool> ExperimentStatus { get; set; }
    }
}