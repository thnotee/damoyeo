using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Damoyeo.Util
{
    public static class StringUtil
    {

        public static string GetAppSetting(string appSettingKey)
        {
            return !string.IsNullOrEmpty(ConfigurationManager.AppSettings[appSettingKey]) ? ConfigurationManager.AppSettings[appSettingKey].ToString() : string.Empty;
        }

        public static string GetSHA256(string strPlain)
        {

            UTF8Encoding UE = new UTF8Encoding();
            byte[] HashValue, MessageBytes = UE.GetBytes(strPlain);
            SHA256Managed SHhash = new SHA256Managed();
            string strHex = "";

            HashValue = SHhash.ComputeHash(MessageBytes);
            foreach (byte b in HashValue)
            {
                strHex += String.Format("{0:x2}", b);
            }
            return strHex;

        } /* GetSHA256 */
    }
}
