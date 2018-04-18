using System;
using System.Collections.Generic;
using System.Linq;

namespace MuParserSharp.Parser
{
    public abstract class ParserMessageProviderBase
    {
        protected ParserMessageProviderBase() { }
        public void Init()
        {
            InitErrorMessages();
            var q = (EErrorCodes[])Enum.GetValues(typeof(EErrorCodes));

            if (!q.All(ec => m_vErrMsg.ContainsKey(ec)))
                throw new ParserError($"Incomplete translation Missing messages for EErrorCodes: ( {string.Join(", ", q.Where(ec2 => !m_vErrMsg.ContainsKey(ec2)).Select(ec => ec.ToString()))} )");

        }

        public string GetErrorMsg(EErrorCodes eError)
        {
            if (m_vErrMsg == null)
                Init();

            return m_vErrMsg.ContainsKey(eError) ? m_vErrMsg[eError] : String.Empty;
        }

        protected abstract void InitErrorMessages();
        protected Dictionary<EErrorCodes, string> m_vErrMsg;
    }
    class ParserMessageProviderEnglish : ParserMessageProviderBase
    {
        public static ParserMessageProviderBase Instance { get; } = new ParserMessageProviderEnglish();
        private ParserMessageProviderEnglish() {}
        protected override void InitErrorMessages()
        {
            m_vErrMsg = new Dictionary<EErrorCodes, string>
            {
                [EErrorCodes.ecUNASSIGNABLE_TOKEN] = "Undefined token \"$IDENT$\" found at position $POS$.",
                [EErrorCodes.ecINTERNAL_ERROR] = "Internal error.",
                [EErrorCodes.ecUNKNOWN_ESCAPE_SEQUENCE] = "Unknown escape sequence.",
                [EErrorCodes.ecINVALID_NAME] = "Invalid function, variable or constant name.",
                [EErrorCodes.ecINVALID_FUN_PTR] = "Invalid pointer to callback function.",
                [EErrorCodes.ecINVALID_VAR_PTR] = "Invalid pointer to variable.",
                [EErrorCodes.ecUNEXPECTED_OPERATOR] ="Unexpected operator \"$IDENT$\" found at position $POS$.",
                [EErrorCodes.ecUNEXPECTED_EOF] = "Unexpected end of expression found at position $POS$.",
                [EErrorCodes.ecUNEXPECTED_COMMA] = "Unexpected comma found at position $POS$.",
                [EErrorCodes.ecUNEXPECTED_PARENS] = "Unexpected parenthesis \"$IDENT$\" found at position $POS$.",
                [EErrorCodes.ecUNEXPECTED_FUN] = "Unexpected function \"$IDENT$\" found at position $POS$.",
                [EErrorCodes.ecUNEXPECTED_VAL] = "Unexpected value \"$IDENT$\" found at position $POS$.",
                [EErrorCodes.ecUNEXPECTED_VAR] = "Unexpected variable \"$IDENT$\" found at position $POS$.",
                [EErrorCodes.ecUNEXPECTED_STR] = "Unexpected string token found at position $POS$.",
                [EErrorCodes.ecUNEXPECTED_CONDITIONAL] = "The \"$IDENT$\" operator must be preceded by a closing bracket.",
                [EErrorCodes.ecUNEXPECTED_NEWLINE] = "Unexprected newline.",
                [EErrorCodes.ecMISSING_PARENS] = "Missing parenthesis.",
                [EErrorCodes.ecMISSING_ELSE_CLAUSE] = "If-then-else operator is missing an else clause.",
                [EErrorCodes.ecMISPLACED_COLON] = "Misplaced colon at position $POS$.",
                [EErrorCodes.ecTOO_MANY_PARAMS] = "Too many parameters passed to function \"$IDENT$\".",
                [EErrorCodes.ecTOO_FEW_PARAMS] = "Too few parameters passed to function \"$IDENT$\".",
                [EErrorCodes.ecDIV_BY_ZERO] = "Division by zero occurred.",
                [EErrorCodes.ecDOMAIN_ERROR] = "The value passed as argument to function/operator \"$IDENT$\" is not part of its domain.",
                [EErrorCodes.ecNAME_CONFLICT] = "Name conflict.",
                [EErrorCodes.ecOPT_PRI] = "Invalid value for operator priority (must be greater or equal to zero).",
                [EErrorCodes.ecBUILTIN_OVERLOAD] = "Binary operator identifier conflicts with a built in operator.",
                [EErrorCodes.ecUNTERMINATED_STRING] = "Unterminated string starting at position $POS$.",
                [EErrorCodes.ecSTRING_EXPECTED] = "String function called with a non string type of argument.",
                [EErrorCodes.ecVAL_EXPECTED] = "Numerical function called with a non value type of argument.",
                [EErrorCodes.ecTYPE_CONFLICT] = "Value \"$IDENT$\" is of type '$TYPE1$'. There is no implicit conversion to type '$TYPE2$'.",
                [EErrorCodes.ecTYPE_CONFLICT_FUN] = "Argument $ARG$ of function/operator \"$IDENT$\" is of type '$TYPE1$' whereas type '$TYPE2$' was expected.",
                [EErrorCodes.ecTYPE_CONFLICT_IDX] = "Index to \"$IDENT$\" must be a positive integer value. '$TYPE1$' is not an acceptable type.",
                [EErrorCodes.ecGENERIC] = "Parser error.",
                [EErrorCodes.ecINVALID_TYPE] = "Invalid argument type.",
                [EErrorCodes.ecINVALID_TYPECAST] = "Value type conversion from type '$TYPE1$' to '$TYPE2$' is not supported!",
                [EErrorCodes.ecARRAY_SIZE_MISMATCH] = "Array size mismatch.",
                [EErrorCodes.ecNOT_AN_ARRAY] = "Using the index operator on the scalar variable \"$IDENT$\" is not allowed.",
                [EErrorCodes.ecUNEXPECTED_SQR_BRACKET] = "Unexpected \"[]\".",
                [EErrorCodes.ecUNEXPECTED_CURLY_BRACKET] = "Unexpected \"{}\".",
                [EErrorCodes.ecINDEX_OUT_OF_BOUNDS] = "Index to variable \"$IDENT$\" is out of bounds.",
                [EErrorCodes.ecINDEX_DIMENSION] = "Index operator dimension error.",
                [EErrorCodes.ecMISSING_SQR_BRACKET] = "Missing \"]\".",
                [EErrorCodes.ecMISSING_CURLY_BRACKET] = "Missing \"}\".",
                [EErrorCodes.ecASSIGNEMENT_TO_VALUE] = "Assignment operator \"$IDENT$\" can't be used in this context.",
                [EErrorCodes.ecEVAL] = "Can't evaluate function/operator \"$IDENT$\": $HINT$",
                [EErrorCodes.ecINVALID_PARAMETER] = "Parameter $ARG$ of function \"$IDENT$\" is invalid.",
                [EErrorCodes.ecINVALID_NUMBER_OF_PARAMETERS] = "Invalid number of function arguments.",
                [EErrorCodes.ecOVERFLOW] = "Possible arithmetic overflow occurred in function/operator \"$IDENT$\".",
                [EErrorCodes.ecCONVERSION_OVERFLOW] = "Possible value overflow occurred in numeric conversion: \"$IDENT$\".",
                [EErrorCodes.ecMATRIX_DIMENSION_MISMATCH] = "Matrix dimension error.",
                [EErrorCodes.ecVARIABLE_DEFINED] = "Variable \"$IDENT$\" is already defined.",
                [EErrorCodes.ecCONSTANT_DEFINED] = "Constant \"$IDENT$\" is already defined.",
                [EErrorCodes.ecFUNOPRT_DEFINED] = "Function/operator \"$IDENT$\" is already defined.",
                [EErrorCodes.ecUNDEFINED] = "Undefined behaviour"
            };

        }
    }
    class ParserMessageProviderGerman : ParserMessageProviderBase
    {
        public static ParserMessageProviderBase Instance { get; } = new ParserMessageProviderGerman();
        protected override void InitErrorMessages()
        {
            m_vErrMsg = new Dictionary<EErrorCodes, string>
            {
                [EErrorCodes.ecUNASSIGNABLE_TOKEN] = "Unbekanntes Token \"$IDENT$\" an Position $POS$ gefunden.",
                [EErrorCodes.ecINTERNAL_ERROR] = "Interner Fehler.",
                [EErrorCodes.ecUNKNOWN_ESCAPE_SEQUENCE] = "Unbekannte Escape-Sequenz.",
                [EErrorCodes.ecINVALID_NAME] = "Ungültiger Funktions-, Variablen- oder Konstantenbezeichner.",
                [EErrorCodes.ecINVALID_FUN_PTR] = "Ungültiger Zeiger auf eine Callback-Funktion.",
                [EErrorCodes.ecINVALID_VAR_PTR] = "Ungültiger Variablenzeiger.",
                [EErrorCodes.ecUNEXPECTED_OPERATOR] = "Unerwarteter Operator \"$IDENT$\" an Position $POS$.",
                [EErrorCodes.ecUNEXPECTED_EOF] = "Unerwartetes Formelende an Position $POS$.",
                [EErrorCodes.ecUNEXPECTED_COMMA] = "Unerwartetes Komma an Position $POS$.",
                [EErrorCodes.ecUNEXPECTED_PARENS] = "Unerwartete Klammer \"$IDENT$\" an Position $POS$ gefunden.",
                [EErrorCodes.ecUNEXPECTED_FUN] = "Unerwartete Funktion \"$IDENT$\" an Position $POS$ gefunden.",
                [EErrorCodes.ecUNEXPECTED_VAL] = "Unerwarteter Wert \"$IDENT$\" an Position $POS$ gefunden.",
                [EErrorCodes.ecUNEXPECTED_VAR] = "Unerwartete Variable \"$IDENT$\" an Position $POS$ gefunden.",
                [EErrorCodes.ecUNEXPECTED_STR] = "Unerwarteter Text an Position $POS$ gefunden.",
                [EErrorCodes.ecUNEXPECTED_CONDITIONAL] = "Der Operator \"$IDENT$\" muss stets auf eine schließenden Klammer folgen.",
                [EErrorCodes.ecUNEXPECTED_NEWLINE] = "Unerwarteter Zeilenumbruch.",
                [EErrorCodes.ecMISSING_PARENS] = "Fehlende Klammer.",
                [EErrorCodes.ecMISSING_ELSE_CLAUSE] = "\"If-then-else\" Operator ohne \"else\" Zweig verwendet.",
                [EErrorCodes.ecMISPLACED_COLON] = "Komma an unerwarteter Position $POS$ gefunden.",
                [EErrorCodes.ecTOO_MANY_PARAMS] = "Der Funktion \"$IDENT$\" wurden zu viele Argumente übergeben.",
                [EErrorCodes.ecTOO_FEW_PARAMS] = "Der Funktion \"$IDENT$\" wurden nicht genug Argumente übergeben.",
                [EErrorCodes.ecDIV_BY_ZERO] = "Division durch Null.",
                [EErrorCodes.ecDOMAIN_ERROR] = "Der Parameter der Funktion \"$IDENT$\" hat einen Wert, der nicht Teil des Definitionsbereiches der Funktion ist.",
                [EErrorCodes.ecNAME_CONFLICT] = "Namenskonflikt",
                [EErrorCodes.ecOPT_PRI] = "Ungültige Operatorpriorität (muss größer oder gleich Null sein).",
                [EErrorCodes.ecBUILTIN_OVERLOAD] = "Die Überladung für diesen Binäroperator steht im Widerspruch zu intern vorhanden operatoren.",
                [EErrorCodes.ecUNTERMINATED_STRING] = "Die Zeichenkette an Position $POS$ wird nicht beendet.",
                [EErrorCodes.ecSTRING_EXPECTED] = "Es wurde eine Zeichenkette als Funktionseingabewert erwartet.",
                [EErrorCodes.ecVAL_EXPECTED] = "Numerische Funktionen können nicht mit nichtnumerischen Parametern aufgerufen werden.",
                [EErrorCodes.ecTYPE_CONFLICT] = "Der Wert \"$IDENT$\" ist vom Typ '$TYPE1$' und es gibt keine passende Typkonversion in den erwarteten Typ '$TYPE2$'.",
                [EErrorCodes.ecTYPE_CONFLICT_FUN] = "Das Argument $ARG$ der Funktion oder des Operators \"$IDENT$\" ist vom Typ '$TYPE1$', erwartet wurde Typ '$TYPE2$'.",
                [EErrorCodes.ecTYPE_CONFLICT_IDX] = "Der Index der Variable \"$IDENT$\" muss ein positiver Ganzzahlwert sein. Der übergebene Indexwert ist vom Typ '$TYPE1$'.",
                [EErrorCodes.ecGENERIC] = "Allgemeiner Parser Fehler.",
                [EErrorCodes.ecINVALID_TYPE] = "Ungültiger Funktionsargumenttyp.",
                [EErrorCodes.ecINVALID_TYPECAST] = "Umwandlungen vom Typ '$TYPE1$' in den Typ '$TYPE2$' werden nicht unterstützt!",
                [EErrorCodes.ecARRAY_SIZE_MISMATCH] = "Feldgrößen stimmen nicht überein.",
                [EErrorCodes.ecNOT_AN_ARRAY] = "Der Indexoperator kann nicht auf die Skalarvariable \"$IDENT$\" angewandt werden.",
                [EErrorCodes.ecUNEXPECTED_SQR_BRACKET] = "Eckige Klammern sind an dieser Position nicht erlaubt.",
                [EErrorCodes.ecUNEXPECTED_CURLY_BRACKET] = "Geschweifte Klammern sind an dieser Position nicht erlaubt.",
                [EErrorCodes.ecINDEX_OUT_OF_BOUNDS] = "Indexüberschreitung bei Variablenzugriff auf \"$IDENT$\".",
                [EErrorCodes.ecINDEX_DIMENSION] = "Die Operation kann nicht auf Felder angewandt werden, deren Größe unterschiedlich ist.",
                [EErrorCodes.ecMISSING_SQR_BRACKET] = "Fehlendes \"]\".",
                [EErrorCodes.ecMISSING_CURLY_BRACKET] = "Fehlendes \"}\".",
                [EErrorCodes.ecASSIGNEMENT_TO_VALUE] = "Der Zuweisungsoperator \"$IDENT$\" kann in diesem Zusammenhang nicht verwendet werden.",
                [EErrorCodes.ecEVAL] = "Die Funktion bzw. der Operator \"$IDENT$\" kann nicht berechnet werden: $HINT$",
                [EErrorCodes.ecINVALID_PARAMETER] = "Der Parameter $ARG$ der Funktion \"$IDENT$\" is ungültig.",
                [EErrorCodes.ecINVALID_NUMBER_OF_PARAMETERS] = "Unzulässige Zahl an Funktionsparametern.",
                [EErrorCodes.ecOVERFLOW] = "Ein arithmetische Überlauf wurde in Funktion/Operator \"$IDENT$\" entdeckt.",
                [EErrorCodes.ecCONVERSION_OVERFLOW] = "Possible value overflow occurred in numeric conversion: \"$IDENT$\".",
                [EErrorCodes.ecMATRIX_DIMENSION_MISMATCH] = "Matrixdimensionen stimmen nicht überein, Operation \"$IDENT$\" kann nicht ausgeführt werden.",
                [EErrorCodes.ecVARIABLE_DEFINED] = "Die Variable \"$IDENT$\" is bereits definiert.",
                [EErrorCodes.ecCONSTANT_DEFINED] = "Die Konstante \"$IDENT$\" is bereits definiert.",
                [EErrorCodes.ecFUNOPRT_DEFINED] = "Ein Element mit der Bezeichnung \"$IDENT$\" ist bereits definiert.",
                [EErrorCodes.ecUNDEFINED] = "Undefined behaviour"
            };
        }
    }
}
