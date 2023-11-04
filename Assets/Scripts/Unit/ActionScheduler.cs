public class ActionScheduler
{
    private IAction currentAction;

    public void StartAction(IAction action)
    {
        if (currentAction == action)
        {
            // If the provided action is the same as the current action, do nothing
            return;
        }

        if (currentAction != null)
        {
            // If there is a current action, cancel it
            currentAction.Cancel();
        }

        // Set the provided action as the new current action
        currentAction = action;
    }

    public void CancelCurrentAction()
    {
        // Cancel the current action by setting it to null
        StartAction(null);
    }
}
