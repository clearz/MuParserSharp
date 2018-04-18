using System;
using System.Linq;
using MuParserSharp.Framework;
using MuParserSharp.Parser;

namespace MuParserSharp.Functions
{
    

    class FunMatrixOnes : ICallback
    {
        public FunMatrixOnes() : base(ECmdCode.cmFUNC, "ones", -1) { }
        public override string GetDesc() => "ones(x[, y]) - Returns a matrix whose elements are all 1.";
        public override void Eval(ref IValue ret, IValue[] a_pArg, int narg = -1)
        {
            if (a_pArg.Length < 1 || a_pArg.Length > 2)
            {
                var err = new ErrorContext();
                err.Errc = EErrorCodes.ecINVALID_NUMBER_OF_PARAMETERS;
                err.Arg = a_pArg.Length;
                err.Ident = GetIdent();
                throw new ParserError(err);
            }
            long m = a_pArg[0].GetInteger(),
                n = (a_pArg.Length == 1) ? m : a_pArg[1].GetInteger();
            var mat = new Matrix((int)m, (int)n, 1);

            if (m == n && n == 1)
            {
                ret = 1.0;  // unboxing of 1x1 matrices
            }
            else
            {
                ret = mat;
            }
        }

        public override IToken Clone() => (FunMatrixOnes)MemberwiseClone();
    }
    class FunMatrixZeros : ICallback
    {
        public FunMatrixZeros() : base(ECmdCode.cmFUNC, "zeros", -1) { }
        public override string GetDesc() => "zeros(x [, y]) - Returns a matrix whose elements are all 0.";
        public override void Eval(ref IValue ret, IValue[] a_pArg, int narg = -1)
        {
            if (a_pArg.Length < 1 || a_pArg.Length > 2)
            {
                var err = new ErrorContext();
                err.Errc = EErrorCodes.ecINVALID_NUMBER_OF_PARAMETERS;
                err.Arg = a_pArg.Length;
                err.Ident = GetIdent();
                throw new ParserError(err);
            }
            long m = a_pArg[0].GetInteger(),
                n = (a_pArg.Length == 1) ? m : a_pArg[1].GetInteger();
            var mat = new Matrix((int)m, (int)n, 0);

            if (m == n && n == 1)
            {
                ret = 0.0;  // unboxing of 1x1 matrices
            }
            else
            {
                ret = mat;
            }
            
        }

        public override IToken Clone() => (FunMatrixZeros)MemberwiseClone();
    }
    class FunMatrixEye : ICallback
    {
        public FunMatrixEye() : base(ECmdCode.cmFUNC, "eye", -1) { }
        public override string GetDesc() => "eye(x, y) - returns a matrix with ones on its diagonal and zeros elsewhere.";
        public override void Eval(ref IValue ret, IValue[] a_pArg, int narg = -1)
        {
            if (a_pArg.Count(v => v != null && v.GetValueType() != '\0') < 1 || a_pArg.Count(v => v != null && v.GetValueType() != '\0') > 2)
            {
                var err = new ErrorContext();
                err.Errc = EErrorCodes.ecINVALID_NUMBER_OF_PARAMETERS;
                err.Arg = a_pArg.Length;
                err.Ident = GetIdent();
                throw new ParserError(err);
            }
            long m = a_pArg[0].GetInteger(),
                n = (narg == 1) ? m : a_pArg[1].GetInteger();
            var eye = new Matrix((int)m, (int)n, (Value)0.0);
            for (int i = 0; i < Math.Min(m, n); ++i)
            {
                eye.At(i, i) = 1.0;
            }
            ret = eye;
        }

        public override IToken Clone() => (FunMatrixEye)MemberwiseClone();
    }
    class FunMatrixSize : ICallback
    {
        public FunMatrixSize() : base(ECmdCode.cmFUNC, "size", -1) { }
        public override string GetDesc() => "size(x) - returns the matrix dimensions.";
        public override void Eval(ref IValue ret, IValue[] a_pArg, int narg = -1)
        {
            if (narg != 1)
            {
                var err = new ErrorContext();
                err.Errc = EErrorCodes.ecINVALID_NUMBER_OF_PARAMETERS;
                err.Arg = a_pArg.Length;
                err.Ident = GetIdent();
                throw new ParserError(err);
            }

            var sz = new Matrix(1, 2, (Value)0);
            sz.At(0) = a_pArg[0].GetRows();
            sz.At(0, 1) = a_pArg[0].GetCols();
            ret = sz;
        }

        public override IToken Clone() => (FunMatrixSize)MemberwiseClone();
    }
}
