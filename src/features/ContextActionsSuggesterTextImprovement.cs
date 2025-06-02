// Pseudocode for A/B experiment feature
namespace App.Features {

    // Component that represents an A/B experiment for text improvement in context actions
    [Component]
    public class ContextActionsSuggesterTextImprovement : IABExperiment
    {
        // Returns the fraction of users to include in experiment (null means equal distribution)
        public double? FractionOfUsers => null;
    }
}