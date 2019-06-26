// Copyright (c) Sven Groot (Ookii.org) 2006
// See LICENSE for details

namespace Ookii.Dialogs.Wpf.Interop
{
    internal static class ComDlgResources
    {
        public enum ComDlgResourceId
        {
            OpenButton = 370,
            Open = 384,
            FileNotFound = 391,
            CreatePrompt = 402,
            ReadOnly = 427,
            ConfirmSaveAs = 435
        }

        private static readonly Win32Resources _resources = new Win32Resources("comdlg32.dll");

        public static string LoadString(ComDlgResourceId id)
        {
            return _resources.LoadString((uint)id);
        }

        public static string FormatString(ComDlgResourceId id, params string[] args)
        {
            return _resources.FormatString((uint)id, args);
        }
    }
}
