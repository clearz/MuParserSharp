using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MuParserSharp.Tests
{
    [TestClass]
    public class MultiArgTests
    {
        //ones function test
        [TestMethod]
        [DataRow("min()", EErrorCodes.ecTOO_FEW_PARAMS)]
        [DataRow("max()", EErrorCodes.ecTOO_FEW_PARAMS)]
        [DataRow("sum()", EErrorCodes.ecTOO_FEW_PARAMS)]
        public void test_missing_params(string s, EErrorCodes e) => Tester.ThrowTest(s, e);

        // application
        [TestMethod]
        [DataRow("max(1,8,9,(int)6)", 9.0, true)]
        [DataRow("max((int)6, 1+2, 4, -9)", 6.0, true)]
        [DataRow("min((int)6, 1+2, 4, -9)", -9.0, true)]
        [DataRow("a=test0()", 0, true)]
        [DataRow("b=a+test0()", 1, true)]
        public void test_application(string s1, dynamic v1, bool t) => Tester.EqnTest(s1, v1, t);



        // added as response to this bugreport:
        // http://code.google.com/p/muparserx/issues/detail?id=1
        // cause of the error: Function tokens were not cloned in the tokenreader when beeing found.
        //                     a pointer to the one and only function onject was returned instead
        //                     consequently the argument counter was overwritten by the second function call
        //                     causing an assertion later on.
        [TestMethod]
        [DataRow("sum(1,2)/sum(3,4)", 0.428571, true)]
        [DataRow("3/sum(3,4,5)", 0.25, true)]
        [DataRow("sum(3)/sum(3,4,5)", 0.25, true)]
        [DataRow("sum(3)+sum(3,4,5)", 15, true)]
        [DataRow("sum(1,2)/sum(3,4,5)", 0.25, true)]
        public void test_bug_report(string s1, dynamic v1, bool t) => Tester.EqnTest(s1, v1, t);
    }
}