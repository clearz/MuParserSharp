using MuParserSharp.Framework;
using MuParserSharp.Parser;
using System.Linq;

namespace MuParserSharp.Operators
{
    class OprtIndex : ICallback
    {
        public OprtIndex() : base(ECmdCode.cmIC, "Index operator", -1){}

        public override IToken Clone() => (OprtIndex)MemberwiseClone();

        public override string GetDesc() => "[,] - The index operator.";

        public override void Eval(ref IValue ret, IValue[] a_pArg, int narg = -1)
        {
            try
            {
                long rows = a_pArg[0].GetRows();
                long cols = a_pArg[0].GetCols();
                bool bArgIsVariable = a_pArg[0].IsVariable();
                switch (narg)
                {
                    case 1:
                        if (cols == 1)
                        {
                            if (bArgIsVariable)
                                ret = new Variable(a_pArg[0].At(a_pArg[1], 0));
                            else
                                ret = a_pArg[0].At(a_pArg[1], 0);
                        }
                        else if (rows == 1)
                        {
                            if (bArgIsVariable)
                                ret = new Variable(a_pArg[0].At(0, a_pArg[1]));
                            else
                                ret = a_pArg[0].At(0, a_pArg[1]);
                        }
                        else
                        {
                            throw new ParserError(new ErrorContext(EErrorCodes.ecINDEX_DIMENSION, -1, GetIdent()));
                        }
                        break;

                    case 2:
                        if (bArgIsVariable)
                            ret = (new Variable(a_pArg[0].At(a_pArg[1], a_pArg[2])));
                        else
                            ret = a_pArg[0].At(a_pArg[1], a_pArg[2]);
                        break;

                    default:
                        throw new ParserError(new ErrorContext(EErrorCodes.ecINDEX_DIMENSION, -1, GetIdent()));
                }

            }
            catch (ParserError exc)
            {
                exc.GetContext().Pos = GetExprPos();
                throw;
            }

        }
    }
}
