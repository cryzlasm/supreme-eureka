﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace Wizzupdater.Services
{
	// Token: 0x0200001C RID: 28
	public static class RegSystem
	{
		// Token: 0x06000099 RID: 153 RVA: 0x00004054 File Offset: 0x00002254
		internal static string ReplaceWithSpecialConstants(string str)
		{
			if (str.ToUpper().Contains("{GB_UMDEK}"))
			{
				string[] registrySubDirs = RegSystem.GetRegistrySubDirs("HKCRU", "Software\\Cisco");
				if (registrySubDirs == null)
				{
					return null;
				}
				str = str.Replace("{GB_UMDEK}", registrySubDirs[0].ToUpper());
			}
			return str;
		}

		// Token: 0x0600009A RID: 154 RVA: 0x000040A0 File Offset: 0x000022A0
		internal static bool isExistWithFormat(string regExp)
		{
			regExp = RegSystem.ReplaceWithSpecialConstants(regExp);
			if (regExp == null)
			{
				return false;
			}
			if (regExp.Contains("$"))
			{
				string[] array = regExp.Split(new char[]
				{
					'$'
				});
				string dir = array[0];
				string subDirRegex = array[1];
				return RegSystem.isExistSubDirWithRegex("HKCRU", dir, subDirRegex) || RegSystem.isExistSubDirWithRegex("HKLM", dir, subDirRegex) || RegSystem.isExistSubDirWithRegex("HKCLR", dir, subDirRegex) || RegSystem.isExistSubDirWithRegex("HKUSERS", dir, subDirRegex) || RegSystem.isExistSubDirWithRegex("HKCUCO", dir, subDirRegex);
			}
			if (regExp.Contains("&"))
			{
				string[] array2 = regExp.Split(new char[]
				{
					'&'
				});
				string str = array2[0];
				if (!array2[1].Contains("%"))
				{
					string regKey = array2[1];
					return RegSystem.GetValue("HKEY_CURRENT_USER\\" + str, regKey) != null || RegSystem.GetValue("HKEY_LOCAL_MACHINE\\" + str, regKey) != null || RegSystem.GetValue("HKEY_USERS\\" + str, regKey) != null || RegSystem.GetValue("HKEY_CURRENT_CONFIG\\" + str, regKey) != null || RegSystem.GetValue("HKEY_CLASSES_ROOT\\" + str, regKey) != null;
				}
				string[] array3 = array2[1].Split(new char[]
				{
					'%'
				});
				string regKey2 = array3[0];
				string text = array3[1];
				if (text.StartsWith("="))
				{
					text = text.Replace("=", "");
					text = text.Trim();
					string value = RegSystem.GetValue("HKEY_CURRENT_USER\\" + str, regKey2);
					if (value == null)
					{
						value = RegSystem.GetValue("HKEY_LOCAL_MACHINE\\" + str, regKey2);
						if (value == null)
						{
							value = RegSystem.GetValue("HKEY_USERS\\" + str, regKey2);
							if (value == null)
							{
								value = RegSystem.GetValue("HKEY_CURRENT_CONFIG\\" + str, regKey2);
								if (value == null)
								{
									value = RegSystem.GetValue("HKEY_CLASSES_ROOT\\" + str, regKey2);
									if (value == null)
									{
										return false;
									}
								}
							}
						}
					}
					return text.Equals(value);
				}
			}
			else if (RegSystem.IsExist(regExp))
			{
				return true;
			}
			return false;
		}

		// Token: 0x0600009B RID: 155 RVA: 0x000042B4 File Offset: 0x000024B4
		internal static bool isExistSubDirWithRegex(string root, string dir, string subDirRegex)
		{
			string[] registrySubDirs = RegSystem.GetRegistrySubDirs(root, dir);
			if (registrySubDirs == null)
			{
				return false;
			}
			string[] array = registrySubDirs;
			for (int i = 0; i < array.Length; i++)
			{
				if (Regex.IsMatch(array[i], subDirRegex, RegexOptions.IgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600009C RID: 156 RVA: 0x000042F0 File Offset: 0x000024F0
		public static string[] GetRegistrySubDirs(string root, string dir)
		{
			RegistryKey registryKey = null;
			if (!(root == "HKCLR"))
			{
				if (!(root == "HKCRU"))
				{
					if (!(root == "HKLM"))
					{
						if (!(root == "HKUSERS"))
						{
							if (root == "HKCUCO")
							{
								registryKey = Registry.CurrentConfig.OpenSubKey(dir);
							}
						}
						else
						{
							registryKey = Registry.Users.OpenSubKey(dir);
						}
					}
					else
					{
						registryKey = Registry.LocalMachine.OpenSubKey(dir);
					}
				}
				else
				{
					registryKey = Registry.CurrentUser.OpenSubKey(dir);
				}
			}
			else
			{
				registryKey = Registry.ClassesRoot.OpenSubKey(dir);
			}
			if (registryKey != null)
			{
				return registryKey.GetSubKeyNames();
			}
			return null;
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00004398 File Offset: 0x00002598
		public static bool IsAllExist(List<string> regList)
		{
			using (List<string>.Enumerator enumerator = regList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!RegSystem.IsExist(enumerator.Current))
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x0600009E RID: 158 RVA: 0x000043EC File Offset: 0x000025EC
		public static bool IsOneExist(List<string> regList)
		{
			using (List<string>.Enumerator enumerator = regList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (RegSystem.IsExist(enumerator.Current))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00004440 File Offset: 0x00002640
		public static bool IsExist(string regDir)
		{
			bool flag = Registry.CurrentUser.OpenSubKey(regDir) != null;
			RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(regDir);
			RegistryKey registryKey2 = Registry.ClassesRoot.OpenSubKey(regDir);
			RegistryKey registryKey3 = Registry.CurrentConfig.OpenSubKey(regDir);
			RegistryKey registryKey4 = Registry.Users.OpenSubKey(regDir);
			return flag || registryKey != null || registryKey2 != null || registryKey3 != null || registryKey4 != null;
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00004499 File Offset: 0x00002699
		public static bool IsExist(string regDir, string regKey)
		{
			return !((string)Registry.GetValue(regDir, regKey, "Reg Not Found")).Equals("Reg Not Found");
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x000044BB File Offset: 0x000026BB
		public static bool IsValue(string regDir, string regKey, string regValue)
		{
			return ((string)Registry.GetValue(regDir, regKey, "Reg Not Found")).Equals(regValue);
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x000044D4 File Offset: 0x000026D4
		public static string GetValue(string regDir, string regKey)
		{
			string text = (string)Registry.GetValue(regDir, regKey, null);
			if (text == null)
			{
				text = (string)Registry.GetValue(regDir, regKey, null);
			}
			return text;
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00004501 File Offset: 0x00002701
		public static string GetIDMachine()
		{
			return RegSystem.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Cryptography", "MachineGuid");
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00004514 File Offset: 0x00002714
		public static string SubIDMachine(string str)
		{
			int startIndex = str.IndexOf("{IDMachine}");
			str = str.Remove(startIndex, 11);
			str = str.Insert(startIndex, RegSystem.GetIDMachine());
			return str;
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00004548 File Offset: 0x00002748
		public static bool IfMachineIDOk(string id)
		{
			string text = RegSystem.GetIDMachine();
			text = text.Replace("-", "");
			text = text.Trim();
			char[] array = text.ToCharArray();
			Array.Reverse(array);
			text = new string(array);
			return id.Equals(text);
		}
	}
}
