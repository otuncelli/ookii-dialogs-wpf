using System;
using System.Runtime.InteropServices;

namespace Ookii.Dialogs.Wpf.Interop
{
    [ComImport]
    [Guid(IIDGuid.IOperationsProgressDialog)]
    [CoClass(typeof(ProgressDialogRCW))]
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

    [ComImport]
    [Guid(IIDGuid.IOperationsProgressDialog)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IOperationsProgressDialog
    {
        [PreserveSig]
        int StartProgressDialog(IntPtr hwndOwner, OperationsProgressDialogFlags flags);

        [PreserveSig]
        int StopProgressDialog();

        [PreserveSig]
        int SetOperation(OperationsProgressDialogAction action);

        [PreserveSig]
        int SetMode(OperationsProgressDialogMode mode);

        [PreserveSig]
        int UpdateProgress(ulong ullPointsCurrent, ulong ullPointsTotal, ulong ullSizeCurrent, ulong ullSizeTotal,
            ulong ullItemsCurrent, ulong ullItemsTotal);

        [PreserveSig]
        int UpdateLocations(IShellItem psiSource, IShellItem psiTarget, IShellItem psiItem);

        [PreserveSig]
        int ResetTimer();

        [PreserveSig]
        int PauseTimer();

        [PreserveSig]
        int ResumeTimer();

        [PreserveSig]
        int GetMilliseconds(out ulong pullElapsed, out ulong pullRemaining);

        [PreserveSig]
        int GetOperationStatus(out OperationsProgressDialogStatus status);
    }
}
