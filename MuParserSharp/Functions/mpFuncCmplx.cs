using System;
using System.Numerics;
using MuParserSharp.Framework;

namespace MuParserSharp.Functions
{
    class FunCmplxReal : ICallback
    {
        public FunCmplxReal() : base(ECmdCode.cmFUNC, "real"){}
        public override string GetDesc() => "real(x) - Returns the real part of the complex number x.";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            ret = a_pArg[0].GetReal();
        }

        public override IToken Clone() => (FunCmplxReal)MemberwiseClone();
    }

    class FunCmplxImag : ICallback
    {
        public FunCmplxImag() : base(ECmdCode.cmFUNC, "imag") { }
        public override string GetDesc() => "imag(x) - Returns the imaginary part of the complex number x.";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            ret = a_pArg[0].GetImag();
        }

        public override IToken Clone() => (FunCmplxImag)MemberwiseClone();
    }
    class FunCmplxConj : ICallback
    {
        public FunCmplxConj() : base(ECmdCode.cmFUNC, "conj") { }
        public override string GetDesc() => "conj(x) - Returns the complex conjugate of the complex number x.";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            var c = new Complex(a_pArg[0].GetReal(), -a_pArg[0].GetImag());

            const double EPSILON = 1e-16;
            double re = c.Real;
            double im = c.Imaginary;
            if (Math.Abs(re) < EPSILON) re = 0;
            if (Math.Abs(im) < EPSILON) im = 0;

            ret = new Complex(re, im);
        }

        public override IToken Clone() => (FunCmplxConj)MemberwiseClone();
    }
    class FunCmplxArg : ICallback
    {
        public FunCmplxArg() : base(ECmdCode.cmFUNC, "arg") { }
        public override string GetDesc() => "arg(x) - Returns the phase angle (or angular component) of the complex number x, expressed in radians.";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            var c = new Complex(a_pArg[0].GetReal(), a_pArg[0].GetImag());
            c = c.Phase;

            const double EPSILON = 1e-16;
            double re = c.Real;
            double im = c.Imaginary;
            if (Math.Abs(re) < EPSILON) re = 0;
            if (Math.Abs(im) < EPSILON) im = 0;

            ret = new Complex(re, im);
        }

        public override IToken Clone() => (FunCmplxArg)MemberwiseClone();
    }
    class FunCmplxNorm : ICallback
    {
        public FunCmplxNorm() : base(ECmdCode.cmFUNC, "norm") { }

        public override string GetDesc()
        {
            return "norm(x) - Returns the norm value of the complex number x." +
                    " The norm value of a complex number is the squared magnitude," +
                    " defined as the addition of the square of both the real part" +
                    " and the imaginary part (without the imaginary unit. This is" +
                    " the square of abs (x.";
        }
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            var c = new Complex(a_pArg[0].GetReal(), a_pArg[0].GetImag());
            ret = c.Magnitude * c.Magnitude;
            
        }

        public override IToken Clone() => (FunCmplxNorm)MemberwiseClone();
    }
    class FunCmplxCos : ICallback
    {
        public FunCmplxCos() : base(ECmdCode.cmFUNC, "cos") { }
        public override string GetDesc() => "cos(x) - Returns the cosine of the number x.";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            if (a_pArg[0].IsNonComplexScalar())
                ret = Math.Cos(a_pArg[0].AsFloat());
            else
            {
                var c = new Complex(a_pArg[0].GetReal(), a_pArg[0].GetImag());
                ret = Complex.Cos(c);
            }
        }

        public override IToken Clone() => (FunCmplxCos)MemberwiseClone();
    }
    class FunCmplxSin : ICallback
    {
        public FunCmplxSin() : base(ECmdCode.cmFUNC, "sin") { }
        public override string GetDesc() => "sin(x) - Returns the sine of the number x.";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            if (a_pArg[0].IsNonComplexScalar())
                ret = Math.Sin(a_pArg[0].AsFloat());
            else
            {
                var c = new Complex(a_pArg[0].GetReal(), a_pArg[0].GetImag());
                ret = Complex.Sin(c);
            }
        }

        public override IToken Clone() => (FunCmplxSin)MemberwiseClone();
    }
    class FunCmplxCosH : ICallback
    {
        public FunCmplxCosH() : base(ECmdCode.cmFUNC, "cosh") { }
        public override string GetDesc() => "cosh(x) - Returns the hyperbolic cosine of the number x.";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            if (a_pArg[0].IsNonComplexScalar())
                ret = Math.Cosh(a_pArg[0].AsFloat());
            else
            {
                var c = new Complex(a_pArg[0].GetReal(), a_pArg[0].GetImag());
                ret = Complex.Cosh(c);
            }
        }

        public override IToken Clone() => (FunCmplxCosH)MemberwiseClone();
    }
    class FunCmplxSinH : ICallback
    {
        public FunCmplxSinH() : base(ECmdCode.cmFUNC, "sinh") { }
        public override string GetDesc() => "sinh(x) - Returns the hyperbolic sine of the complex number x.";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            if (a_pArg[0].IsNonComplexScalar())
                ret = Math.Sinh(a_pArg[0].AsFloat());
            else
            {
                var c = new Complex(a_pArg[0].GetReal(), a_pArg[0].GetImag());
                ret = Complex.Sinh(c);
            }
        }

        public override IToken Clone() => (FunCmplxSinH)MemberwiseClone();
    }
    class FunCmplxTan : ICallback
    {
        public FunCmplxTan() : base(ECmdCode.cmFUNC, "tan") { }
        public override string GetDesc() => "tan(x) - Returns the tangens of the number x.";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            if (a_pArg[0].IsNonComplexScalar())
                ret = Math.Tan(a_pArg[0].AsFloat());
            else
            {
                var c = new Complex(a_pArg[0].GetReal(), a_pArg[0].GetImag());
                ret = Complex.Tan(c);
            }
        }

        public override IToken Clone() => (FunCmplxTan)MemberwiseClone();
    }
    class FunCmplxTanH : ICallback
    {
        public FunCmplxTanH() : base(ECmdCode.cmFUNC, "tanh") { }
        public override string GetDesc() => "tanh(x) - Returns the hyperbolic tangent of the complex number x.";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            if (a_pArg[0].IsNonComplexScalar())
                ret = Math.Tanh(a_pArg[0].AsFloat());
            else
            {
                var c = new Complex(a_pArg[0].GetReal(), a_pArg[0].GetImag());
                ret = Complex.Tanh(c);
            }
        }

        public override IToken Clone() => (FunCmplxTanH)MemberwiseClone();
    }
    class FunCmplxSqrt : ICallback
    {
        public FunCmplxSqrt() : base(ECmdCode.cmFUNC, "sqrt") { }
        public override string GetDesc() => "sqrt(x) - Returns the square root of x.";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            var arg1 = a_pArg[0];
            if (arg1.IsNonComplexScalar() && arg1.AsFloat() > 0)
                ret = Math.Sqrt(arg1.AsFloat());
            else
            {
                const double EPSILON = 1e-16;
                var c = new Complex(a_pArg[0].GetReal(), a_pArg[0].GetImag());
                c = Complex.Sqrt(c);
                double re = c.Real;
                double im = c.Imaginary;

                if (Math.Abs(re) < EPSILON) re = 0;
                if (Math.Abs(im) < EPSILON) im = 0;
                ret = new Complex(re, im);
            }
        }

        public override IToken Clone() => (FunCmplxSqrt)MemberwiseClone();
    }
    class FunCmplxExp : ICallback
    {
        public FunCmplxExp() : base(ECmdCode.cmFUNC, "exp") { }
        public override string GetDesc() => "exp(x) - Returns the base-e exponential of the complex number x.";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            if (a_pArg[0].IsNonComplexScalar())
                ret = Math.Pow(Math.E, a_pArg[0].AsFloat());
            else
            {
                var c = new Complex(a_pArg[0].GetReal(), a_pArg[0].GetImag());
                c = Complex.Exp(c);

                const double EPSILON = 1e-16;
                double re = c.Real;
                double im = c.Imaginary;

                if (Math.Abs(re) < EPSILON) re = 0;
                if (Math.Abs(im) < EPSILON) im = 0;
                ret = new Complex(re, im);
            }
        }

        public override IToken Clone() => (FunCmplxExp)MemberwiseClone();
    }
    class FunCmplxLn : ICallback
    {
        public FunCmplxLn() : base(ECmdCode.cmFUNC, "ln") { }
        public override string GetDesc() => "ln(x) - Returns the natural (base-e) logarithm of the complex number x.";

        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            var arg = a_pArg[0];
            if (arg.IsInteger())
                ret = Math.Log(arg.GetInteger());
            if (arg.IsNonComplexScalar())
                ret = Math.Log(arg.GetInteger());
            else
                ret = Complex.Log(arg.GetComplex());
        }

        public override IToken Clone() => (FunCmplxLn)MemberwiseClone();
    }
    class FunCmplxLog : ICallback
    {
        public FunCmplxLog() : base(ECmdCode.cmFUNC, "log") { }
        public override string GetDesc() => "log(x) - Common logarithm of x, for values of x greater than zero.";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            var arg = a_pArg[0];
            if (arg.IsInteger())
                ret = Math.Log(arg.GetInteger());
            if (arg.IsNonComplexScalar())
                ret = Math.Log(arg.GetInteger());
            else
                ret = Complex.Log(arg.GetComplex());
        }

        public override IToken Clone() => (FunCmplxLog)MemberwiseClone();
    }
    class FunCmplxLog10 : ICallback
    {
        public FunCmplxLog10() : base(ECmdCode.cmFUNC, "log10") { }
        public override string GetDesc() => "log10(x) - Common logarithm of x, for values of x greater than zero.";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            var arg = a_pArg[0];
            if (arg.IsInteger())
                ret = Math.Log10(arg.GetInteger());
            if (arg.IsNonComplexScalar())
                ret = Math.Log10(arg.GetInteger());
            else
                ret = Complex.Log10(arg.GetComplex());
        }

        public override IToken Clone() => (FunCmplxLog10)MemberwiseClone();
    }
    class FunCmplxLog2 : ICallback
    {
        public FunCmplxLog2() : base(ECmdCode.cmFUNC, "log2") { }
        public override string GetDesc() => "log2(x) - Logarithm to base 2 of x, for values of x greater than zero.";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            var arg = a_pArg[0];
            if (arg.IsInteger())
                ret = Math.Log(arg.GetInteger()) * (1 / Math.Log(2));
            if (arg.IsNonComplexScalar())
                ret = Math.Log(arg.GetInteger()) * (1 / Math.Log(2));
            else
                ret = Complex.Log(arg.GetComplex()) * (1 / Math.Log(2));

        }

        public override IToken Clone() => (FunCmplxLog2)MemberwiseClone();
    }
    class FunCmplxAbs : ICallback
    {
        public FunCmplxAbs() : base(ECmdCode.cmFUNC, "abs") { }
        public override string GetDesc() => "abs(x) - Returns the absolute value of x.";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            if (a_pArg[0].IsNonComplexScalar())
                ret = Math.Abs(a_pArg[0].AsFloat());
            else
            {
                double v = Math.Sqrt(a_pArg[0].GetReal() * a_pArg[0].GetReal() +
                                     a_pArg[0].GetImag() * a_pArg[0].GetImag());
                ret = a_pArg[0].GetComplex().Magnitude;
            }
        }

        public override IToken Clone() => (FunCmplxAbs)MemberwiseClone();
    }
    class FunCmplxPow : ICallback
    {
        public FunCmplxPow() : base(ECmdCode.cmFUNC, "pow", 2) { }
        public override string GetDesc() => "pow(x, y) - Raise x to the power of y.";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            var arg1 = a_pArg[0];
            var arg2 = a_pArg[0];
            if (arg1.IsNonComplexScalar() && arg2.IsNonComplexScalar())
                ret = Math.Pow(arg1.AsFloat(), arg2.AsFloat());
            else
                ret = Complex.Pow(a_pArg[0].GetComplex(), a_pArg[1].GetComplex());
        }

        public override IToken Clone() => (FunCmplxPow)MemberwiseClone();
    }
}
