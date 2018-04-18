using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using MuParserSharp.Framework;
using MuParserSharp.Parser;

namespace MuParserSharp.Util
{
    public static class Extensions
    {
        public static int FindFirstNotOf(this string source, string chars, int pos = 0)
        {
            if (source.Length == 0) return -1;
            if (chars.Length == 0) return 0;

            for (var i = pos; i < source.Length; i++)
                if (chars.IndexOf(source[i]) == -1) return i;

            return -1;
        }

        public static int FindFirstOf(this string source, char c, int pos = 0)
        {
            if (source.Length == 0) return -1;

            return source.IndexOf(c, pos);
        }
        public static unsafe string MemoryAddress(this object a)
        {
            var tr = new TypedReference();
            tr = __makeref(a);
            var ptr = **(IntPtr**)(&tr);

            return ptr.ToString("x");
        }


        public static string Dump(this IToken o, params object[] strz)
        {
            Global.MUP_VERIFY(() => strz.Length % 2 == 0);
            const char DELIM = ';';
            var ss = new StringBuilder();

            ss.Append($"{Global.g_sCmdCode[(int)o.GetCode()], -9}");
            ss.Append($" [ addr = 0x{o.MemoryAddress()}");
            for (int i = 0; i < strz.Length; i += 2)
                ss.Append($"{DELIM} {strz[i]} = {strz[i + 1]}");
            ss.Append("]");

            return ss.ToString();
        }
    }

    static class Global
    {
        public const string MUP_PARSER_VERSION = "0.6.7 (29032018; Dev)";
        public static string Version => MUP_PARSER_VERSION + " " + new FileInfo(Assembly.GetExecutingAssembly().Location).LastWriteTime;
        public static string[] g_sCmdCode = { "BRCK. OPEN", "BRCK. CLOSE", "IDX OPEN", "IDX CLOSE", "CURLY BRCK. OPEN", "CURLY BRCK. CLOSE", "ARG_SEP", "IF", "ELSE", "ENDIF", "JMP", "VAL", "FUNC", "OPRT_BIN", "OPRT_IFX", "OPRT_PFX", "END", "SCR_ENDL", "SCR_CMT", "SCR_WHILE", "SCR_GOTO", "SCR_LABEL", "SCR_FOR", "SCR_IF", "SCR_ELSE", "SCR_ELIF", "SCR_ENDIF", "SCR_FUNC", "UNKNOWN" };

        public static int StringLengthComparer(string x, string y)
        {
            int c = y.Length.CompareTo(x.Length);
            return c == 0 ? String.Compare(x, y, StringComparison.Ordinal) : c;
        }


        [Conditional("DEBjUG")]
        [Conditional("TESTING")]
        public static void MUP_VERIFY(Func<bool> exp,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0)
        {
         //   return;
            if (!exp())
            {
                StackTrace st = new StackTrace();
                var estr = exp.ToString();
                var pos = estr.LastIndexOf(".", StringComparison.Ordinal);
                var s = pos != -1 ? $"({estr.Substring( + 1)}" : estr;
                var str = $"Assertion failed in file: '{Path.GetFileName(file)}', line: {line}, member: '{member}()'";
                throw new ParserError(str, file, member, line);
            }
        }
    }
}
