﻿using System;
using System.IO;

namespace Wizzupdater.Services
{
	// Token: 0x02000015 RID: 21
	internal class DiskInfo
	{
		// Token: 0x06000072 RID: 114 RVA: 0x000034E8 File Offset: 0x000016E8
		public static string FormatBytes(long bytes)
		{
			string[] array = new string[]
			{
				"GB",
				"MB",
				"KB",
				"Bytes"
			};
			long num = (long)Math.Pow(1024.0, (double)(array.Length - 1));
			foreach (string text in array)
			{
				if (bytes > num)
				{
					return string.Format("{0:##.##}", decimal.Divide(bytes, num));
				}
				num /= 1024L;
			}
			return "0";
		}

		// Token: 0x06000073 RID: 115 RVA: 0x0000357C File Offset: 0x0000177C
		public static long GetAllHDDFreeSpace()
		{
			long num = -1L;
			foreach (DriveInfo driveInfo in DriveInfo.GetDrives())
			{
				if (driveInfo.IsReady)
				{
					num += driveInfo.TotalFreeSpace;
				}
			}
			return num;
		}
	}
}
