namespace MuParserSharp.Framework
{
    interface IPrecedence
    {
        int GetPri();
        EOprtAsct GetAssociativity();
    }
}
