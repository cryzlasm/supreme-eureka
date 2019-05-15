﻿using System;
using System.IO;

namespace Wizzupdater.Services
{
	// Token: 0x0200001B RID: 27
	internal static class WinPath
	{
		// Token: 0x06000091 RID: 145 RVA: 0x00003E7F File Offset: 0x0000207F
		public static string GetTmpFolder()
		{
			return Path.GetTempPath();
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00003E86 File Offset: 0x00002086
		public static string GetProgrammeFilesFolder64()
		{
			if (WindowsVersion.Is32Bits())
			{
				return Environment.GetEnvironmentVariable("PROGRAMFILES");
			}
			return Environment.GetEnvironmentVariable("ProgramW6432");
		}

		// Token: 0x06000093 RID: 147 RVA: 0x00003EA4 File Offset: 0x000020A4
		public static string GetProgrammeFilesFolder32()
		{
			if (WindowsVersion.Is32Bits())
			{
				return Environment.GetEnvironmentVariable("PROGRAMFILES");
			}
			return Environment.GetEnvironmentVariable("PROGRAMFILES(X86)");
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00003EC2 File Offset: 0x000020C2
		public static string GetAppdataFolder()
		{
			return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00003ECB File Offset: 0x000020CB
		public static string GetLocalAppDataFolder()
		{
			return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00003ED4 File Offset: 0x000020D4
		public static string GetUserFolder()
		{
			return Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)).ToString();
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00003EE8 File Offset: 0x000020E8
		public static bool CheckContent(string path, string content)
		{
			string[] array = content.Split(new char[]
			{
				'~'
			});
			string[] array2 = File.ReadAllLines(path);
			if (array.Length != array2.Length)
			{
				return false;
			}
			for (int i = 0; i < array2.Length; i++)
			{
				if (array2[i] != array[i])
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00003F38 File Offset: 0x00002138
		public static bool isExistWithFormat(string name, bool directory, string content)
		{
			if (name.Contains("{user}"))
			{
				name = name.Replace("{user}", WinPath.GetUserFolder());
			}
			if (name.Contains("{localappdata}"))
			{
				name = name.Replace("{localappdata}", WinPath.GetLocalAppDataFolder());
			}
			if (name.Contains("{appdata}"))
			{
				name = name.Replace("{appdata}", WinPath.GetAppdataFolder());
			}
			if (name.Contains("{tmp}"))
			{
				name = name.Replace("{tmp}", WinPath.GetTmpFolder());
			}
			if (name.Contains("{temp}"))
			{
				name = name.Replace("{temp}", WinPath.GetTmpFolder());
			}
			string text = name;
			string text2 = name;
			if (name.Contains("{pf}"))
			{
				text = text.Replace("{pf}", WinPath.GetProgrammeFilesFolder32());
				text2 = text2.Replace("{pf}", WinPath.GetProgrammeFilesFolder64());
			}
			if (directory)
			{
				if (Directory.Exists(text) || Directory.Exists(text2))
				{
					return true;
				}
			}
			else if (File.Exists(text) || File.Exists(text2))
			{
				return content == null || WinPath.CheckContent(text, content) || WinPath.CheckContent(text2, content);
			}
			return false;
		}
	}
}
