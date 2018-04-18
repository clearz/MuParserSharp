using MuParserSharp.Framework;
using MuParserSharp.Parser;
using MuParserSharp.Util;
using System.Linq;

namespace MuParserSharp.Operators
{
    class OprtAssign : IOprtBin
    {
        public OprtAssign() : base("=", EOprtPrecedence.prASSIGN, EOprtAsct.oaLEFT) { }

        public override IToken Clone() => (OprtAssign)MemberwiseClone();

        public override string GetDesc() => "'=' assignement operator";

        public override void Eval(ref IValue ret, IValue[] a_pArg, int narg = -1)
        {
            Global.MUP_VERIFY(() => narg == 2);
            if (!a_pArg[0].IsVariable())
                throw new ParserError(
                    new ErrorContext {Arg = 1, Ident = "=", Errc = EErrorCodes.ecASSIGNEMENT_TO_VALUE});

           
            ret = a_pArg[0].Assign(a_pArg[1]);
        }
    }

    class OprtAssignAdd : IOprtBin
    {
        public OprtAssignAdd() : base("+=", EOprtPrecedence.prASSIGN, EOprtAsct.oaLEFT) { }

        public override IToken Clone() => (OprtAssignAdd)MemberwiseClone();

        public override string GetDesc() => "'+=' addition assignement operator";

        public override void Eval(ref IValue ret, IValue[] a_pArg, int narg = -1)
        {
            Global.MUP_VERIFY(() => narg == 2);
            if (!a_pArg[0].IsVariable())
                throw new ParserError(
                    new ErrorContext { Arg = 1, Ident = "+=", Errc = EErrorCodes.ecASSIGNEMENT_TO_VALUE });

            
            ret = a_pArg[0].Assign(a_pArg[0] + a_pArg[1]);

        }
    }

    class OprtAssignSub : IOprtBin
    {
        public OprtAssignSub() : base("-=", EOprtPrecedence.prASSIGN, EOprtAsct.oaLEFT) { }

        public override IToken Clone() => (OprtAssignSub)MemberwiseClone();

        public override string GetDesc() => "'-=' subtraction assignement operator";

        public override void Eval(ref IValue ret, IValue[] a_pArg, int narg = -1)
        {
            Global.MUP_VERIFY(() => narg == 2);
            if (!a_pArg[0].IsVariable())
                throw new ParserError(
                    new ErrorContext { Arg = 1, Ident = "-=", Errc = EErrorCodes.ecASSIGNEMENT_TO_VALUE });


            ret = a_pArg[0].Assign(a_pArg[0] - a_pArg[1]);

        }
    }

    class OprtAssignMul : IOprtBin
    {
        public OprtAssignMul() : base("*=", EOprtPrecedence.prASSIGN, EOprtAsct.oaLEFT) { }

        public override IToken Clone() => (OprtAssignMul)MemberwiseClone();

        public override string GetDesc() => "'*=' multiply assignement operator";

        public override void Eval(ref IValue ret, IValue[] a_pArg, int narg = -1)
        {
            Global.MUP_VERIFY(() => narg == 2);
            if (!a_pArg[0].IsVariable())
                throw new ParserError(
                    new ErrorContext { Arg = 1, Ident = "*=", Errc = EErrorCodes.ecASSIGNEMENT_TO_VALUE });


            ret = a_pArg[0].Assign(a_pArg[0] * a_pArg[1]);

        }
    }

    class OprtAssignDiv : IOprtBin
    {
        public OprtAssignDiv() : base("/=", EOprtPrecedence.prASSIGN, EOprtAsct.oaLEFT) { }

        public override IToken Clone() => (OprtAssignDiv)MemberwiseClone();

        public override string GetDesc() => "'/=' divide assignement operator";

        public override void Eval(ref IValue ret, IValue[] a_pArg, int narg = -1)
        {
            Global.MUP_VERIFY(() => narg == 2);

            if (!a_pArg[0].IsVariable())
                throw new ParserError(
                    new ErrorContext { Arg = 1, Ident = "/=", Errc = EErrorCodes.ecASSIGNEMENT_TO_VALUE });


            ret = a_pArg[0].Assign(a_pArg[0] / a_pArg[1]);
        }
    }
}
