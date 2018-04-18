using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MuParserSharp.Framework;
using MuParserSharp.Parser;

namespace MuParserSharp.Tests
{
    public static class Tester
    {
        public static void EqnTest(string a_str, dynamic d, bool a_fPass, int nExprVar = -1)
        {
            IValue[] fVal = new IValue[5];
            IValue a_val = d;
            try
            {
                // p1 is a pointer since I'm going to delete it in order to test if
                // parsers after copy construction still refer to members of the deleted object.
                // !! If this is the case this function will crash !!
                ParserX p1 = new ParserX();

                // Add variables
                Value[] vVarVal = {1, 2, 3, -2, -1};

                // m1 ist die Einheitsmatrix
                var m1 = new Value(3, 3, 0L);
                m1.At(0, 0) = 1L;
                m1.At(1, 1) = 1L;
                m1.At(2, 2) = 1L;

                // m2 ist die Einheitsmatrix
                Value m2 = new Value(3, 3, 0);
                m2.At(0, 0) = 1;
                m2.At(0, 1) = 2;
                m2.At(0, 2) = 3;
                m2.At(1, 0) = 4;
                m2.At(1, 1) = 5;
                m2.At(1, 2) = 6;
                m2.At(2, 0) = 7;
                m2.At(2, 1) = 8;
                m2.At(2, 2) = 9;

                p1.DefineOprt(new DbgSillyAdd());
                p1.DefineFun(new FunTest0());

                p1.DefineVar("a", new Variable(vVarVal[0]));
                p1.DefineVar("b", new Variable(vVarVal[1]));
                p1.DefineVar("c", new Variable(vVarVal[2]));
                p1.DefineVar("d", new Variable(vVarVal[3]));
                p1.DefineVar("f", new Variable(vVarVal[4]));
                p1.DefineVar("m1", new Variable(m1));
                p1.DefineVar("m2", new Variable(m2));

                // Add constants
                p1.DefineConst("const", 1);
                p1.DefineConst("const1", 2);
                p1.DefineConst("const2", 3);

                // some vector variables
                Value aVal1 = new Value(3, 0);
                aVal1.At(0) = 1;
                aVal1.At(1) = 2;
                aVal1.At(2) = 3;

                Value aVal2 = new Value(3, 0);
                aVal2.At(0) = 4;
                aVal2.At(1) = 3;
                aVal2.At(2) = 2;
                p1.DefineVar("va", new Variable(aVal1));
                p1.DefineVar("vb", new Variable(aVal2));

                // complex variables
                Value[] cVal = new Value[3];
                cVal[0] = new Complex(1, 1);
                cVal[1] = new Complex(2, 3);
                cVal[2] = new Complex(3, 4);
                p1.DefineVar("ca", new Variable(cVal[0]));
                p1.DefineVar("cb", new Variable(cVal[1]));
                p1.DefineVar("cc", new Variable(cVal[2]));

                p1.SetExpr(a_str);

                fVal[0] = p1.Eval();
                p1.DumpRPN();
                // Test copy and assignement operators
                List<ParserX> vParser = new List<ParserX>();
                vParser.Add(p1); // Push p1 into the vector
                ParserX p2 = new ParserX(); // take parser from vector
                p2.Assign(vParser[0]);
                // destroy the originals from p2
                vParser.Clear(); // delete the vector
                p1 = null; // delete the original

                fVal[1] = p2.Eval(); // If copy constructions does not work
                // we may see a crash here

                ParserX p3 = new ParserX(p2);
                fVal[2] = p3.Eval(); // If assignment does not work
                // we may see a crash here

                // Calculating a second time will parse from rpn rather than from
                // string. The result must be the same...
                fVal[3] = p3.Eval();

                // Calculate yet another time. There is the possibility of
                // changing variables as a side effect of expression
                // evaluation. So there are really bugs that could make this fail...
                fVal[4] = p3.Eval();

                // Check i number of used variables is correct
                if (nExprVar != -1)
                {
                    int n2 = p2.GetExprVar().Count;
                    int n3 = p3.GetExprVar().Count;

                    if (n2 + n3 != 2 * n2 || n2 != nExprVar)
                    {
                        var msg =
                            $"Number of expression variables is incorrect. (expected: {nExprVar}; detected: {n2})";
                        Assert.Fail(msg);
                    }
                }

                // Check the three results
                // 1.) computed results must have identic type
                char cType = fVal[0].GetValueType();
                bool bStat = cType == fVal[1].GetValueType() &&
                             cType == fVal[2].GetValueType() &&
                             cType == fVal[3].GetValueType() &&
                             cType == fVal[4].GetValueType();
                if (!bStat)
                {
                    var msg = $"{a_str} :  inconsistent result type " +
                              $"({fVal[0].GetValueType()}, {fVal[1].GetValueType()}, " +
                              $"{fVal[2].GetValueType()}, {fVal[3].GetValueType()}, {fVal[4].GetValueType()})";
                    Assert.Fail(msg);
                }

                if ((cType == 'x' || a_val.GetValueType() == 'x') && cType != a_val.GetValueType())
                {
                    var msg = $"{a_str}:  Complex value sliced!";
                    Assert.Fail(msg);
                }

                // Compare the results
                switch (cType)
                {
                    case 'i':
                    case 'b':
                    case 's':
                        bStat = (a_val == fVal[0] &&
                                 a_val == fVal[1] &&
                                 a_val == fVal[2] &&
                                 a_val == fVal[3] &&
                                 a_val == fVal[4]);
                        break;

                    // We need more attention for comaring float values due to floating point
                    // inaccuracies.
                    case 'f':
                    {
                        bStat = true;
                        int num = fVal.Length;
                        for (int i = 0; i < num; ++i)
                            bStat &= Math.Abs(a_val.GetFloat() - fVal[i].GetFloat()) <=
                                     Math.Abs(fVal[i].GetFloat() * 0.0001);
                    }
                        break;

                    case 'z':
                    {
                        bStat = true;
                        int num = fVal.Length;
                        for (int i = 0; i < num; ++i)
                        {
                            bStat &= Math.Abs(a_val.AsFloat() - fVal[i].AsFloat()) <=
                                     Math.Max(1e-15, Math.Abs(fVal[i].AsFloat() * 0.0000001));
                            bStat &= Math.Abs(a_val.GetImag() - fVal[i].GetImag()) <=
                                     Math.Max(1e-15, Math.Abs(fVal[i].GetImag() * 0.0000001));
                        }
                    }
                        break;

                    case 'm':
                    {
                        bStat = true;
                        int num = fVal.Length;

                        for (int i = 0; i < num; ++i)
                        {
                            bStat = Check(a_val, fVal[i]);
                            if (!bStat)
                                break;
                        }
                    }
                        break;

                    default:
                        Assert.Fail($"Parser return value has an unexpected typecode '{cType}'.");
                        break;
                }

                Assert.AreEqual(bStat, a_fPass);
            }
            catch (ParserError p)
            {
                Assert.Fail(p.GetMsg());
            }
            catch (Exception e)
            {
                var msg = a_str + ": " + e.Message;
                Assert.Fail(msg);
            }

            bool Check(IValue v1, IValue v2, bool checkType = true)
            {
                if (checkType && v1.GetValueType() != v2.GetValueType())
                    return false;

                if (v1.GetRows() != v2.GetRows())
                    return false;

                if (v1.IsMatrix())
                {
                    for (int i = 0; i < v1.GetRows(); ++i)
                    {
                        for (int j = 0; j < v1.GetCols(); ++j)
                        {
                            if (!Check(v1.At(i, j), v2.At(i, j)))
                                return false;
                        }
                    }

                    return true;
                }
                else
                {
                    return (Math.Abs(v1.GetFloat() - v2.GetFloat()) <=
                            Math.Max(1e-15, Math.Abs(v1.GetFloat() * 0.0000001)));
                }
            }
        }

        public static void ThrowTest(string a_sExpr, EErrorCodes a_nErrc, int a_nPos = -1, string a_sIdent = null)
        {
            try
            {
                var p = new ParserX();
                Value[] vVarVal = { 1, 2, 3, -2 };
                p.DefineVar("a", new Variable(vVarVal[0]));
                p.DefineVar("b", new Variable(vVarVal[1]));
                p.DefineVar("c", new Variable(vVarVal[2]));
                p.DefineVar("d", new Variable(vVarVal[3]));

                // array variables
                Value aVal1 = new Value(3, 0);
                aVal1.At(0) = 1.0;
                aVal1.At(1) = 2.0;
                aVal1.At(2) = 3.0;

                Value aVal2 = new Value(3, 0);
                aVal2.At(0) = 4.0;
                aVal2.At(1) = 3.0;
                aVal2.At(2) = 2.0;

                Value aVal3 = new Value(4, 0);
                aVal3.At(0) = 4.0;
                aVal3.At(1) = 3.0;
                aVal3.At(2) = 2.0;
                aVal3.At(3) = 5.0;

                Value aVal4 = new Value(4, 0);
                aVal4.At(0) = 4.0;
                aVal4.At(1) = false;
                aVal4.At(2) = "hallo";

                // Matrix variables
                Value m1 = new Value(3, 3, 0);
                m1.At(0, 0) = 1.0;
                m1.At(1, 1) = 1.0;
                m1.At(2, 2) = 1.0;

                Value m2 = new Value(3, 3, 0);
                m2.At(0, 0) = 1.0; m2.At(0, 1) = 2.0; m2.At(0, 2) = 3.0;
                m2.At(1, 0) = 4.0; m2.At(1, 1) = 5.0; m2.At(1, 2) = 6.0;
                m2.At(2, 0) = 7.0; m2.At(2, 1) = 8.0; m2.At(2, 2) = 9.0;

                p.DefineVar("m1", new Variable(m1));
                p.DefineVar("m2", new Variable(m2));
                p.DefineVar("va", new Variable(aVal1));
                p.DefineVar("vb", new Variable(aVal2));
                p.DefineVar("vc", new Variable(aVal3));
                p.DefineVar("vd", new Variable(aVal4));

                p.SetExpr(a_sExpr);
                IValue fRes = p.Eval();
                Assert.Fail("Evauluation of '" + a_sExpr + "' diddn't throw expected error: ( " + a_nErrc + " )");
            }
            catch(ParserError p)
            {
                Assert.AreEqual(p.GetCode(), a_nErrc);
                if (a_nPos != -1)
                    Assert.AreEqual(p.GetPos(), a_nPos);
                if (a_sIdent != null)
                    Assert.AreEqual(p.GetToken(), a_sIdent);
            }
        }

        public static void ValueThrowCheck<T>(bool v, IValue obj, Func<T> action)
        {
            if (v) Assert.ThrowsException<ParserError>(() => action());
            else try{ action(); }catch(ParserError e) { Assert.Fail(e.GetMsg()); }

        }
    }

    public class DbgSillyAdd : IOprtBin
    {
        public DbgSillyAdd() : base("++", EOprtPrecedence.prADD_SUB, EOprtAsct.oaLEFT) { }
        public override IToken Clone() => (DbgSillyAdd)MemberwiseClone();
        public override string GetDesc() => "internally used operator without special meaning for unit testing";

        public override void Eval(ref IValue ret, IValue[] arg)
        {
            Debug.Assert(arg.Length == 2);
            double a = arg[0].GetFloat();
            double b = arg[1].GetFloat();
            ret = a + b;
        }
    }

    public class FunTest0 : ICallback
    {
        public FunTest0() : base(ECmdCode.cmFUNC, "test0", 0) { }
        public override IToken Clone() => (FunTest0)MemberwiseClone();
        public override string GetDesc() => "";

        public override void Eval(ref IValue ret, IValue[] arg)
        {
            ret = 0;
        }
    }

}
