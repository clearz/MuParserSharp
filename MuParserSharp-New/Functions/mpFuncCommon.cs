using System;
using System.Linq;
using MuParserSharp.Framework;
using MuParserSharp.Parser;
using MuParserSharp.Util;

namespace MuParserSharp.Functions
{
    class FunParserID : ICallback
    {
        public FunParserID() : base(ECmdCode.cmFUNC, "parserid", 0) { }
        public override string GetDesc() => "parserid() - muParserX version information.";
        public override void Eval(ref IValue ret, IValue[] a_pArg, int narg = -1)
        {
            string sVer = "muParser# V" + ParserXBase.GetVersion();
            ret = sVer;
        }

        public override IToken Clone() => (FunParserID)MemberwiseClone();
    }
    class FunMax : ICallback
    {
        public FunMax() : base(ECmdCode.cmFUNC, "max", -1) { }
        public override string GetDesc() => "max(x,y,...,z) - Returns the maximum value from all of its function arguments.";
        public override void Eval(ref IValue ret, IValue[] a_pArg, int narg = -1)
        {
            var len = narg;
            if (len < 1)
                throw new ParserError(new ErrorContext(EErrorCodes.ecTOO_FEW_PARAMS, GetExprPos(), GetIdent()));

            double max = double.MinValue, val = double.MinValue;

            for (int i = 0; i < len; ++i)
            {
                switch (a_pArg[i].GetValueType())
                {
                    case 'f':
                    case 'i': val = a_pArg[i].GetFloat(); break;
                    case 'n': break; // ignore not in list entries (missing parameter)
                    default:
                    {
                        var err = new ErrorContext();
                        err.Errc = EErrorCodes.ecTYPE_CONFLICT_FUN;
                        err.Arg = i + 1;
                        err.Type1 = a_pArg[i].GetValueType();
                        err.Type2 = 'f';
                        throw new ParserError(err);
                    }
                }
                max = Math.Max(max, val);
            }
            ret = max;
        }

        public override IToken Clone() => (FunMax)MemberwiseClone();
    }
    class FunMin : ICallback
    {
        public FunMin() : base(ECmdCode.cmFUNC, "min", -1) { }
        public override string GetDesc() => "min(x,y,...,z) - Returns the minimum value from all of its function arguments.";
        public override void Eval(ref IValue ret, IValue[] a_pArg, int narg = -1)
        {
            var len = narg;
            if (len < 1)
                throw new ParserError(new ErrorContext(EErrorCodes.ecTOO_FEW_PARAMS, GetExprPos(), GetIdent()));

            double min = double.MaxValue, val;

            for (int i = 0; i < len; ++i)
            {
                switch (a_pArg[i].GetValueType())
                {
                    case 'f':
                    case 'i': val = a_pArg[i].GetFloat(); break;
                    default:
                    {
                        var err = new ErrorContext();
                        err.Errc = EErrorCodes.ecTYPE_CONFLICT_FUN;
                        err.Arg = i + 1;
                        err.Type1 = a_pArg[i].GetValueType();
                        err.Type2 = 'f';
                        throw new ParserError(err);
                    }
                }
                min = Math.Min(min, val);
            }
            ret = min;
        }

        public override IToken Clone() => (FunMin)MemberwiseClone();
    }
    class FunSum : ICallback
    {
        public FunSum() : base(ECmdCode.cmFUNC, "sum", -1) { }
        public override string GetDesc() => "sum(x,y,...,z) - Returns the sum of all arguments.";
        public override void Eval(ref IValue ret, IValue[] a_pArg, int narg = -1)
        {

            var len = narg;
            if (len < 1)
                throw new ParserError(new ErrorContext(EErrorCodes.ecTOO_FEW_PARAMS, GetExprPos(), GetIdent()));

            double sum = 0;

            for (int i = 0; i < len; ++i)
            {
                switch (a_pArg[i].GetValueType())
                {
                    case 'f':
                    case 'i': sum += a_pArg[i].GetFloat(); break;
                    default:
                    {
                        var err = new ErrorContext();
                        err.Errc = EErrorCodes.ecTYPE_CONFLICT_FUN;
                        err.Arg = i + 1;
                        err.Type1 = a_pArg[i].GetValueType();
                        err.Type2 = 'f';
                        throw new ParserError(err);
                    }
                }
            }

            ret = sum;
        }

        public override IToken Clone() => (FunSum)MemberwiseClone();
    }
    class FunSizeOf : ICallback
    {
        public FunSizeOf() : base(ECmdCode.cmFUNC, "sizeof") { }
        public override string GetDesc() => "sizeof(a) - Returns the number of elements in a.";
        public override void Eval(ref IValue ret, IValue[] a_pArg, int narg = -1)
        {
            Global.MUP_VERIFY(() => narg == 1);
            ret = a_pArg[0].GetArray().Length();
        }

        public override IToken Clone() => (FunSizeOf)MemberwiseClone();
    }
}
 