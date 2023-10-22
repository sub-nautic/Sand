namespace Playground.Input
{
    public class InputProvider
    {
        public static MainInputActions MainInputActions
        {
            get
            {
                if (mainInputActions == null)
                {
                    mainInputActions = new MainInputActions();
                    mainInputActions.Enable();
                }

                return mainInputActions;
            }
        }

        private static MainInputActions mainInputActions;
    }
}