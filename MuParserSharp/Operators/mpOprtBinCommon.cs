using System;
using System.Linq;
using MuParserSharp.Framework;
using MuParserSharp.Parser;
using MuParserSharp.Util;

namespace MuParserSharp.Operators
{
    class OprtStrAdd : IOprtBin
    {
        public OprtStrAdd() : base("//", EOprtPrecedence.prADD_SUB, EOprtAsct.oaLEFT) { }

        public override string GetDesc() => "string concatenation";

        public override void Eval(ref IValue ret, IValue[] arg)
        {
            Global.MUP_VERIFY(arg.Length == 2);
            if (!arg[0].IsString())
                throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, GetExprPos(), arg[0].GetIdent(), arg[0].GetValueType(), 's', 2));
            if (!arg[1].IsString())
                throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, GetExprPos(), arg[1].GetIdent(), arg[1].GetValueType(), 's', 2));

            string a = arg[0].GetString();
            string b = arg[1].GetString();
            ret = a + b;
        }
        Matrix m = new Matrix(new Value[]{ 1, 2, 3, 4 });
        public override IToken Clone() => (OprtStrAdd)MemberwiseClone();
    }
    class OprtEQ : IOprtBin
    {
        public OprtEQ() : base("==", EOprtPrecedence.prRELATIONAL1, EOprtAsct.oaLEFT) { }

        public override string GetDesc() => "equals operator";

        public override void Eval(ref IValue ret, IValue[] arg)
        {
            Global.MUP_VERIFY(arg.Length == 2);

            ret = arg[0] == arg[1];
        }
        public override IToken Clone() => (OprtEQ)MemberwiseClone();
    }
    class OprtNEQ : IOprtBin
    {
        public OprtNEQ() : base("!=", EOprtPrecedence.prRELATIONAL1, EOprtAsct.oaLEFT) { }

        public override string GetDesc() => "not equal operator";

        public override void Eval(ref IValue ret, IValue[] arg)
        {
            Global.MUP_VERIFY(arg.Length == 2);

            ret = arg[0] != arg[1];
        }
        public override IToken Clone() => (OprtNEQ)MemberwiseClone();
    }
    class OprtLT : IOprtBin
    {
        public OprtLT() : base("<", EOprtPrecedence.prRELATIONAL2, EOprtAsct.oaLEFT) { }

        public override string GetDesc() => "less than operator";

        public override void Eval(ref IValue ret, IValue[] arg)
        {
            Global.MUP_VERIFY(arg.Length == 2);

            ret = arg[0] < arg[1];
        }
        public override IToken Clone() => (OprtLT)MemberwiseClone();
    }
    class OprtGT : IOprtBin
    {
        public OprtGT() : base(">", EOprtPrecedence.prRELATIONAL2, EOprtAsct.oaLEFT) { }

        public override string GetDesc() => "greater than operator";

        public override void Eval(ref IValue ret, IValue[] arg)
        {
            Global.MUP_VERIFY(arg.Length == 2);

            ret = arg[0] > arg[1];
        }
        public override IToken Clone() => (OprtGT)MemberwiseClone();
    }
    class OprtLE : IOprtBin
    {
        public OprtLE() : base("<=", EOprtPrecedence.prRELATIONAL2, EOprtAsct.oaLEFT) { }

        public override string GetDesc() => "less or equal operator";

        public override void Eval(ref IValue ret, IValue[] arg)
        {
            Global.MUP_VERIFY(arg.Length == 2);

            ret = arg[0] <= arg[1];
        }
        public override IToken Clone() => (OprtLE)MemberwiseClone();
    }
    class OprtGE : IOprtBin
    {
        public OprtGE() : base(">=", EOprtPrecedence.prRELATIONAL2, EOprtAsct.oaLEFT) { }

        public override string GetDesc() => "greater or equal operator";

        public override void Eval(ref IValue ret, IValue[] arg)
        {
            Global.MUP_VERIFY(arg.Length == 2);

            ret = arg[0] >= arg[1];
        }
        public override IToken Clone() => (OprtGE)MemberwiseClone();
    }

    class OprtAnd : IOprtBin
    {
        public OprtAnd() : base("&", EOprtPrecedence.prBIT_AND, EOprtAsct.oaLEFT) { }

        public override string GetDesc() => "bitwize and";

        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(a_pArg.Length == 2);
            if (!a_pArg[0].IsInteger())
                throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, GetExprPos(), a_pArg[0].GetIdent(), a_pArg[0].GetValueType(), 'i', 1));

            if (!a_pArg[1].IsInteger())
                throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, GetExprPos(), a_pArg[1].GetIdent(), a_pArg[1].GetValueType(), 'i', 2));

            var a = (int)a_pArg[0].AsInteger();
            var b = (int)a_pArg[1].AsInteger();

            ret = a & b;
        }
        public override IToken Clone() => (OprtAnd)MemberwiseClone();
    }

    class OprtOr : IOprtBin
    {
        public OprtOr() : base("|", EOprtPrecedence.prBIT_OR, EOprtAsct.oaLEFT) { }

        public override string GetDesc() => "bitwize or";

        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(a_pArg.Length == 2);
            if (!a_pArg[0].IsInteger())
                throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, GetExprPos(), a_pArg[0].GetIdent(), a_pArg[0].GetValueType(), 'i', 1));

            if (!a_pArg[1].IsInteger())
                throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, GetExprPos(), a_pArg[1].GetIdent(), a_pArg[1].GetValueType(), 'i', 2));

            var a = (int)a_pArg[0].AsInteger();
            var b = (int)a_pArg[1].AsInteger();

            ret = a | b;
        }
        public override IToken Clone() => (OprtOr)MemberwiseClone();
    }

    class OprtLOr : IOprtBin
    {
        public OprtLOr(string or = "||") : base(or, EOprtPrecedence.prLOGIC_OR, EOprtAsct.oaLEFT) { }

        public override string GetDesc() => "";

        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(a_pArg.Length == 2);

            if (a_pArg[0].GetValueType() != 'b')
                throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, GetExprPos(), a_pArg[0].GetIdent(), a_pArg[0].GetValueType(), 'b', 1));

            if (a_pArg[1].GetValueType() != 'b')
                throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, GetExprPos(), a_pArg[1].GetIdent(), a_pArg[1].GetValueType(), 'b', 2));

            var a = a_pArg[0].GetBool();
            var b = a_pArg[1].GetBool();

            ret = a || b;
        }
        public override IToken Clone() => (OprtLOr)MemberwiseClone();
    }
    class OprtLAnd : IOprtBin
    {
        public OprtLAnd(string and = "&&") : base(and, EOprtPrecedence.prLOGIC_AND, EOprtAsct.oaLEFT) { }

        public override string GetDesc() => "logical and";

        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(a_pArg.Length == 2);

            if (a_pArg[0].GetValueType() != 'b')
                throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, GetExprPos(), a_pArg[0].GetIdent(), a_pArg[0].GetValueType(), 'b', 1));

            if (a_pArg[1].GetValueType() != 'b')
                throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, GetExprPos(), a_pArg[1].GetIdent(), a_pArg[1].GetValueType(), 'b', 2));

            var a = a_pArg[0].GetBool();
            var b = a_pArg[1].GetBool();

            ret = a && b;
        }
        public override IToken Clone() => (OprtLAnd)MemberwiseClone();
    }
    class OprtShl : IOprtBin
    {
        public OprtShl() : base("<<", EOprtPrecedence.prSHIFT, EOprtAsct.oaLEFT) { }

        public override string GetDesc() => "shift left";

        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
             Global.MUP_VERIFY(a_pArg.Length == 2);

            if (!a_pArg[0].IsScalar())
                throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, GetExprPos(), GetIdent(), a_pArg[0].GetValueType(), 'i', 1));

            if (!a_pArg[1].IsScalar())
                throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, GetExprPos(), GetIdent(), a_pArg[1].GetValueType(), 'i', 2));

            if (!a_pArg[0].IsInteger())
                throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, GetExprPos(), a_pArg[0].GetIdent(), a_pArg[0].GetValueType(), 'i', 1));

            if (!a_pArg[1].IsInteger())
                throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, GetExprPos(), a_pArg[1].GetIdent(), a_pArg[1].GetValueType(), 'i', 2));

            checked
            {
                try
                {
                    var a = (long)a_pArg[0].AsInteger();
                    int b = (int)a_pArg[1].AsInteger();

                    ret = a << b;
                }
                catch (OverflowException)
                {
                    throw new ParserError(new ErrorContext(EErrorCodes.ecOVERFLOW, GetExprPos(), GetIdent()));
                }
            }
        }
        public override IToken Clone() => (OprtShl)MemberwiseClone();
    }
    class OprtShr : IOprtBin
    {
        public OprtShr() : base(">>", EOprtPrecedence.prSHIFT, EOprtAsct.oaLEFT) { }

        public override string GetDesc() => "shift right";

        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(a_pArg.Length == 2);
            if (!a_pArg[0].IsScalar())
                throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, GetExprPos(), GetIdent(), a_pArg[0].GetValueType(), 'i', 1));

            if (!a_pArg[1].IsScalar())
                throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, GetExprPos(), GetIdent(), a_pArg[1].GetValueType(), 'i', 2));

            if (!a_pArg[0].IsInteger())
                throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, GetExprPos(), a_pArg[0].GetIdent(), a_pArg[0].GetValueType(), 'i', 1));

            if (!a_pArg[1].IsInteger())
                throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, GetExprPos(), a_pArg[1].GetIdent(), a_pArg[1].GetValueType(), 'i', 2));

            checked
            {
                try
                {
                    int a = (int)a_pArg[0].AsInteger();
                    int b = (int)a_pArg[1].AsInteger();

                    ret = a >> b;
                }
                catch (OverflowException)
                {
                    throw new ParserError(new ErrorContext(EErrorCodes.ecOVERFLOW, GetExprPos(), GetIdent()));
                }
            }
        }
        public override IToken Clone() => (OprtShr)MemberwiseClone();
    }
    class OprtCastToFloat : IOprtInfix
    {
        public OprtCastToFloat() : base("(float)", EOprtPrecedence.prINFIX)
        {
        }
        public override string GetDesc() => "Cast a value into a floating point number";

        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            switch (a_pArg[0].GetValueType())
            {
                case 'f':
                    ret = a_pArg[0].GetFloat();
                    break;
                case 'i':
                    ret = Convert.ToDouble(a_pArg[0].GetInteger());
                    break;
                case 'b':
                    ret = Convert.ToDouble(a_pArg[0].GetBool());
                    break;
                case 'm':
                    ret = a_pArg[0].GetArray().CastTo('f');
                    break;
                case 'z':
                    ret = a_pArg[0].GetReal();
                    break;

                default:
                {
                    var err = new ErrorContext();
                    err.Errc = EErrorCodes.ecINVALID_TYPECAST;
                    err.Type1 = a_pArg[0].GetValueType();
                    err.Type2 = 'i';
                    throw new ParserError(err);
                }
            } // switch value type
        }
        public override IToken Clone() => (OprtCastToFloat)MemberwiseClone();
    }
    class OprtCastToInt : IOprtInfix
    {
        public OprtCastToInt() : base("(int)", EOprtPrecedence.prINFIX) { }
        public override string GetDesc() => "Cast a value into an integer";

        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            switch (a_pArg[0].GetValueType())
            {
                case 'f':
                case 'i':
                case 'c':
                    ret = a_pArg[0].AsInteger();
                    break;
                case 'b':
                    ret = Convert.ToInt32(a_pArg[0].GetBool());
                    break;
                case 'm':
                    ret = a_pArg[0].GetArray().CastTo('i');
                    break;
                default:
                    {
                        var err = new ErrorContext();
                        err.Errc = EErrorCodes.ecINVALID_TYPECAST;
                        err.Type1 = a_pArg[0].GetValueType();
                        err.Type2 = 'i';
                        throw new ParserError(err);
                    }
            } // switch value type
        }
        public override IToken Clone() => (OprtCastToInt)MemberwiseClone();
    }

    class OprtCastToChar : IOprtInfix
    {
        public OprtCastToChar() : base("(char)", EOprtPrecedence.prINFIX) { }
        public override string GetDesc() => "Cast a value into an integer";

        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            switch (a_pArg[0].GetValueType())
            {
                case 'f':
                    ret = (char)a_pArg[0].AsInteger();
                    break;
                case 'i':
                    ret = (char)a_pArg[0].GetInteger();
                    break;
                case 'c':
                    ret = (char)a_pArg[0].GetChar();
                    break;
                case 'b':
                    ret = Convert.ToChar(a_pArg[0].GetBool());
                    break;
                case 'm':
                    ret = a_pArg[0].GetArray().CastTo('c');
                    break;
                default:
                    {
                        var err = new ErrorContext();
                        err.Errc = EErrorCodes.ecINVALID_TYPECAST;
                        err.Type1 = a_pArg[0].GetValueType();
                        err.Type2 = 'i';
                        throw new ParserError(err);
                    }
            } // switch value type
        }
        public override IToken Clone() => (OprtCastToChar)MemberwiseClone();
    }
}
