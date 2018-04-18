using MuParserSharp.Util;

namespace MuParserSharp.Framework
{
    public abstract class IOprtBin : ICallback, IPrecedence
    {
        protected internal EOprtPrecedence m_ePrec;
        protected internal EOprtAsct m_eAsc;

        protected IOprtBin(string a_szIdent, EOprtPrecedence ePrec, EOprtAsct eAsc) : base(ECmdCode.cmOPRT_BIN, a_szIdent, 2)
        {
            m_ePrec = ePrec;
            m_eAsc = eAsc;
        }

        internal override string AsciiDump()
        {
            return this.Dump("pos", GetExprPos(), "id", GetIdent(), "prec", GetPri(), "argc", GetArgc());
        }
        //------------------------------------------------------------------------------
        public int GetPri()
        {
            return (int) m_ePrec;
        }

        //------------------------------------------------------------------------------
        public EOprtAsct GetAssociativity()
        {
            return m_eAsc;
        }

        //------------------------------------------------------------------------------
        internal override IPrecedence AsIPrecedence()
        {
            return this;
        }
    }

    public abstract class IOprtPostfix : ICallback
    {
        protected IOprtPostfix(string a_szIdent) : base(ECmdCode.cmOPRT_POSTFIX, a_szIdent, 1)
        {
           
        }
        internal override string AsciiDump()
        {
            return this.Dump("pos", GetExprPos(), "id", GetIdent(), "argc", GetArgc());
        }
    }

    public abstract class IOprtInfix : ICallback, IPrecedence
    {
        private readonly EOprtPrecedence m_ePrec;

        protected IOprtInfix(string a_szIdent, EOprtPrecedence ePrec) : base(ECmdCode.cmOPRT_INFIX, a_szIdent, 1)
        {
            m_ePrec = ePrec;
        }

        internal override string AsciiDump()
        {
            return this.Dump("pos", GetExprPos(), "id", GetIdent(), "argc", GetArgc());
        }

        //------------------------------------------------------------------------------
        internal override IPrecedence AsIPrecedence()
        {
            return this;
        }

        //------------------------------------------------------------------------------
        public int GetPri()
        {
            return (int)m_ePrec;
        }

        //------------------------------------------------------------------------------
        public EOprtAsct GetAssociativity()
        {
            return EOprtAsct.oaNONE;
        }
    }
}
