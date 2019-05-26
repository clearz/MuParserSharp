using MuParserSharp.Framework;
using MuParserSharp.Functions;
using MuParserSharp.Operators;
using MuParserSharp.Parser;

namespace MuParserSharp.Packages
{
    class PackageMatrix : IPackage
    {
        public static PackageMatrix Instance { get; } = new PackageMatrix();
        private PackageMatrix() { }
        public void AddToParser(ParserXBase pParser)
        {
            // Matrix functions
            pParser.DefineFun(new FunMatrixOnes());
            pParser.DefineFun(new FunMatrixZeros());
            pParser.DefineFun(new FunMatrixEye());
            pParser.DefineFun(new FunMatrixSize());

            // Matrix Operators
            pParser.DefinePostfixOprt(new OprtTranspose());

            // Colon operator
            pParser.DefineOprt(new OprtColon());
            //pParser->DefineAggregator(new AggColon());
        }

        public string GetDesc() => "Matrix functions & operations";

        public string GetPrefix() => "";
    }
}
