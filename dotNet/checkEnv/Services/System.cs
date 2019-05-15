﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using Microsoft.Win32;

namespace Wizzupdater.Services
{
	// Token: 0x0200001D RID: 29
	public static class System
	{
		// Token: 0x060000A6 RID: 166 RVA: 0x0000458C File Offset: 0x0000278C
		public static bool IsRunningVM()
		{
			List<string> list = new List<string>
			{
				"vbox",
				"vmware",
				"parallels",
				"parallels vm",
				"xen",
				"virtual",
				"VM"
			};
			if (RegSystem.IsOneExist(new List<string>
			{
				"SOFTWARE\\Classes\\Virtual.Machine.VMC",
				"SOFTWARE\\Wow6432Node\\Classes\\Virtual.Machine.VMC",
				"Software\\Oracle\\VirtualBox",
				"Software\\VMware, Inc."
			}))
			{
				return false;
			}
			string value = RegSystem.GetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\services\\Disk\\Enum", "0");
			string value2 = RegSystem.GetValue("HKEY_LOCAL_MACHINE\\HARDWARE\\DESCRIPTION\\System\\BIOS", "SystemManufacturer");
			foreach (string text in list)
			{
				if (value.ToLower().Contains(text.ToLower()) || value2.ToLower().Contains(text.ToLower()))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x000046B4 File Offset: 0x000028B4
		public static bool IsProssRunning(string pross)
		{
			return Process.GetProcessesByName(pross).Length != 0;
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x000046C2 File Offset: 0x000028C2
		public static bool IsTaskMgrRunning()
		{
			return System.IsProssRunning("Taskmgr");
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x000046CE File Offset: 0x000028CE
		public static bool IsRegEditRunning()
		{
			return System.IsProssRunning("regedit");
		}

		// Token: 0x060000AA RID: 170 RVA: 0x000046DA File Offset: 0x000028DA
		public static bool IsSoftwareIsInstaled()
		{
			return true;
		}

		// Token: 0x060000AB RID: 171 RVA: 0x000046DD File Offset: 0x000028DD
		public static bool IsDirExist(string dir)
		{
			return Directory.Exists(dir);
		}

		// Token: 0x060000AC RID: 172 RVA: 0x000046E8 File Offset: 0x000028E8
		public static bool IsPortInUse(int port)
		{
			bool result = false;
			IPEndPoint[] activeTcpListeners = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();
			for (int i = 0; i < activeTcpListeners.Length; i++)
			{
				if (activeTcpListeners[i].Port == port)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00004720 File Offset: 0x00002920
		public static bool DetectVM(out System.VirtualMachine virtualMachine)
		{
			virtualMachine = default(System.VirtualMachine);
			bool result = false;
			try
			{
				RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Virtual Machine\\Guest\\Parameters");
				if (registryKey != null)
				{
					virtualMachine = new System.VirtualMachine(registryKey.GetValue("HostName").ToString(), registryKey.GetValue("VirtualMachineName").ToString());
					result = true;
				}
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		// Token: 0x02000035 RID: 53
		public struct VirtualMachine
		{
			// Token: 0x17000020 RID: 32
			// (get) Token: 0x06000125 RID: 293 RVA: 0x00009BB4 File Offset: 0x00007DB4
			// (set) Token: 0x06000126 RID: 294 RVA: 0x00009BBC File Offset: 0x00007DBC
			public string Host { get; set; }

			// Token: 0x17000021 RID: 33
			// (get) Token: 0x06000127 RID: 295 RVA: 0x00009BC5 File Offset: 0x00007DC5
			// (set) Token: 0x06000128 RID: 296 RVA: 0x00009BCD File Offset: 0x00007DCD
			public string MachineName { get; set; }

			// Token: 0x06000129 RID: 297 RVA: 0x00009BD6 File Offset: 0x00007DD6
			public VirtualMachine(string host, string machineName)
			{
				this = default(System.VirtualMachine);
				this.Host = host;
				this.MachineName = machineName;
			}
		}
	}
}
