using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MainSite.Connect.Cryptography
{
    public  class FileData:IDisposable
    {
        static private Dictionary<string, SecureString> UserPass=new Dictionary<string,SecureString>();
        public static void AddPass(string password, string UserId)
        {
            if (password == null)
                throw new ArgumentNullException("password");

            var securePassword = new SecureString();

            foreach (char c in password)
                securePassword.AppendChar(c);

            UserPass.Add(UserId, securePassword);
        }
        public static void RemovePass(string UserId)
        {
            if (UserId == null)
                throw new ArgumentNullException("UserId");

            
            UserPass.Remove(UserId);
        }
        public static byte[] Encrypt(byte[] Data, string UserID)
        {
            if (Data == null)
            {
                throw new ArgumentNullException(nameof(Data));
            }
            var password  = new System.Net.NetworkCredential(string.Empty, UserPass[UserID]).Password; 
            byte[] passwordBytes = UnicodeEncoding.ASCII.GetBytes(password);
            byte[] Key = SHA256Managed.Create().ComputeHash(passwordBytes);
            byte[] IV = MD5.Create().ComputeHash(passwordBytes);
            using (var rijndaelManaged = new RijndaelManaged())
            {
                rijndaelManaged.KeySize = Key.Length * 8;
                rijndaelManaged.Key = Key;
                rijndaelManaged.BlockSize = IV.Length * 8;
                rijndaelManaged.IV = IV;
                rijndaelManaged.Padding = PaddingMode.PKCS7;
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
        public static byte[] Decrypt(byte[] Data, string UserID)
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
            var password = new System.Net.NetworkCredential(string.Empty, UserPass[UserID]).Password;
            byte[] passwordBytes = UnicodeEncoding.ASCII.GetBytes(password);
            byte[] Key = SHA256Managed.Create().ComputeHash(passwordBytes);
            byte[] IV = MD5.Create().ComputeHash(passwordBytes);
          
            using (var rijndaelManaged = new RijndaelManaged())
            {
                
                rijndaelManaged.KeySize = Key.Length * 8;
                rijndaelManaged.Key = Key;
                rijndaelManaged.BlockSize = IV.Length * 8;
                rijndaelManaged.IV = IV;
                rijndaelManaged.Padding = PaddingMode.PKCS7;
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

        public void Dispose()
        {
            UserPass.Clear();
        }

    }
}
