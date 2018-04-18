using MuParserSharp.Parser;
using MuParserSharp.Util;

namespace MuParserSharp.Framework
{
    public abstract class ICallback : IToken
    {
        protected ICallback(ECmdCode a_iCode, string a_sIdent = "", int a_nArgNum = 1) : base(a_iCode, a_sIdent)
        {
            m_nArgc = a_nArgNum;
            m_nArgsPresent = -1;
        }

        //---------------------------------------------------------------------------
        public override ICallback AsICallback()
        {
            return this;
        }

        //---------------------------------------------------------------------------
        public override IValue AsIValue()
        {
            return null;
        }

        //---------------------------------------------------------------------------
        public void SetArgc(int argc)
        {
            m_nArgc = argc;
        }

        //------------------------------------------------------------------------------
        /*  Returns the m´number of arguments required by this callback. 
            \return Number of arguments or -1 if the number of arguments is variable.
        */
        public int GetArgc()
        {
            return m_nArgc;
        }

        //------------------------------------------------------------------------------
        /*  Returns a pointer to the parser object owning this callback. 
            \pre [assert] m_pParent must be defined
        */
        public ParserXBase GetParent()
        {
            Global.MUP_VERIFY(() => m_pParent != null);
            return m_pParent;
        }

        //------------------------------------------------------------------------------
        /*  Assign a parser object to the callback.
            \param a_pParent The parser that belongs to this callback object.

          The parent object can be used in order to access internals of the parser
          from within a callback object. Thus enabling callbacks to delete 
          variables or functions if this is desired.
        */
        internal void SetParent(ParserXBase a_pParent)
        {
            Global.MUP_VERIFY(() => a_pParent != null);
            m_pParent = a_pParent;
        }

        internal override string AsciiDump()
        {
            return this.Dump("pos", GetExprPos(), "id", $"\"{GetIdent()}\"", "argc",
                             $"{GetArgc()} (found: {m_nArgsPresent})");
        }
        //------------------------------------------------------------------------------
        internal IToken SetNumArgsPresent(int argc)
        {
            ICallback cb = this.Clone().AsICallback();
            cb.m_nArgsPresent = argc;
            return cb;
        }

        //------------------------------------------------------------------------------
        public int GetArgsPresent()
        {
            if (m_nArgc != -1)
                return m_nArgc;
            else
                return m_nArgsPresent;
        }

        public abstract string GetDesc();
        public abstract void Eval(ref IValue ret, IValue[] a_pArg);

        protected ParserXBase m_pParent;      // Pointer to the parser object using this callback
        private int m_nArgc;                // Number of this function can take Arguments.
        private int m_nArgsPresent; // Number of arguments actually submitted
    }
}
