// Pseudocode for A/B experiment settings action
using System;
using System.Collections.Generic;

namespace App.Core {

    /// <summary>
    /// Action to configure A/B experiment settings.
    /// </summary>
    [Action("A/B Experiment Settings")]
    public class ABExperimentSettingsAction : IAction
    {
        private readonly ISettingsStore settingsStore;
        private readonly IComponentContainer componentContainer;
        private readonly ABExperimentController experimentController;

        public ABExperimentSettingsAction(
            ISettingsStore settingsStore,
            IComponentContainer componentContainer,
            ABExperimentController experimentController)
        {
            this.settingsStore = settingsStore;
            this.componentContainer = componentContainer;
            this.experimentController = experimentController;
        }

        public void Execute(IActionContext context)
        {
            // Create a lifetime for the popup menu
            var lifetime = Lifetime.Create();
            
            // Get settings
            var settings = settingsStore.GetSettings<ABExperimentsSettings>();
            
            // Get all experiments
            var experiments = componentContainer.GetComponents<IABExperiment>();
            var menuItems = new List<MenuItem>();

            // Create menu items for each experiment
            foreach (var experiment in experiments.OrderBy(e => e.GetType().Name))
            {
                var experimentType = experiment.GetType();
                var experimentTypeName = experimentType.FullName;
                var isEnabled = experimentController.IsEnabled(experimentType);
                var fromSettings = settings.ExperimentStatus.ContainsKey(experimentTypeName);

                // Create menu item with status text
                var text = experimentType.Name;
                var experimentStatus = (fromSettings ? "force " : "auto ") + 
                                      (isEnabled ? "experimental group" : "control group");
                
                var menuItem = new MenuItem(text, experimentStatus);
                
                // Add submenu items for changing status
                menuItem.AddSubmenuItems(
                    new MenuItem("force control group", () => DisableExperiment(experimentTypeName)),
                    new MenuItem("force experimental group", () => EnableExperiment(experimentTypeName)),
                    new MenuItem("auto by machineId", () => RemoveOverride(experimentTypeName))
                );
                
                menuItems.Add(menuItem);
            }

            // Show popup menu with all items
            ShowPopupMenu(menuItems);
            
            // Local functions for menu actions
            void EnableExperiment(string expType) => 
                settings.ExperimentStatus[expType] = true;
                
            void DisableExperiment(string expType) => 
                settings.ExperimentStatus[expType] = false;
                
            void RemoveOverride(string expType) => 
                settings.ExperimentStatus.Remove(expType);
        }

        public bool Update(IActionContext context)
        {
            // Always enable this action
            return true;
        }
    }
}