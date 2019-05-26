using MuParserSharp.Framework;
using MuParserSharp.Functions;
using MuParserSharp.Operators;
using MuParserSharp.Parser;

namespace MuParserSharp.Packages
{
    class PackageStr : IPackage
    {
        public static PackageStr Instance { get; } = new PackageStr();
        private PackageStr(){}
        public void AddToParser(ParserXBase parser)
        {
            parser.AddValueReader(new StrValReader());
            parser.AddValueReader(new CharacterReader());

            // Functions
            parser.DefineFun(new FunStrLen());
            parser.DefineFun(new FunStrToDbl());
            parser.DefineFun(new FunStrToUpper());
            parser.DefineFun(new FunStrToLower());
            parser.DefineFun(new FunStrSplit());
            parser.DefineFun(new FunStrJoin());

            // Operators
            parser.DefineOprt(new OprtStrAdd());
        }

        public string GetDesc() => "A package for string operations.";

        public string GetPrefix() => "";
    }
}
