// Copyright (c) Sven Groot (Ookii.org) 2009
// BSD license; see LICENSE for details.

using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;

namespace Ookii.Dialogs.Wpf
{
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    internal class SafeModuleHandle : SafeHandle
    {
        public SafeModuleHandle()
            : base(IntPtr.Zero, true)
        {
        }

        public override bool IsInvalid => handle == IntPtr.Zero;

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle()
        {
            return NativeMethods.FreeLibrary(handle);
        }
    }

    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    internal class ActivationContextSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public ActivationContextSafeHandle()
            : base(true)
        {
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle()
        {
            NativeMethods.ReleaseActCtx(handle);
            return true;
        }
    }
}
