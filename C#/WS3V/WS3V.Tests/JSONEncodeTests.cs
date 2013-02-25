using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WS3V.JSON;

namespace WS3V.Tests
{
    [TestClass]
    public class JSONEncodeTests
    {
        [TestMethod]
        public void DecodeJSONArray()
        {
            string input = "[3,\"gew8u3rethex\",1,\"Example 0.9.6\",[30,60,true],[1,true,true,true,\"250K\",\"50M\"],true,true,120,{\"api_day_quota\": 1000,\"api_day_reset\": 41268740,\"api_day_used\": 153,\"api_hour_quota\": 100,\"api_hour_reset\": 41267360,\"api_hour_used\": 15,\"sample_feature_1\": true,\"sample_feature_2\": false}]";
            string[] result = JSONDecoders.DecodeJSONArray(input);

            Assert.AreEqual(result[0], "3");
            Assert.AreEqual(result[1], "gew8u3rethex");
            Assert.AreEqual(result[2], "1");
            Assert.AreEqual(result[3], "Example 0.9.6");
            Assert.AreEqual(result[4], "[30,60,true]");
            Assert.AreEqual(result[5], "[1,true,true,true,\"250K\",\"50M\"]");
            Assert.AreEqual(result[6], "true");
            Assert.AreEqual(result[7], "true");
            Assert.AreEqual(result[8], "120");
            Assert.AreEqual(result[9], "{\"api_day_quota\": 1000,\"api_day_reset\": 41268740,\"api_day_used\": 153,\"api_hour_quota\": 100,\"api_hour_reset\": 41267360,\"api_hour_used\": 15,\"sample_feature_1\": true,\"sample_feature_2\": false}");
        }

        [TestMethod]
        public void DecodeJsStringArray()
        {
            string input = "[\"philcollins\",\"Ih8PeterG\"]";
            string[] result = JSONDecoders.DecodeJsStringArray(input);

            Assert.AreEqual(result[0], "philcollins");
            Assert.AreEqual(result[1], "Ih8PeterG");
        }

        [TestMethod]
        public void EncodeJsString()
        {
            string result = JSONEncoders.EncodeJsString("<b>sco</b>");
            Assert.AreEqual(result, "\"\\u003Cb\\u003Esco\\u003C\\/b\\u003E\"");
        }
    }
}
