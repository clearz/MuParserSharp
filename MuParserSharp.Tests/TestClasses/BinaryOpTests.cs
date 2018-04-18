using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MuParserSharp.Tests
{
    [TestClass]
    public class BinaryOpTests
    {
        // standard aperators
        [TestMethod]
        [DataRow("1+7", 8.0, true)]
        [DataRow("10-1", 9.0, true)]
        [DataRow("3*4", 12.0, true)]
        [DataRow("10/2", 5.0, true)]
        public void test_standart_operators(string s1, dynamic v1, bool t) => Tester.EqnTest(s1, v1, t);

        // operator associativity
        [TestMethod]
        [DataRow("2^2^3", 256.0, true)]
        [DataRow("3+4*2/(float)(1-5)^2^3", 3.0001220703125, true)]
        [DataRow("1.0/2/3.0", 1.0 / 6.0, true)]
        public void test_operator_associativity(string s1, dynamic v1, bool t) => Tester.EqnTest(s1, v1, t);

        // operator precedencs
        [TestMethod]
        [DataRow("1+2-3*4/5.0^6", 2.99923, true)]
        [DataRow("a+b-c*4/5.0^6", 2.99923, true)]
        [DataRow("1^2/3.0*4-5+6", 2.3333, true)]
        [DataRow("a^b/(float)c*4.0-5+6", 2.3333, true)]
        [DataRow("1+2*3", 7.0, true)]
        [DataRow("a+b*c", 7.0, true)]
        [DataRow("(1+2)*3", 9.0, true)]
        [DataRow("(a+b)*c", 9.0, true)]
        [DataRow("(1+2)*(-3)", -9.0, true)]
        [DataRow("(a+b)*(-c)", -9.0, true)]
        [DataRow("2/4.0", 0.5, true)]
        [DataRow("4&4", 4.0, true)]
        [DataRow("2+2&(a+b+c)", 4.0, true)]
        [DataRow("3&3", 3.0, true)]
        [DataRow("c&3", 3.0, true)]
        [DataRow("(c)&3", 3.0, true)]
        [DataRow("(a+b)&3", 3.0, true)]
        [DataRow("(a+b+c)&6", 6.0, true)]
        [DataRow("(1+2+3)&6", 6.0, true)]
        [DataRow("3&c", 3.0, true)]
        [DataRow("(a<<1)+2", 4.0, true)]
        [DataRow("(a<<2)+2", 6.0, true)]
        [DataRow("(a<<3)+2", 10.0, true)]
        [DataRow("(a<<4)+2", 18.0, true)]
        [DataRow("(a<<5)+2", 34.0, true)]
        [DataRow("1<<31", 2147483648, true)]
        [DataRow("-1<<31", -2147483648, true)]
        [DataRow("1<<45", 35184372088832, true)]
        [DataRow("-1<<45", -35184372088832, true)]
        [DataRow("8<<-2", 0, true)] // Cant shift by negative numbers in C#
        [DataRow("8<<-4", -9223372036854775808, true)] // Shifting 64bit number with a neg number will always strange answers
        public void test_operator_precedencs(string s1, dynamic v1, bool t) => Tester.EqnTest(s1, v1, t);


        // bool operators for comparing values
        [TestMethod]
        [DataRow("a<b", true, true)]
        [DataRow("b>a", true, true)]
        [DataRow("a>a", false, true)]
        [DataRow("a<a", false, true)]
        [DataRow("a>a", false, true)]
        [DataRow("a<=a", true, true)]
        [DataRow("a<=b", true, true)]
        [DataRow("b<=a", false, true)]
        [DataRow("a>=a", true, true)]
        [DataRow("b>=a", true, true)]
        [DataRow("a>=b", false, true)]
        public void test_bool_comparison(string s1, dynamic v1, bool t) => Tester.EqnTest(s1, v1, t);

        // The following equations were raising type conflict errors once
        // since the result of sqrt(1) is 1 which is an integer as fas as muParserX
        // is concerned:
        [TestMethod]
        [DataRow("sqrt(a)<sin(8)", false, true)]
        [DataRow("sqrt(a)<=sin(8)", false, true)]
        [DataRow("sqrt(a)>sin(8)", true, true)]
        [DataRow("sqrt(a)>=sin(8)", true, true)]
        [DataRow("sqrt(a)==sin(8)", false, true)]
        [DataRow("sqrt(a)!=sin(8)", true, true)]
        [DataRow("sqrt(a)+1.01", 2.01, true)]
        [DataRow("sqrt(a)-1.01", -0.01, true)]
        public void test_type_conflicting_errors(string s1, dynamic v1, bool t) => Tester.EqnTest(s1, v1, t);

        // interaction with sign operator
        [TestMethod]
        [DataRow("3-(-a)", 4.0, true)]
        [DataRow("3--a", 4.0, true)]
        public void test_sign_operator(string s1, dynamic v1, bool t) => Tester.EqnTest(s1, v1, t);

        [TestMethod]
        // Problems with small bogus real/imag values introduced due to limited floating point accuracy
        // may introduce incorrect imaginary value (When computed with the log/exp formula: -8 + 2.93e-15i)
        [DataRow("(-2)^3", -8.0, true)]
        // may introduce incorrect imaginary value (When computed with the log/exp formula: -8 + 2.93e-15i)
        [DataRow("imag((-2)^3)==0", true, true)]
        public void test_floating_point_accuracy(string s1, dynamic v1, bool t) => Tester.EqnTest(s1, v1, t);
    }
}