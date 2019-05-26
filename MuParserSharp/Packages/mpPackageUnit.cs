using MuParserSharp.Framework;
using MuParserSharp.Parser;

namespace MuParserSharp.Packages
{
    class PackageUnit : IPackage
    {
        public static PackageUnit Instance { get; } = new PackageUnit();
        private PackageUnit(){ }

        public void AddToParser(ParserXBase pParser)
        {
            pParser.DefinePostfixOprt(new OprtNano());
            pParser.DefinePostfixOprt(new OprtMicro());
            pParser.DefinePostfixOprt(new OprtMilli());
            pParser.DefinePostfixOprt(new OprtKilo());
            pParser.DefinePostfixOprt(new OprtMega());
            pParser.DefinePostfixOprt(new OprtGiga());
        }

        public string GetDesc() => "Postfix operators for basic unit conversions.";

        public string GetPrefix() => "";
    }
    
   #region Unit Operations

   class OprtNano : IOprtPostfix
   {
       public OprtNano() : base("n") { }

       public override string GetDesc() => "n - unit multiplicator 1e-9";

       public override void Eval(ref IValue ret, IValue[] a_pArg)
       {
           if (!a_pArg[0].IsScalar())
           {
               var err = new ErrorContext(EErrorCodes.ecTYPE_CONFLICT,
                   GetExprPos(),
                   a_pArg[0].ToString(),
                   a_pArg[0].GetValueType(),
                   'c',
                   1);
               throw new ParserError(err);
           }
           ret = a_pArg[0] * 1E-9;
        }
       public override IToken Clone() => (OprtNano)MemberwiseClone();
   }

   class OprtMicro : IOprtPostfix
   {
       public OprtMicro() : base("u") { }

       public override string GetDesc() => "u - unit multiplicator 1e-6";

       public override void Eval(ref IValue ret, IValue[] a_pArg)
       {
           if (!a_pArg[0].IsScalar())
           {
               var err = new ErrorContext(EErrorCodes.ecTYPE_CONFLICT,
                   GetExprPos(),
                   a_pArg[0].ToString(),
                   a_pArg[0].GetValueType(),
                   'c',
                   1);
               throw new ParserError(err);
           }
            ret = a_pArg[0] * 1E-6;
        }
       public override IToken Clone() => (OprtMicro)MemberwiseClone();
    }

   class OprtMilli : IOprtPostfix
   {
       public OprtMilli() : base("m") { }

       public override string GetDesc() => "m - unit multiplicator 1e-3";

       public override void Eval(ref IValue ret, IValue[] a_pArg)
       {
           if (!a_pArg[0].IsScalar())
           {
               var err = new ErrorContext(EErrorCodes.ecTYPE_CONFLICT,
                   GetExprPos(),
                   a_pArg[0].ToString(),
                   a_pArg[0].GetValueType(),
                   'c',
                   1);
               throw new ParserError(err);
           }

           ret = a_pArg[0] * 1E-3;
        }
       public override IToken Clone() => (OprtMilli)MemberwiseClone();
    }

   class OprtKilo : IOprtPostfix
   {
       public OprtKilo() : base("k") { }

       public override string GetDesc() => "k - unit multiplicator 1e3";

       public override void Eval(ref IValue ret, IValue[] a_pArg)
       {
           if (!a_pArg[0].IsScalar())
           {
               var err = new ErrorContext(EErrorCodes.ecTYPE_CONFLICT,
                   GetExprPos(),
                   a_pArg[0].ToString(),
                   a_pArg[0].GetValueType(),
                   'c',
                   1);
               throw new ParserError(err);
           }

           ret = a_pArg[0] * 1000L;
        }
       public override IToken Clone() => (OprtKilo)MemberwiseClone();
    }

   class OprtMega : IOprtPostfix
   {
       public OprtMega() : base("M") { }

       public override string GetDesc() => "M - unit multiplicator 1e6";

       public override void Eval(ref IValue ret, IValue[] a_pArg)
       {
           if (!a_pArg[0].IsScalar())
           {
               var err = new ErrorContext(EErrorCodes.ecTYPE_CONFLICT,
                   GetExprPos(),
                   a_pArg[0].ToString(),
                   a_pArg[0].GetValueType(),
                   'c',
                   1);
               throw new ParserError(err);
           }

           ret = a_pArg[0] * 1_000_000L;
        }
       public override IToken Clone() => (OprtMega)MemberwiseClone();
    }

   class OprtGiga : IOprtPostfix
   {
       public OprtGiga() : base("G") { }

       public override string GetDesc() => "G - unit multiplicator 1e9";

       public override void Eval(ref IValue ret, IValue[] a_pArg)
       {
           if (!a_pArg[0].IsScalar())                                    
           {                                                              
               var err = new ErrorContext(EErrorCodes.ecTYPE_CONFLICT,                            
               GetExprPos(),                               
               a_pArg[0].ToString(),                      
               a_pArg[0].GetValueType(),                       
               'c',                                        
               1);                                         
               throw new ParserError(err);                                      
           }
           
           ret = a_pArg[0] * 1_000_000_000L;
        }
       public override IToken Clone() => (OprtGiga)MemberwiseClone();
    }

   #endregion Unit Operations

}
