
using System;

namespace MuParserSharp
{
    //------------------------------------------------------------------------------
    /*  Bytecode values.
          \attention The order of the operator entries must match the order in
             ParserXBase::c_DefaultOprt!
        */
    public enum ECmdCode
    {
        // The following are codes for built in binary operators
        // apart from built in operators the user has the opportunity to
        // add user defined operators.
        cmBO = 0,  // Operator item:  opening bracket
        cmBC = 1,  // Operator item:  closing bracket
        cmIO = 2,  // Operator item:  index operator opening
        cmIC = 3,  // Operator item:  index operator closing
        cmCBO = 4,  // Operator item:  curly bracket (opening)
        cmCBC = 5,  // Operator item:  curly bracket (closing)
        cmARG_SEP = 6,  // Operator item:  comma
        cmIF = 7,  // Ternary if then else operator
        cmELSE = 8,  // Ternary if then else operator
        cmENDIF = 9,  // Ternary if then else operator
        cmJMP = 10,  // Reserved for future use
        cmVAL = 11,  // value item
        cmFUNC = 12,  // Code for a function item
        cmOPRT_BIN = 13,  // Binary operator
        cmOPRT_INFIX = 14,  // Infix operator
        cmOPRT_POSTFIX = 15,  // Postfix operator
        cmEOE = 16,  // End of expression

        // The following codes are reserved in case i will ever turn this
        // into a scripting language
        cmSCRIPT_NEWLINE = 17,  // Newline
        cmSCRIPT_COMMENT = 18,
        cmSCRIPT_WHILE = 19,  // Reserved for future use
        cmSCRIPT_GOTO = 20,  // Reserved for future use
        cmSCRIPT_LABEL = 21,  // Reserved for future use
        cmSCRIPT_FOR = 22,  // Reserved for future use
        cmSCRIPT_IF = 23,  // Reserved for future use
        cmSCRIPT_ELSE = 24,  // Reserved for future use
        cmSCRIPT_ELSEIF = 25,  // Reserved for future use
        cmSCRIPT_ENDIF = 26,  // Reserved for future use
        cmSCRIPT_FUNCTION = 27,  // Reserved for future use

        // misc codes
        cmUNKNOWN = 28,  // uninitialized item
        cmCOUNT                    // Dummy entry for counting the enum values
    }; // ECmdCode


    [Flags]
    //------------------------------------------------------------------------------
    public enum EPackages
    {
        pckCOMMON = 1 << 0,
        pckUNIT = 1 << 1,
        pckCOMPLEX = 1 << 2,
        pckNON_COMPLEX = 1 << 3,
        pckSTRING = 1 << 4,
        pckMATRIX = 1 << 5,
        pckALL_COMPLEX = pckCOMMON | pckCOMPLEX | pckSTRING | pckUNIT | pckMATRIX,
        pckALL_NON_COMPLEX = pckCOMMON | pckNON_COMPLEX | pckSTRING | pckUNIT | pckMATRIX
    };

    //------------------------------------------------------------------------------
    /*  Syntax codes.

        The syntax codes control the syntax check done during the first time parsing of
        the expression string. They are flags that indicate which tokens are allowed next
        if certain tokens are identified.
        */
    [Flags]
    public enum ESynCodes
    {
        noBO = 1 << 0,  // to avoid i.e. "cos(7)("
        noBC = 1 << 1,  // to avoid i.e. "sin)" or "()"
        noIO = 1 << 2,  // No opening bracket "["
        noIC = 1 << 3,  // No closing bracket "]"
        noCBO = 1 << 4,  // No opening curly bracket
        noCBC = 1 << 5,  // No opening closing bracket
        noVAL = 1 << 6,  // to avoid i.e. "tan 2" or "sin(8)3.14"
        noVAR = 1 << 7,  // to avoid i.e. "sin a" or "sin(8)a"
        noCOMMA = 1 << 8,  // to avoid i.e. ",," or "+," ...
        noFUN = 1 << 9,  // to avoid i.e. "sqrt cos" or "(1)sin"
        noOPT = 1 << 10,  // to avoid i.e. "(+)"
        noPFX = 1 << 11,  // to avoid i.e. "(5!!)" "sin!"
        noIFX = 1 << 12,  // to avoid i.e. "++4" "!!4"
        noEND = 1 << 13,  // to avoid unexpected end of expression
        noIF = 1 << 14,
        noELSE = 1 << 15,
        noNEWLINE = 1 << 16,  // to avoid i.e. "a+\nb" or "sin(\na)"

        sfSTART_OF_LINE = noOPT | noBC | noPFX | noCOMMA | noIO | noIC | noIF | noELSE,
        sfALLOW_NONE = ~0  // All of he above flags set
    };

    //------------------------------------------------------------------------------
    /*  Binary operator associativity values.
        */
    public enum EOprtAsct
    {
        oaNONE = 0,
        oaLEFT = 1,
        oaRIGHT = 2
    };

    //------------------------------------------------------------------------------
    /*  Parser operator precedence values.
      
        These are predefined values for the operator precedence.
        */
    public enum EOprtPrecedence
    {
        // assignment operators
        prASSIGN = -1,

        // if-then-else
        prIF_THEN_ELSE = 0,

        // binary operators
        prLOGIC_OR = 1,
        //  prLOGIC_XOR    =  2,
        prLOGIC_AND = 3,
        prBIT_OR = 4,
        prBIT_XOR = 5,
        prBIT_AND = 6,

        prRELATIONAL1 = 7, // For "==", "!="
        prRELATIONAL2 = 8, // Relational operators "<", "<=", ">", ">="
        prSHIFT = 9, // Shift operators "<<", ">>"

        prCOLON = 10, // Colon operator

        prADD_SUB = 11, // addition
        prMUL_DIV = 12, // multiplication/division
        prPOW = 13, // power operator priority (highest)

        // infix operators
        prINFIX = 12, // Signs have a higher priority than ADD_SUB, but lower than power operator
        prPOSTFIX = 12  // Postfix operator priority (currently unused)
    };

    /*  Error codes.

        This is the complete list of all error codes used by muparserx
        */
    public enum EErrorCodes
    {
        // Expression syntax errors
        ecUNEXPECTED_OPERATOR = 0, // Unexpected binary operator found
        ecUNASSIGNABLE_TOKEN = 1, // Token cant be identified.
        ecUNEXPECTED_EOF = 2, // Unexpected end of expression. (Example: "2+sin(")
        ecUNEXPECTED_COMMA = 3, // An unexpected comma has been found. (Example: "1,23")
        ecUNEXPECTED_VAL = 4, // An unexpected value token has been found
        ecUNEXPECTED_VAR = 5, // An unexpected variable token has been found
        ecUNEXPECTED_PARENS = 6, // Unexpected Parenthesis, opening or closing
        ecUNEXPECTED_STR = 7, // A string has been found at an inapropriate position
        ecUNEXPECTED_CONDITIONAL = 8,
        ecUNEXPECTED_NEWLINE = 9,
        ecSTRING_EXPECTED = 10, // A string function has been called with a different type of argument
        ecVAL_EXPECTED = 11, // A numerical function has been called with a non value type of argument
        ecMISSING_PARENS = 12, // Missing parens. (Example: "3*sin(3")
        ecMISSING_ELSE_CLAUSE = 13,
        ecMISPLACED_COLON = 14,
        ecUNEXPECTED_FUN = 15, // Unexpected function found. (Example: "sin(8)cos(9)")
        ecUNTERMINATED_STRING = 16, // unterminated string constant. (Example: "3*valueof("hello)")
        ecTOO_MANY_PARAMS = 17, // Too many function parameters
        ecTOO_FEW_PARAMS = 18, // Too few function parameters. (Example: "ite(1<2,2)")
        ecTYPE_CONFLICT = 19, // Generic type conflict
        ecTYPE_CONFLICT_FUN = 20, // Function argument type conflict.
        ecTYPE_CONFLICT_IDX = 21, // Function argument type conflict.
        ecINVALID_TYPE = 22,
        ecINVALID_TYPECAST = 23, // Invalid Value token cast.
        ecARRAY_SIZE_MISMATCH = 24, // Array size mismatch during a vector operation
        ecNOT_AN_ARRAY = 25, // Using the index operator on a scalar variable
        ecUNEXPECTED_SQR_BRACKET = 26, // Invalid use of the index operator
        ecUNEXPECTED_CURLY_BRACKET = 27, // Invalid use of the index operator

        ecINVALID_NAME = 28, // Invalid function, variable or constant name.
        ecBUILTIN_OVERLOAD = 29, // Trying to overload builtin operator
        ecINVALID_FUN_PTR = 30, // Invalid callback function pointer
        ecINVALID_VAR_PTR = 31, // Invalid variable pointer
        ecINVALID_PARAMETER = 32, // Invalid function parameter
        ecINVALID_NUMBER_OF_PARAMETERS = 33,

        ecNAME_CONFLICT = 34, // Name conflict
        ecOPT_PRI = 35, // Invalid operator priority
        ecASSIGNEMENT_TO_VALUE = 36, // Assignment to operator (3=4 instead of a=4)

        //
        ecDOMAIN_ERROR = 37, // Trying to use func/oprtr with out-of-domain input args
        ecDIV_BY_ZERO = 38, // Division by zero (currently unused)
        ecGENERIC = 39, // Generic error

        ecINDEX_OUT_OF_BOUNDS = 40, // Array index is out of bounds
        ecINDEX_DIMENSION = 41,
        ecMISSING_SQR_BRACKET = 42, // The index operator was not closed properly (i.e. "v[3")
        ecMISSING_CURLY_BRACKET = 43,
        ecEVAL = 44, // Error while evaluating function / operator
        ecOVERFLOW = 45, // Overflow (possibly) occurred

        // Matrix errors
        ecMATRIX_DIMENSION_MISMATCH = 46,

        // string related errors
        ecUNKNOWN_ESCAPE_SEQUENCE = 47,

        // already-defined item errors
        ecVARIABLE_DEFINED = 48, // Variable is already defined
        ecCONSTANT_DEFINED = 49, // Constant is already defined
        ecFUNOPRT_DEFINED = 50, // Function/operator is already defined

        // internal errors
        ecINTERNAL_ERROR = 51, // Internal error of any kind.

        // The last two are special entries
        ecUNDEFINED = -1  // Undefined message, placeholder to detect unassigned error messages
        ,
        ecCONVERSION_OVERFLOW
    };

}
