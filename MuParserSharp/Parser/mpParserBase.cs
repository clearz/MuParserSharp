using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using MuParserSharp.Framework;
using MuParserSharp.Util;

namespace MuParserSharp.Parser
{
    using TokenMap = Dictionary<string, IToken>;

    public abstract class ParserXBase
    {
        private delegate IValue parse_function_type();
        private static bool s_bDumpStack = false;
        private static bool s_bDumpRPN = false;

        private static readonly string[] c_DefaulOprt = { "(", ")", "[", "]", "{", "}", ",", "?", ":" };
        /* Pointer to the parser function. 
          Eval() calls the function whose address is stored there.
        */
        private parse_function_type m_pParserEngine;

        /*  Managed pointer to the token reader object.
        */
        private TokenReader m_pTokenReader;

        internal List<IValue> m_valDynVarShadow { get; set; }  // Value objects referenced by variables created at parser runtime
        private string m_sNameChars;        // Charset for names
        private string m_sOprtChars;        // Charset for postfix/ binary operator tokens
        private string m_sInfixOprtChars;   // Charset for infix operator tokens
        private int m_nPos;

        /* Index of the final result in the stack array. 
        
          The parser supports expressions using with commas for seperating
          multiple expression. Each comma will increase this number.
          (i.e. "a=10,b=15,c=a*b")
        */
        private int m_nFinalResultIdx;

        /* A flag indicating querying of expression variables is underway.
          

          If this flag is set the parser is momentarily querying the expression 
          variables. In these cases undefined variable errors must be ignored cause 
          the whole point of querying the expression variables is for finding out 
          which variables mut be defined.
        */
        internal bool m_bIsQueryingExprVar;
        internal bool m_bAutoCreateVar;      // If this flag is set unknown variables will be defined automatically
        private readonly RPN m_rpn;                  // reverse polish notation
        private IValue[] m_vStackBuffer;
        private ValueCache m_cache;         // A cache for recycling value items instead of deleting them
        protected internal TokenMap m_FunDef;           // Function definitions
        protected internal TokenMap m_PostOprtDef;  // Postfix operator callbacks
        protected internal TokenMap m_InfixOprtDef; // Infix operator callbacks.
        protected internal IDictionary<string, IToken> m_OprtDef;      // Binary operator callbacks
        protected internal TokenMap m_valDef { get; set; }          // Definition of parser constants
        protected internal TokenMap m_varDef { get; set; }           // user defind variables.


        protected ParserXBase()
        {
            m_FunDef = new TokenMap();
            m_PostOprtDef = new TokenMap();
            m_InfixOprtDef = new TokenMap();
            m_OprtDef = new SortedDictionary<string, IToken>(Comparer<string>.Create(Global.StringLengthComparer));
            m_valDef = new TokenMap();
            m_varDef = new TokenMap();
            m_pParserEngine = ParseFromString;
            m_valDynVarShadow = new List<IValue>();
            m_rpn = new RPN();
            m_cache = new ValueCache();
            // m_vStackBuffer = new List<IValue>();
            InitTokenReader();
        }

        protected ParserXBase(ParserXBase a_Parser) : this()
        {
            m_pTokenReader = new TokenReader(this);
            Assign(a_Parser);
        }


        //---------------------------------------------------------------------------
        /*  Copy state of a parser object to this.
              \param a_Parser the source object.

              Clears Variables and Functions of this parser.
              Copies the states of all internal variables.
              Resets parse function to string parse mode.
        */
        public void Assign(ParserXBase a_Parser)
        {
            if (a_Parser == this)
                return;

            // Don't copy bytecode instead cause the parser to create new bytecode
            // by resetting the parse function.
            ReInit();

            m_pTokenReader = new TokenReader(a_Parser.m_pTokenReader.Clone(this));

            m_OprtDef = a_Parser.m_OprtDef;
            m_FunDef = a_Parser.m_FunDef;
            m_PostOprtDef = a_Parser.m_PostOprtDef;
            m_InfixOprtDef = a_Parser.m_InfixOprtDef;
            m_valDef = a_Parser.m_valDef;
            m_valDynVarShadow = a_Parser.m_valDynVarShadow;
            m_varDef = a_Parser.m_varDef;             // Copy user defined variables

            // Copy charsets
            m_sNameChars = a_Parser.m_sNameChars;
            m_sOprtChars = a_Parser.m_sOprtChars;
            m_sInfixOprtChars = a_Parser.m_sInfixOprtChars;

            m_bAutoCreateVar = a_Parser.m_bAutoCreateVar;

            // Things that should not be copied:
            // - m_vStackBuffer
            // - m_cache
            // - m_rpn
        }

        //---------------------------------------------------------------------------
        /*  Evaluate the expression.
              \pre A formula must be set.
              \pre Variables must have been set (if needed)
              \sa SetExpr
              \return The evaluation result
              \throw ParseException if no Formula is set or in case of any other error related to the formula.

              A note on const correctness:
              I consider it important that Calc is a const function.
              Due to caching operations Calc changes only the state of internal variables with one exception
              m_UsedVar this is reset during string parsing and accessible from the outside. Instead of making
              Calc non const GetExprVar is non const because it explicitely calls Eval() forcing this update.
        */
        public IValue Eval()
        {
            return m_pParserEngine();
        }
        //---------------------------------------------------------------------------
        /*  Return the strings of all Operator identifiers.
              \return Returns a pointer to the c_DefaulOprt array of const char *.
             
              GetOprtDef is a const function returning a pinter to an array of const char pointers.
        */
        public string[] GetOprtDef()
        {
            return c_DefaulOprt;
        }

        //---------------------------------------------------------------------------
        /*  Define the set of valid characters to be used in names of
                      functions, variables, constants.
        */
        public void DefineNameChars(string a_szCharset) { m_sNameChars = a_szCharset; }

        //---------------------------------------------------------------------------
        /*  Define the set of valid characters to be used in names of
                     binary operators and postfix operators.
                     \param a_szCharset A string containing all characters that can be used
                     in operator identifiers.
        */
        public void DefineOprtChars(string a_szCharset) { m_sOprtChars = a_szCharset; }

        //---------------------------------------------------------------------------
        /*  Define the set of valid characters to be used in names of
                     infix operators.
                     \param a_szCharset A string containing all characters that can be used
                     in infix operator identifiers.
        */
        public void DefineInfixOprtChars(string a_szCharset) { m_sInfixOprtChars = a_szCharset; }

        //---------------------------------------------------------------------------
        /*  Virtual function that defines the characters allowed in name identifiers.
              \sa #ValidOprtChars, #ValidPrefixOprtChars
        */
        public string ValidNameChars()
        {
            Global.MUP_VERIFY(() => m_sNameChars.Length > 0);
            return m_sNameChars;
        }

        //---------------------------------------------------------------------------
        /*  Virtual function that defines the characters allowed in operator definitions.
              \sa #ValidNameChars, #ValidPrefixOprtChars
        */
        public string ValidOprtChars()
        {
            Global.MUP_VERIFY(() => m_sOprtChars.Length > 0);
            return m_sOprtChars;
        }

        //---------------------------------------------------------------------------
        /*  Virtual function that defines the characters allowed in infix operator definitions.
              \sa #ValidNameChars, #ValidOprtChars
        */
        public string ValidInfixOprtChars()
        {
            Global.MUP_VERIFY(() => m_sInfixOprtChars.Length > 0);
            return m_sInfixOprtChars;
        }

        //---------------------------------------------------------------------------
        /*  Initialize the token reader.
              \post m_pTokenReader.Get()!=0
             
              Create new token reader object and submit pointers to function, operator,
              constant and variable definitions.
        */
        private void InitTokenReader()
        {
            m_pTokenReader = new TokenReader(this);
        }

        //---------------------------------------------------------------------------
        /*  Reset parser to string parsing mode and clear internal buffers.
             
              Resets the token reader.
        */
        private void ReInit()
        {
            m_pParserEngine = ParseFromString;
            m_pTokenReader.ReInit();
            m_rpn.Reset();
            if (m_vStackBuffer != null)
                Array.Clear(m_vStackBuffer, 0, m_vStackBuffer.Length);
            m_nPos = 0;
        }

        //---------------------------------------------------------------------------
        /*  Adds a new package to the parser.

            The parser becomes the owner of the package pointer and is responsible for
            its deletion.
        */
        public void AddPackage(IPackage p)
        {
            p.AddToParser(this);
        }

        //---------------------------------------------------------------------------
        /*  Add a value reader object to muParserX.
              \param a_pReader Pointer to the value reader object.
        */
        public void AddValueReader(IValueReader a_pReader)
        {
            m_pTokenReader.AddValueReader(a_pReader);
        }

        //---------------------------------------------------------------------------
        /*  Check if a given name contains invalid characters.
              \param a_strName The name to check
              \param a_szCharSet The characterset
              \throw ParserException if the name contains invalid charakters.
        */
        public void CheckName(string a_strName, string a_szCharSet)
        {
            if (a_strName.Length == 0 ||
                (a_strName.FindFirstNotOf(a_szCharSet) != -1) ||
                (a_strName[0] >= '0' && a_strName[0] <= '9'))
            {
                Error(EErrorCodes.ecINVALID_NAME);
            }
        }

        //---------------------------------------------------------------------------
        /*  Set the mathematical expression.
              \param a_sExpr String with the expression
              \throw ParserException in case of syntax errors.

              Triggers first time calculation thus the creation of the bytecode and
              scanning of used variables.
        */
        public void SetExpr(string a_sExpr)
        {
            m_pTokenReader.SetExpr(a_sExpr);
            ReInit();
        }

        //---------------------------------------------------------------------------
        /*  Add a user defined variable.
              \param a_sName The variable name
              \param a_Var The variable to be added to muParserX
        */
        public void DefineVar(string ident, Variable var)
        {
            CheckName(ident, ValidNameChars());
            CheckForEntityExistence(ident, EErrorCodes.ecVARIABLE_DEFINED);
            m_varDef[ident] = var.Clone();
        }

        // Used by by DefineVar and DefineConst methods
        // for better checking of var//oprt/fun existence.
        private void CheckForEntityExistence(string ident, EErrorCodes error_code)
        {
            if (IsVarDefined(ident) ||
                IsConstDefined(ident) ||
                IsFunDefined(ident) ||
                IsOprtDefined(ident) ||
                IsPostfixOprtDefined(ident) ||
                IsInfixOprtDefined(ident))
                throw new ParserError(new ErrorContext(error_code, 0, ident));
        }

        //---------------------------------------------------------------------------
        /*  Define a parser Constant.
                \param a_sName The name of the constant
                \param a_Val Const reference to the constants value

                Parser constants are handed over by const reference as opposed to variables
                which are handed over by reference. Consequently the parser can not change
                their value.
        */
        public void DefineConst(string ident, Value val)
        {
            CheckName(ident, ValidNameChars());
            CheckForEntityExistence(ident, EErrorCodes.ecCONSTANT_DEFINED);
            m_valDef[ident] = val.Clone();
        }

        //---------------------------------------------------------------------------
        /*  Add a callback object to the parser.
                \param a_pFunc Pointer to the intance of a parser callback object
                representing the function.
                \sa GetFunDef, functions

                The parser takes ownership over the callback object.
        */
        public void DefineFun(ICallback fun)
        {
            if (IsFunDefined(fun.GetIdent()))
                throw new ParserError(new ErrorContext(EErrorCodes.ecFUNOPRT_DEFINED, 0, fun.GetIdent()));
            fun.SetParent(this);
            m_FunDef[fun.GetIdent()] = fun;
        }

        //---------------------------------------------------------------------------
        /*  Define a binary operator.
                \param a_pCallback Pointer to the callback object
        */
        public void DefineOprt(IOprtBin oprt)
        {
            if (IsOprtDefined(oprt.GetIdent()))
                throw new ParserError(new ErrorContext(EErrorCodes.ecFUNOPRT_DEFINED, 0, oprt.GetIdent()));
            oprt.SetParent(this);
            m_OprtDef[oprt.GetIdent()] = oprt;

        }

        //---------------------------------------------------------------------------
        /*  Add a user defined operator.
              \post Will reset the Parser to string parsing mode.
              \param a_Poprt Pointer to a unary postfix operator object. The parser will
              become the new owner of this object hence will destroy it.
        */
        public void DefinePostfixOprt(IOprtPostfix oprt)
        {
            if (IsPostfixOprtDefined(oprt.GetIdent()))
                throw new ParserError(new ErrorContext(EErrorCodes.ecFUNOPRT_DEFINED, 0, oprt.GetIdent()));

            // Operator is not added yet, add it.
            oprt.SetParent(this);
            m_PostOprtDef[oprt.GetIdent()] = oprt;
        }

        //---------------------------------------------------------------------------
        /*  Add a user defined operator.
            \param a_Poprt Pointer to a unary postfix operator object. The parser will
                   become the new owner of this object hence will destroy it.
        */
        public void DefineInfixOprt(IOprtInfix oprt)
        {
            if (IsInfixOprtDefined(oprt.GetIdent()))
                throw new ParserError(new ErrorContext(EErrorCodes.ecFUNOPRT_DEFINED, 0, oprt.GetIdent()));

            // Function is not added yet, add it.
            oprt.SetParent(this);
            m_InfixOprtDef[oprt.GetIdent()] = oprt;
        }

        //---------------------------------------------------------------------------
        public void RemoveVar(string ident)
        {
            m_varDef.Remove(ident);
            ReInit();
        }

        //---------------------------------------------------------------------------
        public void RemoveConst(string ident)
        {
            m_valDef.Remove(ident);
            ReInit();
        }

        //---------------------------------------------------------------------------
        public void RemoveFun(string ident)
        {
            m_FunDef.Remove(ident);
            ReInit();
        }

        //---------------------------------------------------------------------------
        public void RemoveOprt(string ident)
        {
            m_OprtDef.Remove(ident);
            ReInit();
        }

        //---------------------------------------------------------------------------
        public void RemovePostfixOprt(string ident)
        {
            m_PostOprtDef.Remove(ident);
            ReInit();
        }

        //---------------------------------------------------------------------------
        public void RemoveInfixOprt(string ident)
        {
            m_InfixOprtDef.Remove(ident);
            ReInit();
        }

        //---------------------------------------------------------------------------
        public bool IsVarDefined(string ident)
        {
            return m_varDef.ContainsKey(ident);
        }

        //---------------------------------------------------------------------------
        public bool IsConstDefined(string ident)
        {
            return m_valDef.ContainsKey(ident);
        }

        //---------------------------------------------------------------------------
        public bool IsFunDefined(string ident)
        {
            return m_FunDef.ContainsKey(ident);
        }

        //---------------------------------------------------------------------------
        public bool IsOprtDefined(string ident)
        {
            return m_OprtDef.ContainsKey(ident);
        }

        //---------------------------------------------------------------------------
        public bool IsPostfixOprtDefined(string ident)
        {
            return m_PostOprtDef.ContainsKey(ident);
        }

        //---------------------------------------------------------------------------
        public bool IsInfixOprtDefined(string ident)
        {
            return m_InfixOprtDef.ContainsKey(ident);
        }

        //---------------------------------------------------------------------------
        /*  Return a map containing the used variables only.
        */
        public TokenMap GetExprVar()
        {
            bool tmpBuf = m_bIsQueryingExprVar;
            try
            {
                m_bIsQueryingExprVar = true;
                CreateRPN();
                return m_pTokenReader.GetUsedVar();
            }
            finally
            {
                m_bIsQueryingExprVar = tmpBuf;
            }
        }

        //---------------------------------------------------------------------------
        /*  Return a map containing the used variables only.
        */
        public TokenMap GetVar() { return m_varDef; }

        //---------------------------------------------------------------------------
        /*  Return a map containing all parser constants.
        */
        public TokenMap GetConst() { return m_valDef; }

        //---------------------------------------------------------------------------
        /*  Return prototypes of all parser functions.
              \return #m_FunDef
              \sa FunProt, functions
             
              The return type is a map of the public type #funmap_type containing the prototype
              definitions for all numerical parser functions. String functions are not part of
              this map. The Prototype definition is encapsulated in objects of the class FunProt
              one per parser function each associated with function names via a map construct.
        */
        public TokenMap GetFunDef() { return m_FunDef; }

        //---------------------------------------------------------------------------
        /*  Retrieve the mathematical expression.
        */
        public string GetExpr() { return m_pTokenReader.GetExpr(); }

        //---------------------------------------------------------------------------
        /*  Get the version number of muParserX.
              \return A string containing the version number of muParserX.
        */
        public static string GetVersion()
        {
            return Global.Version;
        }

        //---------------------------------------------------------------------------
        private void ApplyRemainingOprt(Stack<IToken> stOpt)
        {
            while (stOpt.Any() &&
                   stOpt.Peek().GetCode() != ECmdCode.cmBO &&
                   stOpt.Peek().GetCode() != ECmdCode.cmIO &&
                   stOpt.Peek().GetCode() != ECmdCode.cmCBO &&
                   stOpt.Peek().GetCode() != ECmdCode.cmIF)
            {
                IToken op = stOpt.Peek();
                switch (op.GetCode())
                {
                    case ECmdCode.cmOPRT_INFIX:
                    case ECmdCode.cmOPRT_BIN: ApplyFunc(stOpt, 2); break;
                    case ECmdCode.cmELSE: ApplyIfElse(stOpt); break;
                    default: Error(EErrorCodes.ecINTERNAL_ERROR); break;
                } // switch operator token type
            } // While operator stack not empty
        }

        //---------------------------------------------------------------------------
        /*  Simulates the call of a parser function with its corresponding arguments.
              \param a_stOpt The operator stack
              \param a_stVal The value stack
              \param a_iArgCount The number of function arguments
        */
        private void ApplyFunc(Stack<IToken> a_stOpt, int a_iArgCount)
        {
            if (!a_stOpt.Any())
                return;

            IToken tok = a_stOpt.Pop();
            ICallback pFun = tok.AsICallback();

            int iArgCount = (pFun.GetArgc() >= 0) ? pFun.GetArgc() : a_iArgCount;
            if (pFun.GetArgc() == -1)
                tok = pFun.SetNumArgsPresent(iArgCount);

            m_nPos -= (iArgCount - 1);
            m_rpn.Add(tok);
        }

        //---------------------------------------------------------------------------
        /*  Simulates the effect of the execution of an if-then-else block.
        */
        private void ApplyIfElse(Stack<IToken> a_stOpt)
        {
            while (a_stOpt.Any() && a_stOpt.Peek().GetCode() == ECmdCode.cmELSE)
            {
                Global.MUP_VERIFY(() => a_stOpt.Any());
                Global.MUP_VERIFY(() => m_nPos >= 3);
                Global.MUP_VERIFY(() => a_stOpt.Peek().GetCode() == ECmdCode.cmELSE);

                IToken opElse = a_stOpt.Pop();
                IToken opIf = a_stOpt.Pop();
                Global.MUP_VERIFY(() => opElse.GetCode() == ECmdCode.cmELSE);

                Global.MUP_VERIFY(() => opIf.GetCode() == ECmdCode.cmIF);

                // If then else hat 3 argumente und erzeugt einen rückgabewert (3-1=2)
                m_nPos -= 2;
                m_rpn.Add(new TokenIfThenElse(ECmdCode.cmENDIF));
            }
        }

        //---------------------------------------------------------------------------
        public void DumpRPN() { m_rpn.AsciiDump(); }

        //---------------------------------------------------------------------------
        private void CreateRPN()
        {
            if (string.IsNullOrEmpty(m_pTokenReader.GetExpr()))
                Error(EErrorCodes.ecUNEXPECTED_EOF, 0);

            // The Stacks take the ownership over the tokens
            Stack<IToken> stOpt = new Stack<IToken>();
            Stack<int> stArgCount = new Stack<int>();
            Stack<int> stIdxCount = new Stack<int>();
            IToken pTok = null, pTokPrev;
            Value val;

            ReInit();

            for (; ; )
            {
                pTokPrev = pTok;
                pTok = m_pTokenReader.ReadNextToken();

                ECmdCode eCmd = pTok.GetCode();
                switch (eCmd)
                {
                    case ECmdCode.cmVAL:
                        m_nPos++;
                        m_rpn.Add(pTok);
                        break;

                    case ECmdCode.cmCBC:
                    case ECmdCode.cmIC:
                        {
                            ECmdCode eStarter = eCmd - 1;
                            Global.MUP_VERIFY(() => eStarter == ECmdCode.cmCBO || eStarter == ECmdCode.cmIO);

                            // The argument count for parameterless functions is zero
                            // by default an opening bracket sets parameter count to 1
                            // in preparation of arguments to come. If the last token
                            // was an opening bracket we know better...
                            if (pTokPrev != null && pTokPrev.GetCode() == eStarter)
                                stArgCount.Push(stArgCount.Pop() - 1);

                            ApplyRemainingOprt(stOpt);

                            // if opt is "]" and opta is "[" the bracket content has been evaluated.
                            // Now check whether there is an index operator on the stack.
                            if (stOpt.Any() && stOpt.Peek().GetCode() == eStarter)
                            {
                                //
                                // Find out how many dimensions were used in the index operator.
                                //
                                int iArgc = stArgCount.Pop();
                                stOpt.Pop(); // Take opening bracket from stack

                                ICallback PoprtIndex = pTok.AsICallback();
                                Global.MUP_VERIFY(() => PoprtIndex != null);

                                pTok = PoprtIndex.SetNumArgsPresent(iArgc);
                                m_rpn.Add(pTok);

                                // If this is an index operator there must be something else in the register (the variable to index)
                                Global.MUP_VERIFY(() => eCmd != ECmdCode.cmIC || m_nPos >= iArgc + 1);

                                // Reduce the index into the value registers accordingly
                                m_nPos -= iArgc;

                                if (eCmd == ECmdCode.cmCBC)
                                {
                                    ++m_nPos;
                                }
                            } // if opening index bracket is on Peek of operator stack
                        }
                        break;

                    case ECmdCode.cmBC:
                        {
                            // The argument count for parameterless functions is zero
                            // by default an opening bracket sets parameter count to 1
                            // in preparation of arguments to come. If the last token
                            // was an opening bracket we know better...
                            if (pTokPrev != null && pTokPrev.GetCode() == ECmdCode.cmBO)
                                stArgCount.Push(stArgCount.Pop() - 1);

                            ApplyRemainingOprt(stOpt);

                            // if opt is ")" and opta is "(" the bracket content has been evaluated.
                            // Now its time to check if there is either a function or a sign pending.
                            // - Neither the opening nor the closing bracket will be pushed back to
                            //   the operator stack
                            // - Check if a function is standing in front of the opening bracket,
                            //   if so evaluate it afterwards to apply an infix operator.
                            if (stOpt.Any() && stOpt.Peek().GetCode() == ECmdCode.cmBO)
                            {
                                //
                                // Here is the stuff to evaluate a function token
                                //
                                int iArgc = stArgCount.Pop();

                                stOpt.Pop(); // Take opening bracket from stack
                                if (!stOpt.Any())
                                    break;

                                if ((stOpt.Peek().GetCode() != ECmdCode.cmFUNC) && (stOpt.Peek().GetCode() != ECmdCode.cmOPRT_INFIX))
                                    break;

                                ICallback pFun = stOpt.Peek().AsICallback();

                                if (pFun.GetArgc() != -1 && iArgc > pFun.GetArgc())
                                    Error(EErrorCodes.ecTOO_MANY_PARAMS, pTok.GetExprPos(), pFun);

                                if (iArgc < pFun.GetArgc())
                                    Error(EErrorCodes.ecTOO_FEW_PARAMS, pTok.GetExprPos(), pFun);

                                // Apply function, if present
                                if (stOpt.Any() &&
                                    stOpt.Peek().GetCode() != ECmdCode.cmOPRT_INFIX &&
                                    stOpt.Peek().GetCode() != ECmdCode.cmOPRT_BIN)
                                {
                                    ApplyFunc(stOpt, iArgc);
                                }
                            }
                        }
                        break;

                    case ECmdCode.cmELSE:
                        ApplyRemainingOprt(stOpt);
                        m_rpn.Add(pTok);
                        stOpt.Push(pTok);
                        break;

                    case ECmdCode.cmSCRIPT_NEWLINE:
                        ApplyRemainingOprt(stOpt);
                        m_rpn.AddNewline(pTok, m_nPos);
                        stOpt.Clear();
                        m_nPos = 0;
                        break;

                    case ECmdCode.cmARG_SEP:
                        if (!stArgCount.Any())
                            Error(EErrorCodes.ecUNEXPECTED_COMMA, m_pTokenReader.GetPos() - 1);

                        stArgCount.Push(stArgCount.Pop() + 1);

                        ApplyRemainingOprt(stOpt);
                        break;

                    case ECmdCode.cmEOE:
                        ApplyRemainingOprt(stOpt);
                        m_rpn.Finalise();
                        break;

                    case ECmdCode.cmIF:
                    case ECmdCode.cmOPRT_BIN:
                        {
                            while (stOpt.Any() &&
                                stOpt.Peek().GetCode() != ECmdCode.cmBO &&
                                stOpt.Peek().GetCode() != ECmdCode.cmIO &&
                                stOpt.Peek().GetCode() != ECmdCode.cmCBO &&
                                stOpt.Peek().GetCode() != ECmdCode.cmELSE &&
                                stOpt.Peek().GetCode() != ECmdCode.cmIF)
                            {
                                IToken Poprt1 = stOpt.Peek();
                                IToken Poprt2 = pTok;
                                Global.MUP_VERIFY(() => Poprt1 != null && Poprt2 != null);
                                Global.MUP_VERIFY(() => Poprt1.AsIPrecedence() != null && Poprt2.AsIPrecedence() != null);

                                int nPrec1 = Poprt1.AsIPrecedence().GetPri(),
                                    nPrec2 = Poprt2.AsIPrecedence().GetPri();

                                if (Poprt1.GetCode() == Poprt2.GetCode())
                                {
                                    // Deal with operator associativity
                                    EOprtAsct eOprtAsct = Poprt1.AsIPrecedence().GetAssociativity();
                                    if ((eOprtAsct == EOprtAsct.oaRIGHT && (nPrec1 <= nPrec2)) ||
                                        (eOprtAsct == EOprtAsct.oaLEFT && (nPrec1 < nPrec2)))
                                    {
                                        break;
                                    }
                                }
                                else if (nPrec1 < nPrec2)
                                {
                                    break;
                                }

                                // apply the operator now
                                // (binary operators are identic to functions with two arguments)
                                ApplyFunc(stOpt, 2);
                            } // while ( ... )

                            if (pTok.GetCode() == ECmdCode.cmIF)
                                m_rpn.Add(pTok);

                            stOpt.Push(pTok);
                        }
                        break;

                    //
                    //  Postfix Operators
                    //
                    case ECmdCode.cmOPRT_POSTFIX:
                        Global.MUP_VERIFY(() => m_nPos != 0);
                        m_rpn.Add(pTok);
                        break;

                    case ECmdCode.cmCBO:
                    case ECmdCode.cmIO:
                    case ECmdCode.cmBO:
                        stOpt.Push(pTok);
                        stArgCount.Push(1);
                        break;

                    //
                    // Functions
                    //
                    case ECmdCode.cmOPRT_INFIX:
                    case ECmdCode.cmFUNC:
                        {
                            ICallback pFunc = pTok.AsICallback();
                            Global.MUP_VERIFY(() => pFunc != null);
                            stOpt.Push(pTok);
                        }
                        break;

                    default:
                        Error(EErrorCodes.ecINTERNAL_ERROR);
                        break;
                } // switch Code

                if (s_bDumpStack)
                {
                    StackDump(stOpt);
                }

                if (pTok.GetCode() == ECmdCode.cmEOE)
                    break;
            } // for (all tokens)

            if (s_bDumpRPN)
            {
                m_rpn.AsciiDump();
            }

            if (m_nPos > 1)
            {
                Error(EErrorCodes.ecUNEXPECTED_COMMA, -1);
            }
        }

        //---------------------------------------------------------------------------
        /*  One of the two main parse functions.
              \sa ParseCmdCode(), ParseValue()

              Parse expression from input string. Perform syntax checking and create bytecode.
              After parsing the string and creating the bytecode the function pointer
              #m_pParseFormula will be changed to the second parse routine the uses bytecode instead of string parsing.
        */
        private IValue ParseFromString()
        {
            CreateRPN();

            // Umsachalten RPN
            m_vStackBuffer = new IValue[m_rpn.GetRequiredStackSize()];
            for (int i = 0; i < m_vStackBuffer.Length; ++i)
            {
                Value pValue = new Value();
                pValue.BindToCache(m_cache);
                m_vStackBuffer[i] = pValue;
            }

            m_pParserEngine = ParseFromRPN;

            return m_pParserEngine();
        }

        //---------------------------------------------------------------------------
        private IValue ParseFromRPN()
        {
            IValue[] pStack = m_vStackBuffer;
            if (m_rpn.GetSize() == 0)
            {
                // Passiert bei leeren strings oder solchen, die nur Leerzeichen enthalten
                ErrorContext err = new ErrorContext();
                err.Expr = m_pTokenReader.GetExpr();
                err.Errc = EErrorCodes.ecUNEXPECTED_EOF;
                err.Pos = 0;
                throw new ParserError(err);
            }

            IList<IToken> pRPN = m_rpn.GetData();

            int j = 0;
            int sidx = -1;
            int lenRPN = m_rpn.GetSize();
            for (int i = 0; i < lenRPN; ++i)
            {
                IToken pTok = pRPN[i];
                ECmdCode eCode = pTok.GetCode();

                switch (eCode)
                {
                    case ECmdCode.cmSCRIPT_NEWLINE:
                        sidx = -1;
                        continue;

                    case ECmdCode.cmVAL:
                        {
                            IValue pVal = (IValue)pTok;
                            sidx++;
                            Global.MUP_VERIFY(() => sidx < pStack.Length);
                            pStack[sidx] = pVal;
                            //  m_vStackBuffer[sidx] = rVal;
                            //if (pVal.IsVariable())
                            //{
                            //    // m_vStackBuffer[sidx].Release();
                            //    pStack[sidx] = pVal;
                            //}
                            //else
                            //{
                            //    ref IValue val = ref pStack[sidx];
                            //    if (val.IsVariable())
                            //        m_cache.CreateFromCache(out val);

                            //    val = pVal;
                            //}
                        }
                        continue;
                    case ECmdCode.cmIC:
                        {
                            j = 0;
                            ICallback pIdxOprt = (ICallback)pTok;
                            int nArgs = pIdxOprt.GetArgsPresent();
                            sidx -= nArgs - 1;
                            Global.MUP_VERIFY(() => sidx >= 0);
                            var arr = new IValue[nArgs + 1];
                            ref IValue val = ref pStack[--sidx];
                            Array.Copy(pStack, sidx, arr, 0, nArgs + 1);
                            //int t = sidx + nArgs +1;
                            //arr = new IValue[nArgs+1];
                            //if (t == 1)
                            //    arr[0] = m_vStackBuffer[sidx];
                            //else if (t != 0)
                            //for (int k = sidx; k < t; k++)
                            //    arr[j++] = m_vStackBuffer[k];
                            pIdxOprt.Eval(ref val, arr);
                        }
                        continue;

                    case ECmdCode.cmCBC:
                    case ECmdCode.cmOPRT_POSTFIX:
                    case ECmdCode.cmFUNC:
                    case ECmdCode.cmOPRT_BIN:
                    case ECmdCode.cmOPRT_INFIX:
                        {
                            ICallback pFun = (ICallback)pTok;
                            int nArgs = pFun.GetArgsPresent();
                            sidx -= nArgs - 1;
                            var arr = new IValue[nArgs];
                            Global.MUP_VERIFY(() => sidx >= 0);
                            // ref IValue val = ref pStack[sidx];
                            Array.Copy(pStack, sidx, arr, 0, nArgs);
                            try
                            {
                                pFun.Eval(ref pStack[sidx], arr);
                                //if (pStack[sidx].IsVariable())
                                //{
                                //    m_cache.CreateFromCache(out IValue buf);
                                //    pFun.Eval(ref buf, arr, nArgs);
                                //    pStack[sidx] = buf;
                                //}
                                //else
                                //{
                                //    pFun.Eval(ref pStack[sidx], arr, nArgs);
                                //}
                            }
                            catch (ParserError exc)
                            {
                                // <ibg 20130131> Not too happy about that:
                                // Multiarg functions may throw specific error codes when evaluating.
                                // These codes would be converted to ecEVAL here. I omit the conversion
                                // for certain handpicked errors. (The reason this catch block exists is
                                // that not all exceptions contain proper metadata when thrown out of
                                // a function.)
                                if (exc.GetCode() == EErrorCodes.ecTOO_FEW_PARAMS ||
                                    exc.GetCode() == EErrorCodes.ecDOMAIN_ERROR ||
                                    exc.GetCode() == EErrorCodes.ecOVERFLOW ||
                                    exc.GetCode() == EErrorCodes.ecINVALID_NUMBER_OF_PARAMETERS ||
                                    exc.GetCode() == EErrorCodes.ecASSIGNEMENT_TO_VALUE)
                                {
                                    exc.GetContext().Pos = pFun.GetExprPos();
                                    exc.GetContext().Ident = pFun.GetIdent();
                                    throw;
                                }
                                // </ibg>
                                else
                                {
                                    ErrorContext err = new ErrorContext
                                    {
                                        Expr = m_pTokenReader.GetExpr(),
                                        Ident = pFun.GetIdent(),
                                        Errc = EErrorCodes.ecEVAL,
                                        Pos = pFun.GetExprPos(),
                                        Hint = exc.GetMsg()
                                    };
                                    throw new ParserError(err, exc, exc._file, line: exc._line);
                                }
                            }
                            catch (MatrixError m)
                            {
                                ErrorContext err = new ErrorContext
                                {
                                    Expr = m_pTokenReader.GetExpr(),
                                    Ident = pFun.GetIdent(),
                                    Errc = EErrorCodes.ecMATRIX_DIMENSION_MISMATCH,
                                    Pos = pFun.GetExprPos()
                                };
                                throw new ParserError(err, m);
                            }
                        }
                        continue;

                    case ECmdCode.cmIF:
                        Global.MUP_VERIFY(() => sidx >= 0);
                        if (pStack[sidx--].GetBool() == false)
                            i += ((TokenIfThenElse)pTok).GetOffset();
                        continue;

                    case ECmdCode.cmELSE:
                    case ECmdCode.cmJMP:
                        i += ((TokenIfThenElse)pTok).GetOffset();
                        continue;

                    case ECmdCode.cmENDIF:
                        continue;

                    default:
                        Error(EErrorCodes.ecINTERNAL_ERROR);
                        break;
                } // switch token
            } // for all RPN tokens

            return pStack[0];
        }
        //---------------------------------------------------------------------------
        public void Error(EErrorCodes a_iErrc, int a_iPos = -1, IToken a_pTok = null,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0)
        {
            ErrorContext err = new ErrorContext();
            err.Errc = a_iErrc;
            err.Pos = a_iPos;
            err.Expr = m_pTokenReader.GetExpr();
            err.Ident = a_pTok?.GetIdent() ?? "";
            throw new ParserError(err,null, file, member, line);
        }

        //------------------------------------------------------------------------------
        /* Clear all user defined variables, ants or functions.
         *  Resets the parser to string parsing mode by calling #ReInit.
        */
        public void ClearVar()
        {
            m_varDef.Clear();
            m_valDynVarShadow.Clear();
            ReInit();
        }

        //------------------------------------------------------------------------------
        /*  Clear the expression.
             
              Clear the expression and existing bytecode.
        */
        private void ClearExpr()
        {
            m_pTokenReader.SetExpr("");
            ReInit();
        }

        //------------------------------------------------------------------------------
        /*  Clear all function definitions.
        */
        public void ClearFun()
        {
            m_FunDef.Clear();
            ReInit();
        }

        //------------------------------------------------------------------------------
        /*  Clear all user defined constants.
             
              Both numeric and string constants will be removed from the internal storage.
        */
        public void ClearConst()
        {
            m_valDef.Clear();
            ReInit();
        }

        //------------------------------------------------------------------------------
        /*  Clear all user defined postfix operators.
        */
        public void ClearPostfixOprt()
        {
            m_PostOprtDef.Clear();
            ReInit();
        }

        //------------------------------------------------------------------------------
        /*  Clear all user defined binary operators.
        */
        public void ClearOprt()
        {
            m_OprtDef.Clear();
            ReInit();
        }

        //------------------------------------------------------------------------------
        /*  Clear the user defined Prefix operators.
        */
        public void ClearInfixOprt()
        {
            m_InfixOprtDef.Clear();
            ReInit();
        }

        //------------------------------------------------------------------------------
        public void EnableAutoCreateVar(bool bStat) { m_bAutoCreateVar = bStat; }

        //------------------------------------------------------------------------------
        public void EnableOptimizer(bool bStat)
        {
            m_rpn.EnableOptimizer(bStat);
        }

        //---------------------------------------------------------------------------
        /*   Enable the dumping of bytecode amd stack content on the console.
              \param bDumpCmd Flag to enable dumping of the current bytecode to the console.
              \param bDumpStack Flag to enable dumping of the stack content is written to the console.

              This function is for debug purposes only!
        */
        public static void EnableDebugDump(bool bDumpStack, bool bDumpRPN)
        {
            ParserXBase.s_bDumpRPN = bDumpRPN;
            ParserXBase.s_bDumpStack = bDumpStack;
        }

        //------------------------------------------------------------------------------
        public bool IsAutoCreateVarEnabled()
        {
            return m_bAutoCreateVar;
        }

        //------------------------------------------------------------------------------
        /*   Dump stack content.
             This function is used for debugging only.
        */
        private void StackDump(Stack<IToken> a_stOprt)
        {
            Stack<IToken> stOprt = new Stack<IToken>(a_stOprt);

            var sInfo = "StackDump>  ";
            Console.Write(sInfo);

            if (!stOprt.Any())
                Console.WriteLine($"\n{sInfo}Operator stack is empty.");
            else
                Console.WriteLine($"\n{sInfo}Operator Stack:");

            while (stOprt.Any())
            {
                IToken tok = stOprt.Pop();
                Console.WriteLine($" {Global.g_sCmdCode[(int)tok.GetCode()]} \"{tok.GetIdent()}\"");
            }

            Console.WriteLine();
        }
    }
}
