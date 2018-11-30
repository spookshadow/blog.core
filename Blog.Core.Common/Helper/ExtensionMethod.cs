using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;

namespace Blog.Core.Common.Helper
{
    public class ExtensionMethod
    {
        internal static byte[] AsBytes(object obj)
        {
            using (var ms = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                return ms.GetBuffer();
            }
        }

        internal static string MD5(byte[] source)
        {
            using (MD5 md5 = new MD5CryptoServiceProvider())
            {
                var hash = md5.ComputeHash(source);
                md5.Clear();
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}
