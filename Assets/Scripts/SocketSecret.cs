using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Security.Cryptography; 

public static class SocketSecret
	{
		private static Encoding _DESEncoding = Encoding.UTF8;
		/// <summary>
		/// DES加密字符串
		/// </summary>
		/// <param name="encryptString">待加密的字符串</param>
		/// <param name="encryptKey">加密密钥,要求为8位</param>
	// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
		public static string DESEncrypt(this string cryptString, string key, string iv = "")
		{
			if ( string.IsNullOrEmpty(iv))
			{
				iv = key;
			}
			byte[] bytes = SocketSecret._DESEncoding.GetBytes(cryptString);
			MemoryStream memoryStream = new MemoryStream();
			DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
			CryptoStream cryptoStream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateEncryptor(SocketSecret._DESEncoding.GetBytes(key), SocketSecret._DESEncoding.GetBytes(iv)), CryptoStreamMode.Write);
			cryptoStream.Write(bytes, 0, bytes.Length);
			cryptoStream.FlushFinalBlock();
			return Convert.ToBase64String(memoryStream.ToArray());
		}

		/// <summary>
		/// DES解密字符串
		/// </summary>
		/// <param name="decryptString">待解密的字符串</param>
		/// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
		/// <returns>解密成功返回解密后的字符串，失败返源串</returns>
		public static string DESDecrypt(this string decryptString, string key, string iv = "")
		{
			try {
		if (string.IsNullOrEmpty(iv))
			{
				iv = key;
			}
			byte[] array = Convert.FromBase64String(decryptString);
			MemoryStream memoryStream = new MemoryStream();
			DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
			CryptoStream cryptoStream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateDecryptor(SocketSecret._DESEncoding.GetBytes(key), SocketSecret._DESEncoding.GetBytes(iv)), CryptoStreamMode.Write);
			cryptoStream.Write(array, 0, array.Length);
			cryptoStream.FlushFinalBlock();
			return SocketSecret._DESEncoding.GetString(memoryStream.ToArray());
			}
			catch {
  				return decryptString;
			}
	
		}
	} 

