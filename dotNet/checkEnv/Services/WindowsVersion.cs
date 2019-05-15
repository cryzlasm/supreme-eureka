﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Wizzupdater.Services
{
	// Token: 0x0200001A RID: 26
	public static class WindowsVersion
	{
		// Token: 0x06000083 RID: 131 RVA: 0x00003D1C File Offset: 0x00001F1C
		public static void init()
		{
			WindowsVersion.regValue = RegSystem.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion", "ProductName");
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00003D32 File Offset: 0x00001F32
		public static bool isWindows(string windows)
		{
			if (windows == null)
			{
				return false;
			}
			WindowsVersion.init();
			return WindowsVersion.regValue.Contains(windows.ToUpper());
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00003D4E File Offset: 0x00001F4E
		public static bool IsXP()
		{
			WindowsVersion.init();
			return WindowsVersion.regValue.Contains("XP");
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00003D64 File Offset: 0x00001F64
		public static bool IsVista()
		{
			WindowsVersion.init();
			return WindowsVersion.regValue.Contains("Vista");
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00003D7A File Offset: 0x00001F7A
		public static bool Is8()
		{
			WindowsVersion.init();
			return WindowsVersion.regValue.Contains("8");
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00003D90 File Offset: 0x00001F90
		public static bool Is81()
		{
			WindowsVersion.init();
			return WindowsVersion.regValue.Contains("8.1");
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00003DA6 File Offset: 0x00001FA6
		public static bool Is7()
		{
			WindowsVersion.init();
			return WindowsVersion.regValue.Contains("7");
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00003DBC File Offset: 0x00001FBC
		public static bool Is10()
		{
			WindowsVersion.init();
			return WindowsVersion.regValue.Contains("10");
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00003DD2 File Offset: 0x00001FD2
		public static bool Is32Bits()
		{
			return !WindowsVersion.Is64Bits();
		}

		// Token: 0x0600008C RID: 140
		[DllImport("kernel32", SetLastError = true)]
		public static extern IntPtr LoadLibrary(string libraryName);

		// Token: 0x0600008D RID: 141
		[DllImport("kernel32", SetLastError = true)]
		public static extern IntPtr GetProcAddress(IntPtr hwnd, string procedureName);

		// Token: 0x0600008E RID: 142 RVA: 0x00003DDC File Offset: 0x00001FDC
		public static bool Is64Bits()
		{
			return IntPtr.Size == 8 || (IntPtr.Size == 4 && WindowsVersion.Is32BitProcessOn64BitProcessor());
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00003DF8 File Offset: 0x00001FF8
		private static WindowsVersion.IsWow64ProcessDelegate GetIsWow64ProcessDelegate()
		{
			IntPtr intPtr = WindowsVersion.LoadLibrary("kernel32");
			if (intPtr != IntPtr.Zero)
			{
				IntPtr procAddress = WindowsVersion.GetProcAddress(intPtr, "IsWow64Process");
				if (procAddress != IntPtr.Zero)
				{
					return (WindowsVersion.IsWow64ProcessDelegate)Marshal.GetDelegateForFunctionPointer(procAddress, typeof(WindowsVersion.IsWow64ProcessDelegate));
				}
			}
			return null;
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00003E50 File Offset: 0x00002050
		private static bool Is32BitProcessOn64BitProcessor()
		{
			WindowsVersion.IsWow64ProcessDelegate isWow64ProcessDelegate = WindowsVersion.GetIsWow64ProcessDelegate();
			bool flag;
			return isWow64ProcessDelegate != null && isWow64ProcessDelegate(Process.GetCurrentProcess().Handle, out flag) && flag;
		}

		// Token: 0x04000039 RID: 57
		public static string regValue;

		// Token: 0x02000034 RID: 52
		// (Invoke) Token: 0x06000122 RID: 290
		private delegate bool IsWow64ProcessDelegate([In] IntPtr handle, out bool isWow64Process);
	}
}
