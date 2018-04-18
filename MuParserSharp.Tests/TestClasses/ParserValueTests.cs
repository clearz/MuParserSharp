using System.Numerics;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MuParserSharp.Framework;
using MuParserSharp.Parser;

namespace MuParserSharp.Tests
{

    [TestClass]
    public class ParserValueTests
    {
        private static Value bVal, fVal, sVal, sVal1, cVal, aVal, iVal, matrix;
        private static Variable bVar, fVar, sVar, sVar1, cVar, aVar, iVar;

        [TestInitialize]
        public void TestInitialize()
        {
            bVal = true;
            iVal = 12;
            fVal = 3.14;
            sVal = "hello world";
            sVal1 = "hello world";   // Test assignment from const char* to string
            cVal = new Complex(1, 1);
            aVal = new Value(2, 0);
            aVal.At(0) = 2.0;
            aVal.At(1) = 3.0;
            matrix = new Value(3, 0);
            matrix.At(0) = new Value(3, 0);
            matrix.At(1) = new Value(3, 0);
            matrix.At(2) = new Value(3, 0);
            bVar = new Variable(bVal);
            fVar = new Variable(fVal);
            sVar = new Variable(sVal);
            sVar1 = new Variable(sVal1);
            cVar = new Variable(cVal);
            aVar = new Variable(aVal);
            iVar = new Variable(iVal);
        }

        [TestMethod]
        public void test_variable_remains_unchanged_after_operation()
        {
            var p = new ParserX();
            p.EnableAutoCreateVar(true);

            p.SetExpr("v = 3.14");
            var v = p.Eval();
            v.GetValueType().Should().Be('f');
            v.AsFloat().Should().Be(3.14);

            p.SetExpr("v * 2");
            var ans = p.Eval(); // Perform arithmitic operation on variable

            ans.GetValueType().Should().Be('f');
            ans.AsFloat().Should().Be(6.28);

            v.GetValueType().Should().Be('f');
            v.AsFloat().Should().Be(3.14);

        }

        [TestMethod]
        public void test_if_matrix_values_work()
        {
            matrix.IsMatrix().Should().BeTrue();
            matrix.GetRows().Should().Be(3);
            var rows = matrix.GetRows();

            for (int i = 0; i < rows; i++)
            {
                var dimRow = matrix.At(i).GetRows();
                dimRow.Should().Be(3);
            }
        }

        [TestMethod]
        public void test_type_checking_of_float_value()
        {
            fVal.IsScalar().Should().BeTrue();
            fVal.IsNonComplexScalar().Should().BeTrue();
            fVal.IsMatrix().Should().BeFalse();
            fVal.IsInteger().Should().BeFalse();
            fVal.GetValueType().Should().Be('f');
        }

        [TestMethod]
        public void test_type_checking_of_complex_value()
        {
            cVal.IsScalar().Should().BeTrue();
            cVal.IsNonComplexScalar().Should().BeFalse();
            cVal.IsMatrix().Should().BeFalse();
            cVal.IsInteger().Should().BeFalse();
            cVal.GetValueType().Should().Be('z');
            cVal.GetValueType().Should().Be('z');
        }

        [TestMethod]
        public void test_type_checking_of_matrix_value()
        {
            aVal.IsScalar().Should().BeFalse();
            aVal.IsNonComplexScalar().Should().BeFalse();
            aVal.IsMatrix().Should().BeTrue();
            aVal.IsInteger().Should().BeFalse();
            aVal.GetValueType().Should().Be('m');
        }

        [TestMethod]
        public void test_type_checking_of_string_value()
        {
            sVal.IsScalar().Should().BeFalse();
            sVal.IsNonComplexScalar().Should().BeFalse();
            sVal.IsMatrix().Should().BeFalse();
            sVal.IsInteger().Should().BeFalse();
            sVal.GetValueType().Should().Be('s');
        }

        [TestMethod]
        public void test_type_checking_of_bool_value()
        {
            bVal.IsScalar().Should().BeFalse();
            bVal.IsNonComplexScalar().Should().BeFalse();
            bVal.IsMatrix().Should().BeFalse();
            bVal.IsInteger().Should().BeFalse();
            bVal.GetValueType().Should().Be('b');
        }

        [TestMethod]
        public void test_type_checking_of_integer_value()
        {
            iVal.IsScalar().Should().BeTrue();
            iVal.IsNonComplexScalar().Should().BeTrue();
            iVal.IsMatrix().Should().BeFalse();
            iVal.IsInteger().Should().BeTrue();
            iVal.GetValueType().Should().Be('i');
        }

        [TestMethod]
        public void test_type_checking_of_float_variable()
        {
            fVar.IsScalar().Should().BeTrue();
            fVar.IsNonComplexScalar().Should().BeTrue();
            fVar.IsMatrix().Should().BeFalse();
            fVar.IsInteger().Should().BeFalse();
            fVar.GetValueType().Should().Be('f');
        }

        [TestMethod]
        public void test_type_checking_of_complex_variable()
        {
            cVar.IsScalar().Should().BeTrue();
            cVar.IsNonComplexScalar().Should().BeFalse();
            cVar.IsMatrix().Should().BeFalse();
            cVar.IsInteger().Should().BeFalse();
            cVar.GetValueType().Should().Be('z');
        }

        [TestMethod]
        public void test_type_checking_of_matrix_variable()
        {
            aVar.IsScalar().Should().BeFalse();
            aVar.IsNonComplexScalar().Should().BeFalse();
            aVar.IsMatrix().Should().BeTrue();
            aVar.IsInteger().Should().BeFalse();
            aVar.GetValueType().Should().Be('m');
        }

        [TestMethod]
        public void test_type_checking_of_string_variable()
        {
            sVar.IsScalar().Should().BeFalse();
            sVar.IsNonComplexScalar().Should().BeFalse();
            sVar.IsMatrix().Should().BeFalse();
            sVar.IsInteger().Should().BeFalse();
            sVar.GetValueType().Should().Be('s');
        }

        [TestMethod]
        public void test_type_checking_of_bool_variable()
        {
            bVar.IsScalar().Should().BeFalse();
            bVar.IsNonComplexScalar().Should().BeFalse();
            bVar.IsMatrix().Should().BeFalse();
            bVar.IsInteger().Should().BeFalse();
            bVar.GetValueType().Should().Be('b');
        }

        [TestMethod]
        public void test_type_checking_of_integer_variable()
        {
            iVar.IsScalar().Should().BeTrue();
            iVar.IsNonComplexScalar().Should().BeTrue();
            iVar.IsMatrix().Should().BeFalse();
            iVar.IsInteger().Should().BeTrue();
            iVar.GetValueType().Should().Be('i');
        }

        [TestMethod]
        public void test_complex_add_assign()
        {
            IValue x = 1.0;
            IValue y = new Complex(0, 1);
            x += y;

            x.GetImag().Should().Be(1);
            x.GetReal().Should().Be(1);
            x.GetValueType().Should().Be('z');
        }

        [TestMethod]
        public void test_complex_sub_assign()
        {
            IValue x = 1.0;
            IValue y = new Complex(0, 1);
            x -= y;

            x.GetImag().Should().Be(-1);
            x.GetReal().Should().Be(1);
            x.GetValueType().Should().Be('z');
        }


        [TestMethod]
        public void test_float_throw_check()
        {
            Tester.ValueThrowCheck(false, fVal, fVal.GetFloat);
            Tester.ValueThrowCheck(true, fVal, fVal.GetInteger);
            Tester.ValueThrowCheck(false, fVal, fVal.GetReal);
            Tester.ValueThrowCheck(false, fVal, fVal.GetImag);
            Tester.ValueThrowCheck(true, fVal, fVal.GetBool);
            Tester.ValueThrowCheck(true, fVal, fVal.GetString);
            Tester.ValueThrowCheck(true, fVal, fVal.GetArray);
        }

        [TestMethod]
        public void test_float_var_throw_check()
        {
            Tester.ValueThrowCheck(false, fVar, fVar.GetFloat);
            Tester.ValueThrowCheck(true, fVar, fVar.GetInteger);
            Tester.ValueThrowCheck(false, fVar, fVar.GetReal);
            Tester.ValueThrowCheck(false, fVar, fVar.GetImag);
            Tester.ValueThrowCheck(true, fVar, fVar.GetBool);
            Tester.ValueThrowCheck(true, fVar, fVar.GetString);
            Tester.ValueThrowCheck(true, fVar, fVar.GetArray);
        }

        [TestMethod]
        public void test_bool_throw_check()
        {
            Tester.ValueThrowCheck(true, bVal, bVal.GetFloat);
            Tester.ValueThrowCheck(true, bVal, bVal.GetInteger);
            Tester.ValueThrowCheck(true, bVal, bVal.GetReal);
            Tester.ValueThrowCheck(true, bVal, bVal.GetImag);
            Tester.ValueThrowCheck(false, bVal, bVal.GetBool);
            Tester.ValueThrowCheck(true, bVal, bVal.GetString);
            Tester.ValueThrowCheck(true, bVal, bVal.GetArray);
        }

        [TestMethod]
        public void test_bool_var_throw_check()
        {
            Tester.ValueThrowCheck(true, bVar, bVar.GetFloat);
            Tester.ValueThrowCheck(true, bVar, bVar.GetInteger);
            Tester.ValueThrowCheck(true, bVar, bVar.GetReal);
            Tester.ValueThrowCheck(true, bVar, bVar.GetImag);
            Tester.ValueThrowCheck(false, bVar, bVar.GetBool);
            Tester.ValueThrowCheck(true, bVar, bVar.GetString);
            Tester.ValueThrowCheck(true, bVar, bVar.GetArray);
        }

        [TestMethod]
        public void test_complex_throw_check()
        {
            Tester.ValueThrowCheck(true, cVal, cVal.GetFloat);
            Tester.ValueThrowCheck(true, cVal, cVal.GetInteger);
            Tester.ValueThrowCheck(false, cVal, cVal.GetReal);
            Tester.ValueThrowCheck(false, cVal, cVal.GetImag);
            Tester.ValueThrowCheck(true, cVal, cVal.GetBool);
            Tester.ValueThrowCheck(true, cVal, cVal.GetString);
            Tester.ValueThrowCheck(true, cVal, cVal.GetArray);
        }

        [TestMethod]
        public void test_complex_var_throw_check()
        {
            Tester.ValueThrowCheck(true, cVar, cVar.GetFloat);
            Tester.ValueThrowCheck(true, cVar, cVar.GetInteger);
            Tester.ValueThrowCheck(false, cVar, cVar.GetReal);
            Tester.ValueThrowCheck(false, cVar, cVar.GetImag);
            Tester.ValueThrowCheck(true, cVar, cVar.GetBool);
            Tester.ValueThrowCheck(true, cVar, cVar.GetString);
            Tester.ValueThrowCheck(true, cVar, cVar.GetArray);
        }

        [TestMethod]
        public void test_string_throw_check()
        {
            Tester.ValueThrowCheck(true, sVal, sVal.GetFloat);
            Tester.ValueThrowCheck(true, sVal, sVal.GetInteger);
            Tester.ValueThrowCheck(true, sVal, sVal.GetReal);
            Tester.ValueThrowCheck(true, sVal, sVal.GetImag);
            Tester.ValueThrowCheck(true, sVal, sVal.GetBool);
            Tester.ValueThrowCheck(false, sVal, sVal.GetString);
            Tester.ValueThrowCheck(true, sVal, sVal.GetArray);
        }

        [TestMethod]
        public void test_string_var_throw_check()
        {
            Tester.ValueThrowCheck(true, sVar, sVar.GetFloat);
            Tester.ValueThrowCheck(true, sVar, sVar.GetInteger);
            Tester.ValueThrowCheck(true, sVar, sVar.GetReal);
            Tester.ValueThrowCheck(true, sVar, sVar.GetImag);
            Tester.ValueThrowCheck(true, sVar, sVar.GetBool);
            Tester.ValueThrowCheck(false, sVar, sVar.GetString);
            Tester.ValueThrowCheck(true, sVar, sVar.GetArray);
        }

        [TestMethod]
        public void test_matrix_throw_check()
        {
            Tester.ValueThrowCheck(true, aVal, aVal.GetFloat);
            Tester.ValueThrowCheck(true, aVal, aVal.GetInteger);
            Tester.ValueThrowCheck(true, aVal, aVal.GetReal);
            Tester.ValueThrowCheck(true, aVal, aVal.GetImag);
            Tester.ValueThrowCheck(true, aVal, aVal.GetBool);
            Tester.ValueThrowCheck(true, aVal, aVal.GetString);
            Tester.ValueThrowCheck(false, aVal, aVal.GetArray);
        }

        [TestMethod]
        public void test_matrix_var_throw_check()
        {
            Tester.ValueThrowCheck(true, aVar, aVar.GetFloat);
            Tester.ValueThrowCheck(true, aVar, aVar.GetInteger);
            Tester.ValueThrowCheck(true, aVar, aVar.GetReal);
            Tester.ValueThrowCheck(true, aVar, aVar.GetImag);
            Tester.ValueThrowCheck(true, aVar, aVar.GetBool);
            Tester.ValueThrowCheck(true, aVar, aVar.GetString);
            Tester.ValueThrowCheck(false, aVar, aVar.GetArray);
        }

        [TestMethod]
        public void test_integer_throw_check()
        {
            Tester.ValueThrowCheck(false, iVal, iVal.GetFloat);
            Tester.ValueThrowCheck(false, iVal, iVal.GetInteger);
            Tester.ValueThrowCheck(false, iVal, iVal.GetReal);
            Tester.ValueThrowCheck(false, iVal, iVal.GetImag);
            Tester.ValueThrowCheck(true, iVal, iVal.GetBool);
            Tester.ValueThrowCheck(true, iVal, iVal.GetString);
            Tester.ValueThrowCheck(true, iVal, iVal.GetArray);
        }

        [TestMethod]
        public void test_integer_var_throw_check()
        {
            Tester.ValueThrowCheck(false, iVar, iVar.GetFloat);
            Tester.ValueThrowCheck(false, iVar, iVar.GetInteger);
            Tester.ValueThrowCheck(false, iVar, iVar.GetReal);
            Tester.ValueThrowCheck(false, iVar, iVar.GetImag);
            Tester.ValueThrowCheck(true, iVar, iVar.GetBool);
            Tester.ValueThrowCheck(true, iVar, iVar.GetString);
            Tester.ValueThrowCheck(true, iVar, iVar.GetArray);
        }

    }
}
