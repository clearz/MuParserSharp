using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MuParserSharp.Framework;
using MuParserSharp.Parser;

namespace MuParserSharp.Tests
{
    [TestClass]
    public class IssueReportTests
    {
        // Issue 68 (and related issues):
        [TestMethod]
        [DataRow("(abs(-3)+2)>=min(6,5)", true, true)]
        [DataRow("(abs(-3))>abs(2)", true, true)]
        [DataRow("min(1,2,-3)>-4", true, true)]
        [DataRow("(abs(-3))>-2", true, true)]
        [DataRow("abs(-3)>abs(2)", true, true)]
        public void test_issue68(string s1, dynamic v1, bool t) => Tester.EqnTest(s1, v1, t);


        // Issue 16: http://code.google.com/p/muparserx/issues/detail?id=16
        [TestMethod]
        [DataRow("true  == true && false", true == true && false, true)]
        [DataRow("false == true && false", false == true && false, true)]
        public void test_issue16a(string s1, dynamic v1, bool t) => Tester.EqnTest(s1, v1, t);


        // Issue 16: http://code.google.com/p/muparserx/issues/detail?id=16
        [TestMethod]
        public void test_issue16b()
        {
            Value a = 1.0;
            Tester.EqnTest("a==1.0 && a==1.0", a == 1.0 && a == 1.0, true);
        }

        // Issue 25: http://code.google.com/p/muparserx/issues/detail?id=25
        [TestMethod]
        public void test_issue25()
        {
            Tester.ThrowTest("55<<2222222222", EErrorCodes.ecOVERFLOW); // Changed to longer number to overflow on 64bit
        }

        // Issue 42:
        // https://code.google.com/p/muparserx/issues/detail?id=42
        [TestMethod]
        public void test_issue42()
        {
            var v = new Value(3, 0);   
            
            v.At(0) = 1;
            v.At(1) = 0;
            v.At(2) = 0;
            Tester.EqnTest("{1,0,0}'", v, true);
            Tester.EqnTest("{(1),0,0}'", v, true);

        }



        // Issue 33: https://code.google.com/p/muparserx/issues/detail?id=33
        // Remark: Type information was not properly set when invoking +=, -= operators
        [TestMethod]
        public void test_issue33()
        {
            IValue x = 1.0;
            IValue y = new Complex(0, 1);
            x += y;
            Assert.AreEqual(x.GetImag(), 1);
            Assert.AreEqual(x.GetReal(), 1);
            Assert.AreEqual(x.GetValueType(), 'z');
            x = 1.0;
            y = new Complex(0, 1);
            x -= y;
            Assert.AreEqual(x.GetImag(), -1);
            Assert.AreEqual(x.GetReal(), 1);
            Assert.AreEqual(x.GetValueType(), 'z');
        }
    }
}