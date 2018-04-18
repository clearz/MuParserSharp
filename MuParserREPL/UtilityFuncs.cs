using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using MuParserSharp;
using MuParserSharp.Framework;
using MuParserSharp.Parser;
using MuParserSharp.Util;

namespace MuParserREPL
{

    class FunPrint : ICallback
    {
        public FunPrint() : base(ECmdCode.cmFUNC, "print", -1) { }
        public override string GetDesc() => "";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Debug.Assert(a_pArg.Length > 0);
            string s = a_pArg[0].GetString();
            if(a_pArg.Length == 1)
                Console.WriteLine(s);
            else
                Console.WriteLine(s, a_pArg.Skip(1).Select(v => (object)v).ToArray());
            ret = 0;
        }

        public override IToken Clone() => (FunPrint)MemberwiseClone();
    }

    class FunTest0 : ICallback
    {
        public FunTest0() : base(ECmdCode.cmFUNC, "test0", 0) { }
        public override string GetDesc() => "";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            ret = 0;
        }

        public override IToken Clone() => (FunTest0)MemberwiseClone();
    }

    class FunListVar : ICallback
    {
        public FunListVar() : base(ECmdCode.cmFUNC, "list_var", 0) { }
        public override string GetDesc() => "list_var() - List all variables of the parser bound to this function and returns the number of defined variables.";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            ParserXBase parser = GetParent();
            Console.WriteLine("\nParser variables:");
            Console.WriteLine("-----------------\n");

            var vars = parser.GetVar();
            if(vars.Count == 0)
                Console.WriteLine("Expression does not contain variables\n");
            else
            {
                foreach (var item in vars)
                {
                    var v = (Variable) item.Value;
                    Console.Write($"  {item.Key} = {item.Value}");
                    Console.WriteLine($"  (type=\"{v.GetValueType()}\"; ptr=0x{v.MemoryAddress()})");
                }
            }

            ret = vars.Count;
        }

        public override IToken Clone() => (FunListVar)MemberwiseClone();
    }

    class FunListConst : ICallback
    {
        public FunListConst() : base(ECmdCode.cmFUNC, "list_const", 0) { }
        public override string GetDesc() => "list_const() - List all constants of the parser bound to this function and returns the number of defined constants.";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            ParserXBase parser = GetParent();
            Console.WriteLine("\nParser constants:");
            Console.WriteLine("-----------------\n");

            var consts = parser.GetConst();
            if (consts.Count == 0)
                Console.WriteLine("No constants defined\n");
            else
            {
                foreach (var item in consts)
                {
                    var v = (Value)item.Value;
                    Console.WriteLine($"  {item.Key} = {item.Value}");
                }
            }

            ret = consts.Count;
        }

        public override IToken Clone() => (FunListConst)MemberwiseClone();
    }

    class FunBenchmark : ICallback
    {
        public FunBenchmark() : base(ECmdCode.cmFUNC, "bench", -1) { }
        public override string GetDesc() => "bench() - Perform a benchmark with a set of standard functions.";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
           // Debug.Assert(a_pArg.Length <= 1);
            StreamWriter pFile = null;
            bool logToFile = false;

            int iCount = 400000;

            if (a_pArg.Length > 0)
                if (a_pArg[0].IsInteger())
                    iCount = (int) a_pArg[0].GetInteger();
                else
                    logToFile = a_pArg[0].GetBool();

            string[] sExpr =
            {
                "sin(a)", "cos(a)", "tan(a)", "sqrt(a)", "(a+b)*3", "a^2+b^2", "a^3+b^3", "a^4+b^4",
                "a^5+b^5", "a*2.43854357347+b*2.43854357347", "-(b^1.1)", "a + b * c", "a * b + c", "a+b*(a+b)", "(1+b)*(-3.43854357347)",
                "e^log(7.43854357347*a)", "10^log(3+b)", "a+b-e*pi/5^6", "a^b/e*pi-5+6", "sin(a)+sin(b)",
                "(cos(2.41)/b)", "-(sin(pi+a)+1.43854357347)", "a-(e^(log(7+b)))", "sin(((a-a)+b)+a)",
                "((0.09/a)+2.58)-1.67", "abs(sin(sqrt(a^2+b^2))*255)", "abs(sin(sqrt(a*a+b*b))*255)",
                "cos(0.90-((cos(b)/2.89)/e)/a)", "(1*(2*(3*(4*(5*(6*(a+b)))))))",
                "abs(sin(sqrt(a^2.1+b^2.1))*255)", "(1.43854357347*(2.43854357347*(3.43854357347*(4.43854357347*(5.43854357347*(6.43854357347*(7.43854357347*(a+b))))))))",
                "1.43854357347/(a*sqrt(2.43854357347*pi))*e^(-0.543854357347*((b-a)/a)^2.43854357347)", "1.43854357347+2.43854357347-3.43854357347*4.43854357347/5.43854357347^6.43854357347*(2.43854357347*(1.43854357347-5.43854357347+(3.43854357347*7.43854357347^9.43854357347)*(4.43854357347+6.43854357347*7.43854357347-3.43854357347)))+12",
                "1+b-3*4.0/5.43854357347^6*(2*(1.43854357347-5.43854357347+(3.43854357347*7.43854357347^9.43854357347)*(4+6*7-3)))+12.43854357347*a",
                "(b+1)*(b+2)*(b+3)*(b+4)*(b+5)*(b+6)*(b+7)*(b+8)*(b+9)*(b+10)*(b+11)*(b+12)",
                "(a/((((b+(((e*(((((pi*((((3.43854357347*((pi+a)+pi))+b)+b)*a))+0.43854357347)+e)+a)/a))+a)+b))+b)*a)-pi))",
                "(((-9.43854357347))-e/(((((((pi-(((-7.43854357347)+(-3.1238723947329)/4.43897589288/e))))/(((-5.43854357347))-2.43854357347)-((pi+(-0))*(sqrt((e+e))*(-8.43854357347))*(((-pi)+(-pi)-(-9.43854357347)*(6.43854357347*5.43854357347))/(-e)-e))/2.43854357347)/((((sqrt(2.43854357347/(-e)+6.43854357347)-(4.43854357347-2.43854357347))+((5.43854357347/(-2.43854357347))/(1*(-pi)+3.43854357347))/8.43854357347)*pi*((pi/((-2.43854357347)/(-6.43854357347)*1.43854357347*(-1.43854357347))*(-6.43854357347)+(-e)))))/((e+(-2.43854357347)+(-e)*((((-3.43854357347)*9.43854357347+(-e)))+(-9)))))))-((((e-7.43854357347+(((5.43854357347/pi-(3.43854357347/1.43854357347+pi)))))/e)/(-5))/(sqrt((((((1+(-7))))+((((-e)*(-e)))-8.43854357347))*(-5.43854357347)/((-e)))*(-6.43854357347)-((((((-2.43854357347)-(-9.43854357347)-(-e)-1)/3))))/(sqrt((8.43854357347+(e-((-6.43854357347))+(9.43854357347*(-9.43854357347))))*(((3.43854357347+2.43854357347-8.43854357347))*(7.43854357347+6.43854357347+(-5.43854357347))+((0/(-e)*(-pi))+7)))+(((((-e)/e/e)+((-6)*5)*e+(3+(-5)/pi))))+pi))/sqrt((((9.43854357347))+((((pi))-8.43854357347+2.43854357347))+pi))/e*4.43854357347)*((-5.43854357347)/(((-pi))*(sqrt(e)))))-(((((((-e)*(e)-pi))/4.43854357347+(pi)*(-9.43854357347)))))))+(-pi)"
            };
            
#if DEBUG
            var outstr = $"{Directory.GetCurrentDirectory()}/Result_{DateTime.Now.ToString(CultureInfo.InvariantCulture).Replace(':', '-').Replace(' ', '-').Replace('/', '-')}_debug.txt";
#else
            var outstr = $"{Directory.GetCurrentDirectory()}/Result_{DateTime.Now.ToString(CultureInfo.InvariantCulture).Replace(':', '-').Replace(' ', '-').Replace('/', '-')}_release.txt";
#endif
            var parser = new ParserX(EPackages.pckALL_NON_COMPLEX);
            Value a = new Value(1.0);
            Value b = new Value(2.0);
            Value c = new Value(3.0);

            parser.DefineVar("a", new Variable(a));
            parser.DefineVar("b", new Variable(b));
            parser.DefineVar("c", new Variable(c));
           // parser.DefineConst("pi", 3.14159265);
           // parser.DefineConst("e", 2.718281828459);

            var timer = new Timer();

#if DEBUG
            string sMode = "# debug mode\n";
#else
    string sMode = "# release mode\n";
#endif
            if (logToFile)
            {
                pFile = new StreamWriter(outstr);
                pFile.Write("{0}; muParserX V{1}\n", sMode, ParserXBase.GetVersion());
                pFile.Write("\"Eqn no.\", \"number\", \"result\", \"time in ms\", \"eval per second\", \"expr\"\n");
            }

            Console.Write(sMode);
            Console.Write("\"Eqn no.\", \"number\", \"result\", \"time in ms\", \"eval per second\", \"expr\"\n");
            



            double avgEvalPerSec = 0;
            int ct = 0;
            for (int i = 0; i < sExpr.Length; ++i)
            {
                ct++;
                timer.Start();
                IValue value = 0;
                parser.SetExpr(sExpr[i]);

                // implicitely create reverse polish notation
                parser.Eval();

                for (int n = 0; n < iCount; ++n)
                {
                    value = parser.Eval();
                }
                timer.Stop();
                double diff = (double)timer.Duration(iCount);

                double evalPerSec = iCount * 1000.0 / diff;
                avgEvalPerSec += evalPerSec;
                if(logToFile) pFile.Write("Eqn_{0}, {1:n}, {2,-5:n}, {3,-10:n}, {4,-10:n}, {5}\n", i, iCount, value.AsFloat(), diff, evalPerSec, sExpr[i]);
                Console.Write("Eqn_{0}, {1:n}, {2,-5:n}, {3,-10:n}, {4,-10:n}, {5}\n", i, iCount, value.AsFloat(), diff, evalPerSec, sExpr[i]);
            }

            avgEvalPerSec /= ct;

            if (logToFile) pFile.Write("# Eval per s: {0}", (long)avgEvalPerSec);
            Console.WriteLine("# Eval per s: {0}", (long)avgEvalPerSec);
            if (logToFile)
            {
                pFile.Flush();
                pFile.Close();
            }

            ret = avgEvalPerSec;
        }

        public override IToken Clone() => (FunBenchmark)MemberwiseClone();
    }

    class FunListFunctions : ICallback
    {
        public FunListFunctions() : base(ECmdCode.cmFUNC, "list_fun", 0) { }
        public override string GetDesc() => "list_fun() - List all parser functions and returns the total number of defined functions.";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            ParserXBase parser = GetParent();
            Console.WriteLine("\nParser functions:");
            Console.WriteLine("-----------------\n");

            var funcs = parser.GetFunDef();
            if (funcs.Count == 0)
                Console.WriteLine("No functions defined\n");
            else
            {
                foreach (var item in funcs)
                {
                    var v = (ICallback)item.Value;
                    if (v.GetDesc() == "") continue;
                    Console.WriteLine($"  {v.GetDesc()}");
                }
            }

            ret = funcs.Count;
        }

        public override IToken Clone() => (FunListFunctions)MemberwiseClone();
    }

    class FunEnableOptimizer : ICallback
    {
        public FunEnableOptimizer() : base(ECmdCode.cmFUNC, "enable_optimizer", 1) { }
        public override string GetDesc() => "enable_optimizer(bool) - Enables the parsers built in expression optimizer.";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            ParserXBase parser = GetParent();
            parser.EnableOptimizer(a_pArg[0].GetBool());
            ret = a_pArg[0].GetBool();
        }

        public override IToken Clone() => (FunEnableOptimizer)MemberwiseClone();
    }
    class FunSelfTest : ICallback
    {
        public FunSelfTest() : base(ECmdCode.cmFUNC, "test", 1) { }
        public override string GetDesc() => "test() - Runs the unit test of muparserx.";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            ParserXBase.EnableDebugDump(false, false);
            var pt =  new ParserTester();
            pt.Run();
            ret = 0;
        }

        public override IToken Clone() => (FunSelfTest)MemberwiseClone();
    }

    class FunEnableDebugDump : ICallback
    {
        public FunEnableDebugDump() : base(ECmdCode.cmFUNC, "debug", 2) { }
        public override string GetDesc() => "debug(bDumpRPN, bDumpStack) - Enable dumping of RPN and stack content.";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            bool bo = a_pArg[0].GetBool();
            bool so = a_pArg[1].GetBool();
            Console.WriteLine($"Bytecode output {(bo ? "de" : "")}activated.");
            Console.WriteLine($"Stack output {(so ? "de" : "")}activated.");
            ParserXBase.EnableDebugDump(so, bo);
            ret = 0;
        }

        public override IToken Clone() => (FunEnableDebugDump)MemberwiseClone();
    }

    class FunLang : ICallback
    {
        public FunLang() : base(ECmdCode.cmFUNC, "lang", 1) { }
        public override string GetDesc() => "lang(sLang) - Set the language of error messages (i.e. \"de\" or \"en\").";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            string sID = a_pArg[0].GetString();

            ParserX.ResetErrorMessageProvider(sID);

            ret = 0;
        }

        public override IToken Clone() => (FunLang)MemberwiseClone();
    }


    /*

    class FunGeneric : ICallback
    {
        public FunGeneric() : base(ECmdCode.cmFUNC, "strlen", 1) { }
        public override string GetDesc() => "";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(() => a_pArg.Length == 1);
            ret = a_pArg[0].GetString().Length;
        }

        public override IToken Clone() => (FunGeneric)MemberwiseClone();
    }

    class FunDefine : ICallback
    {
        public FunDefine() : base(ECmdCode.cmFUNC, "strlen", 1) { }
        public override string GetDesc() => "";
        public override void Eval(ref IValue ret, IValue[] a_pArg)
        {
            Global.MUP_VERIFY(() => a_pArg.Length == 1);
            ret = a_pArg[0].GetString().Length;
        }

        public override IToken Clone() => (FunDefine)MemberwiseClone();
    }
    */
}
