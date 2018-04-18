using System;
using System.Linq;
using System.Numerics;
using MuParserSharp.Framework;
using MuParserSharp.Parser;
using MuParserSharp.Util;

namespace MuParserSharp.Operators
{

    class OprtSignCmplx : IOprtInfix
    {
        public OprtSignCmplx() : base("-", EOprtPrecedence.prINFIX)
        {
        }

        public override IToken Clone() => (OprtSignCmplx)MemberwiseClone();

        public override string GetDesc() => "negative sign operator";

        public override void Eval(ref IValue ret, IValue[] a_pArg, int narg = -1)
        {
            Global.MUP_VERIFY(() => narg == 1);

            if (a_pArg[0].IsScalar())
            {
                double re = a_pArg[0].AsFloat();
                double im = a_pArg[0].GetImag();

                // Do not omit the test for zero! Multiplying 0 with -1 
                // will yield -0 on IEEE754 compliant implementations!
                // This would change the result of complex calculations:
                // 
                // i.e. sqrt(-1 + (-0)i) !=  sqrt(-1 + 0i)
                //                   -i  !=  i  
                var v = new Complex(re == 0 ? 0 : -re, im == 0 ? 0 : -im);
                ret = v;
            }
            else if (a_pArg[0].IsMatrix())
            {
                var val = new Matrix(a_pArg[0].GetArray());

                for (int n = 0; n < a_pArg[0].GetRows(); ++n)
                for (int m = 0; m < a_pArg[0].GetCols(); ++m)
                {
                    val *= -1;
                }

                ret = val;
            }
            else
            {
                var err = new ErrorContext();
                err.Errc = EErrorCodes.ecINVALID_TYPE;
                err.Type1 = a_pArg[0].GetValueType();
                err.Type2 = 's';
                throw new ParserError(err);
            }

        }
    }

    class OprtAddCmplx : IOprtBin
       {
           public OprtAddCmplx() : base("+", EOprtPrecedence.prADD_SUB, EOprtAsct.oaLEFT){}

           public override IToken Clone() => (OprtAddCmplx)MemberwiseClone();

           public override string GetDesc() => "addition";

           public override void Eval(ref IValue ret, IValue[] a_pArg, int narg = -1)
        {
            Global.MUP_VERIFY(() => narg == 2);

            if (a_pArg[0].IsNonComplexScalar() && a_pArg[1].IsNonComplexScalar())
            {
                if (a_pArg[0].IsInteger() && a_pArg[1].IsInteger())
                {
                    ret = a_pArg[0].AsInteger() + a_pArg[1].AsInteger();
                }
                else
                {
                    ret = a_pArg[0].AsFloat() + a_pArg[1].AsFloat();
                }
            }
            else if (a_pArg[0].IsMatrix() && a_pArg[1].IsMatrix())
            {
                ret = a_pArg[0].GetArray() + a_pArg[1].GetArray();
            }
            else
            {


                if (!a_pArg[0].IsScalar())
                    throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, GetExprPos(), GetIdent(), a_pArg[0].GetValueType(), 'z', 1));

                if (!a_pArg[1].IsScalar())
                    throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, GetExprPos(), GetIdent(), a_pArg[1].GetValueType(), 'z', 2));

                // addition of two imaginary numbers      
                ret = new Complex(a_pArg[0].AsFloat() + a_pArg[1].AsFloat(), a_pArg[0].GetImag() + a_pArg[1].GetImag());
            }
        }
    }

       class OprtSubCmplx : IOprtBin
    {
        public OprtSubCmplx() : base("-", EOprtPrecedence.prADD_SUB, EOprtAsct.oaLEFT) { }

        public override IToken Clone() => (OprtSubCmplx)MemberwiseClone();

        public override string GetDesc() => "subtraction";

        public override void Eval(ref IValue ret, IValue[] a_pArg, int narg = -1)
        {
            Global.MUP_VERIFY(() => narg == 2);

            if (a_pArg[0].IsNonComplexScalar() && a_pArg[1].IsNonComplexScalar())
            {
                if (a_pArg[0].IsInteger() && a_pArg[1].IsInteger())
                {
                    ret = a_pArg[0].AsInteger() - a_pArg[1].AsInteger();
                }
                else
                {
                    ret = a_pArg[0].AsFloat() - a_pArg[1].AsFloat();
                }
            }
            else if (a_pArg[0].IsMatrix() && a_pArg[1].IsMatrix())
            {
                ret = a_pArg[0].GetArray() - a_pArg[1].GetArray();
            }
            else
            {


                if (!a_pArg[0].IsScalar())
                    throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, GetExprPos(), GetIdent(), a_pArg[0].GetValueType(), 'z', 1));

                if (!a_pArg[1].IsScalar())
                    throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, GetExprPos(), GetIdent(), a_pArg[1].GetValueType(), 'z', 2));

                // subtraction of two imaginary numbers      
                ret = new Complex(a_pArg[0].AsFloat()-a_pArg[1].AsFloat(), a_pArg[0].GetImag() - a_pArg[1].GetImag());
            }
        }
    }

    class OprtMulCmplx : IOprtBin
    {
        public OprtMulCmplx() : base("*", EOprtPrecedence.prMUL_DIV, EOprtAsct.oaLEFT) { }

        public override IToken Clone() => (OprtMulCmplx)MemberwiseClone();

        public override string GetDesc() => "multiplication";

        public override void Eval(ref IValue ret, IValue[] a_pArg, int narg = -1)
        {
            Global.MUP_VERIFY(() => narg == 2);

            if (a_pArg[0].IsNonComplexScalar() && a_pArg[1].IsNonComplexScalar())
            {
                if (a_pArg[0].IsInteger() && a_pArg[1].IsInteger())
                {
                    ret = a_pArg[0].AsInteger() * a_pArg[1].AsInteger();
                }
                else
                {
                    ret = a_pArg[0].AsFloat() * a_pArg[1].AsFloat();
                }
            }
            else if (a_pArg[0].IsComplex() && a_pArg[1].IsComplex())
            {
                // multiplication of two imaginary numbers      
                ret = Complex.Multiply(a_pArg[0].GetComplex(), a_pArg[1].GetComplex());
            }

            else if (a_pArg[0].IsComplex())
            {
                // multiplication of two imaginary numbers      
                ret = Complex.Multiply(a_pArg[0].GetComplex(), a_pArg[1].AsComplex());
            }

            else if (a_pArg[1].IsComplex())
            {
                // multiplication of two imaginary numbers      
                ret = Complex.Multiply(a_pArg[0].AsComplex(), a_pArg[1].GetComplex());
            }
            else 
            {
                ret = a_pArg[0] * a_pArg[1];
            }

            if (ret.IsComplex())
            {
                const double EPSILON = 1e-16;
                Complex c = ret.GetComplex();
                double re = c.Real;
                double im = c.Imaginary;
                if (Math.Abs(re) < EPSILON) re = 0;
                if (Math.Abs(im) < EPSILON) im = 0;
                ret = new Complex(re, im);
            }
        }
    }

       class OprtDivCmplx : IOprtBin
    {
        public OprtDivCmplx() : base("/", EOprtPrecedence.prMUL_DIV, EOprtAsct.oaLEFT) { }

        public override IToken Clone() => (OprtDivCmplx)MemberwiseClone();

        public override string GetDesc() => "division";

        public override void Eval(ref IValue ret, IValue[] a_pArg, int narg = -1)
        {
            Global.MUP_VERIFY(() => narg == 2);

            if (a_pArg[0].IsNonComplexScalar() && a_pArg[1].IsNonComplexScalar())
            {
                //if (a_pArg[0].IsInteger() && a_pArg[1].IsInteger())
                //{
                //    ret = a_pArg[0].AsInteger() / a_pArg[1].AsInteger();
                //}
                //else
                //{
                //}

                ret = a_pArg[0].AsFloat() / a_pArg[1].AsFloat();
            }
            else
            {
                // multiplication of two imaginary numbers      
                double a = a_pArg[0].GetFloat(),
                    b = a_pArg[0].GetImag(),
                    c = a_pArg[1].GetFloat(),
                    d = a_pArg[1].GetImag(),
                    n = c * c + d * d;
                ret = new Complex((a * c + b * d) / n, (b * c - a * d) / n);
            }
        }
    }

       class OprtPowCmplx : IOprtBin
    {
        public OprtPowCmplx() : base("^", EOprtPrecedence.prPOW, EOprtAsct.oaRIGHT) { }

        public override IToken Clone() => (OprtPowCmplx)MemberwiseClone();

        public override string GetDesc() => "raise x to the power of y";

        public override void Eval(ref IValue ret, IValue[] arg, int narg = -1)
        {
            Global.MUP_VERIFY(() => arg.Count(v => v != null) == 2);

            if (arg[0].IsComplex() && arg[1].IsComplex())
            {
                ret = Complex.Pow(arg[0].GetComplex(), arg[1].GetComplex()); ;
            }

            else if (arg[0].IsComplex())
            {
                // Exponentation of two complex numbers      
                ret = Complex.Pow(arg[0].GetComplex(), arg[1].AsComplex());
            }

            else if (arg[1].IsComplex())
            {
                // Exponentation of two complex numbers      
                ret = Complex.Pow(arg[0].AsComplex(), arg[1].GetComplex());
            }
            else if (arg[0].IsInteger() && arg[1].IsInteger())
            {
                if (arg[0].AsInteger() < 0)
                    ret = Complex.Pow(arg[0].AsComplex(), arg[1].AsComplex());
                else
                    ret = (long)Math.Pow(arg[0].GetInteger(), arg[1].GetInteger());
            }
            else
            {
                if (arg[0].AsFloat() < 0)
                    ret = Complex.Pow(arg[0].AsComplex(), arg[1].AsComplex());
                else
                    ret = Math.Pow(arg[0].AsFloat(), arg[1].AsFloat());
            }

            if (ret.IsComplex())
            {
                const double EPSILON = 1e-14;
                Complex c = ret.GetComplex();
                double re = c.Real;
                double im = c.Imaginary;

                if (Math.Abs(re) < EPSILON) re = 0;
                if (Math.Abs(im) < EPSILON) im = 0;
                ret = new Complex(re, im);
            }
        }
    }
       
}