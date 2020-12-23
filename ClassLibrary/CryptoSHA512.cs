using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NHRIDBLibrary
{
    public class CryptoSHA512 
    {
        public string CryptoString(string StrCryptoValue)
        {
            byte[] bytes = new SHA512CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(StrCryptoValue));
            string passwd = Convert.ToBase64String(bytes);

            return passwd;
        }
    }
}
