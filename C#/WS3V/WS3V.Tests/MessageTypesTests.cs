using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WS3V.MessageTypes;
using WS3V.JSON;
using System.Text;

namespace WS3V.Tests
{
    [TestClass]
    public class MessageTypesTests
    {
        [TestMethod]
        public void gatekeeper()
        {
            gatekeeper g = new gatekeeper("api_key", 5, 403, "example.com websocket api requires an api key", "http://example.com/api/error#403");
            string expected = "[1,[\"api_key\"],5,403,\"example.com websocket api requires an api key\",\"http:\\/\\/example.com\\/api\\/error#403\"]";
            Assert.AreEqual(g.ToString(), expected);
        }

        [TestMethod]
        public void signature()
        {
            string input = "[2,[\"98eac98feeaf8e25410ce135076d688a\"]]";
            string[] message = JSONDecoders.DecodeJSONArray(input);
            Assert.AreEqual(message[0], "2");

            signature s = new signature(message);
            Assert.AreEqual(s.credentials[0], "98eac98feeaf8e25410ce135076d688a");

            input = "[2,[\"philcollins\",\"Ih8PeterG\"]]";
            message = JSONDecoders.DecodeJSONArray(input);
            Assert.AreEqual(message[0], "2");

            s = new signature(message);
            Assert.AreEqual(s.credentials[0], "philcollins");
            Assert.AreEqual(s.credentials[1], "Ih8PeterG");
        }

        [TestMethod]
        public void howdy()
        {
            string session_id = Guid.NewGuid().ToString();

            // test default howdy
            howdy h = new howdy(session_id);
            string expected = "[3,\"" + session_id + "\",1,\"\",[-1,-1,false],[0,false,false,false,\"0\",\"0\"],false,false,0]";
            string result = h.ToString();
            Assert.AreEqual(result, expected);

            // test documentation howdy
            h = new howdy(session_id, "Example 0.9.6", 30, 60, true, 1, 120);

            h.filetransfer.allow_pausing = true;
            h.filetransfer.force_chunk_integrity = true;
            h.filetransfer.force_file_integrity = true;
            h.filetransfer.max_chunk_size = "250K";
            h.filetransfer.max_file_size = "50M";

            h.recovery = true;
            h.channel_listing = true;

            fake_headers headers = new fake_headers();
            h.headers = headers.ToString();

            expected = "[3,\"" + session_id + "\",1,\"Example 0.9.6\",[30,60,true],[1,true,true,true,\"250K\",\"50M\"],true,true,120,{\"api_day_quota\":1000,\"api_day_reset\":41268740,\"api_day_used\":153,\"api_hour_quota\":100,\"api_hour_reset\":41267360,\"api_hour_used\":15,\"sample_feature_1\":true,\"sample_feature_2\":false}]";
            result = h.ToString();
            Assert.AreEqual(result, expected);

            // check the shorthand file sizes
            h.filetransfer.max_chunk_size = "250";
            Assert.AreEqual(h.filetransfer.max_chunk_size_real, 250);

            h.filetransfer.max_chunk_size = "250K";
            Assert.AreEqual(h.filetransfer.max_chunk_size_real, 256000);

            h.filetransfer.max_chunk_size = "250M";
            Assert.AreEqual(h.filetransfer.max_chunk_size_real, 262144000);

            h.filetransfer.max_chunk_size = "250G";
            Assert.AreEqual(h.filetransfer.max_chunk_size_real, 268435456000);

            h.filetransfer.max_chunk_size = "250T";
            Assert.AreEqual(h.filetransfer.max_chunk_size_real, 274877906944000);
        }

        [TestMethod]
        public void recover()
        {
            string input = "[4,\"druspuzef2ze\"]";
            string[] message = JSONDecoders.DecodeJSONArray(input);
            Assert.AreEqual(message[0], "4");

            recover r = new recover(message);
            Assert.AreEqual(r.session_id, "druspuzef2ze");
        }

        [TestMethod]
        public void send()
        {
            // example 1
            string input = "[5,\"dres2nec9ute\",\"GET\",\"\\/music\\/top100\",\"artist=Phil%20Collins&sortby=year\"]";
            string[] message = JSONDecoders.DecodeJSONArray(input);
            Assert.AreEqual(message[0], "5");

            send s = new send(message);
            Assert.AreEqual(s.message_id, "dres2nec9ute");
            Assert.AreEqual(s.method, "GET");
            Assert.AreEqual(s.uri, "/music/top100");
            Assert.AreEqual(s.parameters, "artist=Phil%20Collins&sortby=year");

            // example 2
            input = "[5,\"chuprer2frap\",\"DELETE\",\"\\/user\\/favorite\\/artists\",\"Peter Gabriel\"]";
            message = JSONDecoders.DecodeJSONArray(input);
            Assert.AreEqual(message[0], "5");

            s = new send(message);
            Assert.AreEqual(s.message_id, "chuprer2frap");
            Assert.AreEqual(s.method, "DELETE");
            Assert.AreEqual(s.uri, "/user/favorite/artists");
            Assert.AreEqual(s.parameters, "Peter Gabriel");
        }

        [TestMethod]
        public void receive()
        {
            // documentation example 1
            fake_headers_short headers = new fake_headers_short();
            receive r = new receive("swezataf3as5", JSONEncoders.EncodeJsString("definitly slightly positive"), headers.ToString());
            string expected = "[6,\"swezataf3as5\",\"definitly slightly positive\",{\"api_day_quota\":1000,\"api_day_reset\":41268740}]";
            string result = r.ToString();
            Assert.AreEqual(result, expected);

            // documentation example 2
            demo_object obj = new demo_object();
            r = new receive("ruvabras3ade", obj.ToString());
            expected = "[6,\"ruvabras3ade\",{\"deploy\":true,\"location\":2,\"changes\":392}]";
            result = r.ToString();
            Assert.AreEqual(result, expected);
        }

        [TestMethod]
        public void error()
        {
            // documentation example 1
            fake_headers_short headers = new fake_headers_short();
            error e = new error("8hewrafrey2z", 404, "method not found", "http://example.com/api/error#404", headers.ToString());
            string expected = "[7,\"8hewrafrey2z\",404,\"method not found\",\"http:\\/\\/example.com\\/api\\/error#404\",{\"api_day_quota\":1000,\"api_day_reset\":41268740}]";
            string result = e.ToString();
            Assert.AreEqual(result, expected);
        }

        [TestMethod]
        public void farewell()
        {
            // example 1
            string input = "[25]";
            string[] message = JSONDecoders.DecodeJSONArray(input);
            Assert.AreEqual(message[0], "25");
        }

        [TestMethod]
        public void terminated()
        {
            // documentation example 1
            terminated t = new terminated(200, "goodbye");
            string expected = "[26,200,\"goodbye\"]";
            string result = t.ToString();
            Assert.AreEqual(result, expected);

            // documentation example 2
            t = new terminated(403, "forbidden, access denied", "http://example.com/api/error#403");
            expected = "[26,403,\"forbidden, access denied\",\"http:\\/\\/example.com\\/api\\/error#403\"]";
            result = t.ToString();
            Assert.AreEqual(result, expected);
        }
    }


    public class fake_headers
    {
        public int api_day_quota = 1000;
        public int api_day_reset = 41268740;
        public int api_day_used = 153;
        public int api_hour_quota = 100;
        public int api_hour_reset = 41267360;
        public int api_hour_used = 15;
        public bool sample_feature_1 = true;
        public bool sample_feature_2 = false;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('{');
            sb.Append(JSONEncoders.EncodeJsString("api_day_quota"));
            sb.Append(':');
            sb.Append(api_day_quota);
            sb.Append(',');
            sb.Append(JSONEncoders.EncodeJsString("api_day_reset"));
            sb.Append(':');
            sb.Append(api_day_reset);
            sb.Append(',');
            sb.Append(JSONEncoders.EncodeJsString("api_day_used"));
            sb.Append(':');
            sb.Append(api_day_used);
            sb.Append(',');
            sb.Append(JSONEncoders.EncodeJsString("api_hour_quota"));
            sb.Append(':');
            sb.Append(api_hour_quota);
            sb.Append(',');
            sb.Append(JSONEncoders.EncodeJsString("api_hour_reset"));
            sb.Append(':');
            sb.Append(api_hour_reset);
            sb.Append(',');
            sb.Append(JSONEncoders.EncodeJsString("api_hour_used"));
            sb.Append(':');
            sb.Append(api_hour_used);
            sb.Append(',');
            sb.Append(JSONEncoders.EncodeJsString("sample_feature_1"));
            sb.Append(':');
            sb.Append((sample_feature_1) ? "true" : "false");
            sb.Append(',');
            sb.Append(JSONEncoders.EncodeJsString("sample_feature_2"));
            sb.Append(':');
            sb.Append((sample_feature_2) ? "true" : "false");
            sb.Append('}');

            return sb.ToString();
        }
    }

    public class fake_headers_short
    {
        public int api_day_quota = 1000;
        public int api_day_reset = 41268740;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('{');
            sb.Append(JSONEncoders.EncodeJsString("api_day_quota"));
            sb.Append(':');
            sb.Append(api_day_quota);
            sb.Append(',');
            sb.Append(JSONEncoders.EncodeJsString("api_day_reset"));
            sb.Append(':');
            sb.Append(api_day_reset);
            sb.Append('}');

            return sb.ToString();
        }
    }

    public class demo_object
    {
        public bool deploy = true;
        public int location = 2;
        public int changes = 392;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('{');
            sb.Append(JSONEncoders.EncodeJsString("deploy"));
            sb.Append(':');
            sb.Append((deploy) ? "true" : "false");
            sb.Append(',');
            sb.Append(JSONEncoders.EncodeJsString("location"));
            sb.Append(':');
            sb.Append(location);
            sb.Append(',');
            sb.Append(JSONEncoders.EncodeJsString("changes"));
            sb.Append(':');
            sb.Append(changes);
            sb.Append('}');

            return sb.ToString();
        }
    }
}
