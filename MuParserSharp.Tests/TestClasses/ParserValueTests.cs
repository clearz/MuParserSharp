using System.Numerics;
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
            Assert.AreEqual('f', v.GetValueType());
            Assert.AreEqual(3.14, v.AsFloat());


            p.SetExpr("v * 2");
            var ans = p.Eval(); // Perform arithmitic operation on variable

            Assert.AreEqual('f', ans.GetValueType());
            Assert.AreEqual(6.28, ans.AsFloat());


            Assert.AreEqual('f', v.GetValueType());
            Assert.AreEqual(3.14, v.AsFloat());

        }

        [TestMethod]
        public void test_if_matrix_values_work()
        {

            Assert.IsTrue(matrix.IsMatrix());
            Assert.AreEqual(3, matrix.GetRows());
            var rows = matrix.GetRows();

            for (int i = 0; i < rows; i++)
            {
                var dimRow = matrix.At(i).GetRows();
                Assert.AreEqual(3, dimRow);
            }
        }

        [TestMethod]
        public void test_type_checking_of_float_value()
        {
            Assert.IsTrue(fVal.IsScalar());
            Assert.IsTrue(fVal.IsNonComplexScalar());
            Assert.IsFalse(fVal.IsMatrix());
            Assert.IsFalse(fVal.IsInteger());
            Assert.AreEqual('f', fVal.GetValueType());
        }

        [TestMethod]
        public void test_type_checking_of_complex_value()
        {
			Assert.IsTrue(cVal.IsScalar());
			Assert.IsFalse(cVal.IsNonComplexScalar());
			Assert.IsFalse(cVal.IsMatrix());
			Assert.IsFalse(cVal.IsInteger());
            Assert.AreEqual('z', cVal.GetValueType()); Assert.AreEqual('z', cVal.GetValueType());
        }

        [TestMethod]
        public void test_type_checking_of_matrix_value()
        {
			Assert.IsFalse(aVal.IsScalar());
			Assert.IsFalse(aVal.IsNonComplexScalar());
			Assert.IsTrue(aVal.IsMatrix());
			Assert.IsFalse(aVal.IsInteger()); Assert.AreEqual('m', aVal.GetValueType());
        }

        [TestMethod]
        public void test_type_checking_of_string_value()
        {
			Assert.IsFalse(sVal.IsScalar());
			Assert.IsFalse(sVal.IsNonComplexScalar());
			Assert.IsFalse(sVal.IsMatrix());
			Assert.IsFalse(sVal.IsInteger()); Assert.AreEqual('s', sVal.GetValueType());
        }

        [TestMethod]
        public void test_type_checking_of_bool_value()
        {
			Assert.IsFalse(bVal.IsScalar());
			Assert.IsFalse(bVal.IsNonComplexScalar());
			Assert.IsFalse(bVal.IsMatrix());
			Assert.IsFalse(bVal.IsInteger()); Assert.AreEqual('b', bVal.GetValueType());
        }

        [TestMethod]
        public void test_type_checking_of_integer_value()
        {
			Assert.IsTrue(iVal.IsScalar());
			Assert.IsTrue(iVal.IsNonComplexScalar());
			Assert.IsFalse(iVal.IsMatrix());
			Assert.IsTrue(iVal.IsInteger()); Assert.AreEqual('i', iVal.GetValueType());
        }

        [TestMethod]
        public void test_type_checking_of_float_variable()
        {
			Assert.IsTrue(fVar.IsScalar());
			Assert.IsTrue(fVar.IsNonComplexScalar());
			Assert.IsFalse(fVar.IsMatrix());
			Assert.IsFalse(fVar.IsInteger()); Assert.AreEqual('f', fVar.GetValueType());
        }

        [TestMethod]
        public void test_type_checking_of_complex_variable()
        {
			Assert.IsTrue(cVar.IsScalar());
			Assert.IsFalse(cVar.IsNonComplexScalar());
			Assert.IsFalse(cVar.IsMatrix());
			Assert.IsFalse(cVar.IsInteger()); Assert.AreEqual('z', cVar.GetValueType());
        }

        [TestMethod]
        public void test_type_checking_of_matrix_variable()
        {
			Assert.IsFalse(aVar.IsScalar());
			Assert.IsFalse(aVar.IsNonComplexScalar());
			Assert.IsTrue(aVar.IsMatrix());
			Assert.IsFalse(aVar.IsInteger()); Assert.AreEqual('m', aVar.GetValueType());
        }

        [TestMethod]
        public void test_type_checking_of_string_variable()
        {
			Assert.IsFalse(sVar.IsScalar());
			Assert.IsFalse(sVar.IsNonComplexScalar());
			Assert.IsFalse(sVar.IsMatrix());
			Assert.IsFalse(sVar.IsInteger()); Assert.AreEqual('s', sVar.GetValueType());
        }

        [TestMethod]
        public void test_type_checking_of_bool_variable()
        {
			Assert.IsFalse(bVar.IsScalar());
			Assert.IsFalse(bVar.IsNonComplexScalar());
			Assert.IsFalse(bVar.IsMatrix());
			Assert.IsFalse(bVar.IsInteger()); Assert.AreEqual('b', bVar.GetValueType());
        }

        [TestMethod]
        public void test_type_checking_of_integer_variable()
        {
			Assert.IsTrue(iVar.IsScalar());
			Assert.IsTrue(iVar.IsNonComplexScalar());
			Assert.IsFalse(iVar.IsMatrix());
			Assert.IsTrue(iVar.IsInteger()); Assert.AreEqual('i', iVar.GetValueType());
        }

        [TestMethod]
        public void test_complex_add_assign()
        {
            IValue x = 1.0;
            IValue y = new Complex(0, 1);
            x += y; Assert.AreEqual(1, x.GetImag()); Assert.AreEqual(1, x.GetReal()); Assert.AreEqual('z', x.GetValueType());
        }

        [TestMethod]
        public void test_complex_sub_assign()
        {
            IValue x = 1.0;
            IValue y = new Complex(0, 1);
            x -= y; Assert.AreEqual(-1, x.GetImag()); Assert.AreEqual(1, x.GetReal()); Assert.AreEqual('z', x.GetValueType());
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
