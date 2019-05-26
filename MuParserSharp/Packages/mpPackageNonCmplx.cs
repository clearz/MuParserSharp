using MuParserSharp.Framework;
using MuParserSharp.Functions;
using MuParserSharp.Operators;
using MuParserSharp.Parser;

namespace MuParserSharp.Packages
{
    class PackageNonCmplx : IPackage
    {
        public static PackageNonCmplx Instance { get; } = new PackageNonCmplx();
        private PackageNonCmplx() { }
        public void AddToParser(ParserXBase pParser)
        {
            
            pParser.AddValueReader(new NumericReader());

            pParser.DefineFun(new FunSin());
            pParser.DefineFun(new FunCos());
            pParser.DefineFun(new FunTan());
            pParser.DefineFun(new FunSinH());
            pParser.DefineFun(new FunCosH());
            pParser.DefineFun(new FunTanH());
            pParser.DefineFun(new FunASin());
            pParser.DefineFun(new FunACos());
            pParser.DefineFun(new FunATan());
            pParser.DefineFun(new FunASinH());
            pParser.DefineFun(new FunACosH());
            pParser.DefineFun(new FunATanH());
            pParser.DefineFun(new FunLog());
            pParser.DefineFun(new FunLog10());
            pParser.DefineFun(new FunLog2());
            pParser.DefineFun(new FunLn());
            pParser.DefineFun(new FunExp());
            pParser.DefineFun(new FunSqrt());
            pParser.DefineFun(new FunCbrt());
            pParser.DefineFun(new FunAbs());

            // binary functions
            pParser.DefineFun(new FunPow());
            pParser.DefineFun(new FunHypot());
            pParser.DefineFun(new FunAtan2());
            pParser.DefineFun(new FunFmod());
            pParser.DefineFun(new FunRemainder());

            // Operator callbacks
            pParser.DefineInfixOprt(new OprtSign());
            pParser.DefineInfixOprt(new OprtSignPos());
            pParser.DefineOprt(new OprtAdd());
            pParser.DefineOprt(new OprtSub());
            pParser.DefineOprt(new OprtMul());
            pParser.DefineOprt(new OprtDiv());
            pParser.DefineOprt(new OprtPow());
        }

        public string GetDesc() => "";

        public string GetPrefix() => "";
    }
}
