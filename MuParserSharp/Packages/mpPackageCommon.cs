using System;
using MuParserSharp.Framework;
using MuParserSharp.Functions;
using MuParserSharp.Operators;
using MuParserSharp.Parser;

namespace MuParserSharp.Packages
{
    class PackageCommon : IPackage
    {
        public static PackageCommon Instance { get; } = new PackageCommon();
        private PackageCommon() { }
        public void AddToParser(ParserXBase pParser)
        {
            // Readers that need fancy decorations on their values must
            // be added first (i.e. hex . "0x...") Otherwise the
            // zero in 0x will be read as a value of zero!
            pParser.AddValueReader(new HexValReader());
            pParser.AddValueReader(new BinValReader());
            pParser.AddValueReader(new BoolValReader());

            // Constants
            pParser.DefineConst("pi", Math.PI);
            pParser.DefineConst("e", Math.E);

            // Vector
            pParser.DefineFun(new FunSizeOf());

            // Generic functions
            pParser.DefineFun(new FunMax());
            pParser.DefineFun(new FunMin());
            pParser.DefineFun(new FunSum());

            // misc
            pParser.DefineFun(new FunParserID());

            // integer package
            pParser.DefineOprt(new OprtLAnd());
            pParser.DefineOprt(new OprtLOr());
            pParser.DefineOprt(new OprtAnd());
            pParser.DefineOprt(new OprtOr());
            pParser.DefineOprt(new OprtShr());
            pParser.DefineOprt(new OprtShl());

            // booloean package
            pParser.DefineOprt(new OprtLE());
            pParser.DefineOprt(new OprtGE());
            pParser.DefineOprt(new OprtLT());
            pParser.DefineOprt(new OprtGT());
            pParser.DefineOprt(new OprtEQ());
            pParser.DefineOprt(new OprtNEQ());
            pParser.DefineOprt(new OprtLAnd("and"));  // add logic and with a different identifier
            pParser.DefineOprt(new OprtLOr("or"));    // add logic and with a different identifier
                                                      //  pParser.DefineOprt(new OprtBXor());

            // assignement operators
            pParser.DefineOprt(new OprtAssign());
            pParser.DefineOprt(new OprtAssignAdd());
            pParser.DefineOprt(new OprtAssignSub());
            pParser.DefineOprt(new OprtAssignMul());
            pParser.DefineOprt(new OprtAssignDiv());

            // infix operators
            pParser.DefineInfixOprt(new OprtCastToFloat());
            pParser.DefineInfixOprt(new OprtCastToInt());
            pParser.DefineInfixOprt(new OprtCastToChar());

            // postfix operators
            pParser.DefinePostfixOprt(new OprtFact());
            // <ibg 20130708> commented: "%" is a reserved sign for either the 
            //                modulo operator or comment lines. 
              pParser.DefinePostfixOprt(new OprtPercentage());
            // </ibg>
        }

        public string GetDesc() => "";

        public string GetPrefix() => "";
    }
}
