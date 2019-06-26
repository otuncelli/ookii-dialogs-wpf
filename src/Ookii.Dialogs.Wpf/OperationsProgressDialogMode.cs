using System;

namespace Ookii.Dialogs.Wpf
{
    [Flags]
    public enum OperationsProgressDialogMode : uint
    {
        /// <summary>
        /// Use the default progress dialog operations mode.
        /// </summary>
        Default = 0x00000000,

        /// <summary>
        /// The operation is running.
        /// </summary>
        Run = 0x00000001,

        /// <summary>
        /// The operation is gathering data before it begins, such as calculating the predicted operation time.
        /// </summary>
        Preflight = 0x00000002,

        /// <summary>
        /// The operation is rolling back due to an Undo command from the user.
        /// </summary>
        Undoing = 0x00000004,

        /// <summary>
        /// Error dialogs are blocking progress from continuing.
        /// </summary>
        /// <remarks>
        /// Appears to only work when progress has already begun.
        /// </remarks>
        ErrorsBlocking = 0x00000008,

        /// <summary>
        /// The length of the operation is indeterminate. Do not show a timer and display the progress bar in marquee mode.
        /// </summary>
        Indeterminate = 0x00000010
    }
}
