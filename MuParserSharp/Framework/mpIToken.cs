using System;
using MuParserSharp.Util;

namespace MuParserSharp.Framework
{
    public abstract class IToken
    {

        protected IToken(ECmdCode a_iCode, string a_sIdent = "")
        {
            m_eCode = a_iCode;
            m_sIdent = a_sIdent;
            m_nPosExpr = -1;
            m_flags = 0;
        }

        protected IToken(IToken tref)
        {
            m_eCode = tref.m_eCode;
            m_sIdent = tref.m_sIdent;
            m_flags = tref.m_flags;
            m_nPosExpr = tref.m_nPosExpr;           
        }

        protected internal void Init(ECmdCode a_iCode, string a_sIdent = "")
        {
            m_eCode = a_iCode;
            m_sIdent = a_sIdent;
            m_nPosExpr = -1;
            m_flags = 0;
        }
        //------------------------------------------------------------------------------
        public override string ToString()
        {
            return AsciiDump();
        }

        //------------------------------------------------------------------------------
        public int GetExprPos()
        {
            return m_nPosExpr;
        }

        //------------------------------------------------------------------------------
        public void SetExprPos(int nPos)
        {
            m_nPosExpr = nPos;
        }

        //------------------------------------------------------------------------------
        /*  return the token code. 
   
            \sa ECmdCode
        */
        public ECmdCode GetCode()
        {
            return m_eCode;
        }

        //------------------------------------------------------------------------------
        public virtual string GetIdent()
        {
            return m_sIdent;
        }

        //------------------------------------------------------------------------------
        public void SetIdent(string a_sIdent)
        {
            m_sIdent = a_sIdent;
        }

        //------------------------------------------------------------------------------
        internal virtual string AsciiDump()
        {
            return Global.g_sCmdCode[(int)m_eCode];
        }

        //---------------------------------------------------------------------------
        public void AddFlags(EFlags flags)
        {
            m_flags |= flags;
        }

        //---------------------------------------------------------------------------
        public bool IsFlagSet(EFlags flags)
        {
            return (m_flags & flags)==flags;
        }

        //---------------------------------------------------------------------------
        public virtual ICallback AsICallback()
        {
            return null;
        }

        //---------------------------------------------------------------------------
        public virtual IValue AsIValue()
        {
            return null;
        }

        //---------------------------------------------------------------------------
        internal virtual IPrecedence AsIPrecedence()
        {
            return null;
        }

        //---------------------------------------------------------------------------
        public abstract IToken Clone();

        //------------------------------------------------------------------------------
        internal void Compile(string sArg)
        {
        }

        //------------------------------------------------------------------------------
        internal virtual void Release() { }
        private ECmdCode m_eCode;
        private string m_sIdent;
        private int m_nPosExpr;           // Original position of the token in the expression
        //private long m_nRefCount; // Reference counter.
        private EFlags m_flags;


    }


    class GenericToken : IToken
    {
        public GenericToken(ECmdCode a_iCode, string a_sIdent = "") : base(a_iCode, a_sIdent){ }
        protected GenericToken(GenericToken a_Tok) : base(a_Tok) { }

        public override IToken Clone()
        {
            return new GenericToken(this);
        }

        internal override string AsciiDump()
        {
            return this.Dump();
        }
    }
   
    [Flags]
    public enum EFlags
    {
        flNONE = 0,
        flVOLATILE = 1
    }
    

    class TokenPtr<T>
    {
        private readonly T _type;

        public TokenPtr(T type)
        {
            _type = type;
        }

        public T Get()
        {
            return _type;
        }

        public static implicit operator T(TokenPtr<T> tp) => tp.Get();
        public static implicit operator TokenPtr<T>(T type) => new TokenPtr<T>(type);
    }
}
