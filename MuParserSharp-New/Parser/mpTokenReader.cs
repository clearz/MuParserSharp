using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using MuParserSharp.Framework;
using MuParserSharp.Operators;
using MuParserSharp.Util;

namespace MuParserSharp.Parser
{
    using TokenMap = Dictionary<string, IToken>;
    public class TokenReader
    {
        //---------------------------------------------------------------------------
        /*  Copy constructor.
            \sa Assign
        */
        internal TokenReader(TokenReader a_Reader)
        {
            m_vValueReader = new List<IValueReader>();
            Assign(a_Reader);
        }

        //---------------------------------------------------------------------------
        /*  Assign state of a token reader to this token reader.
            \param a_Reader Object from which the state should be copied.
        */
        private void Assign(TokenReader obj)
        {
            m_pParser = obj.m_pParser;
            m_sExpr = obj.m_sExpr;
            m_nPos = obj.m_nPos;
            m_nNumBra = obj.m_nNumBra;
            m_nNumIndex = obj.m_nNumIndex;
            m_nNumCurly = obj.m_nNumCurly;
            m_nNumIfElse = obj.m_nNumIfElse;
            m_nSynFlags = obj.m_nSynFlags;
            m_UsedVar = obj.m_UsedVar;
            m_pVarDef = obj.m_pVarDef;
            m_pPostOprtDef = obj.m_pPostOprtDef;
            m_pInfixOprtDef = obj.m_pInfixOprtDef;
            m_pOprtDef = obj.m_pOprtDef;
            m_pFunDef = obj.m_pFunDef;
            m_pConstDef = obj.m_pConstDef;
            m_pDynVarShadowValues = obj.m_pDynVarShadowValues;
            m_vTokens = obj.m_vTokens;

            DeleteValReader();
            int i, iSize = obj.m_vValueReader.Count;
            for (i = 0; i < iSize; ++i)
            {
                m_vValueReader.Add(obj.m_vValueReader[i].Clone(this));
            }
        }

        //---------------------------------------------------------------------------
        /*  Constructor.

            Create a Token reader and bind it to a parser object.

            \pre [assert] a_pParser may not be nullptr
            \post #m_pParser==a_pParser
            \param a_pParent Parent parser object of the token reader.
        */
        internal TokenReader(ParserXBase a_pParent)
        {
            m_sExpr = "";
            m_nPos = 0;
            m_nNumBra = 0;
            m_nNumIndex = 0;
            m_nNumCurly = 0;
            m_nNumIfElse = 0;
            m_nSynFlags = 0;
            m_vTokens = new List<IToken>();
            m_eLastTokCode = ECmdCode.cmUNKNOWN;
            m_vValueReader = new List<IValueReader>();
            m_UsedVar = new TokenMap();
            m_fZero = 0;
            SetParent(a_pParent);
        }

        //---------------------------------------------------------------------------
        internal void DeleteValReader()
        {
            if(m_vValueReader != null)
            m_vValueReader.Clear();

        }

        //---------------------------------------------------------------------------
        /*  Create instance of a ParserTokenReader identical with this
                  and return its pointer.

                  This is a factory method the calling function must take care of the object destruction.

                  \return A new ParserTokenReader object.
        */
        internal TokenReader Clone(ParserXBase parserXBase1)
        {
            return new TokenReader(this);
        }

        //---------------------------------------------------------------------------
        internal void AddValueReader(IValueReader a_pReader)
        {
            a_pReader.SetParent(this);
            m_vValueReader.Add(a_pReader);
        }

        //---------------------------------------------------------------------------
        internal void AddSynFlags(ESynCodes flag)
        {
            m_nSynFlags |= flag;
        }

        //---------------------------------------------------------------------------
        internal List<IToken> GetTokens()
        {
            return m_vTokens;
        }

        //---------------------------------------------------------------------------
        internal int GetPos()
        {
            return m_nPos;
        }

        //---------------------------------------------------------------------------
        internal string GetExpr()
        {
            return m_sExpr;
        }

        //---------------------------------------------------------------------------
        /*  Return a map containing the used variables only.
        */
        internal TokenMap GetUsedVar()
        {
            return m_UsedVar;
        }

        /*  Initialize the token Reader.

	    Sets the expression position index to zero and set Syntax flags to
	    default for initial parsing.
        */
        internal void SetExpr(string a_sExpr)
        {
            m_sExpr = a_sExpr; // + string_type(_T(" "));
            ReInit();
        }

        //---------------------------------------------------------------------------
        /*  Reset the token reader to the start of the formula.
            \post #m_nPos==0, #m_nSynFlags = noOPT | noBC | noPOSTOP | noSTR
            \sa ESynCodes

            The syntax flags will be reset to a value appropriate for the
            start of a formula.
        */
        internal void ReInit()
        {
            m_nPos = 0;
            m_nNumBra = 0;
            m_nNumIndex = 0;
            m_nNumCurly = 0;
            m_nNumIfElse = 0;
            m_nSynFlags = ESynCodes.noOPT | ESynCodes.noBC | ESynCodes.noCBC | ESynCodes.noPFX | ESynCodes.noCOMMA | ESynCodes.noIO | ESynCodes.noIC | ESynCodes.noIF | ESynCodes.noELSE;
            m_UsedVar.Clear();
            m_eLastTokCode = ECmdCode.cmUNKNOWN;
            m_vTokens.Clear();
        }

        internal IToken Store(IToken t, int token_pos)
        {
            m_eLastTokCode = t.GetCode();
            t.SetExprPos(token_pos);
            m_vTokens.Add(t);
            return t;
        }

        //---------------------------------------------------------------------------
        internal void SkipCommentsAndWhitespaces()
        {
            bool bSkip = true;
            while (m_nPos < m_sExpr.Length && bSkip)
            {
                switch (m_sExpr[m_nPos])
                {
                    // skip comments
                    case '#':
                        int i = m_sExpr.FindFirstOf('\n', m_nPos + 1);
                        m_nPos = i != -1 ? i : m_sExpr.Length;
                        break;
                    // skip whitespaces
                    case ' ':
                        ++m_nPos;
                        break;
                    default:
                        bSkip = false;
                        break;
                } 
            }
        }

        public IToken ReadNextToken()
        {
            Global.MUP_VERIFY(() => m_pParser != null);

            SkipCommentsAndWhitespaces();

            int token_pos = m_nPos;
            IToken pTok = null;

            // Check for end of expression
            if (IsEOF(ref pTok))
                return Store(pTok, token_pos);

            if (IsNewline(ref pTok))
                return Store(pTok, token_pos);

            if ((m_nSynFlags & ESynCodes.noOPT) == 0 && IsOprt(ref pTok))
                return Store(pTok, token_pos); // Check for user defined binary operator

            if ((m_nSynFlags & ESynCodes.noIFX) == 0 && IsInfixOpTok(ref pTok))
                return Store(pTok, token_pos); // Check for unary operators

            if (IsValTok(ref pTok))
                return Store(pTok, token_pos); // Check for values / constant tokens

            if (IsBuiltIn(ref pTok))
                return Store(pTok, token_pos); // Check built in operators / tokens

            if (IsVarOrConstTok(ref pTok))
                return Store(pTok, token_pos); // Check for variable tokens

            if (IsFunTok(ref pTok))
                return Store(pTok, token_pos);

            if ((m_nSynFlags & ESynCodes.noPFX) == 0 && IsPostOpTok(ref pTok))
                return Store(pTok, token_pos); // Check for unary operators

            // 2.) We have found no token, maybe there is a token that we don't expect here.
            //     Again call the Identifier functions but this time only those we don't expect
            //     to find.
            if ((m_nSynFlags & ESynCodes.noOPT) > 0 && IsOprt(ref pTok))
                return Store(pTok, token_pos); // Check for user defined binary operator

            if ((m_nSynFlags & ESynCodes.noIFX) > 0 && IsInfixOpTok(ref pTok))
                return Store(pTok, token_pos); // Check for unary operators

            if ((m_nSynFlags & ESynCodes.noPFX) > 0 && IsPostOpTok(ref pTok))
                return Store(pTok, token_pos); // Check for unary operators
                                               // </ibg>

            // Now we are in trouble because there is something completely unknown....

            // Check the string for an undefined variable token. This is done
            // only if a flag is set indicating to ignore undefined variables.
            // This is a way to conditionally avoid an error if undefined variables
            // occur. The GetExprVar function must supress the error for undefined
            // variables in order to collect all variable names including the
            // undefined ones.
            if ((m_pParser.m_bIsQueryingExprVar || m_pParser.m_bAutoCreateVar) && IsUndefVarTok(ref pTok))
                return Store(pTok, token_pos);

            // Check for unknown token
            //
            // !!! From this point on there is no exit without an exception possible...
            //
            string sTok = "";
            int iEnd = ExtractToken(m_pParser.ValidNameChars(), ref sTok, m_nPos);

            var err = new ErrorContext();
            err.Errc = EErrorCodes.ecUNASSIGNABLE_TOKEN;
            err.Expr = m_sExpr;
            err.Pos = m_nPos;

            if (iEnd != m_nPos)
                err.Ident = sTok;
            else
                err.Ident = m_sExpr.Substring(m_nPos);

            throw new ParserError(err);
        }

        //---------------------------------------------------------------------------
        void SetParent(ParserXBase a_pParent)
        {
            m_pParser = a_pParent;
            m_pFunDef = a_pParent.m_FunDef;
            m_pOprtDef = a_pParent.m_OprtDef;
            m_pInfixOprtDef = a_pParent.m_InfixOprtDef;
            m_pPostOprtDef = a_pParent.m_PostOprtDef;
            m_pVarDef = a_pParent.m_varDef;
            m_pConstDef = a_pParent.m_valDef;
            m_pDynVarShadowValues = a_pParent.m_valDynVarShadow;
        }

        /*  Extract all characters that belong to a certain charset.
	    \param a_szCharSet [in] Const char array of the characters allowed in the token.
	    \param a_strTok [out]  The string that consists entirely of characters listed in a_szCharSet.
	    \param a_iPos [in] Position in the string from where to start reading.
	    \return The Position of the first character not listed in a_szCharSet.
        */
        int ExtractToken(string a_szCharSet, ref string a_sTok, int a_iPos)
        {

            int iEnd = m_sExpr.FindFirstNotOf(a_szCharSet, a_iPos);

            if (iEnd == -1)
                iEnd = m_sExpr.Length;

            if (iEnd != a_iPos)
                a_sTok = m_sExpr.Substring(a_iPos, iEnd - a_iPos);

            return iEnd;
        }

        //---------------------------------------------------------------------------
        /*  Check if a built in operator or other token can be found.
        */
        private bool IsBuiltIn(ref IToken a_Tok)
        {
            string[] pOprtDef = m_pParser.GetOprtDef();

            //string szFormula = m_sExpr;
            int i;
            // Compare token with function and operator strings
            // check string for operator/function
            for (i = 0; i < pOprtDef.Length; i++)
            {
                int len = pOprtDef[i].Length;
                if (pOprtDef[i] == m_sExpr.Substring(m_nPos, len))
                {
                    var cmd = (ECmdCode) i;
                    switch (cmd)
                    {
                        case ECmdCode.cmARG_SEP:
                            if ((m_nSynFlags & ESynCodes.noCOMMA) > 0)
                                throw Error(EErrorCodes.ecUNEXPECTED_COMMA);

                            m_nSynFlags = ESynCodes.noBC | ESynCodes.noCBC | ESynCodes.noOPT | ESynCodes.noEND |
                                          ESynCodes.noNEWLINE | ESynCodes.noCOMMA | ESynCodes.noPFX | ESynCodes.noIC |
                                          ESynCodes.noIO | ESynCodes.noIF | ESynCodes.noELSE;
                            a_Tok = (new GenericToken((ECmdCode) i, pOprtDef[i]));
                            break;

                        case ECmdCode.cmELSE:
                            if ((m_nSynFlags & ESynCodes.noELSE) > 0)
                                throw Error(EErrorCodes.ecMISPLACED_COLON);

                            m_nNumIfElse--;
                            if (m_nNumIfElse < 0)
                                throw Error(EErrorCodes.ecMISPLACED_COLON);

                            m_nSynFlags = ESynCodes.noBC | ESynCodes.noCBC | ESynCodes.noIO | ESynCodes.noIC |
                                          ESynCodes.noPFX | ESynCodes.noEND | ESynCodes.noNEWLINE | ESynCodes.noCOMMA |
                                          ESynCodes.noOPT | ESynCodes.noIF | ESynCodes.noELSE;
                            a_Tok = (new TokenIfThenElse(ECmdCode.cmELSE));
                            break;

                        case ECmdCode.cmIF:
                            if ((m_nSynFlags & ESynCodes.noIF) > 0)
                                throw Error(EErrorCodes.ecUNEXPECTED_CONDITIONAL);

                            m_nNumIfElse++;
                            m_nSynFlags = ESynCodes.noBC | ESynCodes.noCBC | ESynCodes.noIO | ESynCodes.noPFX |
                                          ESynCodes.noIC | ESynCodes.noEND | ESynCodes.noNEWLINE | ESynCodes.noCOMMA |
                                          ESynCodes.noOPT | ESynCodes.noIF | ESynCodes.noELSE;
                            a_Tok = (new TokenIfThenElse(ECmdCode.cmIF));
                            break;

                        case ECmdCode.cmBO:
                            if ((m_nSynFlags & ESynCodes.noBO) > 0)
                                throw Error(EErrorCodes.ecUNEXPECTED_PARENS);

                            if (m_eLastTokCode == ECmdCode.cmFUNC)
                            {
                                m_nSynFlags = ESynCodes.noOPT | ESynCodes.noEND | ESynCodes.noNEWLINE |
                                              ESynCodes.noCOMMA | ESynCodes.noPFX | ESynCodes.noIC | ESynCodes.noIO |
                                              ESynCodes.noIF | ESynCodes.noELSE | ESynCodes.noCBC;
                            }
                            else
                            {
                                m_nSynFlags = ESynCodes.noBC | ESynCodes.noOPT | ESynCodes.noEND | ESynCodes.noNEWLINE |
                                              ESynCodes.noCOMMA | ESynCodes.noPFX | ESynCodes.noIC | ESynCodes.noIO |
                                              ESynCodes.noIF | ESynCodes.noELSE | ESynCodes.noCBC;
                            }

                            m_nNumBra++;
                            a_Tok = (new GenericToken((ECmdCode) i, pOprtDef[i]));
                            break;

                        case ECmdCode.cmBC:
                            if ((m_nSynFlags & ESynCodes.noBC) > 0)
                                throw Error(EErrorCodes.ecUNEXPECTED_PARENS);

                            m_nSynFlags = ESynCodes.noBO | ESynCodes.noVAR | ESynCodes.noVAL | ESynCodes.noFUN |
                                          ESynCodes.noIFX | ESynCodes.noCBO;
                            m_nNumBra--;

                            if (m_nNumBra < 0)
                                throw Error(EErrorCodes.ecUNEXPECTED_PARENS);

                            a_Tok = (new GenericToken((ECmdCode) i, pOprtDef[i]));
                            break;

                        case ECmdCode.cmIO:
                            if ((m_nSynFlags & ESynCodes.noIO) > 0)
                                throw Error(EErrorCodes.ecUNEXPECTED_SQR_BRACKET);

                            m_nSynFlags = ESynCodes.noIC | ESynCodes.noIO | ESynCodes.noOPT | ESynCodes.noPFX |
                                          ESynCodes.noBC | ESynCodes.noNEWLINE | ESynCodes.noCBC | ESynCodes.noCOMMA;
                            m_nNumIndex++;
                            a_Tok = (new GenericToken((ECmdCode) i, pOprtDef[i]));
                            break;

                        case ECmdCode.cmIC:
                            if ((m_nSynFlags & ESynCodes.noIC) > 0)
                                throw Error(EErrorCodes.ecUNEXPECTED_SQR_BRACKET);

                            m_nSynFlags = ESynCodes.noBO | ESynCodes.noIFX | ESynCodes.noCBO;
                            m_nNumIndex--;

                            if (m_nNumIndex < 0)
                                throw Error(EErrorCodes.ecUNEXPECTED_SQR_BRACKET);

                            a_Tok = (new OprtIndex());
                            break;

                        case ECmdCode.cmCBO:
                            if ((m_nSynFlags & ESynCodes.noVAL) > 0)
                                throw Error(EErrorCodes.ecUNEXPECTED_CURLY_BRACKET);

                            m_nSynFlags = ESynCodes.noCBC | ESynCodes.noIC | ESynCodes.noIO | ESynCodes.noOPT |
                                          ESynCodes.noPFX | ESynCodes.noBC | ESynCodes.noNEWLINE | ESynCodes.noCOMMA |
                                          ESynCodes.noIF;
                            m_nNumCurly++;
                            a_Tok = (new GenericToken((ECmdCode) i, pOprtDef[i]));
                            break;

                        case ECmdCode.cmCBC:
                            if ((m_nSynFlags & ESynCodes.noIC) > 0)
                                throw Error(EErrorCodes.ecUNEXPECTED_CURLY_BRACKET);

                            m_nSynFlags = ESynCodes.noBO | ESynCodes.noCBO | ESynCodes.noIFX;
                            m_nNumCurly--;

                            if (m_nNumCurly < 0)
                                throw Error(EErrorCodes.ecUNEXPECTED_CURLY_BRACKET);

                            a_Tok = (new OprtCreateArray());
                            break;

                        default: // The operator is listed in c_DefaultOprt, but not here. This is a bad thing...
                            throw Error(EErrorCodes.ecINTERNAL_ERROR, pOprtDef[i]);

                        
                    } // switch operator id

                    m_nPos += len;
                    return true;
                } // if operator string found
            } // end of for all operator strings

            return false;
        }
        //---------------------------------------------------------------------------
        /*  Check for End of expression
        */
        private bool IsNewline(ref IToken a_Tok)
        {
            // nicht nach:  bionop, infixop, argumentseparator,
            // erlaubt nach:   Werten, variablen, schließenden klammern, schliessendem index
            bool bRet = false;

            if (m_sExpr[m_nPos] == '\n')
            {
                // Check if all brackets were closed
                if ((m_nSynFlags & ESynCodes.noNEWLINE) > 0)
                    throw Error(EErrorCodes.ecUNEXPECTED_NEWLINE);

                if (m_nNumBra > 0)
                    throw Error(EErrorCodes.ecMISSING_PARENS);

                if (m_nNumIndex > 0)
                    throw Error(EErrorCodes.ecMISSING_SQR_BRACKET);

                if (m_nNumCurly > 0)
                    throw Error(EErrorCodes.ecMISSING_CURLY_BRACKET);

                if (m_nNumIfElse > 0)
                    throw Error(EErrorCodes.ecMISSING_ELSE_CLAUSE);

                m_nPos++;
                m_nSynFlags = ESynCodes.sfSTART_OF_LINE;
                a_Tok = (new TokenNewline());
                bRet = true;
            }


            return bRet;
        }

        //---------------------------------------------------------------------------
        /*  Check for End of expression
        */
        private bool IsEOF(ref IToken a_Tok)
        {
            bool bRet = false;

                if (m_sExpr.Length > 0 && m_nPos >= m_sExpr.Length)
                {
                    if ((m_nSynFlags & ESynCodes.noEND) > 0)
                        throw Error(EErrorCodes.ecUNEXPECTED_EOF);

                    if (m_nNumBra > 0)
                        throw Error(EErrorCodes.ecMISSING_PARENS);

                    if (m_nNumCurly > 0)
                        throw Error(EErrorCodes.ecMISSING_CURLY_BRACKET);

                    if (m_nNumIndex > 0)
                        throw Error(EErrorCodes.ecMISSING_SQR_BRACKET);

                    if (m_nNumIfElse > 0)
                        throw Error(EErrorCodes.ecMISSING_ELSE_CLAUSE);

                    m_nSynFlags = 0;
                    a_Tok = new GenericToken(ECmdCode.cmEOE);
                    bRet = true;
                }

            return bRet;
        }

        private ParserError Error(EErrorCodes code, string ident = "", bool isLen = false,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0)
        {
            return new ParserError(
                new ErrorContext
                {
                    Errc = code,
                    Ident = ident,
                    Expr = m_sExpr,
                    Pos = isLen ? m_nPos - m_sExpr.Length : m_nPos
                },null, file, member, line);
        }
        //---------------------------------------------------------------------------
        /*  Check if a string position contains a unary infix operator.
            \return true if a function token has been found false otherwise.
        */
        private bool IsInfixOpTok(ref IToken a_Tok)
        {
            string sTok = "";
            int iEnd = ExtractToken(m_pParser.ValidInfixOprtChars(), ref sTok, m_nPos);

            if (iEnd == m_nPos)
                return false;
            
            foreach(var item in m_pInfixOprtDef)
            {
                if (sTok.IndexOf(item.Key, StringComparison.Ordinal) != 0)
                    continue;

                a_Tok = (item.Value);
                m_nPos += item.Key.Length;

                if ((m_nSynFlags & ESynCodes.noIFX) > 0)
                    throw Error(EErrorCodes.ecUNEXPECTED_OPERATOR);

                m_nSynFlags = ESynCodes.noPFX | ESynCodes.noIFX | ESynCodes.noOPT | ESynCodes.noBC | ESynCodes.noIC | ESynCodes.noIO | ESynCodes.noEND | ESynCodes.noCOMMA | ESynCodes.noNEWLINE | ESynCodes.noIF | ESynCodes.noELSE;
                return true;
            }

            return false;
        }

        //---------------------------------------------------------------------------
        /*  Check expression for function tokens.
        */
        private bool IsFunTok(ref IToken a_Tok)
        {
            if (!m_pFunDef.Any())
                return false;

            string sTok = "";
            int iEnd = ExtractToken(m_pParser.ValidNameChars(), ref sTok, m_nPos);

            if (iEnd == m_nPos)
                return false;

            if (!m_pFunDef.ContainsKey(sTok))
                return false;

            var item = m_pFunDef[sTok];
            m_nPos = iEnd;
            a_Tok = item;
            if ((m_nSynFlags & ESynCodes.noFUN) > 0)
                throw Error(EErrorCodes.ecUNEXPECTED_FUN,  a_Tok.GetIdent(), isLen: true);

            m_nSynFlags = ESynCodes.sfALLOW_NONE ^ ESynCodes.noBO;
            return true;
        }

        //---------------------------------------------------------------------------
        /*  Check if a string position contains a unary post value operator.
        */
        private bool IsPostOpTok(ref IToken a_Tok)
        {
            if ((m_nSynFlags & ESynCodes.noPFX) > 0)
            {
                // <ibg 2014-05-30/> Only look for postfix operators if they are allowed at the given position.
                //                   This will prevent conflicts with variable names such as:
                //                   "sin(n)" where n is the postfix for "nano"
                return false;
                // </ibg>
            }

            string sTok = "";
            int iEnd = ExtractToken(m_pParser.ValidOprtChars(), ref sTok, m_nPos);

            if (iEnd == m_nPos)
                return false;

            foreach (var item in m_pPostOprtDef)
            {
                if (sTok.IndexOf(item.Key, StringComparison.Ordinal) != 0)
                    continue;

                a_Tok = (item.Value);
                m_nPos += item.Key.Length;

                if ((m_nSynFlags & ESynCodes.noPFX) > 0)
                    throw Error(EErrorCodes.ecUNEXPECTED_OPERATOR, a_Tok.GetIdent(), isLen: true);

                m_nSynFlags = ESynCodes.noVAL | ESynCodes.noVAR | ESynCodes.noFUN | ESynCodes.noBO | ESynCodes.noPFX | ESynCodes.noIF;
                return true;
            }

            return false;
        }

        //---------------------------------------------------------------------------
        /*  Check if a string position contains a binary operator.
        */
        private bool IsOprt(ref IToken a_Tok)
        {
            string sTok = "";
            int iEnd = ExtractToken(m_pParser.ValidOprtChars(), ref sTok, m_nPos);

            if (iEnd == m_nPos)
                return false;
            // Note:
            // All tokens in oprt_bin_maptype are have been sorted by their length
            // Long operators must come first! Otherwise short names (like: "add") that
            // are part of long token names (like: "add123") will be found instead
            // of the long ones.
            // Length sorting is done with ascending length so we use a reverse iterator here.
            foreach (var item in m_pOprtDef)
            {
                if (sTok.IndexOf(item.Key, StringComparison.Ordinal) != 0)
                    continue;

                // operator found, check if we expect one...
                if ((m_nSynFlags & ESynCodes.noOPT) > 0)
                {
                    // An operator was found but is not expected to occur at
                    // this position of the formula, maybe it is an infix
                    // operator, not a binary operator. Both operator types
                    // can use the same characters in their identifiers.
                    if (IsInfixOpTok(ref a_Tok))
                        return true;

                    // nope, it's no infix operator and we dont expect
                    // an operator
                    throw Error(EErrorCodes.ecUNEXPECTED_OPERATOR, item.Key);
                }
                
                a_Tok = item.Value;
                m_nPos += a_Tok.GetIdent().Length;
                
                m_nSynFlags = ESynCodes.noBC | ESynCodes.noIO | ESynCodes.noIC | ESynCodes.noOPT | ESynCodes.noCOMMA | ESynCodes.noEND | ESynCodes.noNEWLINE | ESynCodes.noPFX | ESynCodes.noIF | ESynCodes.noELSE;
                return true;
            }

            return false;
        }

        //---------------------------------------------------------------------------
        /*  Check whether the token at a given position is a value token.

          Value tokens are either values or constants.

          \param a_Tok [out] If a value token is found it will be placed here.
          \return true if a value token has been found.
        */
        private bool IsValTok(ref IToken a_Tok)
        {
            if (m_vValueReader.Count == 0)
                return false;

            string sTok = "";
            // call the value recognition functions provided by the user
            // Call user defined value recognition functions
            int iSize = m_vValueReader.Count;
            Value val = null;
            for (int i = 0; i < iSize; ++i)
            {
                int iStart = m_nPos;
                if (m_vValueReader[i].IsValue(m_sExpr, ref m_nPos, ref val))
                {
                    sTok = m_sExpr.Substring(iStart, m_nPos- iStart);
                    if ((m_nSynFlags & ESynCodes.noVAL) > 0)
                        throw Error(EErrorCodes.ecUNEXPECTED_VAL, sTok);

                    m_nSynFlags = ESynCodes.noVAL | ESynCodes.noVAR | ESynCodes.noFUN | ESynCodes.noBO | ESynCodes.noIFX | ESynCodes.noIO;
                    a_Tok = val;
                    a_Tok.SetIdent(sTok.Substring(0, m_nPos - iStart));
                    return true;
                }
            }

            return false;
        }

        //---------------------------------------------------------------------------
        /*  Check wheter a token at a given position is a variable token.
            \param a_Tok [out] If a variable token has been found it will be placed here.
            \return true if a variable token has been found.
        */
        private bool IsVarOrConstTok(ref IToken a_Tok)
        {
            if (!m_pVarDef.Any() && !m_pConstDef.Any() && !m_pFunDef.Any())
                return false;

            string sTok = "";
            int iEnd = ExtractToken(m_pParser.ValidNameChars(), ref sTok, m_nPos);
            if (iEnd == m_nPos || (sTok.Any() && sTok[0] >= '0' && sTok[0] <= '9'))
                return false;

            // Check for variables
            if (m_pVarDef.ContainsKey(sTok))
            {
                if ((m_nSynFlags & ESynCodes.noVAR) > 0)
                    throw Error(EErrorCodes.ecUNEXPECTED_VAR, sTok);

                var item = m_pVarDef[sTok];
                m_nPos = iEnd;
                m_nSynFlags = ESynCodes.noVAL | ESynCodes.noVAR | ESynCodes.noFUN | ESynCodes.noBO | ESynCodes.noIFX;
                a_Tok = item;
                a_Tok.SetIdent(sTok);
                m_UsedVar[sTok] = item;  // Add variable to used-var-list
                return true;
            }

            // Check for constants
            if (m_pConstDef.ContainsKey(sTok))
            {
                if ((m_nSynFlags & ESynCodes.noVAL) > 0)
                    throw Error(EErrorCodes.ecUNEXPECTED_VAL, sTok);

                var item = m_pConstDef[sTok];
                m_nPos = iEnd;
                m_nSynFlags = ESynCodes.noVAL | ESynCodes.noVAR | ESynCodes.noFUN | ESynCodes.noBO | ESynCodes.noIFX | ESynCodes.noIO;
                a_Tok = (item);
                a_Tok.SetIdent(sTok);
                return true;
            }

            return false;
        }

        //---------------------------------------------------------------------------
        private bool IsComment()
        {
            return false;
        }

        //---------------------------------------------------------------------------
        /*  Check wheter a token at a given position is an undefined variable.
            \param a_Tok [out] If a variable tom_pParser->m_vStringBufken has been found it will be placed here.
            \return true if a variable token has been found.
        */
        private bool IsUndefVarTok(ref IToken a_Tok)
        {
            string sTok = "";
            int iEnd = ExtractToken(m_pParser.ValidNameChars(), ref sTok, m_nPos);
            if (iEnd == m_nPos || (sTok.Any() && sTok[0] >= '0' && sTok[0] <= '9'))
                return false;
            if ((m_nSynFlags & ESynCodes.noVAR) > 0)
                throw Error(EErrorCodes.ecUNEXPECTED_VAR, sTok);

            // Create a variable token
            if (m_pParser.m_bAutoCreateVar)
            {
                IValue val = new Value();                   // Create new value token
                m_pDynVarShadowValues.Add(val);         // push to the vector of shadow values
                a_Tok = (new Variable(val)); // bind variable to the new value item
                m_pVarDef[sTok] = a_Tok;                    // add new variable to the variable list
            }
            else
                a_Tok = new Variable(null);      // bind variable to empty variable

            a_Tok.SetIdent(sTok);
            m_UsedVar[sTok] = a_Tok;     // add new variable to used-var-list

            m_nPos = iEnd;
            m_nSynFlags = ESynCodes.noVAL | ESynCodes.noVAR | ESynCodes.noFUN | ESynCodes.noBO | ESynCodes.noIFX;
            return true;
        }







        private ParserXBase m_pParser; // Pointer to the parser bound to this token reader
        private string m_sExpr; // The expression beeing currently parsed
        private int m_nPos; // Current parsing position in the expression
        private int m_nNumBra; // Number of open parenthesis
        private int m_nNumIndex; // Number of open index paranethesis    
        private int m_nNumCurly; // Number of open curly brackets
        private int m_nNumIfElse; // Coubter for if-then-else levels
        private ESynCodes m_nSynFlags; // Flags to controll the syntax flow

        private List<IToken> m_vTokens;
        private ECmdCode m_eLastTokCode;

        private TokenMap m_pFunDef;
        private IDictionary<string, IToken> m_pOprtDef;
        private TokenMap m_pInfixOprtDef;
        private TokenMap m_pPostOprtDef;
        private TokenMap m_pConstDef;

        private List<IValue> m_pDynVarShadowValues; // Value items created for holding values of variables created at parser runtime

        private TokenMap m_pVarDef; // The only non const pointer to parser internals

        private readonly List<IValueReader> m_vValueReader; // Value token identification function
        private TokenMap m_UsedVar;
        private float m_fZero; // Dummy value of zero, referenced by undefined variables

    }
}
