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

        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            var type = a_pArg[0].GetValueType();
            try
            {
             
                if (type == 's')
                {
                    if (!a_pArg[1].IsInteger())
                    {
                        throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_IDX, -1, a_pArg[0].GetIdent())
                        {
                            Type1 = a_pArg[1].GetValueType(),
                            Type2 = 'i'
                        });
                    }

                    var idx = (int) a_pArg[1].GetInteger();
                    var str = a_pArg[0].GetString();
                    if(idx < 0 || idx > str.Length)
                        throw new ParserError(new ErrorContext(EErrorCodes.ecINDEX_OUT_OF_BOUNDS, -1, a_pArg[0].GetIdent()));
                    ret = str[idx];
                }
                else
                {
                    long rows = a_pArg[0].GetRows();
                    long cols = a_pArg[0].GetCols();
                    bool bArgIsVariable = a_pArg[0].IsVariable();
                    switch (a_pArg.Length - 1)
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
                                    ret = (new Variable(a_pArg[0].At(0, a_pArg[1])));
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

            }
            catch (ParserError exc)
            {
                exc.GetContext().Pos = GetExprPos();
                throw;
            }

        }
    }
}
