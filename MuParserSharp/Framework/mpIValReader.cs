using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MuParserSharp.Parser;
using MuParserSharp.Util;

namespace MuParserSharp.Framework
{
    public abstract class IValueReader
    {
        protected IValueReader() { }

        //--------------------------------------------------------------------------------------------
        protected IValueReader(IValueReader refr){
            m_pTokenReader = refr.m_pTokenReader;
        }

        protected IValueReader(string pattern)
        {
            _regex = new Regex(pattern, RegexOptions.Compiled);
        }

        //--------------------------------------------------------------------------------------------
        protected bool FindToken(string text, ref int pos, out string token)
        {
            for (var i = text.Length; i > pos; i--)
                if(Char.IsWhiteSpace(text[i-1])) continue;
                else if (_regex.IsMatch(token = text.Substring(pos, i - pos)))
                {
                    pos = i;
                    return true;
                }

            token = null;
            return false;
        }

        //--------------------------------------------------------------------------------------------
        protected internal virtual void SetParent(TokenReader pTokenReader)
        {

            Global.MUP_VERIFY(() => pTokenReader != null);
            m_pTokenReader = pTokenReader;
        }

        //--------------------------------------------------------------------------------------------
        protected IToken TokenHistory(int pos)
        {
            List<IToken> buf = m_pTokenReader.GetTokens();
            int size = buf.Count;
            return (pos >= size) ? null : buf[size - 1 - pos];
        }

        internal abstract IValueReader Clone(TokenReader pParent);
        //--------------------------------------------------------------------------------------------
        protected IValueReader Clone<T>(TokenReader pParent) where T : IValueReader, new()
        {
            IValueReader pReader = new T();
            pReader.SetParent(pParent);

            return pReader;
        }
        internal abstract bool IsValue(string a_szExpr, ref int a_iPos, ref Value a_Val );

        private TokenReader m_pTokenReader;  // Pointer to the TokenReader class used for token recognition
        private readonly Regex _regex;
    }
}
