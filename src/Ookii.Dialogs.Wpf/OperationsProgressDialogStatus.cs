namespace Ookii.Dialogs.Wpf
{
    public enum OperationsProgressDialogStatus : uint
    {
        /// <summary>
        /// Operation is running, no user intervention.
        /// </summary>
        Running = 1,

        /// <summary>
        /// Operation has been paused by the user.
        /// </summary>
        Paused = 2,

        /// <summary>
        /// Operation has been canceled by the user - now go undo.
        /// </summary>
        Cancelled = 3,

        /// <summary>
        /// Operation has been stopped by the user - terminate completely.
        /// </summary>
        Stopped = 4,

        /// <summary>
        /// Operation has gone as far as it can go without throwing error dialogs.
        /// </summary>
        Errors = 5
    }
}
