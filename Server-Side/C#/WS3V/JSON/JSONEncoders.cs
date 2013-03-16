using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WS3V.JSON
{
    public static class JSONEncoders
    {
        public static string EncodeJsStringArray(string[] s)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            if (s != null && s.Length > 0)
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (i > 0)
                        sb.Append(',');

                    sb.Append(EncodeJsString(s[i]));
                }
            }
            sb.Append(']');

            return sb.ToString();
        }

        public static string EncodeJsObjectArray(string[] s)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            if (s != null && s.Length > 0)
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (i > 0)
                        sb.Append(',');

                    sb.Append(s[i]);
                }
            }
            sb.Append(']');

            return sb.ToString();
        }


        /// Credit http://www.west-wind.com/weblog/posts/2007/Jul/14/Embedding-JavaScript-Strings-from-an-ASPNET-Page
        /// 
        /// Edited to add reverse solidus
        /// 
        /// <summary>
        /// Encodes a string to be represented as a string literal. The format
        /// is essentially a JSON string.
        /// 
        /// The string returned includes outer quotes 
        /// Example Output: "Hello \"Rick\"!\r\nRock on"
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        
        public static string EncodeJsString(string s)
        {
            //return HttpUtility.JavaScriptStringEncode(s, true);

            StringBuilder sb = new StringBuilder(s.Length);
            sb.Append("\"");
            foreach (char c in s)
            {
                switch (c)
                {
                    case '\"':
                        sb.Append("\\\"");
                        break;
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '/':
                        sb.Append("\\/");
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;

                    case '<':
                        sb.Append("\\u003C");
                        break;

                    case '>':
                        sb.Append("\\u003E");
                        break;

                    default:
                        int i = (int)c;
                        if (i < 32 || i > 127)
                        {
                            sb.AppendFormat("\\u{0:X04}", i);
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }
            sb.Append("\"");

            return sb.ToString();
        }
    }
}
