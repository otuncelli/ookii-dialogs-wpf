namespace Ookii.Dialogs.Wpf
{
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
}
