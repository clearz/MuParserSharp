using System;
using System.Globalization;
using System.Numerics;
using System.Text;
using MuParserSharp.Framework;

namespace MuParserSharp.Parser
{


    class NumericReader : IValueReader
    {
        internal override bool IsValue(string expression, ref int pos, ref Value value)
        {
            int spos = pos;
            if (expression[pos] <= '9' && expression[pos] >= '0')
            {
                int len = expression.Length;
                ++pos;
                while (pos < len && expression[pos] <= '9' && expression[pos] >= '0') ++pos;
                if (pos < len)
                    if (expression[pos] == '.')
                    {
                        ++pos;
                        while (pos < len && expression[pos] <= '9' && expression[pos] >= '0') ++pos;

                        var i1 = pos - spos;
                        var token1 = expression.Substring(spos, i1);
                        if (double.TryParse(token1, NumberStyles.AllowDecimalPoint, null,
                            out double dres))
                        {
                            value = dres;
                            return true;
                        }
                    }
                    else if (expression[pos] == 'e' || expression[pos] == 'E')
                    {
                        ++pos;
                        if (pos < len && (expression[pos] == '-' || expression[pos] == '+')) ++pos;
                        if (pos < len && expression[pos] <= '9' && expression[pos] >= '0')
                        {
                            ++pos;
                            while (pos < len && expression[pos] <= '9' && expression[pos] >= '0') ++pos;
                            var i1 = pos - spos;
                            var token1 = expression.Substring(spos, i1);
                            if (double.TryParse(token1, NumberStyles.AllowExponent, null,
                                out double dres))
                            {
                                value = dres;
                                return true;
                            }

                            pos = spos;
                            return false;
                        }
                    }


                var i2 = pos - spos;
                var token2 = expression.Substring(spos, i2);
                if (long.TryParse(token2, out long ires))
                {
                    value = ires;
                    return true;
                }
            }

            return false;
        }

        internal override IValueReader Clone(TokenReader reader) => Clone<NumericReader>(reader);
    }

    class CmplxValReader : NumericReader
    {
        internal override bool IsValue(string a_szExpr, ref int a_iPos, ref Value a_Val)
        {
            if (base.IsValue(a_szExpr, ref a_iPos, ref a_Val))
            {
                // Finally check if the next character is an 'i' for the imaginary unit
                // if so this is an imaginary value
                if (a_iPos < a_szExpr.Length && a_szExpr[a_iPos] == 'i')
                {
                    a_Val = new Complex(0.0, (double)a_Val);
                    a_iPos++;
                }

                return true;
            }

            return false;
        }
        internal override IValueReader Clone(TokenReader reader) => Clone<CmplxValReader>(reader);
    }

    class BoolValReader : IValueReader
    {
        public BoolValReader() : base(@"^(true|false)$") { }
        internal override bool IsValue(string a_szExpr, ref int a_iPos, ref Value a_Val)
        {
            if (FindToken(a_szExpr, ref a_iPos, out string token))
            {
                a_Val = bool.Parse(token);
                return true;
            }

            return false;
        }
        internal override IValueReader Clone(TokenReader reader) => Clone<BoolValReader>(reader);
    }

    class HexValReader : IValueReader
    {
        public HexValReader() : base(@"^0[xX][a-fA-F0-9]+$") { }
        internal override bool IsValue(string a_szExpr, ref int a_iPos, ref Value a_Val)
        {
            if (FindToken(a_szExpr, ref a_iPos, out string token))
            {
                try
                {
                    a_Val = Convert.ToInt64(token, 16);
                }
                catch (OverflowException)
                {
                    throw new ParserError(new ErrorContext(EErrorCodes.ecCONVERSION_OVERFLOW, a_iPos, a_szExpr));
                }

                return true;
            }

            return false;
        }
        internal override IValueReader Clone(TokenReader reader) => Clone<HexValReader>(reader);
    }

    class BinValReader : IValueReader
    {
        public BinValReader() : base(@"^0[bB][01]+$") { }
        internal override bool IsValue(string a_szExpr, ref int a_iPos, ref Value a_Val)
        {
            if (FindToken(a_szExpr, ref a_iPos, out string token))
            {
                try
                {
                    a_Val = Convert.ToInt64(token.Substring(2), 2);
                }
                catch (OverflowException)
                {
                    throw new ParserError(new ErrorContext(EErrorCodes.ecCONVERSION_OVERFLOW, a_iPos, a_szExpr));
                }
                return true;
            }

            return false;
        }
        internal override IValueReader Clone(TokenReader reader) => Clone<BinValReader>(reader);
    }

    class StrValReader : IValueReader
    {
        internal override bool IsValue(string a_szExpr, ref int a_iPos, ref Value a_Val)
        {
            if (a_szExpr[a_iPos] != '"')
                return false;

            a_Val = Unescape(a_szExpr, ref a_iPos);
            return true;
        }
        internal override IValueReader Clone(TokenReader reader) => Clone<StrValReader>(reader);

        private string Unescape(string szExpr, ref int nPos)
        {
            nPos++;
            var sb = new StringBuilder();
            bool bEscape = false;

            for (char c = szExpr[nPos]; c != 0; c = szExpr[++nPos])
            {
                switch (c)
                {
                    case '\\':
                        if (!bEscape)
                        {
                            bEscape = true;
                        }
                        else
                            goto default;
                        break;
                    case '"':
                        if (!bEscape)
                        {
                            nPos++;
                            return sb.ToString();
                        }
                        goto default;
                    default:

                        if (bEscape)
                        {
                            switch (c)
                            {
                                case 'n':
                                    sb.Append('\n');
                                    break;
                                case 'r':
                                    sb.Append('\r');
                                    break;
                                case 't':
                                    sb.Append('\t');
                                    break;
                                case '"':
                                    sb.Append('\"');
                                    break;
                                case '\\':
                                    sb.Append('\\');
                                    break;
                                default:
                                    throw new ParserError(new ErrorContext(EErrorCodes.ecUNKNOWN_ESCAPE_SEQUENCE, nPos));
                            }

                            bEscape = false;
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }
            throw new ParserError(new ErrorContext(EErrorCodes.ecUNTERMINATED_STRING, nPos));
        }
    }
}
