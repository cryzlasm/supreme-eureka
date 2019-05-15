﻿using System;
using System.Diagnostics;
using Microsoft.Win32;

namespace Wizzupdater.Services
{
	// Token: 0x02000018 RID: 24
	public static class Navigator
	{
		// Token: 0x0600007B RID: 123 RVA: 0x000039C0 File Offset: 0x00001BC0
		public static bool isExistWithFormat(string format)
		{
			if (format == null)
			{
				return false;
			}
			if (format.Contains("$"))
			{
				string[] array = format.Split(new char[]
				{
					'$'
				});
				return Navigator.IsGreater(array[0].ToUpper(), array[1]);
			}
			string a = format.ToUpper();
			if (a == "IE")
			{
				return Navigator.GetIEVersion() != null;
			}
			if (!(a == "FF"))
			{
				return a == "CH" && Navigator.GetChromeVersion() != null;
			}
			return Navigator.GetFirefoxVersion() != null;
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00003A54 File Offset: 0x00001C54
		public static string GetChromeVersion()
		{
			object value = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths\\chrome.exe", "", null);
			if (value != null)
			{
				return FileVersionInfo.GetVersionInfo(value.ToString()).FileVersion;
			}
			return null;
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00003A88 File Offset: 0x00001C88
		public static string GetFirefoxVersion()
		{
			object value = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths\\firefox.exe", "", null);
			if (value != null)
			{
				return FileVersionInfo.GetVersionInfo(value.ToString()).FileVersion;
			}
			return null;
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00003ABC File Offset: 0x00001CBC
		public static bool IsGreater(string navigator, string version)
		{
			if (!version.Contains("."))
			{
				version += ".0";
			}
			Version version2 = new Version(version + ".0");
			if (!(navigator == "IE"))
			{
				if (!(navigator == "FF"))
				{
					if (!(navigator == "CH"))
					{
						return false;
					}
					string text = Navigator.GetChromeVersion();
					if (text == null)
					{
						return false;
					}
					if (!text.Contains("."))
					{
						text += ".0";
					}
					return new Version(text).CompareTo(version2) >= 0;
				}
				else
				{
					string text2 = Navigator.GetFirefoxVersion();
					if (text2 == null)
					{
						return false;
					}
					if (!text2.Contains("."))
					{
						text2 += ".0";
					}
					Version value = new Version(text2);
					return version2.CompareTo(value) >= 0;
				}
			}
			else
			{
				string text3 = Navigator.GetIEVersion();
				if (text3 == null)
				{
					return false;
				}
				if (!text3.Contains("."))
				{
					text3 += ".0";
				}
				if (text3.Contains(" "))
				{
					text3 = text3.Split(new char[]
					{
						' '
					})[0];
				}
				return new Version(text3).CompareTo(version2) >= 0;
			}
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00003BF4 File Offset: 0x00001DF4
		public static string GetIEVersion()
		{
			object value = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths\\IEXPLORE.EXE", "", null);
			if (value != null)
			{
				return FileVersionInfo.GetVersionInfo(value.ToString()).FileVersion;
			}
			return null;
		}
	}
}
