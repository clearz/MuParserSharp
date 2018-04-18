using MuParserSharp.Framework;
using MuParserSharp.Parser;
using MuParserSharp.Util;
using System.Linq;

namespace MuParserSharp.Functions
{
    class FunStrLen : ICallback
    {
        public FunStrLen() : base(ECmdCode.cmFUNC, "strlen") { }
        public override string GetDesc() => "strlen(s) - Returns the length of the string s.";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(a_pArg.Length == 1);
            ret = a_pArg[0].GetString().Length;
        }

        public override IToken Clone() => (FunStrLen)MemberwiseClone();
    }

    class FunStrSplit : ICallback
    {
        public FunStrSplit() : base(ECmdCode.cmFUNC, "split") { }
        public override string GetDesc() => "split(s) - split a string into an array of characters.";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(a_pArg.Length == 1);
            ret = new Matrix(a_pArg[0].GetString().ToCharArray().Select(c => (IValue)c));
        }

        public override IToken Clone() => (FunStrSplit)MemberwiseClone();
    }

    class FunStrJoin : ICallback
    {
        public FunStrJoin() : base(ECmdCode.cmFUNC, "join", 2) { }
        public override string GetDesc() => "join(a, s) - Returns a string of the elements in a seperated by the string s.";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(a_pArg.Length == 2 && a_pArg[0].IsMatrix() && a_pArg[1].IsString());
            ret = string.Join(a_pArg[1].GetString(), a_pArg[0].GetArray().m_vData.Cast<object>().ToArray()); ;
        }

        public override IToken Clone() => (FunStrJoin)MemberwiseClone();
    }
    class FunStrToUpper : ICallback
    {
        public FunStrToUpper() : base(ECmdCode.cmFUNC, "toupper") { }
        public override string GetDesc() => "toupper(s) - Converts the string s to uppercase characters.";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(a_pArg.Length == 1 && a_pArg[0].IsString());
            ret = a_pArg[0].GetString().ToUpper();
        }

        public override IToken Clone() => (FunStrToUpper)MemberwiseClone();
    }
    class FunStrToLower : ICallback
    {
        public FunStrToLower() : base(ECmdCode.cmFUNC, "tolower") { }
        public override string GetDesc() => "tolower(s) - Converts the string s to lowercase characters.";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(a_pArg.Length == 1);
            ret = a_pArg[0].GetString().ToLower();
        }

        public override IToken Clone() => (FunStrToLower)MemberwiseClone();
    }
    class FunStrToDbl : ICallback
    {
        public FunStrToDbl() : base(ECmdCode.cmFUNC, "str2dbl") { }
        public override string GetDesc() => "str2dbl(s) - Converts the string stored in s into a floating foint value.";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(a_pArg.Length == 1);

            if (!a_pArg[0].IsString())
                throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, GetExprPos(), a_pArg[0].GetIdent(), a_pArg[0].GetValueType(), 's', 1));

            if (double.TryParse(a_pArg[0].GetString(), out double val))
                ret = val;
            else ret = double.NaN;
        }

        public override IToken Clone() => (FunStrToDbl)MemberwiseClone();
    }
}
