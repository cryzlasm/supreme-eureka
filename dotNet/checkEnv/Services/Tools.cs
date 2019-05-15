﻿using System;
using System.Net;

namespace Wizzupdater.Services
{
	// Token: 0x02000019 RID: 25
	internal class Tools
	{
		// Token: 0x06000080 RID: 128 RVA: 0x00003C28 File Offset: 0x00001E28
		public static string RandomString(int length)
		{
			Random random = new Random();
			string text = "";
			for (int i = 0; i < length; i++)
			{
				int index = random.Next(0, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".Length);
				text += "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"[index].ToString();
			}
			return text;
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00003C7C File Offset: 0x00001E7C
		public static string GetRederectedurlWithFormat(string url)
		{
			url = url.Remove(0, 8);
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.AllowAutoRedirect = false;
			httpWebRequest.Timeout = 10000;
			httpWebRequest.Method = "HEAD";
			HttpWebResponse httpWebResponse2;
			HttpWebResponse httpWebResponse = httpWebResponse2 = (HttpWebResponse)httpWebRequest.GetResponse();
			try
			{
				if (httpWebResponse.StatusCode >= HttpStatusCode.MultipleChoices && httpWebResponse.StatusCode <= (HttpStatusCode)399)
				{
					string result = httpWebResponse.Headers["Location"];
					httpWebResponse.Close();
					return result;
				}
			}
			finally
			{
				if (httpWebResponse2 != null)
				{
					((IDisposable)httpWebResponse2).Dispose();
				}
			}
			return null;
		}
	}
}
