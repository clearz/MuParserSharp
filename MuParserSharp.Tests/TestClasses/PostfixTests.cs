using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MuParserSharp.Tests
{
    [TestClass]
    public class PostfixTests
    {

        [TestMethod]
        [DataRow("(-5)!", EErrorCodes.ecDOMAIN_ERROR)]
        public void test_domain_error(string s, EErrorCodes e) => Tester.ThrowTest(s, e);

        // application
        [TestMethod]
        [DataRow("1n", 1e-9, true)]
        [DataRow("8n", 8e-9, true)]
        [DataRow("8n", 123.0, false)]
        [DataRow("3m+5", 5.003, true)]
        [DataRow("1000m", 1.0, true)]
        [DataRow("1000 m", 1.0, true)]
        [DataRow("(a)m", 1e-3, true)]
        [DataRow("-(a)m", -1e-3, true)]
        [DataRow("-2m", -2e-3, true)]
        [DataRow("a++b", 3.0, true)]
        [DataRow("a ++ b", 3.0, true)]
        [DataRow("1++2", 3.0, true)]
        [DataRow("1 ++ 2", 3.0, true)]
        [DataRow("2+(a*1000)m", 3.0, true)]
        [DataRow("1000m", 0.1, false)]
        [DataRow("(a)m", 2.0, false)]
        [DataRow("5!", 120.0, true)]
        [DataRow("-5!", -120.0, true)]
        public void test_postfix_basic(string s1, dynamic v1, bool t) => Tester.EqnTest(s1, v1, t);
    }
}