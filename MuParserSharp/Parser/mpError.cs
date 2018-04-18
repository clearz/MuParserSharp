using System;
using System.Runtime.CompilerServices;

namespace MuParserSharp.Parser
{
    public class ParserErrorMsg
    {
        public static ParserMessageProviderBase Instance()
        {
            if (m_pInstance == null)
            {
                m_pInstance = ParserMessageProviderEnglish.Instance;
                m_pInstance.Init();
            }

            return m_pInstance;
        }

        public static void Reset(ParserMessageProviderBase pProvider)
        {
            if (pProvider != null)
            {
                m_pInstance = pProvider;
                m_pInstance.Init();
            }
        }

        public string GetErrorMsg(EErrorCodes eError)
        {
            if (m_pInstance == null)
                return "";
            else
                return m_pInstance.GetErrorMsg(eError);
        }

        private static ParserMessageProviderBase m_pInstance;
    }

    public class ErrorContext
    {
        public ErrorContext(EErrorCodes a_iErrc = EErrorCodes.ecUNDEFINED, int a_iPos = -1, string a_sIdent = "")
            : this(a_iErrc, a_iPos, a_sIdent, '\0', '\0', -1)
        { }

        public ErrorContext(EErrorCodes a_iErrc, int a_iPos, string a_sIdent, char cType1, char cType2, int nArg)
        {
            Expr = "";
            Ident = a_sIdent;
            Hint = "";
            Errc = a_iErrc;
            Type1 = cType1;
            Type2 = cType2;
            Arg = nArg;
            Pos = a_iPos;
        }

        public string Expr;  ///> The expression string.
        public string Ident; ///> The identifier of the token that caused the error.
        public string Hint;  ///> Additional message
        public EErrorCodes Errc;  ///> The error code
        public char Type1;   ///> For type conflicts only! This is the type that was actually found.
        public char Type2;   ///> For type conflicts only! This is the type that was expected.
        public int Arg;           ///> The number of arguments that were expected.
        public int Pos;           ///> Position inside the expression where the error occured.
    }

    public class ParserError : Exception
    {

        public ParserError(string sMsg = "",
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0) : base(sMsg)
        {
            _file = file.Substring(file.LastIndexOf('\\') + 1);
            _member = member;
            _line = line;
            m_sMsg = sMsg;
            m_ErrMsg = ParserErrorMsg.Instance();
            m_Err = new ErrorContext();
        }

        public ParserError(ErrorContext a_Err, Exception e = null,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0) : base(GenMsg(a_Err, ParserMessageProviderEnglish.Instance.GetErrorMsg(a_Err.Errc)), e)
        {
            _file = file.Substring(file.LastIndexOf('\\') + 1);
            _member = member;
            _line = line;

            m_ErrMsg = ParserErrorMsg.Instance();
            m_sMsg = m_ErrMsg.GetErrorMsg(a_Err.Errc);
            m_Err = a_Err;
        }

        public ParserError(ParserError a_Obj) : this()
        {
            m_Err = a_Obj.m_Err;
            m_sMsg = a_Obj.m_sMsg;
        }

        public string GetExpr()
        {
            return m_Err.Expr;
        }

        public string GetToken()
        {
            return m_Err.Ident;
        }

        public string GetMsg()
        {
            return GenMsg(m_Err, m_sMsg);
        }

        private static string GenMsg(ErrorContext m_Err, string sMsg)
        {
            ;
            if (m_Err != null)
            {
                ReplaceSubString(ref sMsg, "$EXPR$", m_Err.Expr);
                ReplaceSubString(ref sMsg, "$IDENT$", m_Err.Ident);
                ReplaceSubString(ref sMsg, "$POS$", m_Err.Pos);
                ReplaceSubString(ref sMsg, "$ARG$", m_Err.Arg);
                ReplaceSubString(ref sMsg, "$TYPE1$", m_Err.Type1);
                ReplaceSubString(ref sMsg, "$TYPE2$", m_Err.Type2);
                ReplaceSubString(ref sMsg, "$HINT$", m_Err.Hint);
            }

            return sMsg;
        }
        public int GetPos()
        {
            return m_Err.Pos;
        }
        public EErrorCodes GetCode()
        {
            return m_Err.Errc;
        }

        public ErrorContext GetContext()
        {
            return m_Err;
        }


        private static void ReplaceSubString(ref string strSource, string strFind, object replaceWith)
        {
            strSource = strSource.Replace(strFind, replaceWith.ToString());
        }
        public static implicit operator ParserError(EErrorCodes e) => new ParserError();
        private void Reset()
        {
            m_sMsg = "";
            m_Err = new ErrorContext();
        }

        public string _file;
        private string _member;
        public int _line;
        private ErrorContext m_Err;  // Error context data
        private string m_sMsg;  // The message string with all wildcards still in place.
        private readonly ParserMessageProviderBase m_ErrMsg;

        public void Print()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"{GetMsg()} - ErrorCode: ({200 + (int)GetCode()}) {GetCode()}");
#if DEBUG
            if (!string.IsNullOrWhiteSpace(_file))
                Console.WriteLine($"  File: '{_file}', Member: '{_member}()', Line: {_line}");
#endif
            Console.ResetColor();
        }
    }
}
