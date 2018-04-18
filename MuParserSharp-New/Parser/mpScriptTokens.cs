using MuParserSharp.Framework;

namespace MuParserSharp.Parser
{
    class TokenNewline : IToken
    {
        private int m_nOffset;

        public TokenNewline() : base(ECmdCode.cmSCRIPT_NEWLINE) { }

        public TokenNewline(TokenNewline tref) : base(tref)
        {
            this.m_nOffset = tref.m_nOffset;
        }

        public override IToken Clone()
        {
            return new TokenNewline(this);
        }

        internal int GetStackOffset()
        {
            return m_nOffset;
        }

        internal void SetStackOffset(int nOffset)
        {
            m_nOffset = nOffset;
        }
    }
}
