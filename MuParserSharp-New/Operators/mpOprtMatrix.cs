using MuParserSharp.Framework;
using MuParserSharp.Parser;
using MuParserSharp.Util;
using System.Linq;

namespace MuParserSharp.Operators
{
    

    class OprtTranspose : IOprtPostfix
    {
        public OprtTranspose() : base("'"){}

        public override IToken Clone() => (OprtTranspose)MemberwiseClone();

        public override string GetDesc() => "foo' - An operator for transposing a matrix.";

        public override void Eval(ref IValue ret, IValue[] a_pArg, int narg = -1)
        {
            if (a_pArg[0].IsMatrix())
            {
                var matrix = new Matrix(a_pArg[0].GetArray());
                matrix.Transpose();
                ret = matrix;
            }
            else
                ret = (IValue)a_pArg[0].Clone();
        }
    }
    class OprtCreateArray : ICallback
    {
        internal OprtCreateArray() : base(ECmdCode.cmCBC, "Arrayconstructor", -1){ }

        public override IToken Clone() => (OprtCreateArray)MemberwiseClone();

        public override string GetDesc() => "{,} - Array construction operator.";

        public override void Eval(ref IValue ret, IValue[] a_pArg, int narg = -1)
        {
            try
            {
                var len = narg;

                if (len <= 0)
                {
                    throw new ParserError(new ErrorContext(EErrorCodes.ecINVALID_PARAMETER, -1, GetIdent()));
                }

                var m = new Matrix(len, 1, 0.0);
                for (int i = 0; i < len; ++i)
                {
                    if (a_pArg[i].GetDim() != 0)
                    {
                        // Prevent people from using this constructor for matrix creation.
                        // This would not work as expected and i dont't want them
                        // to get used to awkward workarounds. It's just not working right now ok?
                        var errc = new ErrorContext(EErrorCodes.ecINVALID_PARAMETER, -1, GetIdent());
                        errc.Arg = i + 1;
                        throw new ParserError(errc);
                    }

                    m.At(i) = a_pArg[i];
                }
                m.Transpose();

                ret = m;
            }
            catch (ParserError exc)
            {
                exc.GetContext().Pos = GetExprPos();
                throw;
            }
        }
    }

    class OprtColon : IOprtBin
    {
        public OprtColon() : base("~", EOprtPrecedence.prCOLON, EOprtAsct.oaLEFT){}

        public override IToken Clone() => (OprtColon)MemberwiseClone();

        public override string GetDesc() => "{,} - Array construction operator.";

        public override void Eval(ref IValue ret, IValue[] a_pArg, int narg = -1)
        {
            Global.MUP_VERIFY(() => narg == 2);

            IValue argMin = a_pArg[0];
            IValue argMax = a_pArg[1];

            if (!argMin.IsNonComplexScalar())
                throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, -1, GetIdent(), argMin.GetValueType(), 'i', 1));

            if (!argMax.IsNonComplexScalar())
                throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, -1, GetIdent(), argMax.GetValueType(), 'i', 1));

            if (argMax < argMin)
                throw new ParserError("Colon operator: Maximum value smaller than Minimum!");

            long n = argMax.AsInteger() - argMin.AsInteger() + 1;
            var arr = new Matrix(n);
            for (long i = 0; i < n; ++i)
                arr.At(i) = argMin.AsFloat() + i;

            ret = arr;
        }

    }
}
