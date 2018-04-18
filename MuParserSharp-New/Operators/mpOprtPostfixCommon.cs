using System;
using MuParserSharp.Framework;
using MuParserSharp.Parser;

namespace MuParserSharp.Operators
{
    class OprtFact : IOprtPostfix
    {
        public OprtFact() : base("!"){}

        public override IToken Clone() => (OprtFact)MemberwiseClone();

        public override string GetDesc() => "x! - Returns factorial of a non-negative integer.";

        public override void Eval(ref IValue ret, IValue[] a_pArg, int narg = -1)
        {
            char type = a_pArg[0].GetValueType();
            if (!a_pArg[0].IsInteger())
                throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, GetExprPos(), GetIdent(), type, 'i', 1));
            var input = a_pArg[0].AsInteger();

            if (input < 0)
                throw new ParserError(new ErrorContext(EErrorCodes.ecDOMAIN_ERROR, GetExprPos(), GetIdent()));
            checked
            {
                try
                {
                    long fact(long i) => i < 2 ? 1 : i * fact(i - 1);
                    ret = fact(input);
                }
                catch (OverflowException)
                {
                    throw new ParserError(new ErrorContext(EErrorCodes.ecOVERFLOW, GetExprPos(), GetIdent()));
                }
            }
        }
    }

    class OprtPercentage : IOprtPostfix
    {
        public OprtPercentage() : base("%"){}

        public override IToken Clone() => (OprtPercentage)MemberwiseClone();

        public override string GetDesc() => "x% - Returns percentage of integer/float.";

        public override void Eval(ref IValue ret, IValue[] a_pArg, int narg = -1)
        {
            char type = a_pArg[0].GetValueType();
            if (type == 'i' || type == 'f')
            {
                var input = a_pArg[0].AsFloat();
                ret = input / 100.0;
            }
            else
                throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, GetExprPos(), GetIdent(), type, 'f', 1));
        }
    }
}
