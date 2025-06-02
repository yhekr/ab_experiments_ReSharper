// Pseudocode for A/B experiment interface
namespace App.Core {

    /// <summary>
    /// Represents an A/B experiment with configurable user allocation.
    /// Users not assigned to any experiment will be in the control group.
    /// To define a new A/B experiment, create a component implementing this interface.
    /// </summary>
    public interface IABExperiment
    {
        /// <summary>
        /// Fraction of users in the experimental group (0.0 to 0.5).
        /// If null, users are distributed equally among experiments with null fractions.
        /// </summary>
        double? FractionOfUsers { get; }
    }
}