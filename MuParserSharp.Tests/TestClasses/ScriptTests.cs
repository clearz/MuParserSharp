using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MuParserSharp.Tests
{
    [TestClass]
    public class ScriptTests
    {
        [TestMethod]
        [DataRow("sin(\n", EErrorCodes.ecUNEXPECTED_NEWLINE)]
        [DataRow("1+\n", EErrorCodes.ecUNEXPECTED_NEWLINE)]
        [DataRow("a*\n", EErrorCodes.ecUNEXPECTED_NEWLINE)]
        [DataRow("va[\n", EErrorCodes.ecUNEXPECTED_NEWLINE)]
        [DataRow("(true) ? \n", EErrorCodes.ecUNEXPECTED_NEWLINE)]
        [DataRow("(true) ? 10:\n", EErrorCodes.ecUNEXPECTED_NEWLINE)]
        public void test_error_detection(string s, EErrorCodes e) => Tester.ThrowTest(s, e);

        // Expressions spanning multiple lines
        [TestMethod]
        [DataRow("a=1\nb=2\nc=3\na+b+c\n", 6.0, true)]
        public void test_expressions_spaning_multiple_lines(string s1, dynamic v1, bool t) => Tester.EqnTest(s1, v1, t);

        // Ending an expression with a newline
        [TestMethod]
        [DataRow("3\n", 3.0, true)]
        [DataRow("1+2\n", 3.0, true)]
        [DataRow("\n1+2\n", 3.0, true)]
        [DataRow("\n1+2\n\na+b", 3.0, true)]
        public void test_expression_with_new_line(string s1, dynamic v1, bool t) => Tester.EqnTest(s1, v1, t);
    }
}