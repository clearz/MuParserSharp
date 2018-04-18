using Microsoft.VisualStudio.TestTools.UnitTesting;
using MuParserSharp.Parser;

namespace MuParserSharp.Tests
{
    [TestClass]
    public class UndefVariableTests
    {
        [TestMethod]
        public void test_detection_of_undefined_variables()
        {
            var p = new ParserX();
            p.SetExpr("a+b+c+d");
            var expr_var = p.GetExprVar();
            var var = p.GetVar();			
Assert.AreEqual(4, expr_var.Count);
            Assert.AreEqual(0, var.Count);

        }

        [TestMethod]
        public void test_detection_of_auto_created_variables()
        {
            var p = new ParserX();
            p.EnableAutoCreateVar(true);
            p.SetExpr("a+b+c+d");
            var expr_var = p.GetExprVar();
            var var = p.GetVar();			
Assert.AreEqual(4, expr_var.Count);			
Assert.AreEqual(4, var.Count);
        }

        [TestMethod]
        public void test_detection_of_variables()
        {
            var p = new ParserX();
            Value[] vVarVal = { 1.0, 2.0, 3.0, 4.0 };
            p.DefineVar("a", new Variable(vVarVal[0]));
            p.DefineVar("b", new Variable(vVarVal[1]));
            p.DefineVar("c", new Variable(vVarVal[2]));
            p.DefineVar("d", new Variable(vVarVal[3]));

            p.SetExpr("a+b+c+d");
            var expr_var = p.GetExprVar();
            var var = p.GetVar();			
Assert.AreEqual(4, expr_var.Count);			
Assert.AreEqual(4, var.Count);

        }
    }
}