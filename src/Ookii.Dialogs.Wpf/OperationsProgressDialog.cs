// Copyright (c) Sven Groot (Ookii.org) 2009
// BSD license; see LICENSE for details.

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Ookii.Dialogs.Wpf.Interop;
using Ookii.Dialogs.Wpf.Properties;

namespace Ookii.Dialogs.Wpf
{
    [DefaultEvent("DoWork")]
    [Description("Represents a dialog that can be used to report progress to the user.")]
    public partial class OperationsProgressDialog : Component
    {
        private class ProgressChangedData
        {
            public IShellItem Source { get; set; }
            public IShellItem Destination { get; set; }
            public IShellItem Current { get; set; }
            public int CurrentProgress { get; set; }
            public int TotalProgress { get; set; }
            public long ProcessedSize { get; set; }
            public long TotalSize { get; set; }
            public long ProcessedItems { get; set; }
            public long TotalItems { get; set; }
            public object UserState { get; set; }
        }

        private IOperationsProgressDialog _dialog;
        private bool _cancellationPending;

        /// <summary>
        /// Event raised when the dialog is displayed.
        /// </summary>
        /// <remarks>
        /// Use this event to perform the operation that the dialog is showing the progress for.
        /// This event will be raised on a different thread than the UI thread.
        /// </remarks>
        public event DoWorkEventHandler DoWork;

        /// <summary>
        /// Event raised when the operation completes.
        /// </summary>
        public event RunWorkerCompletedEventHandler RunWorkerCompleted;

        /// <summary>
        /// Event raised when <see cref="UpdateProgress(int,int,long,long,int,int,object)"/> or
        /// <see cref="UpdateLocations(string,string,string,bool)"/> is called.
        /// </summary>
        public event ProgressChangedEventHandler ProgressChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationsProgressDialog"/> class.
        /// </summary>
        public OperationsProgressDialog()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationsProgressDialog"/> class, adding it to the specified container.
        /// </summary>
        /// <param name="container">The <see cref="IContainer"/> to which the component should be added.</param>
        public OperationsProgressDialog(IContainer container)
        {
            container?.Add(this);

            InitializeComponent();

            ProgressBarStyle = ProgressBarStyle.ProgressBar;
            ShowCancelButton = true;
            MinimizeBox = true;
        }

        /// <summary>
        /// Gets or sets a value that indicates whether an estimate of the remaining time will be shown.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if an estimate of remaining time will be shown; otherwise, <see langword="false"/>. The
        /// default value is <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// <para>
        ///   This property must be set before <see cref="ShowDialog()"/> or <see cref="Show()"/> is called. Changing property has
        ///   no effect while the dialog is being displayed.
        /// </para>
        /// </remarks>
        [Category("Appearance")]
        [Description("Indicates whether an estimate of the remaining time will be shown.")]
        [DefaultValue(false)]
        public bool ShowTimeRemaining { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the dialog has a cancel button.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the dialog has a cancel button; otherwise, <see langword="false"/>. The default
        /// value is <see langword="true"/>.
        /// </value>
        /// <remarks>
        /// <note>
        ///   This property requires Windows Vista or later; on older versions of Windows, the cancel button will always
        ///   be displayed.
        /// </note>
        /// <para>
        ///   The event handler for the <see cref="DoWork"/> event must periodically check the value of the
        ///   <see cref="CancellationPending"/> property to see if the operation has been cancelled if this
        ///   property is <see langword="true"/>.
        /// </para>
        /// <para>
        ///   Setting this property to <see langword="false"/> is not recommended unless absolutely necessary.
        /// </para>
        /// </remarks>
        [Category("Appearance")]
        [Description("Indicates whether the dialog has a cancel button. Do not set to false unless absolutely necessary.")]
        [DefaultValue(true)]
        public bool ShowCancelButton { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the progress dialog has a minimize button.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the dialog has a minimize button; otherwise, <see langword="false"/>. The default
        /// value is <see langword="true"/>.
        /// </value>
        /// <remarks>
        /// <note>
        ///   This property has no effect on modal dialogs (which do not have a minimize button). It only applies
        ///   to modeless dialogs shown by using the <see cref="Show()"/> method.
        /// </note>
        /// <para>
        ///   This property must be set before <see cref="Show()"/> is called. Changing property has
        ///   no effect while the dialog is being displayed.
        /// </para>
        /// </remarks>
        [Category("Window Style")]
        [Description("Indicates whether the progress dialog has a minimize button.")]
        [DefaultValue(true)]
        public bool MinimizeBox { get; set; }

        /// <summary>
        /// Gets a value indicating whether the user has requested cancellation of the operation.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the user has cancelled the progress dialog; otherwise, <see langword="false" />. The default is <see langword="false" />.
        /// </value>
        /// <remarks>
        /// The event handler for the <see cref="DoWork"/> event must periodically check this property and abort the operation
        /// if it returns <see langword="true"/>.
        /// </remarks>
        [Browsable(false)]
        public bool CancellationPending
        {
            get
            {
                _backgroundWorker.ReportProgress(-1); // Call with an out-of-range percentage will update the value of
                // _cancellationPending but do nothing else.
                return _cancellationPending;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether a regular or marquee style progress bar should be used.
        /// </summary>
        /// <value>
        /// One of the values of <see cref="Ookii.Dialogs.Wpf.ProgressBarStyle"/>. 
        /// The default value is <see cref="Ookii.Dialogs.Wpf.ProgressBarStyle.ProgressBar"/>.
        /// </value>
        /// <remarks>
        /// <note>
        ///   Operating systems older than Windows Vista do not support marquee progress bars on the progress dialog. On those operating systems, the
        ///   progress bar will be hidden completely if this property is <see cref="Ookii.Dialogs.Wpf.ProgressBarStyle.MarqueeProgressBar"/>.
        /// </note>
        /// <para>
        ///   When this property is set to <see cref="Ookii.Dialogs.Wpf.ProgressBarStyle.ProgressBar" />, use the <see cref="ReportProgress(int)"/> method to set
        ///   the value of the progress bar. When this property is set to <see cref="Ookii.Dialogs.Wpf.ProgressBarStyle.MarqueeProgressBar"/>
        ///   you can still use the <see cref="ReportProgress(int,string,string)"/> method to update the text of the dialog,
        ///   but the percentage will be ignored.
        /// </para>
        /// <para>
        ///   This property must be set before <see cref="ShowDialog()"/> or <see cref="Show()"/> is called. Changing property has
        ///   no effect while the dialog is being displayed.
        /// </para>
        /// </remarks>
        [Category("Appearance")]
        [Description("Indicates the style of the progress bar.")]
        [DefaultValue(ProgressBarStyle.ProgressBar)]
        public ProgressBarStyle ProgressBarStyle { get; set; }


        /// <summary>
        /// Gets a value that indicates whether the <see cref="ProgressDialog"/> is running an asynchronous operation.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the <see cref="ProgressDialog"/> is running an asynchronous operation; 
        /// otherwise, <see langword="false"/>.
        /// </value>
        [Browsable(false)]
        public bool IsBusy => _backgroundWorker.IsBusy;

        public bool DontDisplaySourcePath { get; set; }

        public bool DontDisplayDestPath { get; set; }

        public bool DontDisplayLocations { get; set; }

        public bool EnablePause { get; set; }

        public bool NoMultiDayEstimates { get; set; }

        public bool AllowUndo { get; set; }

        public bool NoTime { get; set; }


        /// <summary>
        /// Displays the progress dialog as a modeless dialog.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   This function will not block the parent window and will return immediately.
        /// </para>
        /// <para>
        ///   Although this function returns immediately, you cannot use the UI thread to do any processing. The dialog
        ///   will not function correctly unless the UI thread continues to handle window messages, so that thread may
        ///   not be blocked by some other activity. All processing related to the progress dialog must be done in
        ///   the <see cref="DoWork"/> event handler.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">The animation specified in the <see cref="Animation"/> property
        /// could not be loaded.</exception>
        public void Show()
        {
            Show(null);
        }

        /// <summary>
        /// Displays the progress dialog as a modeless dialog.
        /// </summary>
        /// <param name="argument">A parameter for use by the background operation to be executed in the <see cref="DoWork"/> event handler.</param>
        /// <remarks>
        /// <para>
        ///   This function will not block the parent window and return immediately.
        /// </para>
        /// <para>
        ///   Although this function returns immediately, you cannot use the UI thread to do any processing. The dialog
        ///   will not function correctly unless the UI thread continues to handle window messages, so that thread may
        ///   not be blocked by some other activity. All processing related to the progress dialog must be done in
        ///   the <see cref="DoWork"/> event handler.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">The animation specified in the <see cref="Animation"/> property
        /// could not be loaded.</exception>
        public void Show(object argument)
        {
            RunProgressDialog(IntPtr.Zero, argument);
        }

        /// <summary>
        /// Displays the progress dialog as a modal dialog.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   The ShowDialog function for most .Net dialogs will not return until the dialog is closed. However,
        ///   the <see cref="ShowDialog()"/> function for the <see cref="ProgressDialog"/> class will return immediately.
        ///   The parent window will be disabled as with all modal dialogs.
        /// </para>
        /// <para>
        ///   Although this function returns immediately, you cannot use the UI thread to do any processing. The dialog
        ///   will not function correctly unless the UI thread continues to handle window messages, so that thread may
        ///   not be blocked by some other activity. All processing related to the progress dialog must be done in
        ///   the <see cref="DoWork"/> event handler.
        /// </para>
        /// <para>
        ///   The progress dialog's window will appear in the taskbar. This behaviour is also contrary to most .Net dialogs,
        ///   but is part of the underlying native progress dialog API so cannot be avoided.
        /// </para>
        /// <para>
        ///   When possible, it is recommended that you use a modeless dialog using the <see cref="Show()"/> function.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">The animation specified in the <see cref="Animation"/> property
        /// could not be loaded.</exception>
        public void ShowDialog()
        {
            ShowDialog(null, null);
        }

        /// <summary>
        /// Displays the progress dialog as a modal dialog.
        /// </summary>
        /// <param name="owner">The window that owns the dialog.</param>
        /// <remarks>
        /// <para>
        ///   The ShowDialog function for most .Net dialogs will not return until the dialog is closed. However,
        ///   the <see cref="ShowDialog()"/> function for the <see cref="ProgressDialog"/> class will return immediately.
        ///   The parent window will be disabled as with all modal dialogs.
        /// </para>
        /// <para>
        ///   Although this function returns immediately, you cannot use the UI thread to do any processing. The dialog
        ///   will not function correctly unless the UI thread continues to handle window messages, so that thread may
        ///   not be blocked by some other activity. All processing related to the progress dialog must be done in
        ///   the <see cref="DoWork"/> event handler.
        /// </para>
        /// <para>
        ///   The progress dialog's window will appear in the taskbar. This behaviour is also contrary to most .Net dialogs,
        ///   but is part of the underlying native progress dialog API so cannot be avoided.
        /// </para>
        /// <para>
        ///   When possible, it is recommended that you use a modeless dialog using the <see cref="Show()"/> function.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">The animation specified in the <see cref="Animation"/> property
        /// could not be loaded, or the operation is already running.</exception>
        public void ShowDialog(Window owner)
        {
            ShowDialog(owner, null);
        }

        /// <summary>
        /// Displays the progress dialog as a modal dialog.
        /// </summary>
        /// <param name="owner">The window that owns the dialog.</param>
        /// <param name="argument">A parameter for use by the background operation to be executed in the <see cref="DoWork"/> event handler.</param>
        /// <remarks>
        /// <para>
        ///   The ShowDialog function for most .Net dialogs will not return until the dialog is closed. However,
        ///   the <see cref="ShowDialog()"/> function for the <see cref="ProgressDialog"/> class will return immediately.
        ///   The parent window will be disabled as with all modal dialogs.
        /// </para>
        /// <para>
        ///   Although this function returns immediately, you cannot use the UI thread to do any processing. The dialog
        ///   will not function correctly unless the UI thread continues to handle window messages, so that thread may
        ///   not be blocked by some other activity. All processing related to the progress dialog must be done in
        ///   the <see cref="DoWork"/> event handler.
        /// </para>
        /// <para>
        ///   The progress dialog's window will appear in the taskbar. This behaviour is also contrary to most .Net dialogs,
        ///   but is part of the underlying native progress dialog API so cannot be avoided.
        /// </para>
        /// <para>
        ///   When possible, it is recommended that you use a modeless dialog using the <see cref="Show()"/> function.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">The animation specified in the <see cref="Animation"/> property
        /// could not be loaded, or the operation is already running.</exception>
        public void ShowDialog(Window owner, object argument)
        {
            RunProgressDialog(owner == null ? NativeMethods.GetActiveWindow() : new WindowInteropHelper(owner).Handle,
                argument);
        }

        public void UpdateLocations(string source, string destination, string current, bool ensurePathsExists)
        {
            if (_dialog == null)
                throw new InvalidOperationException(Resources.ProgressDialogNotRunningError);

            IShellItem shSource;
            IShellItem shDestination;
            IShellItem shCurrent;

            if (source != null)
            {
                shSource = ensurePathsExists
                    ? NativeMethods.CreateItemFromParsingName(source)
                    : NativeMethods.CreateVirtualItem(source);
            }
            else
            {
                shSource = null;
            }

            if (destination != null)
            {
                shDestination = ensurePathsExists
                    ? NativeMethods.CreateItemFromParsingName(destination)
                    : NativeMethods.CreateVirtualItem(destination);
            }
            else
            {
                shDestination = null;
            }

            if (current != null)
            {
                shCurrent = ensurePathsExists
                    ? NativeMethods.CreateItemFromParsingName(current)
                    : NativeMethods.CreateVirtualItem(current);
            }
            else
            {
                shCurrent = null;
            }

            _backgroundWorker.ReportProgress(-2, new ProgressChangedData
            {
                Source = shSource,
                Destination = shDestination,
                Current = shCurrent
            });
        }

        public void UpdateProgress(int currentProgress, int totalProgress, long processedSize, long totalSize,
            int processedItems, int totalItems, object userState)
        {
            if (_dialog == null)
                throw new InvalidOperationException(Resources.ProgressDialogNotRunningError);

            if (currentProgress < 0 || currentProgress > 100)
                throw new ArgumentOutOfRangeException(nameof(currentProgress));

            if (totalProgress < 0 || totalProgress > 100)
                throw new ArgumentOutOfRangeException(nameof(totalProgress));

            if (processedSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(processedSize));

            if (totalSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(totalSize));

            if (processedItems <= 0)
                throw new ArgumentOutOfRangeException(nameof(processedItems));

            if (totalItems <= 0)
                throw new ArgumentOutOfRangeException(nameof(totalItems));

            int percentProgress = (int) Math.Floor(currentProgress * 100 / (double) totalProgress);
            _backgroundWorker.ReportProgress(percentProgress, new ProgressChangedData
            {
                CurrentProgress = currentProgress,
                TotalProgress = totalProgress,
                ProcessedItems = processedItems,
                TotalItems = totalItems,
                ProcessedSize = processedSize,
                TotalSize = totalSize,
                UserState = userState
            });
        }

        public void SetOperation(OperationsProgressDialogAction operation)
        {
            if (_dialog == null)
                throw new InvalidOperationException(Resources.ProgressDialogNotRunningError);

            _dialog.SetOperation(operation);
        }

        /// <summary>
        /// Raises the <see cref="DoWork"/> event.
        /// </summary>
        /// <param name="e">The <see cref="DoWorkEventArgs"/> containing data for the event.</param>
        protected virtual void OnDoWork(DoWorkEventArgs e)
        {
            DoWorkEventHandler handler = DoWork;
            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="RunWorkerCompleted"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> containing data for the event.</param>
        protected virtual void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
        {
            RunWorkerCompletedEventHandler handler = RunWorkerCompleted;
            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="ProgressChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="ProgressChangedEventArgs"/> containing data for the event.</param>
        protected virtual void OnProgressChanged(ProgressChangedEventArgs e)
        {
            ProgressChangedEventHandler handler = ProgressChanged;
            handler?.Invoke(this, e);
        }

        private void RunProgressDialog(IntPtr owner, object argument)
        {
            if (_backgroundWorker.IsBusy)
                throw new InvalidOperationException(Resources.ProgressDialogRunning);

            _cancellationPending = false;
            _dialog = new Interop.OperationsProgressDialog();

            OperationsProgressDialogFlags flags = OperationsProgressDialogFlags.Normal;
            if (owner != IntPtr.Zero)
                flags |= OperationsProgressDialogFlags.Modal;

            ProgressBarStyle style = ProgressBarStyle;
            if (style == ProgressBarStyle.None)
                flags |= OperationsProgressDialogFlags.NoProgressBar;
            else if (style == ProgressBarStyle.MarqueeProgressBar)
                flags |= OperationsProgressDialogFlags.MarqueeProgress;

            if (ShowTimeRemaining)
                flags |= OperationsProgressDialogFlags.AutoTime;
            if (!ShowCancelButton)
                flags |= OperationsProgressDialogFlags.NoCancel;
            if (!MinimizeBox)
                flags |= OperationsProgressDialogFlags.NoMinimize;
            if (DontDisplaySourcePath)
                flags |= OperationsProgressDialogFlags.DontDisplaySourcePath;
            if (DontDisplayDestPath)
                flags |= OperationsProgressDialogFlags.DontDisplayDestPath;
            if (DontDisplayLocations)
                flags |= OperationsProgressDialogFlags.DontDisplayLocations;
            if (EnablePause)
                flags |= OperationsProgressDialogFlags.EnablePause;
            if (AllowUndo)
                flags |= OperationsProgressDialogFlags.AllowUndo;
            if (NoTime)
                flags |= OperationsProgressDialogFlags.NoTime;
            if (NoMultiDayEstimates)
                flags |= OperationsProgressDialogFlags.NoMultiDayEstimates;

            _dialog.StartProgressDialog(owner, flags);
            _backgroundWorker.RunWorkerAsync(argument);
        }

        private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            OnDoWork(e);
        }

        private void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _dialog.StopProgressDialog();
            Marshal.ReleaseComObject(_dialog);
            _dialog = null;
            OnRunWorkerCompleted(new RunWorkerCompletedEventArgs((!e.Cancelled && e.Error == null) ? e.Result : null,
                e.Error, e.Cancelled));
        }

        private void _backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int hr = _dialog.GetOperationStatus(out OperationsProgressDialogStatus status);
            if (hr < 0)
                Marshal.ThrowExceptionForHR(hr);
            _cancellationPending = status == OperationsProgressDialogStatus.Cancelled;

            if (!(e.UserState is ProgressChangedData data))
                return;

            // ReportProgress doesn't allow values outside this range. However, CancellationPending will call
            // BackgroundWorker.ReportProgress directly with a value that is outside this range to update the value of the property.
            if (e.ProgressPercentage >= 0 && e.ProgressPercentage <= 100)
            {
                _dialog.UpdateProgress((ulong) data.CurrentProgress, (ulong) data.TotalProgress,
                    (ulong) data.ProcessedSize, (ulong) data.TotalSize,
                    (ulong) data.ProcessedItems, (ulong) data.TotalItems);
                OnProgressChanged(new ProgressChangedEventArgs(e.ProgressPercentage, data.UserState));
            }
            else if (e.ProgressPercentage == -2)
            {
                _dialog.UpdateLocations(data.Source, data.Destination, data.Current);
                OnProgressChanged(new ProgressChangedEventArgs(e.ProgressPercentage, data.UserState));
            }
        }
    }
}
