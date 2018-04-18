using MuParserSharp.Packages;

namespace MuParserSharp.Parser
{
    public class ParserX : ParserXBase
    {

        public ParserX(ParserX p) : base(p) { }
        public ParserX(EPackages ePackages = EPackages.pckALL_COMPLEX)
        {
            DefineNameChars("0123456789_abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");
            DefineOprtChars("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ+-*^/?<>=#!$%&|~'_µ{}");
            DefineInfixOprtChars("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ()/+-*^?<>=#!$%&|~'_");

            if ((ePackages & EPackages.pckUNIT) > 0)
                AddPackage(PackageUnit.Instance);

            if ((ePackages & EPackages.pckSTRING) > 0)
                AddPackage(PackageStr.Instance);

            if ((ePackages & EPackages.pckCOMMON) > 0)
                AddPackage(PackageCommon.Instance);

            if ((ePackages & EPackages.pckCOMPLEX) > 0)
                AddPackage(PackageCmplx.Instance);

            if ((ePackages & EPackages.pckNON_COMPLEX) > 0)
                AddPackage(PackageNonCmplx.Instance);

            if ((ePackages & EPackages.pckMATRIX) > 0)
                AddPackage(PackageMatrix.Instance);
        }

        public static void ResetErrorMessageProvider(ParserMessageProviderBase pProvider)
        {
            ParserErrorMsg.Reset(pProvider);
        }

        public static void ResetErrorMessageProvider(string sID)
        {
            if (sID == "en")
                ParserX.ResetErrorMessageProvider(ParserMessageProviderEnglish.Instance);
            else if (sID == "en")
                ParserX.ResetErrorMessageProvider(ParserMessageProviderGerman.Instance);
        }
    }
}
