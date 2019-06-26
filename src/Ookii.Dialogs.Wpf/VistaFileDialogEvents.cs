// Copyright (c) Sven Groot (Ookii.org) 2006
// See LICENSE for details

using System;
using Ookii.Dialogs.Wpf.Interop;
// ReSharper disable InconsistentNaming

namespace Ookii.Dialogs.Wpf
{
    internal class VistaFileDialogEvents : IFileDialogEvents, IFileDialogControlEvents
    {
        private const uint S_OK = 0;
        private const uint S_FALSE = 1;
        private const uint E_NOTIMPL = 0x80004001;

        private readonly VistaFileDialog _dialog;

        public VistaFileDialogEvents(VistaFileDialog dialog)
        {
            _dialog = dialog ?? throw new ArgumentNullException(nameof(dialog));
        }

        #region IFileDialogEvents Members

        public HRESULT OnFileOk(IFileDialog pfd)
        {
            if( _dialog.DoFileOk(pfd) )
                return HRESULT.S_OK;
            return HRESULT.S_FALSE;
        }

        public HRESULT OnFolderChanging(IFileDialog pfd, IShellItem psiFolder)
        {
            return HRESULT.S_OK;
        }

        public void OnFolderChange(IFileDialog pfd)
        {
        }

        public void OnSelectionChange(IFileDialog pfd)
        {
        }

        public void OnShareViolation(IFileDialog pfd, IShellItem psi, out NativeMethods.FDE_SHAREVIOLATION_RESPONSE pResponse)
        {
            pResponse = NativeMethods.FDE_SHAREVIOLATION_RESPONSE.FDESVR_DEFAULT;
        }

        public void OnTypeChange(IFileDialog pfd)
        {
        }

        public void OnOverwrite(IFileDialog pfd, IShellItem psi, out NativeMethods.FDE_OVERWRITE_RESPONSE pResponse)
        {
            pResponse = NativeMethods.FDE_OVERWRITE_RESPONSE.FDEOR_DEFAULT;
        }

        #endregion

        #region IFileDialogControlEvents Members

        public void OnItemSelected(IFileDialogCustomize pfdc, int dwIDCtl, int dwIDItem)
        {
        }

        public void OnButtonClicked(IFileDialogCustomize pfdc, int dwIDCtl)
        {
        }

        public void OnCheckButtonToggled(IFileDialogCustomize pfdc, int dwIDCtl, bool bChecked)
        {
        }

        public void OnControlActivating(IFileDialogCustomize pfdc, int dwIDCtl)
        {
        }

        #endregion


    }
}
