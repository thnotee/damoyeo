using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web;

namespace Damoyeo.Common
{
    public class CookieSerializer
    {
        // 객체를 쿠키에 직렬화하여 저장
        /*
        public void SerializeToCookie(object obj, string key)
        {
            HttpCookie encodedCookie = new HttpCookie(key);
            MemoryStream memoryStream = new MemoryStream();
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            binaryFormatter.Serialize(memoryStream, obj);
            byte[] byteArr = memoryStream.ToArray();
            encodedCookie.Value = Convert.ToBase64String(byteArr);
            HttpContext.Current.Response.Cookies.Add(encodedCookie);
        }
        */
        // 쿠키에서 객체를 역직렬화하여 반환

        /*
        public object DeserializeFromCookie(string key)
        {
            HttpCookie encodedCookie = HttpContext.Current.Request.Cookies[key];
            MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(encodedCookie.Value));
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            return binaryFormatter.Deserialize(memoryStream);
        }
        */
    }
}
