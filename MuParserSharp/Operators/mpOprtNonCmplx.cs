using System;
using System.Linq;
using MuParserSharp.Framework;
using MuParserSharp.Parser;
using MuParserSharp.Util;

namespace MuParserSharp.Operators
{

    class OprtSign : IOprtInfix
    {
        public OprtSign() : base("-", EOprtPrecedence.prINFIX) { }

        public override IToken Clone() => (OprtSign)MemberwiseClone();

        public override string GetDesc() => "-x - negative sign operator";

        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(() => a_pArg.Length == 1);

            if (a_pArg[0].IsScalar())
            {
                if(a_pArg[0].IsInteger())
                    ret = -a_pArg[0].GetInteger();
                else
                    ret = -a_pArg[0].AsFloat();

            }
            else if (a_pArg[0].IsScalarMatrix())
            {
                var v = new Matrix(a_pArg[0].GetRows(), 0);
                if (a_pArg[0].At(0).IsInteger())
                    for (int i = 0; i < a_pArg[0].GetRows(); ++i)
                    {
                        v.At(i) = -a_pArg[0].At(i).GetInteger();
                    }
                else
                    for (int i = 0; i < a_pArg[0].GetRows(); ++i)
                    {
                        v.At(i) = -a_pArg[0].At(i).AsFloat();
                    }

                ret = v;
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

    class OprtSignPos : IOprtInfix
    {
        public OprtSignPos() : base("+", EOprtPrecedence.prINFIX) { }

        public override IToken Clone() => (OprtSignPos)MemberwiseClone();

        public override string GetDesc() => "+x - positive sign operator";

        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(() => a_pArg.Length == 1);

            if (a_pArg[0].IsScalar() || a_pArg[0].IsScalarMatrix())
            {
                ret = a_pArg[0];
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

    class OprtAdd : IOprtBin
    {
        public OprtAdd() : base("+", EOprtPrecedence.prADD_SUB, EOprtAsct.oaLEFT) { }

        public override IToken Clone() => (OprtAdd)MemberwiseClone();

        public override string GetDesc() => "x+y - Addition for noncomplex values";

        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(() => a_pArg.Length == 2);

            IValue arg1 = a_pArg[0];
            IValue arg2 = a_pArg[1];
            if (arg1.IsNonComplexScalar() || arg2.IsNonComplexScalar())
            {
                if (!arg1.IsNonComplexScalar())
                    throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, -1, GetIdent(), arg1.GetValueType(), 'f', 1));

                if (!arg2.IsNonComplexScalar())
                    throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, -1, GetIdent(), arg2.GetValueType(), 'f', 2));

                if (arg1.IsInteger() && arg2.IsInteger())
                    ret = arg1.GetInteger() + arg2.GetInteger();
                else
                    ret = arg1.GetFloat() + arg2.GetFloat();
            }
            else 
            {
                // Vector + Vector
                Matrix a1 = arg1.GetArray(), a2 = arg2.GetArray();
                if (a1.GetRows() != a2.GetRows())
                    throw new ParserError(new ErrorContext(EErrorCodes.ecARRAY_SIZE_MISMATCH, -1, GetIdent(), 'm', 'm', 2));
                ret = a1 + a2;
                return;
                var rv = new Matrix(a1.GetRows());
                for (int i = 0; i < a1.GetRows(); ++i)
                {
                    if (!a1.At(i).IsNonComplexScalar())
                        throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, -1, GetIdent(), a1.At(i).GetValueType(), 'f', 1));

                    if (!a2.At(i).IsNonComplexScalar())
                        throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, -1, GetIdent(), a2.At(i).GetValueType(), 'f', 1));

                    if(a1.At(i).IsInteger() && a2.At(i).IsInteger())
                        rv.At(i) = a1.At(i).GetInteger() + a2.At(i).GetInteger();
                    else
                        rv.At(i) = a1.At(i).GetFloat() + a2.At(i).GetFloat();
                }

                ret = rv;
            }
        }
    }

    class OprtSub : IOprtBin
    {
        public OprtSub() : base("-", EOprtPrecedence.prADD_SUB, EOprtAsct.oaLEFT) { }

        public override IToken Clone() => (OprtSub)MemberwiseClone();

        public override string GetDesc() => "x-y - Subtraction for noncomplex values";

        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(() => a_pArg.Length == 2);

            IValue arg1 = a_pArg[0];
            IValue arg2 = a_pArg[1];
            if (arg1.IsNonComplexScalar() || arg2.IsNonComplexScalar())
            {
                if (!arg1.IsNonComplexScalar())
                    throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, -1, GetIdent(), arg1.GetValueType(), 'f', 1));

                if (!arg2.IsNonComplexScalar())
                    throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, -1, GetIdent(), arg2.GetValueType(), 'f', 2));

                if (arg1.IsInteger() && arg2.IsInteger())
                    ret = arg1.GetInteger() - arg2.GetInteger();
                else
                    ret = arg1.GetFloat() - arg2.GetFloat();
            }
            else //if (arg1.IsMatrix() && arg2.IsMatrix())
            {
                // Vector + Vector
                //Matrix a1 = arg1.GetArray(), a2 = arg2.GetArray();
                //if (a1.GetRows() != a2.GetRows())
                //    throw new ParserError(new ErrorContext(EErrorCodes.ecARRAY_SIZE_MISMATCH, -1, GetIdent(), 'm', 'm', 2));

                //var rv = new Matrix(a1.GetRows());
                //for (int i = 0; i < a1.GetRows(); ++i)
                //{
                //    if (!a1.At(i).IsNonComplexScalar())
                //        throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, -1, GetIdent(), a1.At(i).GetValueType(), 'f', 1));

                //    if (!a2.At(i).IsNonComplexScalar())
                //        throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, -1, GetIdent(), a2.At(i).GetValueType(), 'f', 1));

                //    if (a1.At(i).IsInteger() && a2.At(i).IsInteger())
                //        rv.At(i) = a1.At(i).GetInteger() - a2.At(i).GetInteger();
                //    else
                //        rv.At(i) = a1.At(i).GetFloat() - a2.At(i).GetFloat();
                //}

                ret = arg1 - arg2;
            }
            
        }
    }

    class OprtMul : IOprtBin
    {
        public OprtMul() : base("*", EOprtPrecedence.prMUL_DIV, EOprtAsct.oaLEFT)
        {
        }

        public override IToken Clone() => (OprtMul) MemberwiseClone();

        public override string GetDesc() => "x*y - Multiplication for noncomplex values";

        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(() => a_pArg.Length == 2);

            IValue arg1 = a_pArg[0];
            IValue arg2 = a_pArg[1];



            ret = arg1 * arg2;
        }
    }

    class OprtDiv : IOprtBin
    {
        public OprtDiv() : base("/", EOprtPrecedence.prMUL_DIV, EOprtAsct.oaLEFT) { }

        public override IToken Clone() => (OprtDiv)MemberwiseClone();

        public override string GetDesc() => "x/y - Division for noncomplex values";

        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(() => a_pArg.Length == 2);

            IValue arg1 = a_pArg[0];
            IValue arg2 = a_pArg[1];

            if (!arg1.IsNonComplexScalar())
                throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, -1, GetIdent(), arg1.GetValueType(), 'f', 1));

            if (!arg2.IsNonComplexScalar())
                throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, -1, GetIdent(), arg2.GetValueType(), 'f', 2));

            ret = arg1.GetFloat() / arg2.GetFloat();
        }
    }

    class OprtPow : IOprtBin
    {
        public OprtPow() : base("^", EOprtPrecedence.prPOW, EOprtAsct.oaRIGHT) { }

        public override IToken Clone() => (OprtPow)MemberwiseClone();

        public override string GetDesc() => "x^y - Exponentation for noncomplex values";

        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(() => a_pArg.Length == 2);

            IValue arg1 = a_pArg[0];
            IValue arg2 = a_pArg[1];

            if (!arg1.IsNonComplexScalar())
                throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, -1, GetIdent(), arg1.GetValueType(), 'f', 1));

            if (!arg2.IsNonComplexScalar())
                throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, -1, GetIdent(), arg2.GetValueType(), 'f', 2));
            if (arg1.IsInteger() && arg2.IsInteger())
            {
                var i1 = arg1.GetInteger();
                var i2 = arg2.GetInteger();
                switch (i2)
                {
                    case 1:
                        ret = i1;
                        break;
                    case 2:
                        ret = i1 * i1;
                        break;
                    case 3:
                        ret = i1 * i1 * i1;
                        break;
                    case 4:
                        ret = i1 * i1 * i1 * i1;
                        break;
                    case 5:
                        ret = i1 * i1 * i1 * i1 * i1;
                        break;
                    default:
                        ret = (long)Math.Pow(i1, i2);
                        break;
                }
            }
            else
            {
                var i1 = arg1.GetFloat();
                var i2 = arg2.GetFloat();
                switch (i2)
                {
                    case 1:
                        ret = i1;
                        break;
                    case 2:
                        ret = i1 * i1;
                        break;
                    case 3:
                        ret = i1 * i1 * i1;
                        break;
                    case 4:
                        ret = i1 * i1 + i1 * i1;
                        break;
                    case 5:
                        ret = i1 * i1 * i1 * i1 * i1;
                        break;
                    default:
                        ret = Math.Pow(i1, i2);
                        break;
                }
            }
        }
    }
}
