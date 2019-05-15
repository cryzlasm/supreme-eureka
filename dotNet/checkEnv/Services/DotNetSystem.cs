﻿using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace Wizzupdater.Services
{
	// Token: 0x02000016 RID: 22
	public static class DotNetSystem
	{
		// Token: 0x06000075 RID: 117 RVA: 0x000035B8 File Offset: 0x000017B8
		public static string GetMaxValue()
		{
			string name = "SOFTWARE\\Microsoft\\NET Framework Setup\\NDP";
			List<Version> list = new List<Version>();
			string[] subKeyNames = Registry.LocalMachine.OpenSubKey(name).GetSubKeyNames();
			Version version = new Version("0.0.0");
			for (int i = 1; i <= subKeyNames.Length - 1; i++)
			{
				Version version2 = new Version(subKeyNames[i].ToString().Remove(0, 1) + ".0");
				list.Add(version2);
				if (version < version2)
				{
					version = version2;
				}
			}
			return version.ToString();
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00003640 File Offset: 0x00001840
		public static bool isOkWithFormat(string format)
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
				if (array.Length >= 2)
				{
					return DotNetSystem.IsValueIntervalOk(array[0], array[1]);
				}
			}
			return false;
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00003684 File Offset: 0x00001884
		public static bool IsValueIntervalOk(string minVersion, string maxVersion)
		{
			Version value = new Version(minVersion);
			Version value2 = new Version(maxVersion);
			Version version = new Version(DotNetSystem.GetMaxValue());
			int num = version.CompareTo(value);
			int num2 = version.CompareTo(value2);
			return num >= 0 && num2 <= 0;
		}
	}
}
