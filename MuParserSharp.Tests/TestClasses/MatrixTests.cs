using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MuParserSharp.Parser;

namespace MuParserSharp.Tests
{
    [TestClass]
    public class MatrixTests
    {
        private Dictionary<string, Value> _dict;
            Value unity,
            va,
            m1_plus_m2,
            m2_minus_m1,
            m2_times_10,
            va_times_vb_transp,
            size_3x6,
            size_3x3,
            size_3x1,
            size_1x3, ones_3, ones_3x3;

        [TestInitialize]
        public void TestInitialize()
        {
            unity = new Value(3, 3, 0L);
            unity.At(0, 0) = 1;
            unity.At(1, 1) = 1;
            unity.At(2, 2) = 1;

            va = new Value(3, 0);
            va.At(0) = 1;
            va.At(1) = 2;
            va.At(2) = 3;

            //m2 = new Value(3, 3, 0);
            //m2.At(0, 0) = 1;  m2.At(0, 1) = 2;  m2.At(0, 2) = 3;
            //m2.At(1, 0) = 4;  m2.At(1, 1) = 5;  m2.At(1, 2) = 6;
            //m2.At(2, 0) = 7;  m2.At(2, 1) = 8;  m2.At(2, 2) = 9;

            m1_plus_m2 = new Value(3, 3, 0L);
            m1_plus_m2.At(0, 0) = 2;
            m1_plus_m2.At(0, 1) = 2;
            m1_plus_m2.At(0, 2) = 3;
            m1_plus_m2.At(1, 0) = 4;
            m1_plus_m2.At(1, 1) = 6;
            m1_plus_m2.At(1, 2) = 6;
            m1_plus_m2.At(2, 0) = 7;
            m1_plus_m2.At(2, 1) = 8;
            m1_plus_m2.At(2, 2) = 10;

            m2_minus_m1 = new Value(3, 3, 0l);
            m2_minus_m1.At(0, 0) = 0;
            m2_minus_m1.At(0, 1) = 2;
            m2_minus_m1.At(0, 2) = 3;
            m2_minus_m1.At(1, 0) = 4;
            m2_minus_m1.At(1, 1) = 4;
            m2_minus_m1.At(1, 2) = 6;
            m2_minus_m1.At(2, 0) = 7;
            m2_minus_m1.At(2, 1) = 8;
            m2_minus_m1.At(2, 2) = 8;

            m2_times_10 = new Value(3, 3, 0l);
            m2_times_10.At(0, 0) = 10;
            m2_times_10.At(0, 1) = 20;
            m2_times_10.At(0, 2) = 30;
            m2_times_10.At(1, 0) = 40;
            m2_times_10.At(1, 1) = 50;
            m2_times_10.At(1, 2) = 60;
            m2_times_10.At(2, 0) = 70;
            m2_times_10.At(2, 1) = 80;
            m2_times_10.At(2, 2) = 90;

            va_times_vb_transp = new Value(3, 3, 0l);
            va_times_vb_transp.At(0, 0) = 4;
            va_times_vb_transp.At(0, 1) = 3;
            va_times_vb_transp.At(0, 2) = 2;
            va_times_vb_transp.At(1, 0) = 8;
            va_times_vb_transp.At(1, 1) = 6;
            va_times_vb_transp.At(1, 2) = 4;
            va_times_vb_transp.At(2, 0) = 12;
            va_times_vb_transp.At(2, 1) = 9;
            va_times_vb_transp.At(2, 2) = 6;

            size_3x6 = new Value(1, 2, 0l);
            size_3x6.At(0, 0) = 3;
            size_3x6.At(0, 1) = 6;

            size_3x3 = new Value(1, 2, 0l);
            size_3x3.At(0, 0) = 3;
            size_3x3.At(0, 1) = 3;

            size_3x1 = new Value(1, 2, 0l);
            size_3x1.At(0, 0) = 3;
            size_3x1.At(0, 1) = 1;

            size_1x3 = new Value(1, 2, 0l);
            size_1x3.At(0, 0) = 1;
            size_1x3.At(0, 1) = 3;


            ones_3 = new Value(3, 1);
            ones_3x3 = new Value(3, 3, 1);
            _dict = new Dictionary<string, Value>()
            {
                [nameof(unity)] = unity,
                [nameof(va)] = va,
                [nameof(m1_plus_m2)] = m1_plus_m2,
                [nameof(m2_minus_m1)] = m2_minus_m1,
                [nameof(m2_times_10)] = m2_times_10,
                [nameof(va_times_vb_transp)] = va_times_vb_transp,
                [nameof(size_3x6)] = size_3x6,
                [nameof(size_3x3)] = size_3x3,
                [nameof(size_3x1)] = size_3x1,
                [nameof(size_1x3)] = size_1x3,
                [nameof(ones_3)] = ones_3,
                [nameof(ones_3x3)] = ones_3x3,
            };
        }

        [TestMethod]
        // Check matrix dimension mismatch error
        [DataRow("\"hallo\"+m1", EErrorCodes.ecEVAL)]
        [DataRow("m1+\"hallo\"", EErrorCodes.ecEVAL)]
        [DataRow("va+m1", EErrorCodes.ecMATRIX_DIMENSION_MISMATCH)]
        [DataRow("m1+va", EErrorCodes.ecMATRIX_DIMENSION_MISMATCH)]
        [DataRow("va-m1", EErrorCodes.ecMATRIX_DIMENSION_MISMATCH)]
        [DataRow("m1-va", EErrorCodes.ecMATRIX_DIMENSION_MISMATCH)]
        [DataRow("va*m1", EErrorCodes.ecMATRIX_DIMENSION_MISMATCH)]
        [DataRow("va+eye(2)", EErrorCodes.ecMATRIX_DIMENSION_MISMATCH)]
        // Issue 63:
        [DataRow("0-0-eye()", EErrorCodes.ecINVALID_NUMBER_OF_PARAMETERS)]

        [DataRow("m1[1]", EErrorCodes.ecINDEX_DIMENSION)]
        [DataRow("m1[1,2,3]", EErrorCodes.ecINDEX_DIMENSION)]
        // va has 1 column, 3 rows -> the coulumn index is referencing the third column
        [DataRow("va[1,2]", EErrorCodes.ecINDEX_OUT_OF_BOUNDS)]
        [DataRow("a+m1", EErrorCodes.ecEVAL)]
        [DataRow("m1+a", EErrorCodes.ecEVAL)]
        [DataRow("a-m1", EErrorCodes.ecEVAL)]
        [DataRow("m1-a", EErrorCodes.ecEVAL)]
        [DataRow("va[,1]", EErrorCodes.ecUNEXPECTED_COMMA)]
        [DataRow("va[{1]", EErrorCodes.ecMISSING_CURLY_BRACKET)]
        [DataRow("{,1}", EErrorCodes.ecUNEXPECTED_COMMA)]

        public void test_matrix_dimension_mismatch_error(string s, EErrorCodes e) => Tester.ThrowTest(s, e);

        // sample expressions
        [TestMethod]
        [DataRow("m1", nameof(unity), true)]
        [DataRow("m1*m1", nameof(unity), true)]
        [DataRow("m1+m2", nameof(m1_plus_m2), true)]
        [DataRow("m2-m1", nameof(m2_minus_m1), true)]
        [DataRow("10*m2", nameof(m2_times_10), true)]
        [DataRow("m2*10", nameof(m2_times_10), true)]
        [DataRow("5*m2*b", nameof(m2_times_10), true)]
        [DataRow("b*m2*5", nameof(m2_times_10), true)]
        [DataRow("m1*va", nameof(va), true)]
        public void test_sample_expressions(string s1, string v1, bool t) => Tester.EqnTest(s1, _dict[v1], t);

        //ones function test
        [TestMethod]
        [DataRow("ones(1,2,3)", EErrorCodes.ecINVALID_NUMBER_OF_PARAMETERS)]
        [DataRow("ones()", EErrorCodes.ecINVALID_NUMBER_OF_PARAMETERS)]
        public void test_ones_EErrorCodes(string s, EErrorCodes e) => Tester.ThrowTest(s, e);

        [TestMethod]
        [DataRow("ones(1,1)", 1, true)]
        [DataRow("ones(1)", 1, true)]
        public void test_ones_expressions(string s1, dynamic v1, bool t) => Tester.EqnTest(s1, v1, t);

        [TestMethod]
        [DataRow("ones(3,3)", nameof(ones_3x3), true)]
        [DataRow("ones(3,1)", nameof(ones_3), true)]
        [DataRow("ones(3)", nameof(ones_3), true)]
        [DataRow("size(ones(3,3))", nameof(size_3x3), true)]  // check return value dimension
        [DataRow("size(ones(1,3))", nameof(size_1x3), true)]  // check return value dimension
        [DataRow("size(ones(3,1))", nameof(size_3x1), true)]  // check return value dimension
        [DataRow("size(ones(3))", nameof(size_3x3), true)]    // check return value dimension
        public void test_ones_value(string s1, string v1, bool t) => Tester.EqnTest(s1, _dict[v1], t);



        //zeros function tests
        [TestMethod]
        public void test_zeros_error_throwing() => Tester.ThrowTest("zeros()", EErrorCodes.ecINVALID_NUMBER_OF_PARAMETERS);

        [TestMethod]
        [DataRow("size(zeros(3,3))", nameof(size_3x3), true)]  // check return value dimension
        [DataRow("size(zeros(1,3))", nameof(size_1x3), true)]  // check return value dimension
        [DataRow("size(zeros(3,1))", nameof(size_3x1), true)]  // check return value dimension
        [DataRow("size(zeros(3))", nameof(size_3x3), true)]  // check return value dimension
        public void test_zeros_value(string s1, string v1, bool t) => Tester.EqnTest(s1, _dict[v1], t);



        //eye function tests
        [TestMethod]
        public void test_eye_error_throwing() => Tester.ThrowTest("eye()", EErrorCodes.ecINVALID_NUMBER_OF_PARAMETERS);

        [TestMethod]
        [DataRow("size(eye(3,3))", nameof(size_3x3), true)]  // check return value dimension
        [DataRow("size(eye(1,3))", nameof(size_1x3), true)]  // check return value dimension
        [DataRow("size(eye(3,1))", nameof(size_3x1), true)]  // check return value dimension
        [DataRow("size(eye(3))", nameof(size_3x3), true)]  // check return value dimension
        [DataRow("size(eye(3,6))", nameof(size_3x6), true)]  // check return value dimension
        public void test_eye_value(string s1, string v1, bool t) => Tester.EqnTest(s1, _dict[v1], t);



        //transposition
        [TestMethod]
        [DataRow("va'*vb", 16.0, true)]
        [DataRow("2*va'*vb", 32.0, true)]
        public void test_matrix_transposition(string s1, dynamic v1, bool t) => Tester.EqnTest(s1, v1, t);

        [TestMethod]
        public void test_matrix_transposition() => Tester.EqnTest("va*vb'", _dict[nameof(va_times_vb_transp)], true);



        //index operations
        [TestMethod]
        [DataRow("va[0]", 1, true)]
        [DataRow("va[1]", 2, true)]
        [DataRow("va[2]", 3, true)]
        public void test_one_dimentional_index_operator(string s1, dynamic v1, bool t) => Tester.EqnTest(s1, v1, t);

        [TestMethod]
        [DataRow("va[0,0]", 1, true)]
        [DataRow("va[1, 0]", 2, true)]
        [DataRow("va[2,0]", 3, true)]
        public void test_two_dimentional_index_operator(string s1, dynamic v1, bool t) => Tester.EqnTest(s1, v1, t);

        [TestMethod]
        [DataRow("va'[0]", 1, true)]
        [DataRow("va'[1]", 2, true)]
        [DataRow("va'[2]", 3, true)]
        public void test_one_dim_tranposed_index_operator(string s1, dynamic v1, bool t) => Tester.EqnTest(s1, v1, t);

        [TestMethod]
        [DataRow("va'[0,0]", 1, true)]
        [DataRow("va'[0, 1]", 2, true)]
        [DataRow("va'[0,2]", 3, true)]
        [DataRow("(va')[0,2]", 3, true)]
        public void test_two_dim_tranposed_index_operator(string s1, dynamic v1, bool t) => Tester.EqnTest(s1, v1, t);


        // vector creation
        [TestMethod]
        [DataRow("{1,2,3}'", nameof(va), true)]
        [DataRow("{a,2,3}'", nameof(va), true)]        // that was an actual bug: variable a was overwritten
        public void test_vector_creation_literal(string s1, string v1, bool t) => Tester.EqnTest(s1, _dict[v1], t);



        // assignment to element
        [TestMethod]
        public void test_assignment_to_element_throwing() => Tester.ThrowTest("va'[0]=123", EErrorCodes.ecASSIGNEMENT_TO_VALUE);

    }
}