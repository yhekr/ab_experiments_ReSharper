// Pseudocode for context actions suggester
using System;

namespace App.Features {

    /// <summary>
    /// Suggests the context actions feature to users.
    /// Uses A/B testing to determine the best message.
    /// </summary>
    [Component]
    public class ShowContextActionsSuggester : IFeatureSuggester
    {
        private readonly IActionManager actionManager;
        private readonly ILocks locks;
        private readonly ISettingsStore settingsStore;
        private readonly IAction showContextActionsAction;
        private readonly string documentationUrl = "https://www.example.com/help/context-actions";

        public ShowContextActionsSuggester(
            IActionManager actionManager,
            ILocks locks,
            ISettingsStore settingsStore)
        {
            this.actionManager = actionManager;
            this.locks = locks;
            this.settingsStore = settingsStore;
            this.showContextActionsAction = actionManager.GetAction("ShowContextActions");
        }

        public string Id => nameof(ShowContextActionsSuggester);

        public bool IsApplicable(ISolution solution, ITextControl textControl)
        {
            // Check if the action is available
            if (showContextActionsAction == null)
            {
                // Log warning if action not found
                return false;
            }

            return true;
        }

        public void Show(ILifetime lifetime, ISolution solution, ITextControl textControl, IProperty<FeatureSuggestionNotification> suggestion)
        {
            // Create notification
            var notification = new FeatureSuggestionNotification();

            // Get shortcut texts
            var goToActionShortcutText = GetShortcutText("GoToAction");
            var contextActionsShortcutText = GetShortcutText("ShowContextActions");

            // Create control group message
            var controlGroupMessage = new RichText()
                .AppendLine("Boost Your Efficiency", TextStyle.Bold)
                .AppendLine()
                .Append("Use ")
                .Append(goToActionShortcutText)
                .Append(" or ")
                .Append(contextActionsShortcutText)
                .Append(" to access powerful code actions and refactorings.");

            // Create experimental group message
            var experimentalGroupMessage = new RichText()
                .AppendLine("Find Actions Faster", TextStyle.Bold)
                .AppendLine()
                .Append("Press ")
                .Append(goToActionShortcutText)
                .Append(" or ")
                .Append(contextActionsShortcutText)
                .Append(" to quickly find and apply available actions.");

            // Check if user is in experimental group
            var experimentController = GetComponent<ABExperimentController>();
            if (experimentController.IsInExperimentalGroupForExperiment<ContextActionsSuggesterTextImprovement>())
            {
                notification.ShortDescription = "Find Actions Faster";
                notification.SuggestionMessage = experimentalGroupMessage;
            }
            else
            {
                notification.ShortDescription = "Boost Your Efficiency";
                notification.SuggestionMessage = controlGroupMessage;
            }

            // Set common properties
            notification.HelpUrl = documentationUrl;
            notification.IconId = "BulbYellow";
            notification.AcceptText = "Try Now";

            // Handle accept action
            notification.Accepted.Advise(lifetime, () => locks.ExecuteOrQueue(
                lifetime,
                "Open ShowContextActions",
                () => showContextActionsAction.Execute(actionManager)
            ));

            // Show notification
            locks.ExecuteOrQueue(
                lifetime,
                "Show ShowContextActionsSuggester",
                () =>
                {
                    notification.Lifetime.OnTermination(() => suggestion.SetValue(null));
                    suggestion.SetValue(notification);
                }
            );
        }

        private RichText GetShortcutText(string actionId)
        {
            // Get shortcut for action
            var action = actionManager.GetAction(actionId);
            if (action == null)
                return new RichText("?");

            var shortcut = action.GetFirstShortcut();
            if (shortcut == null)
                return new RichText("?");

            return new RichText(shortcut.ToString(), TextStyle.Highlight);
        }
    }
}