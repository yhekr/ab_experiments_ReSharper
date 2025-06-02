// Pseudocode for menu opened counter
using System.Collections.Generic;

namespace App.Core {

    /// <summary>
    /// Counts the number of times the application menu has been opened.
    /// </summary>
    [Component]
    public class MenuOpenedCounter
    {
        private readonly ILifetime lifetime;
        private readonly ILogger logger;
        private readonly List<INotifyMenuOpened> subscribers = new();
        private readonly IScheduler scheduler;
        private int updateCalledCount;

        public MenuOpenedCounter(ILifetime lifetime, ILogger logger, IThreading threading)
        {
            this.lifetime = lifetime;
            this.logger = logger;
            this.scheduler = threading.BackgroundScheduler;
        }

        /// <summary>
        /// Registers a component to be notified when the menu is opened.
        /// </summary>
        public void RegisterSubscriber(INotifyMenuOpened subscriber)
        {
            subscribers.Add(subscriber);
        }

        /// <summary>
        /// Called by MenuOpenedAction when its Update method is called.
        /// </summary>
        internal void OnActionUpdateCalled()
        {
            // Queue notification on background thread to avoid UI freezes
            scheduler.Queue(NotifyMenuOpened);
        }
        
        private void NotifyMenuOpened()
        {
            // Check if component is still alive
            if (!lifetime.IsAlive)
                return;

            // Increment counter
            updateCalledCount += 1;
            
            // Calculate menu opened count
            var menuOpenedCount = CalculateMenuOpenedCount(updateCalledCount);
            var previousCount = CalculateMenuOpenedCount(updateCalledCount - 1);
            
            // Only notify if count has increased
            if (menuOpenedCount != 0 && menuOpenedCount == previousCount + 1)
            {
                // Notify all subscribers
                foreach (var subscriber in subscribers)
                {
                    try
                    {
                        subscriber.NotifyMenuOpened(menuOpenedCount);
                    }
                    catch (System.Exception ex)
                    {
                        logger.LogError(ex);
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the number of menu opens based on the number of Update calls.
        /// The Update method is called multiple times per menu open.
        /// </summary>
        private static int CalculateMenuOpenedCount(int updateCalledCount)
        {
            // First menu open triggers 4 updates, subsequent ones trigger 2 updates each
            return (updateCalledCount - 2) / 2;
        }
    }
}