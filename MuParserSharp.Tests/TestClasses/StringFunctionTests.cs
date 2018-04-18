using Microsoft.VisualStudio.TestTools.UnitTesting;
using MuParserSharp.Parser;

namespace MuParserSharp.Tests
{
    [TestClass]
    public class StringFunctionTests
    {
        [TestMethod]
        [DataRow("\"\\\"quoted_string\\\"\"", "\"quoted_string\"")] // "\"quoted_string\"" -> "quoted_string"
        [DataRow("\"\\\"\\\"\"", "\"\"")] // "\"\""              -> ""
        [DataRow("\"\\\\\"", "\\")] // "\\"                -> \     (single backslash)
        public void test_escape_sequences(string s1, string s2) => Tester.EqnTest(s1, s2, true);

        [TestMethod]
        [DataRow("strlen(\"12345\")", 5.0)]
        [DataRow("strlen(toupper(\"abcde\"))", 5.0)]
        [DataRow("sin(0)+(float)strlen(\"12345\")", 5.0)]
        [DataRow("10*(float)strlen(toupper(\"12345\"))", 50.0)]
        [DataRow("\"hello \"//\"world\"", "hello world")]
        [DataRow("toupper(\"hello \")//\"world\"", "HELLO world")]
        [DataRow("\"hello \"//toupper(\"world\")//\" !!!\"", "hello WORLD !!!")]
        public void test_string_functions(string s1, dynamic v1) => Tester.EqnTest(s1, v1, true);

        [TestMethod]
        public void test_string_split_join()
        {
            const string name = "Jimmy";
            const string splitname = "J-i-m-m-y";
            var p = new ParserX();
            p.DefineVar(nameof(name), new Variable(name));

            p.SetExpr("split(name)");
            var res = p.Eval();
            Assert.IsTrue(res.IsMatrix());
            Matrix m = res.GetArray();
            for (var i = 0; i < name.Length; i++)
            {
                var c = name[i];
                Assert.AreEqual(c, m.At(i));
            }
            p.DefineVar("list", new Variable(m));
            p.SetExpr("join(list, \"-\")");
            res = p.Eval();
            Assert.IsTrue(res.IsString());
            Assert.AreEqual(res.GetString(), splitname);
        }
    }
}