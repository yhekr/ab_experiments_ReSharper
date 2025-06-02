// Pseudocode for populating About box with A/B experiment status
using System;
using System.Text;

namespace App.Core {

    /// <summary>
    /// Adds A/B Experiments' status to the About Box.
    /// </summary>
    [Component]
    public class ABExperimentsPopulateAboutBox : IPopulateAboutBox
    {
        private readonly Lazy<ABExperimentController> experimentController;

        public ABExperimentsPopulateAboutBox(Lazy<ABExperimentController> experimentController)
        {
            this.experimentController = experimentController;
        }

        // Priority for display order in About box
        double IPopulateAboutBox.Priority => 3;

        // Text to copy to clipboard when user copies About box content
        string IPopulateAboutBox.ClipboardData => GetExperimentsStatusText();

        // Rich text to display in About box (null means use ClipboardData)
        RichText IPopulateAboutBox.DisplayText => null;

        private string GetExperimentsStatusText()
        {
            var builder = new StringBuilder("Optional Features Status:\n");

            // Get experiment statuses sorted by name
            var sortedExperimentStatuses = experimentController.Value
                .GetDataForStatistics()
                .experimentsStatus
                .OrderBy(e => e.experimentType.Name, StringComparer.InvariantCultureIgnoreCase);

            // Add each experiment's status to the text
            foreach (var (experimentType, isEnabled) in sortedExperimentStatuses)
            {
                builder.AppendLine($"{experimentType.Name}: {(isEnabled ? "on" : "off")}");
            }

            return builder.ToString();
        }
    }
}