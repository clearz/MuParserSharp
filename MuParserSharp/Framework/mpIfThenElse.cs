using MuParserSharp.Util;

namespace MuParserSharp.Framework
{
    class TokenIfThenElse : IToken, IPrecedence
    {

        public TokenIfThenElse(ECmdCode eCode) : base(eCode, Global.g_sCmdCode[(int)eCode] ){}

        private TokenIfThenElse(TokenIfThenElse t) : base(t)
        {
            m_nOffset = t.m_nOffset;
        }

        public int GetOffset()
        {
            return m_nOffset;
        }
        public void SetOffset(int nOffset)
        {
            m_nOffset = nOffset;
        }
        public override IToken Clone()
        {
            return new TokenIfThenElse(this);
        }

        public int GetPri()
        {
            return (int) EOprtPrecedence.prIF_THEN_ELSE;
        }

        //---------------------------------------------------------------------------
        internal override IPrecedence AsIPrecedence()
        {
            return this;
        }
        public EOprtAsct GetAssociativity()
        {
            return EOprtAsct.oaNONE;
        }

        private int m_nOffset;
    }
}
