using System;
using System.Linq;
using MuParserSharp.Framework;
using MuParserSharp.Util;

namespace MuParserSharp.Functions
{
    class FunSin : ICallback
    {
        public FunSin() : base(ECmdCode.cmFUNC, "sin") { }
        public override string GetDesc() => "sin(x), sine function";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(() => a_pArg.Length == 1);
            ret = Math.Sin(a_pArg[0].AsFloat());
        }

        public override IToken Clone() => (FunSin)MemberwiseClone();
    }

    class FunCos : ICallback
    {
        public FunCos() : base(ECmdCode.cmFUNC, "cos") { }
        public override string GetDesc() => "cos(x), cosine function";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(() => a_pArg.Length == 1);
            ret = Math.Cos(a_pArg[0].AsFloat());
        }

        public override IToken Clone() => (FunCos)MemberwiseClone();
    }
    class FunTan : ICallback
    {
        public FunTan() : base(ECmdCode.cmFUNC, "tan") { }
        public override string GetDesc() => "tan(x), tangent function";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(() => a_pArg.Length == 1);
            ret = Math.Tan(a_pArg[0].AsFloat());
        }

        public override IToken Clone() => (FunTan)MemberwiseClone();
    }
    class FunASin : ICallback
    {
        public FunASin() : base(ECmdCode.cmFUNC, "asin") { }
        public override string GetDesc() => "asin(x), arc sine";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(() => a_pArg.Length == 1);
            ret = Math.Asin(a_pArg[0].AsFloat());
        }

        public override IToken Clone() => (FunASin)MemberwiseClone();
    }

    class FunACos : ICallback
    {
        public FunACos() : base(ECmdCode.cmFUNC, "acos") { }
        public override string GetDesc() => "acos(x), arc cosine";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(() => a_pArg.Length == 1);
            ret = Math.Acos(a_pArg[0].AsFloat());
        }

        public override IToken Clone() => (FunACos)MemberwiseClone();
    }

    class FunATan : ICallback
    {
        public FunATan() : base(ECmdCode.cmFUNC, "atan") { }
        public override string GetDesc() => "atan(x), arc tangent";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(() => a_pArg.Length == 1);
            ret = Math.Atan(a_pArg[0].AsFloat());
        }

        public override IToken Clone() => (FunATan)MemberwiseClone();
    }

    class FunSinH : ICallback
    {
        public FunSinH() : base(ECmdCode.cmFUNC, "sinh") { }
        public override string GetDesc() => "sinh(x), hyperbolic sine";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(() => a_pArg.Length == 1);
            ret = Math.Sinh(a_pArg[0].AsFloat());
        }

        public override IToken Clone() => (FunSinH)MemberwiseClone();
    }

    class FunCosH : ICallback
    {
        public FunCosH() : base(ECmdCode.cmFUNC, "cosh") { }
        public override string GetDesc() => "cosh(x), hyperbolic cosine";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(() => a_pArg.Length == 1);
            ret = Math.Cosh(a_pArg[0].AsFloat());
        }

        public override IToken Clone() => (FunCosH)MemberwiseClone();
    }

    class FunTanH : ICallback
    {
        public FunTanH() : base(ECmdCode.cmFUNC, "tanh") { }
        public override string GetDesc() => "tanh(x), hyperbolic tangent";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(() => a_pArg.Length == 1);
            ret = Math.Tanh(a_pArg[0].AsFloat());
        }

        public override IToken Clone() => (FunTanH)MemberwiseClone();
    }

    class FunASinH : ICallback
    {
        public FunASinH() : base(ECmdCode.cmFUNC, "asinh") { }
        public override string GetDesc() => "asinh(x), hyperbolic arc sine";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(() => a_pArg.Length == 1);
            var x = a_pArg[0].AsFloat();
            ret = Math.Log(x + Math.Sqrt(x * x + 1));
        }

        public override IToken Clone() => (FunASinH)MemberwiseClone();
    }

    class FunACosH : ICallback
    {
        public FunACosH() : base(ECmdCode.cmFUNC, "acosh") { }
        public override string GetDesc() => "acosh(x), hyperbolic arc cosine";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(() => a_pArg.Length == 1);
            var x = a_pArg[0].AsFloat();
            ret = Math.Log(x + Math.Sqrt(x * x - 1));
        }

        public override IToken Clone() => (FunACosH)MemberwiseClone();
    }

    class FunATanH : ICallback
    {
        public FunATanH() : base(ECmdCode.cmFUNC, "atanh") { }
        public override string GetDesc() => "atanh(x), hyperbolic arc tangent";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(() => a_pArg.Length == 1);
            var x = a_pArg[0].AsFloat();
            ret = Math.Log((1 + x) / (1 - x)) / 2;
        }

        public override IToken Clone() => (FunATanH)MemberwiseClone();
    }

    class FunLog : ICallback
    {
        public FunLog() : base(ECmdCode.cmFUNC, "log") { }
        public override string GetDesc() => "log(x), Natural logarithm";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(() => a_pArg.Length == 1);
            ret = Math.Log(a_pArg[0].AsFloat());
        }

        public override IToken Clone() => (FunLog)MemberwiseClone();
    }

    class FunLog10 : ICallback
    {
        public FunLog10() : base(ECmdCode.cmFUNC, "log10") { }
        public override string GetDesc() => "log10(x), Base 10 Logarithm";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(() => a_pArg.Length == 1);
            ret = Math.Log10(a_pArg[0].AsFloat());
        }

        public override IToken Clone() => (FunLog10)MemberwiseClone();
    }

    class FunLog2 : ICallback
    {
        const double LOG2 = 0.693147180559945;
        public FunLog2() : base(ECmdCode.cmFUNC, "log2") { }
        public override string GetDesc() => "log2(x), Base 2 Logarithm";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(() => a_pArg.Length == 1);
            ret = Math.Log(a_pArg[0].AsFloat()) / LOG2;
        }

        public override IToken Clone() => (FunLog2)MemberwiseClone();
    }

    class FunLn : ICallback
    {
        public FunLn() : base(ECmdCode.cmFUNC, "ln") { }
        public override string GetDesc() => "ln(x), Natural logarithm";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(() => a_pArg.Length == 1);
            ret = Math.Log(a_pArg[0].AsFloat());
        }

        public override IToken Clone() => (FunLn)MemberwiseClone();
    }

    class FunSqrt : ICallback
    {
        public FunSqrt() : base(ECmdCode.cmFUNC, "sqrt") { }
        public override string GetDesc() => "sqrt(x), Square Root";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(() => a_pArg.Length == 1);
            ret = Math.Sqrt(a_pArg[0].AsFloat());
        }

        public override IToken Clone() => (FunSqrt)MemberwiseClone();
    }

    class FunCbrt : ICallback
    {
        private const double ONE_THIRD = 0.3333333333333333;
        public FunCbrt() : base(ECmdCode.cmFUNC, "cbrt") { }
        public override string GetDesc() => "cbrt(x), Cube Root";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(() => a_pArg.Length == 1);
            ret = Math.Pow(a_pArg[0].AsFloat(), ONE_THIRD);
        }

        public override IToken Clone() => (FunCbrt)MemberwiseClone();
    }

    class FunExp : ICallback
    {
        public FunExp() : base(ECmdCode.cmFUNC, "exp") { }
        public override string GetDesc() => "exp(x), Exponential (e^x)";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(() => a_pArg.Length == 1);
            ret = Math.Exp(a_pArg[0].AsFloat());
        }

        public override IToken Clone() => (FunExp)MemberwiseClone();
    }

    class FunAbs : ICallback
    {
        public FunAbs() : base(ECmdCode.cmFUNC, "abs") { }
        public override string GetDesc() => "abs(x), Absolute Value";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(() => a_pArg.Length == 1);
            ret = Math.Abs(a_pArg[0].AsFloat());
        }

        public override IToken Clone() => (FunAbs)MemberwiseClone();
    }

    class FunPow : ICallback
    {
        public FunPow() : base(ECmdCode.cmFUNC, "pow", 2) { }
        public override string GetDesc() => "pow(x, y) - raise x to the power of y";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(() => a_pArg.Length == 2);
            ret = Math.Pow(a_pArg[0].AsFloat(), a_pArg[1].AsFloat());
        }

        public override IToken Clone() => (FunPow)MemberwiseClone();
    }

    class FunHypot : ICallback
    {
        public FunHypot() : base(ECmdCode.cmFUNC, "hypot", 2) { }
        public override string GetDesc() => "hypot(x, y) - compute the length of the vector x,y";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(() => a_pArg.Length == 2);
            double d1 = a_pArg[0].AsFloat(), d2 = a_pArg[1].AsFloat();
            ret = Math.Sqrt(d1*d1+d2*d2);
        }

        public override IToken Clone() => (FunHypot)MemberwiseClone();
    }

    class FunAtan2 : ICallback
    {
        public FunAtan2() : base(ECmdCode.cmFUNC, "atan2", 2) { }
        public override string GetDesc() => "arc tangent with quadrant fix";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(() => a_pArg.Length == 2);
            ret = Math.Atan2(a_pArg[0].AsFloat(), a_pArg[1].AsFloat());
        }

        public override IToken Clone() => (FunAtan2)MemberwiseClone();
    }

    class FunFmod : ICallback
    {
        public FunFmod() : base(ECmdCode.cmFUNC, "fmod", 2) { }
        public override string GetDesc() => "fmod(x, y) - floating point remainder of x / y";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(() => a_pArg.Length == 2);
            ret = a_pArg[0].AsFloat() % a_pArg[1].AsFloat();
        }

        public override IToken Clone() => (FunFmod)MemberwiseClone();
    }

    class FunRemainder : ICallback
    {
        public FunRemainder() : base(ECmdCode.cmFUNC, "remainder", 2) { }
        public override string GetDesc() => "remainder(x, y) - IEEE remainder of x / y";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(() => a_pArg.Length == 2);
            ret = Math.IEEERemainder(a_pArg[0].AsFloat(), a_pArg[1].AsFloat());
        }

        public override IToken Clone() => (FunRemainder)MemberwiseClone();
    }
}
