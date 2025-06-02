// Pseudocode for menu opened notification interface
namespace App.Core {

    /// <summary>
    /// Interface for components that need to be notified when the application menu is opened.
    /// </summary>
    public interface INotifyMenuOpened
    {
        /// <summary>
        /// Called when the application menu is opened.
        /// </summary>
        /// <param name="count">The number of times the menu has been opened.</param>
        void NotifyMenuOpened(int count);
    }
}