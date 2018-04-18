using MuParserSharp.Parser;

namespace MuParserSharp.Framework
{
    public interface IPackage
    {
        void AddToParser(ParserXBase parserXBase);
        string GetDesc();
        string GetPrefix();
    }
}
