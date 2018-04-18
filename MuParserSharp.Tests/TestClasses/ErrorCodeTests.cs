using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MuParserSharp.Tests
{
    [TestClass]
    public class ErrorCodeTests
    {

        [TestMethod]
        [DataRow("a,b", EErrorCodes.ecUNEXPECTED_COMMA)]
        [DataRow("(a,b)", EErrorCodes.ecUNEXPECTED_COMMA)]
        [DataRow("((a,b))", EErrorCodes.ecUNEXPECTED_COMMA)]
        [DataRow("2*1,2", EErrorCodes.ecUNEXPECTED_COMMA)]
        [DataRow("sin(1,2)", EErrorCodes.ecTOO_MANY_PARAMS)]
        public void test_basic_error_throwing(string s, EErrorCodes e) => Tester.ThrowTest(s, e);

        [TestMethod]
        public void test_try_assign_nonexistent_var()
        {
            Tester.ThrowTest("sin(nonexistent_var)", EErrorCodes.ecUNASSIGNABLE_TOKEN, 4, "nonexistent_var");
        }

        [TestMethod]
        [DataRow("sin(\"test\")", EErrorCodes.ecEVAL, 0)]
        [DataRow("max(1, \"test\")", EErrorCodes.ecEVAL, 0)]
        [DataRow("max(1,sin(8), \"t\")", EErrorCodes.ecEVAL, 0)]
        [DataRow("str2dbl(sin(3.14))", EErrorCodes.ecEVAL, 0)]
        public void test_invalid_function_arguments(string s, EErrorCodes e, int i) => Tester.ThrowTest(s, e, i);


        [TestMethod]
        [DataRow("\"test\"n", EErrorCodes.ecEVAL, 6)] // (nano can only be applied to floats)
        [DataRow("(1+3i)/(8*9i)+\"hallo\"", EErrorCodes.ecEVAL, -1)]
        [DataRow("(1+3i)/(8*9i)-\"hallo\"", EErrorCodes.ecEVAL, -1)]
        [DataRow("(1+3i)/(8*9i)*\"hallo\"", EErrorCodes.ecEVAL, -1)]
        [DataRow("(1+3i)/(8*9i)/\"hallo\"", EErrorCodes.ecEVAL, -1)]
        [DataRow("10+va", EErrorCodes.ecEVAL, 2)]
        public void test_invalid_unary_operator_arguments(string s, EErrorCodes e, int p) => Tester.ThrowTest(s, e, p);


        [TestMethod]
        [DataRow("\"test\" // 8", EErrorCodes.ecEVAL, 7)]
        [DataRow("8//\"test\"", EErrorCodes.ecEVAL, 1)]
        [DataRow("5//8", EErrorCodes.ecEVAL, 1)]
        [DataRow("\"t\"//sin(8)", EErrorCodes.ecEVAL, 3)]
        [DataRow("sin(8)//\"t\"", EErrorCodes.ecEVAL, 6)]
        public void test_binary_op_type_conflicts(string s, EErrorCodes e, int i) => Tester.ThrowTest(s, e, i);

        [TestMethod]
        [DataRow("3+", EErrorCodes.ecUNEXPECTED_EOF)]
        [DataRow("8*", EErrorCodes.ecUNEXPECTED_EOF)]
        [DataRow("3+(", EErrorCodes.ecUNEXPECTED_EOF)]
        [DataRow("3+sin", EErrorCodes.ecUNEXPECTED_EOF)]
        [DataRow("(2+", EErrorCodes.ecUNEXPECTED_EOF)]
        public void test_unexpected_end_of_expression(string s, EErrorCodes e) => Tester.ThrowTest(s, e);

        [DataRow("3+)", EErrorCodes.ecUNEXPECTED_PARENS)]
        [DataRow("3)", EErrorCodes.ecUNEXPECTED_PARENS)]
        [DataRow("(3))", EErrorCodes.ecUNEXPECTED_PARENS)]
        [DataRow("()", EErrorCodes.ecUNEXPECTED_PARENS)]
        [DataRow("(2+)", EErrorCodes.ecUNEXPECTED_PARENS)]
        [DataRow("sin(cos)", EErrorCodes.ecUNEXPECTED_PARENS)]
        [DataRow("sin(())", EErrorCodes.ecUNEXPECTED_PARENS)]
        [DataRow("sin()", EErrorCodes.ecTOO_FEW_PARAMS)]
        [DataRow("sin)", EErrorCodes.ecUNEXPECTED_PARENS)]
        [DataRow("pi)", EErrorCodes.ecUNEXPECTED_PARENS)]
        [DataRow("a)", EErrorCodes.ecUNEXPECTED_PARENS)]
        [DataRow("2(-m)", EErrorCodes.ecUNEXPECTED_PARENS)]
        [DataRow("2(m)", EErrorCodes.ecUNEXPECTED_PARENS)]
        public void test_unexpected_parens(string s, EErrorCodes e) => Tester.ThrowTest(s, e);


        [TestMethod]
        [DataRow("(1+2", EErrorCodes.ecMISSING_PARENS)]
        [DataRow("((3)", EErrorCodes.ecMISSING_PARENS)]
        public void test_missing_parens(string s, EErrorCodes e) => Tester.ThrowTest(s, e);


        [TestMethod]
        [DataRow("5z)", EErrorCodes.ecUNASSIGNABLE_TOKEN)]
        [DataRow("sin(3)xyz", EErrorCodes.ecUNASSIGNABLE_TOKEN)]
        [DataRow("5t6", EErrorCodes.ecUNASSIGNABLE_TOKEN)]
        [DataRow("5 t 6", EErrorCodes.ecUNASSIGNABLE_TOKEN)]
        [DataRow("ksdfj", EErrorCodes.ecUNASSIGNABLE_TOKEN)]
        [DataRow("-m", EErrorCodes.ecUNASSIGNABLE_TOKEN)]
        [DataRow("m4", EErrorCodes.ecUNASSIGNABLE_TOKEN)]
        [DataRow("sin(m)", EErrorCodes.ecUNASSIGNABLE_TOKEN)]
        [DataRow("m m", EErrorCodes.ecUNASSIGNABLE_TOKEN)]
        [DataRow("m(8)", EErrorCodes.ecUNASSIGNABLE_TOKEN)]
        [DataRow("4 + m", EErrorCodes.ecUNASSIGNABLE_TOKEN)]
        public void test_unassignable_token(string s, EErrorCodes e) => Tester.ThrowTest(s, e);


        [TestMethod]
        public void test_unexpected_operator()
        {
            // unexpected operator
            Tester.ThrowTest("5+*3)", EErrorCodes.ecUNEXPECTED_OPERATOR);


        }

        [TestMethod]
        [DataRow(",3", EErrorCodes.ecUNEXPECTED_COMMA)]
        [DataRow("sin(,sin(8))", EErrorCodes.ecUNEXPECTED_COMMA)]
        public void test_unexpeted_comma(string s, EErrorCodes e) => Tester.ThrowTest(s, e);


        [TestMethod]
        public void test_unexpected_variable()
        {
            // unexpected variable
            // if a variable factory is installed ecUNEXPECTED_VAR
            Tester.ThrowTest("a _xxx_ b", EErrorCodes.ecUNASSIGNABLE_TOKEN, 2);
        }

        [TestMethod]
        [DataRow("sin(3)cos(3)", EErrorCodes.ecUNEXPECTED_FUN)]
        [DataRow("sin(3)3", EErrorCodes.ecUNEXPECTED_VAL)]
        [DataRow("sin(3)+", EErrorCodes.ecUNEXPECTED_EOF)]
        public void test_unexpected_tokens(string s, EErrorCodes e) => Tester.ThrowTest(s, e);



        [TestMethod]
        [DataRow("0x", EErrorCodes.ecUNASSIGNABLE_TOKEN)] // incomplete hex value
        [DataRow("1+0x", EErrorCodes.ecUNASSIGNABLE_TOKEN)] // incomplete hex value
        [DataRow("a+0x", EErrorCodes.ecUNASSIGNABLE_TOKEN)] // incomplete hex value
        public void test_value_recognition(string s, EErrorCodes e) => Tester.ThrowTest(s, e);



        [TestMethod]
        [DataRow("3n[1]", EErrorCodes.ecINDEX_OUT_OF_BOUNDS)]
        [DataRow("min(3,]", EErrorCodes.ecUNEXPECTED_SQR_BRACKET)]
        [DataRow("sin(]", EErrorCodes.ecUNEXPECTED_SQR_BRACKET)]
        [DataRow("va[]", EErrorCodes.ecUNEXPECTED_SQR_BRACKET)]
        [DataRow("3+]", EErrorCodes.ecUNEXPECTED_SQR_BRACKET)]
        [DataRow("sin[a)", EErrorCodes.ecUNEXPECTED_SQR_BRACKET)]
        [DataRow("1+[8]", EErrorCodes.ecUNEXPECTED_SQR_BRACKET)]
        [DataRow("1[8]", EErrorCodes.ecUNEXPECTED_SQR_BRACKET)]
        [DataRow("[1]", EErrorCodes.ecUNEXPECTED_SQR_BRACKET)]
        [DataRow("]1", EErrorCodes.ecUNEXPECTED_SQR_BRACKET)]
        [DataRow("va[[3]]", EErrorCodes.ecUNEXPECTED_SQR_BRACKET)]
        public void test_index_out_of_bounds(string s, EErrorCodes e) => Tester.ThrowTest(s, e);


    }
}