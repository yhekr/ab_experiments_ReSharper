// Pseudocode for menu opened collector
namespace App.Core {

    /// <summary>
    /// Collects statistics about menu opens.
    /// </summary>
    [Component]
    public class MenuOpenedCollector : CounterCollector, INotifyMenuOpened
    {
        private readonly EventLogger eventLogger;

        public MenuOpenedCollector(EventLogger eventLogger)
        {
            this.eventLogger = eventLogger;
            
            // Register event for tracking menu opens
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            // Register event for tracking menu opens
        }

        /// <summary>
        /// Called when the menu is opened.
        /// </summary>
        public void NotifyMenuOpened(int count)
        {
            // Log that the menu was opened
            eventLogger.LogEvent("menu.opened", count);
        }
    }
}