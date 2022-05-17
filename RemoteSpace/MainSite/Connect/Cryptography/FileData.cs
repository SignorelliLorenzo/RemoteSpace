using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace MainSite.Connect.Cryptography
{
    public static class FileData
    {
        public static byte[] Encrypt(byte[] Data, byte[] Key, byte[] IV)
        {
            if (Data == null)
            {
                throw new ArgumentNullException(nameof(Data));
            }

            using (var rijndaelManaged = new RijndaelManaged())
            {
                rijndaelManaged.KeySize = Key.Length * 8;
                rijndaelManaged.Key = Key;
                rijndaelManaged.BlockSize = IV.Length * 8;
                rijndaelManaged.IV = IV;

                using (var encryptor = rijndaelManaged.CreateEncryptor())
                using (var ms = new MemoryStream())
                using (var cryptoStream = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(Data, 0, Data.Length);
                    cryptoStream.FlushFinalBlock();

                    return ms.ToArray();
                }
            }
        }
        public static byte[] Decrypt(byte[] Data, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (Data == null)
            {
                throw new ArgumentNullException(nameof(Data));
            }
            else if (Data.Length == 0)
            {
                return Data;
            }

            using (var rijndaelManaged = new RijndaelManaged())
            {
                rijndaelManaged.KeySize = Key.Length * 8;
                rijndaelManaged.Key = Key;
                rijndaelManaged.BlockSize = IV.Length * 8;
                rijndaelManaged.IV = IV;

                using (var decryptor = rijndaelManaged.CreateDecryptor())
                using (var ms = new MemoryStream(Data))
                using (var cryptoStream = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {

                    var dycrypted = new byte[Data.Length];
                    var bytesRead = cryptoStream.Read(dycrypted, 0, Data.Length);

                    return dycrypted.Take(bytesRead).ToArray();
                }
            }
        }
    }
}
