using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MuParserSharp.Tests
{
    [TestClass]
    public class ComplexTests
    {

        [TestMethod]
        [DataRow("ca==1+i", true, true)]
        [DataRow("ca==ca", true, true)]
        [DataRow("ca!=1+i", false, true)]
        [DataRow("ca!=ca", false, true)]
        [DataRow("ca!=cb", true, true)]
        [DataRow("ca!=va", true, true)]
        [DataRow("ca==va", false, true)]
        public void test_equality_expressions(string s1, dynamic v1, bool t) => Tester.EqnTest(s1, v1, t);


        [TestMethod]
        [DataRow("ca<10+i", true, true)]
        [DataRow("ca>10+i", false, true)]
        [DataRow("ca<=10+i", true, true)]
        [DataRow("ca>=10+i", false, true)]
        [DataRow("ca<=1", true, true)]
        [DataRow("ca>=1", true, true)]
        public void test_real_part(string s1, dynamic v1, bool t) => Tester.EqnTest(s1, v1, t);

        [TestMethod]
        public void test_complex_pow()
        {
            Tester.EqnTest("(-3)^(4/3)", Complex.Pow(new Complex(-3, 0), new Complex(4.0 / 3, 0)), true, 0);
            // Issue 41:  Complex pow of small numbers zeros out the imaginary part
            //            https://code.google.com/p/muparserx/issues/detail?id=41
            Tester.EqnTest("(1e-15 + 1e-15*i) ^ 2", 0, true, 0);
        }

        [TestMethod]
        [DataRow("i*i", -1.0, 0, true, 0)]
        [DataRow("1i", 0, 1, true, 0)]
        [DataRow("norm(3+4i)", 25.0, 0, true, 0)]
        [DataRow("norm(4i+3)", 25.0, 0, true, 0)]
        [DataRow("norm(3i+4)", 25.0, 0, true, 0)]
        [DataRow("real(4.1i+3.1)", 3.1, 0, true, 0)]
        [DataRow("imag(3.1i+4.1)", 3.1, 0, true, 0)]
        [DataRow("real(3.1)", 3.1, 0, true, 0)]
        [DataRow("imag(2.1i)", 2.1, 0, true, 0)]
        [DataRow("-(4i+5)", -5, -4, true, 0)]
        [DataRow("sqrt(-1)", 0, 1, true, 0)]
        [DataRow("(-1)^0.5", 0, 1, true, 0)]
        [DataRow("sqrt(i*i)", 0, 1, true, 0)]
        [DataRow("sqrt(f)", 0, 1, true, 1)]
        [DataRow("sqrt(2-3)", 0, 1, true, 0)]
        [DataRow("sqrt(a-b)", 0, 1, true, 2)]
        [DataRow("sqrt((2-3))", 0, 1, true, 0)]
        [DataRow("sqrt((a-b))", 0, 1, true, 2)]
        [DataRow("sqrt(-(1))", 0, 1, true, 0)]
        [DataRow("sqrt((-1))", 0, 1, true, 0)]
        [DataRow("sqrt(-(-1))", 1, 0, true, 0)]
        [DataRow("sqrt(1)", 1, 0, true, 0)]
        [DataRow("a=1+2i", 1, 2, true, 1)]
        [DataRow("-(1+2i)", -1, -2, true, 0)]
        [DataRow("-(-1-2i)", 1, 2, true, 0)]
        [DataRow("a*i", 0, 1, true, 1)]
        [DataRow("-(a+b*i)", -1, -2, true, 2)]
        [DataRow("-(-a-b*i)", 1, 2, true, 2)]
        [DataRow("(2+4i)*(8-6i)", 40, 20, true, 0)]
        [DataRow("c=(a=1+2i)", 1, 2, true, 2)]
        // Issue 17:  Wrong result on complex power.
        [DataRow("(-0.27 + 0.66*i)^2", -0.3627, -0.3564, true, 0)]
        [DataRow("(-1+5i)^2", -24, -10, true, 0)]
        public void test_complex_numbers(string s1, double d1, double d2, bool t, int i) => Tester.EqnTest(s1, new Complex(d1, d2), t, i);
    }
}

