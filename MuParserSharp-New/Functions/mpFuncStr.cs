using MuParserSharp.Framework;
using MuParserSharp.Util;
using System.Linq;

namespace MuParserSharp.Functions
{
    class FunStrLen : ICallback
    {
        public FunStrLen() : base(ECmdCode.cmFUNC, "strlen") { }
        public override string GetDesc() => "strlen(s) - Returns the length of the string s.";
        public override void Eval(ref IValue ret, IValue[] a_pArg, int narg = -1)
        {
            Global.MUP_VERIFY(() => narg == 1);
            ret = a_pArg[0].GetString().Length;
        }

        public override IToken Clone() => (FunStrLen)MemberwiseClone();
    }
    class FunStrToUpper : ICallback
    {
        public FunStrToUpper() : base(ECmdCode.cmFUNC, "toupper") { }
        public override string GetDesc() => "toupper(s) - Converts the string s to uppercase characters.";
        public override void Eval(ref IValue ret, IValue[] a_pArg, int narg = -1)
        {
            Global.MUP_VERIFY(() => narg == 1);
            ret = a_pArg[0].GetString().ToUpper();
        }

        public override IToken Clone() => (FunStrToUpper)MemberwiseClone();
    }
    class FunStrToLower : ICallback
    {
        public FunStrToLower() : base(ECmdCode.cmFUNC, "tolower") { }
        public override string GetDesc() => "tolower(s) - Converts the string s to lowercase characters.";
        public override void Eval(ref IValue ret, IValue[] a_pArg, int narg = -1)
        {
            Global.MUP_VERIFY(() => narg == 1);
            ret = a_pArg[0].GetString().ToLower();
        }

        public override IToken Clone() => (FunStrToLower)MemberwiseClone();
    }
    class FunStrToDbl : ICallback
    {
        public FunStrToDbl() : base(ECmdCode.cmFUNC, "str2dbl") { }
        public override string GetDesc() => "str2dbl(s) - Converts the string stored in s into a floating foint value.";
        public override void Eval(ref IValue ret, IValue[] a_pArg, int narg = -1)
        {
            Global.MUP_VERIFY(() => narg == 1);
            if (double.TryParse(a_pArg[0].GetString(), out double val))
                ret = val;
            else ret = double.NaN;
        }

        public override IToken Clone() => (FunStrToDbl)MemberwiseClone();
    }
}
