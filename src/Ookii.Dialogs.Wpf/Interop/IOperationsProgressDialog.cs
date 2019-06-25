using System;
using System.Runtime.InteropServices;

namespace Ookii.Dialogs.Wpf.Interop
{
    [ComImport,
     Guid(IIDGuid.IOperationsProgressDialog),
     CoClass(typeof(ProgressDialogRCW))]
    internal interface OperationsProgressDialog : IOperationsProgressDialog
    {
    }

    [Flags]
    internal enum OperationsProgressDialogFlags : uint
    {
        Normal = 0x00000000,
        Modal = 0x00000001,
        AutoTime = 0x00000002,
        NoTime = 0x00000004,
        NoMinimize = 0x00000008,
        NoProgressBar = 0x00000010,
        MarqueeProgress = 0x00000020,
        NoCancel = 0x00000040,
        EnablePause = 0x00000080,
        AllowUndo = 0x00000100,
        DontDisplaySourcePath = 0x00000200,
        DontDisplayDestPath = 0x00000400,
        NoMultiDayEstimates = 0x00000800,
        DontDisplayLocations = 0x00001000
    }

    [Flags]
    internal enum OperationsProgressDialogMode : uint
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

    public enum OperationsProgressDialogAction : uint
    {
        /// <summary>
        /// No action is being performed.
        /// </summary>
        None = 0,

        /// <summary>
        /// Files are being moved.
        /// </summary>
        Moving,

        /// <summary>
        /// Files are being copied.
        /// </summary>
        Copying,

        /// <summary>
        /// Files are being deleted.
        /// </summary>
        Recycling,

        /// <summary>
        /// A set of attributes are being applied to files.
        /// </summary>
        ApplyingAttributes,

        /// <summary>
        /// A file is being downloaded from a remote source.
        /// </summary>
        Downloading,

        /// <summary>
        /// An Internet search is being performed.
        /// </summary>
        SearchingInternet,

        /// <summary>
        /// A calculation is being performed.
        /// </summary>
        Calculating,

        /// <summary>
        /// A file is being uploaded to a remote source.
        /// </summary>
        Uploading,

        /// <summary>
        /// A local search is being performed.
        /// </summary>
        SearchingFiles,

        /// <summary>
        /// Windows Vista and later. A deletion is being performed.
        /// </summary>
        Deleting,

        /// <summary>
        /// Windows Vista and later. A renaming action is being performed.
        /// </summary>
        Renaming,

        /// <summary>
        /// Windows Vista and later. A formatting action is being performed.
        /// </summary>
        Formatting,

        /// <summary>
        /// Windows 7 and later. A copy or move action is being performed.
        /// </summary>
        CopyMoving
    }

    public enum PDOPSTATUS : uint
    {
        /// <summary>
        /// Operation is running, no user intervention.
        /// </summary>
        PDOPS_RUNNING = 1,

        /// <summary>
        /// Operation has been paused by the user.
        /// </summary>
        PDOPS_PAUSED = 2,

        /// <summary>
        /// Operation has been canceled by the user - now go undo.
        /// </summary>
        PDOPS_CANCELLED = 3,

        /// <summary>
        /// Operation has been stopped by the user - terminate completely.
        /// </summary>
        PDOPS_STOPPED = 4,

        /// <summary>
        /// Operation has gone as far as it can go without throwing error dialogs.
        /// </summary>
        PDOPS_ERRORS = 5
    }

    [ComImport]
    [Guid(IIDGuid.IOperationsProgressDialog)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IOperationsProgressDialog
    {
        [PreserveSig]
        void StartProgressDialog(IntPtr hwndParent, OperationsProgressDialogFlags flags);

        [PreserveSig]
        void StopProgressDialog();

        [PreserveSig]
        void SetOperation(OperationsProgressDialogAction action);

        [PreserveSig]
        void SetMode(OperationsProgressDialogMode mode);

        [PreserveSig]
        void UpdateProgress(ulong ullPointsCurrent, ulong ullPointsTotal, ulong ullSizeCurrent, ulong ullSizeTotal,
            ulong ullItemsCurrent, ulong ullItemsTotal);

        [PreserveSig]
        void UpdateLocations(IShellItem psiSource, IShellItem psiTarget, IShellItem psiItem);

        [PreserveSig]
        void ResetTimer();

        [PreserveSig]
        void PauseTimer();

        [PreserveSig]
        void ResumeTimer();

        [PreserveSig]
        void GetMilliseconds(ulong pullElapsed, ulong pullRemaining);

        [PreserveSig]
        PDOPSTATUS GetOperationStatus();
    }
}
