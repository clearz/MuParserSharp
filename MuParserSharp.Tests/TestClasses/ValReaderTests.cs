using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MuParserSharp.Tests
{
    [TestClass]
    public class ValReaderTests
    {
        // Hex value reader
        [TestMethod]
        [DataRow("0x1", 1.0, true)]
        [DataRow("0x1+0x2", 3.0, true)]
        [DataRow("0xff", 255.0, true)]
        void test_hex_reader(string s1, dynamic v1, bool t) => Tester.EqnTest(s1, v1, t);

        // Reading of binary values
        [TestMethod]
        [DataRow("0b1", 1, true)]
        [DataRow("0b01", 1, true)]
        [DataRow("0b11", 3, true)]
        [DataRow("0b011", 3, true)]
        [DataRow("0b11111111", 255, true)]
        [DataRow("b*0b011", 6, true)]
        [DataRow("0b0111111111111111111111111111111111111111111111111111111111111111", 9223372036854775807, true)]
        [DataRow("0b1000000000000000000000000000000000000000000000000000000000000000", -9223372036854775807 - 1, true)]
        [DataRow("0b1111111111111111111111111111111111111111111111111111111111111111", -1, true)]
        public void test_binary_reader(string s1, dynamic v1, bool t) => Tester.EqnTest(s1, v1, t);

        [TestMethod]
        public void test_boolean_throw_undefined()
        {
            Tester.ThrowTest("0b10000000000000000000000000000000000000000000000000000000000000001", EErrorCodes.ecCONVERSION_OVERFLOW);
        }

        // string value reader
        [TestMethod]
        [DataRow("\"hallo\"", "hallo", true)]
        public void test_string_reader(string s1, dynamic v1, bool t) => Tester.EqnTest(s1, v1, t);

        // boolean value reader
        [TestMethod]
        [DataRow("true", true, true)]
        [DataRow("false", false, true)]
        public void test_boolean_reader(string s1, dynamic v1, bool t) => Tester.EqnTest(s1, v1, t);

        // mixed
        [TestMethod]
        [DataRow("0b011+0xef", 242, true)]
        public void test_mix_hex_bool(string s1, dynamic v1, bool t) => Tester.EqnTest(s1, v1, t);
    }
}