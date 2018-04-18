using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MuParserSharp.Tests
{
    [TestClass]
    public class EquationTests
    {
        [TestMethod]
        [DataRow("1e1234", EErrorCodes.ecUNASSIGNABLE_TOKEN)]
        [DataRow("-1e1234", EErrorCodes.ecUNASSIGNABLE_TOKEN)]
        public void test_error_detection(string s, EErrorCodes e) => Tester.ThrowTest(s, e);



        [TestMethod]
        [DataRow("-2--8", 6.0, true)]
        [DataRow("2*(a=9)*3", 54.0, true)]
        [DataRow("10*strlen(toupper(\"12345\"))", 50.0, true)]
        public void test_misc_expressions(string s1, dynamic v1, bool t) => Tester.EqnTest(s1, v1, t);

        [TestMethod]
        [DataRow("0xff", 255.0, true)]
        [DataRow("10+0xff", 265.0, true)]
        [DataRow("0xff+10", 265.0, true)]
        [DataRow("10*0xff", 2550.0, true)]
        [DataRow("0xff*10", 2550.0, true)]
        [DataRow("10+0xff+1", 266.0, true)]
        [DataRow("1+0xff+10", 266.0, true)]
        public void test_hex_expressions(string s1, dynamic v1, bool t) => Tester.EqnTest(s1, v1, t);

        [TestMethod]
        [DataRow("exp(ln(7))", 7.0, true)]
        [DataRow("e^ln(7)", 7.0, true)]
        [DataRow("e^(ln(7))", 7.0, true)]
        [DataRow("(e^(ln(7)))", 7.0, true)]
        [DataRow("1-(e^(ln(7)))", -6.0, true)]
        [DataRow("2*(e^(ln(7)))", 14.0, true)]
        [DataRow("10^log10(5)", 5.0, true)]
        [DataRow("10^log10(5)", 5.0, true)]
        [DataRow("2^log2(4)", 4.0, true)]
        [DataRow("-(sin(0)+1)", -1.0, true)]
        [DataRow("-(2^1.1)", -2.14354692, true)]
        public void test_log_exp_expressions(string s1, dynamic v1, bool t) => Tester.EqnTest(s1, v1, t);


        [TestMethod]
        [DataRow("-sin(8)m*6", -0.00593614947974029, true)]
        [DataRow("-sin(8)m/6", -0.000164893041103897, true)]
        [DataRow("-sin(8)m+6", 5.99901064175338, true)]
        [DataRow("-sin(8)m-6", -6.00098935824662, true)]
        [DataRow("(cos(2.41)/b)", -0.372055682695796, true)]
        public void test_trig_expressions(string s1, dynamic v1, bool t) => Tester.EqnTest(s1, v1, t);

        [TestMethod]
        // long formulas (Reference: Matlab)
        [DataRow(
            "(((-9))-e/(((((((pi-(((-7)+(-3)/4/e))))/(((-5))-2)-((pi+(-0))*(sqrt((e+e))*(-8))*(((-pi)+(-pi)-(-9)*(6*5))"+
            "/(-e)-e))/2)/((((sqrt(2/(-e)+6)-(4-2))+((5/(-2))/(1*(-pi)+3))/8)*pi*((pi/((-2)/(-6)*1*(-1))*(-6)+(-e)))))/"+
            "((e+(-2)+(-e)*((((-3)*9+(-e)))+(-9)))))))-((((e-7+(((5/pi-(3/1+pi)))))/e)/(-5))/(sqrt((((((1+(-7))))+((((-"+
            "e)*(-e)))-8))*(-5)/((-e)))*(-6)-((((((-2)-(-9)-(-e)-1)/3))))/(sqrt((8+(e-((-6))+(9*(-9))))*(((3+2-8))*(7+6"+
            "+(-5))+((0/(-e)*(-pi))+7)))+(((((-e)/e/e)+((-6)*5)*e+(3+(-5)/pi))))+pi))/sqrt((((9))+((((pi))-8+2))+pi))/e"+
            "*4)*((-5)/(((-pi))*(sqrt(e)))))-(((((((-e)*(e)-pi))/4+(pi)*(-9)))))))+(-pi)", -12.23016549, true)]
        [DataRow("1+2-3*4.0/5^6*(2*(1-5+(3*7^9)*(4+6*7-3)))+12", -7995810.099264, true)]
       // [DataRow("(atan(sin((((((((((((((((pi/cos((a/((((0.53-b)-pi)*e)/b))))+2.51)+a)-0.54)/0.98)+b)*b)+e)/a)+b)+a)+b)+pi)/e)+a)))*2.77)", -2.16995656, true)]
        public void test_long_expressions(string s1, dynamic v1, bool t) => Tester.EqnTest(s1, v1, t);

    }
}