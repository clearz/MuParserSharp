using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MuParserSharp.Tests
{
    [TestClass]
    public class IfElseTests
    {

        [TestMethod]
        [DataRow(": 2", EErrorCodes.ecMISPLACED_COLON)]
        [DataRow("? 1 : 2", EErrorCodes.ecUNEXPECTED_CONDITIONAL)]
        [DataRow("(a<b) ? (b<c) ? 1 : 2", EErrorCodes.ecMISSING_ELSE_CLAUSE)]
        [DataRow("(a<b) ? 1", EErrorCodes.ecMISSING_ELSE_CLAUSE)]
        [DataRow("(a<b) ? a", EErrorCodes.ecMISSING_ELSE_CLAUSE)]
        [DataRow("(a<b) ? a+b", EErrorCodes.ecMISSING_ELSE_CLAUSE)]
        [DataRow("(true) ? 1 : 2 : 3", EErrorCodes.ecMISPLACED_COLON)]
        [DataRow("1==?", EErrorCodes.ecUNEXPECTED_CONDITIONAL)]
        [DataRow("1+?", EErrorCodes.ecUNEXPECTED_CONDITIONAL)]  // bin oprt + ?
        [DataRow("1m?", EErrorCodes.ecUNEXPECTED_CONDITIONAL)]  // postfix + ?
        [DataRow("-?", EErrorCodes.ecUNEXPECTED_CONDITIONAL)]  // infix + ?
        public void test_error_detection(string s, EErrorCodes e) => Tester.ThrowTest(s, e);

        [TestMethod]
        [DataRow("a : b", EErrorCodes.ecMISPLACED_COLON, 2)]
        [DataRow("1 : 2", EErrorCodes.ecMISPLACED_COLON, 2)]
        public void test_error_detection(string s, EErrorCodes e, int p) => Tester.ThrowTest(s, e, p);

        [TestMethod]
        [DataRow("(true) ? 128 : 255", 128.0, true)]
        [DataRow("(1<2) ? 128 : 255", 128.0, true)]
        [DataRow("(a<b) ? 128 : 255", 128.0, true)]
        [DataRow("((a>b) ? true : false) ? 1 : 2", 2.0, true)]
        [DataRow("((a>b) ? true : false) ? 1 : sum((a>b) ? 1 : 2)", 2.0, true)]
        [DataRow("((a>b) ? false : true) ? 1 : sum((a>b) ? 1 : 2)", 1.0, true)]
        [DataRow("(true) ? 10 : 11", 10.0, true)]
        [DataRow("(true) ? a+b : c+d", 3.0, true)]
        [DataRow("(true) ? false : true", false, true)]
        [DataRow("(false) ? 10 : 11", 11.0, true)]
        [DataRow("(false) ? a+b : c+d", 1.0, true)]
        [DataRow("(false) ? false : true", true, true)]
        [DataRow("(a<b) ? 10 : 11", 10.0, true)]
        [DataRow("(a>b) ? 10 : 11", 11.0, true)]
        [DataRow("(a<b) ? c : d", 3.0, true)]
        [DataRow("(a>b) ? c : d", -2.0, true)]
        [DataRow("(a>b) ? true : false", false, true)]
        public void test_basic_ifelse(string s1, dynamic v1, bool t) => Tester.EqnTest(s1, v1, t);

        [TestMethod]
        [DataRow("sum((a>b) ? 1 : 2)", 2.0, true)]
        [DataRow("sum((a>b) ? 1 : 2, 100)", 102.0, true)]
        [DataRow("sum((true) ? 1 : 2)", 1.0, true)]
        [DataRow("sum((true) ? 1 : 2, 100)", 101.0, true)]
        [DataRow("sum(3, (a>b) ? 3 : 10)", 13.0, true)]
        [DataRow("sum(3, (a<b) ? 3 : 10)", 6.0, true)]
        [DataRow("sum(3, (a>b) ? 3 : 10)*10", 130.0, true)]
        [DataRow("sum(3, (a<b) ? 3 : 10)*10", 60.0, true)]
        [DataRow("10*sum(3, (a>b) ? 3 : 10)", 130.0, true)]
        [DataRow("10*sum(3, (a<b) ? 3 : 10)", 60.0, true)]
        [DataRow("(a<b) ? sum(3, (a<b) ? 3 : 10)*10 : 99", 60.0, true)]
        [DataRow("(a>b) ? sum(3, (a<b) ? 3 : 10)*10 : 99", 99.0, true)]
        [DataRow("(a<b) ? sum(3, (a<b) ? 3 : 10,10,20)*10 : 99", 360.0, true)]
        [DataRow("(a>b) ? sum(3, (a<b) ? 3 : 10,10,20)*10 : 99", 99.0, true)]
        [DataRow("(a>b) ? sum(3, (a<b) ? 3 : 10,10,20)*10 : sum(3, (a<b) ? 3 : 10)*10", 60.0, true)]
        public void test_multiarg_functions(string s1, dynamic v1, bool t) => Tester.EqnTest(s1, v1, t);

        [TestMethod]
        [DataRow("(a<b)&&(a<b) ? 128 : 255", 128.0, true)]
        [DataRow("(a>b)&&(a<b) ? 128 : 255", 255.0, true)]
        [DataRow("(1<2)&&(1<2) ? 128 : 255", 128.0, true)]
        [DataRow("(1>2)&&(1<2) ? 128 : 255", 255.0, true)]
        [DataRow("((1<2)&&(1<2)) ? 128 : 255", 128.0, true)]
        [DataRow("((1>2)&&(1<2)) ? 128 : 255", 255.0, true)]
        [DataRow("((a<b)&&(a<b)) ? 128 : 255", 128.0, true)]
        [DataRow("((a>b)&&(a<b)) ? 128 : 255", 255.0, true)]
        public void test_multiple_logic(string s1, dynamic v1, bool t) => Tester.EqnTest(s1, v1, t);

        [TestMethod]
        [DataRow("(a<b) ? ((b<c) ? 2*(a+b) : 2) : 3", 6.0, true)]
        [DataRow("(a<b) ? 3 : ((b<c) ? 2*(a+b) : 2)", 3.0, true)]
        [DataRow("(a<b) ? ((b>c) ? 1 : 2) : 3", 2.0, true)]
        [DataRow("(a>b) ? ((b<c) ? 1 : 2) : 3", 3.0, true)]
        [DataRow("(a>b) ? ((b>c) ? 1 : 2) : 3", 3.0, true)]
        public void test_conditional_with_brackets(string s1, dynamic v1, bool t) => Tester.EqnTest(s1, v1, t);

        [TestMethod]
        [DataRow("(a<b) ? (b<c) ? 1 : 2 : 3", 1.0, true)]
        [DataRow("(a<b) ? (b>c) ? 1 : 2 : 3", 2.0, true)]
        [DataRow("(a>b) ? (b<c) ? 1 : 2 : 3", 3.0, true)]
        [DataRow("(a>b) ? (b>c) ? 1 : 2 : 3", 3.0, true)]
        public void test_conditional_without_brackets(string s1, dynamic v1, bool t) => Tester.EqnTest(s1, v1, t);

        [TestMethod]
        [DataRow("(a<b)&&(a<b) ? 128 : 255", 128.0, true)]
        [DataRow("(a>b)&&(a<b) ? 128 : 255", 255.0, true)]
        [DataRow("(1<2)&&(1<2) ? 128 : 255", 128.0, true)]
        [DataRow("(1>2)&&(1<2) ? 128 : 255", 255.0, true)]
        [DataRow("((1<2)&&(1<2)) ? 128 : 255", 128.0, true)]
        [DataRow("((1>2)&&(1<2)) ? 128 : 255", 255.0, true)]
        [DataRow("((a<b)&&(a<b)) ? 128 : 255", 128.0, true)]
        [DataRow("((a>b)&&(a<b)) ? 128 : 255", 255.0, true)]
        public void test_new_tests(string s1, dynamic v1, bool t) => Tester.EqnTest(s1, v1, t);

        [TestMethod]
        [DataRow("1>0 ? 1>2 ? 128 : 255 : 1>0 ? 32 : 64", 255.0, true)]
        [DataRow("1>0 ? 1>2 ? 128 : 255 :(1>0 ? 32 : 64)", 255.0, true)]
        [DataRow("1>0 ? 1>0 ? 128 : 255 : 1>2 ? 32 : 64", 128.0, true)]
        [DataRow("1>0 ? 1>0 ? 128 : 255 :(1>2 ? 32 : 64)", 128.0, true)]
        [DataRow("1>2 ? 1>2 ? 128 : 255 : 1>0 ? 32 : 64", 32.0, true)]
        [DataRow("1>2 ? 1>0 ? 128 : 255 : 1>2 ? 32 : 64", 64.0, true)]
        [DataRow("1>0 ? 50 :  1>0 ? 128 : 255", 50.0, true)]
        [DataRow("1>0 ? 50 : (1>0 ? 128 : 255)", 50.0, true)]
        [DataRow("1>0 ? 1>0 ? 128 : 255 : 50", 128.0, true)]
        [DataRow("1>2 ? 1>2 ? 128 : 255 : 1>0 ? 32 : 1>2 ? 64 : 16", 32.0, true)]
        [DataRow("1>2 ? 1>2 ? 128 : 255 : 1>0 ? 32 :(1>2 ? 64 : 16)", 32.0, true)]
        [DataRow("1>0 ? 1>2 ? 128 : 255 :  1>0 ? 32 :1>2 ? 64 : 16", 255.0, true)]
        [DataRow("1>0 ? 1>2 ? 128 : 255 : (1>0 ? 32 :1>2 ? 64 : 16)", 255.0, true)]
        [DataRow("true ? false ? 128 : 255 : true ? 32 : 64", 255.0, true)]
        public void test_embedded_statements(string s1, dynamic v1, bool t) => Tester.EqnTest(s1, v1, t);

        [TestMethod]
        [DataRow("a= false ? 128 : 255", 255.0, true)]
        [DataRow("a=((a>b)&&(a<b)) ? 128 : 255", 255.0, true)]
        [DataRow("c=(a<b)&&(a<b) ? 128 : 255", 128.0, true)]
        [DataRow("a=true?b=true?3:4:5", 3.0, true)]
        [DataRow("a=false?b=true?3:4:5", 5.0, true)]
        [DataRow("a=true?5:b=true?3:4", 5.0, true)]
        [DataRow("a=false?5:b=true?3:4", 3.0, true)]
        public void test_assignment_operators(string s1, dynamic v1, bool t) => Tester.EqnTest(s1, v1, t);

        [TestMethod]
        public void test_issue42()
        {
            double a = 1;
            Tester.EqnTest("abs(0.1) < 0.25 ? (-1) : (1) + 1", Math.Abs(0.1) < 0.25 ? (-1.0) : (1.0) + 1, true);
            Tester.EqnTest("abs(a) < 0.25 ? (-1) : (1) + 1", Math.Abs(a) < 0.25 ? (-1.0) : (1.0) + 1, true);
            Tester.EqnTest("(abs(a) < 0.25 ? -1 : 1)", Math.Abs(a) < 0.25 ? -1.0 : 1.0, true);
        }
    }
}