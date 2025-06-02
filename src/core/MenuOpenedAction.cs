// Pseudocode for menu opened action
namespace App.Core {

    /// <summary>
    /// Tracks the number of times the application menu has been opened.
    /// This menu item is always invisible.
    /// When the Menu is accessed, the Update method is called multiple times.
    /// </summary>
    [Action("Menu Opened")]
    public class MenuOpenedAction : IAction
    {
        public bool Update(IActionContext context)
        {
            // Make this action invisible
            context.Presentation.Visible = false;
            
            // Get the counter component and notify it that the action was updated
            var menuOpenedCounter = context.GetComponent<MenuOpenedCounter>();
            menuOpenedCounter.OnActionUpdateCalled();
            
            return true;
        }

        public void Execute(IActionContext context)
        {
            // This action doesn't do anything when executed
            // It only tracks menu opens via the Update method
        }
    }
}