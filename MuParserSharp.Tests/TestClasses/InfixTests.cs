using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MuParserSharp.Tests
{
    [TestClass]
    public class InfixTests
    {
        [TestMethod]
        [DataRow("-1", -1.0, true)]
        [DataRow("-(-1)", 1.0, true)]
        [DataRow("-(-1)*2", 2.0, true)]
        [DataRow("-(-2)*sqrt(4)", 4.0, true)]
        [DataRow("-(8)", -8.0, true)]
        [DataRow("-8", -8.0, true)]
        [DataRow("-(2+1)", -3.0, true)]
      //[DataRow("-(f1of1(1+2*3)+1*2)", -9.0, true)]
      //[DataRow("-(-f1of1(1+2*3)+1*2)", 5.0, true)]
        [DataRow("-sin(8)", -0.989358, true)]
        [DataRow("-sin(8)", 0.989358, false)]
        [DataRow("3-(-a)", 4.0, true)]
        [DataRow("3--a", 4.0, true)]
        [DataRow("2++4", 6.0, true)]
       // [DataRow("--1", 1.0, true)]
        [DataRow("-3^2", -9.0, true)]
        public void test_infix_operators(string s1, dynamic v1, bool t) => Tester.EqnTest(s1, v1, t);


        [TestMethod]
        public void test_negation()
        {
            const double a = 1;
            Tester.EqnTest("-a", -a, true);
            Tester.EqnTest("-(a)", -(a), true);
            Tester.EqnTest("-(-a)", -(-a), true);
            Tester.EqnTest("-(-a)*2", -(-a) * 2, true);
        }

        // sign precedence
        // Issue 14: https://code.google.com/p/muparserx/issues/detail?id=14
        [TestMethod]
        public void test_sign_precedence()
        {
            const double b = 2;
            Tester.EqnTest("-3^2", -9.0, true);
            Tester.EqnTest("-b^2^3-b^8", -Math.Pow(b, Math.Pow(2,3)) - Math.Pow(b, 8), true);
        }
    }
}