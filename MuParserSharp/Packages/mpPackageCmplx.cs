using System.Numerics;
using MuParserSharp.Framework;
using MuParserSharp.Functions;
using MuParserSharp.Operators;
using MuParserSharp.Parser;

namespace MuParserSharp.Packages
{
    class PackageCmplx : IPackage
    {
        public static PackageCmplx Instance { get; } = new PackageCmplx();
        private PackageCmplx() { }
        public void AddToParser(ParserXBase pParser)
        {
            pParser.DefineConst("i", new Complex(0.0, 1.0));

            pParser.AddValueReader(new CmplxValReader());

            // Complex valued functions
            pParser.DefineFun(new FunCmplxReal());
            pParser.DefineFun(new FunCmplxImag());
            pParser.DefineFun(new FunCmplxConj());
            pParser.DefineFun(new FunCmplxArg());
            pParser.DefineFun(new FunCmplxNorm());
            pParser.DefineFun(new FunCmplxSin());
            pParser.DefineFun(new FunCmplxCos());
            pParser.DefineFun(new FunCmplxTan());
            pParser.DefineFun(new FunCmplxSinH());
            pParser.DefineFun(new FunCmplxCosH());
            pParser.DefineFun(new FunCmplxTanH());
            pParser.DefineFun(new FunCmplxSqrt());
            pParser.DefineFun(new FunCmplxExp());
            pParser.DefineFun(new FunCmplxLn());
            pParser.DefineFun(new FunCmplxLog());
            pParser.DefineFun(new FunCmplxLog2());
            pParser.DefineFun(new FunCmplxLog10());
            pParser.DefineFun(new FunCmplxAbs());
            pParser.DefineFun(new FunCmplxPow());

            // Complex valued operators
            pParser.DefineOprt(new OprtAddCmplx());
            pParser.DefineOprt(new OprtSubCmplx());
            pParser.DefineOprt(new OprtMulCmplx());
            pParser.DefineOprt(new OprtDivCmplx());
            pParser.DefineOprt(new OprtPowCmplx());
            pParser.DefineInfixOprt(new OprtSignCmplx());
        }

        public string GetDesc() => "Complex i constant, functions and operations.";

        public string GetPrefix() => "";
    }
}
