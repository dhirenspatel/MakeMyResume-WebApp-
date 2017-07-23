using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;


    public class md5
    {
        public md5()
        {
        }
        public md5(String pwd)
        {
            this.pwd = pwd;
        }

        public virtual String pwd { get; set; }

        public String calculateMD5()
        {
            pwd = this.pwd;
            MD5 md5Hasher = MD5.Create();
            Byte[] hashedBytes;

            hashedBytes = md5Hasher.ComputeHash(Encoding.Default.GetBytes(pwd));

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hashedBytes.Length; i++)
            {
                sb.Append(hashedBytes[i].ToString("X2"));
            }

            return sb.ToString();
        }

    }
