using System;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using MuParserSharp;
using MuParserSharp.Parser;

namespace MuParserREPL
{
    class MuParserREPL3
    {
        static class Program
        {
            const string PROMPT = ":> ";
            static void Main()
            {
                EnvDTE80.DTE2 vs_env;
                (string file, int line) error = (null, -1);
                var parser = new ParserX(); //EPackages.pckALL_NON_COMPLEX);
                Initialise(parser);
                DrawSplash();
                SelfTest();
                vs_env = (EnvDTE80.DTE2)Marshal.GetActiveObject("VisualStudio.DTE.15.0");
                while (true)
                {
                    try
                    {
                        Console.Write(PROMPT);
                        var inStr = Console.ReadLine();
                        if (string.IsNullOrEmpty(inStr) || CheckKeywords(inStr)) continue;
                        parser.SetExpr(inStr);
                        var ans = parser.Eval();

                        Console.WriteLine($"Result (type: '{ans.GetValueType()}'):");
                        Console.WriteLine($"ans = {ans.TidyString()}\n");
                        error.line = -1;
                    }
                    catch (ParserError pe)
                    {
                    
                        //if (error.line != -1)
                        //{
                        //    var len = vs_env.Debugger.Breakpoints.Count;
                        //    vs_env.Debugger.Breakpoints.Item(len).Delete();
                        //}
                        //else
                        pe.Print();
                        error = (pe._file, pe._line);
                    }
                
                }



                bool CheckKeywords(string sLine)
                {

                    if (sLine == "quit" || sLine == "exit")
                    {
                        Environment.Exit(0);
                    }
                    else if (sLine == "exprvar")
                    {
                        ListExprVar(parser);
                        return true;
                    }
                    else if (sLine == "cls")
                    {
                        Console.Clear();
                        return true;
                    }
#if DEBUG
                    else if (sLine == "break")
                    {
                        if (error.line != -1)
                        {
                            vs_env.Debugger.Breakpoints.Add(Line: error.line, File: error.file);
                            parser.Eval();
                            var len = vs_env.Debugger.Breakpoints.Count;
                            vs_env.Debugger.Breakpoints.Item(len).Delete();
                            error = ("", -1);
                        }
                        else Console.WriteLine("  No error to break on!");
                        return true;
                    }
#endif
                    else if (sLine == "clear")
                    {
                        Console.Clear();
                        DrawSplash();
                        return true;
                    }
                    else if (sLine == "rpn")
                    {
                        parser.DumpRPN();
                        return true;
                    }

                    return false;
                }
            }

            private static void Initialise(ParserX parser)
            {
                // Create an array variable
                Value arr1 = new Value(3, 0);
                arr1[0] = (1.0);
                arr1[1] = 2.0;
                arr1[2] = 3.0;

                Value arr2 = new Value(3, 0);
                arr2[0] = 4.0;
                arr2[1] = 3.0;
                arr2[2] = 2.0;

                Value arr3 = new Value(4, 0);
                arr3[0] = 1.0;
                arr3[1] = 2.0;
                arr3[2] = 3.0;
                arr3[3] = 4.0;

                Value arr4 = new Value(3, 0);
                arr4[0] = 4.0;
                arr4[1] = false;
                arr4[2] = "hallo";

                // Create a 3x3 matrix with zero elements
                Value m1 = new Value(3, 3, 0);
                m1[0, 0] = 1.0;
                m1[1, 1] = 1.0;
                m1[2, 2] = 1.0;

                Value m2 = new Value(3, 3, 0);
                m2[0, 0] = 1.0;
                m2[0, 1] = 2.0;
                m2[0, 2] = 3.0;
                m2[1, 0] = 4.0;
                m2[1, 1] = 5.0;
                m2[1, 2] = 6.0;
                m2[2, 0] = 7.0;
                m2[2, 1] = 8.0;
                m2[2, 2] = 9.0;

                Value[] val = new Value[5];
                val[0] = 1.1;
                val[1] = 1.0;
                val[2] = false;
                val[3] = "Hello";
                val[4] = "World";

                Value[] fVal = new Value[3];
                fVal[0] = 1;
                fVal[1] = 2.22;
                fVal[2] = 3.33;

                Value[] sVal = new Value[3];
                sVal[0] = "hello";
                sVal[1] = "world";
                sVal[2] = "test";

                Value[] cVal = new Value[3];
                cVal[0] = new Complex(1, 1);
                cVal[1] = new Complex(2, 2);
                cVal[2] = new Complex(3, 3);

                var size_3x1 = new Value(1, 2, 0);
                size_3x1.At(0, 0) = 3.0;
                size_3x1.At(0, 1) = 1.0;

                parser.DefineVar("s31", new Variable(size_3x1));
                Value ans = new Value(0);
                parser.DefineVar("ans", new Variable(ans));

                // some tests for vectors
                parser.DefineVar("va", new Variable(arr1));
                parser.DefineVar("vb", new Variable(arr2));
                parser.DefineVar("vc", new Variable(arr3));
                parser.DefineVar("vd", new Variable(arr4));
                parser.DefineVar("m1", new Variable(m1));
                parser.DefineVar("m2", new Variable(m2));

                parser.DefineVar("a", new Variable(fVal[0]));
                parser.DefineVar("b", new Variable(fVal[1]));
                parser.DefineVar("c", new Variable(fVal[2]));

                parser.DefineVar("ca", new Variable(cVal[0]));
                parser.DefineVar("cb", new Variable(cVal[1]));
                parser.DefineVar("cc", new Variable(cVal[2]));

                parser.DefineVar("sa", new Variable(sVal[0]));
                parser.DefineVar("sb", new Variable(sVal[1]));

                // Add functions for inspecting the parser properties
                parser.DefineFun(new FunListVar());
                parser.DefineFun(new FunListFunctions());
                parser.DefineFun(new FunListConst());
                parser.DefineFun(new FunBenchmark());
                parser.DefineFun(new FunEnableOptimizer());
                parser.DefineFun(new FunSelfTest());
                parser.DefineFun(new FunEnableDebugDump());
                parser.DefineFun(new FunTest0());
                parser.DefineFun(new FunPrint());


                parser.DefineFun(new FunLang());
                parser.EnableAutoCreateVar(true);

#if DEBUG
                ParserXBase.EnableDebugDump(bDumpStack: false, bDumpRPN: false);
#endif

                Value x = 1.0;
                Value y = new Complex(0, 1);
                parser.DefineVar("x", new Variable(x));
                parser.DefineVar("y", new Variable(y));

            }

            private static void ListExprVar(ParserX parser)
            {
                Console.WriteLine($"   Variables found in : \"{parser.GetExpr()}\"");
                Console.WriteLine("   -----------------------------\n");
                // Query the used variables (must be done after calc)
                var vmap = parser.GetExprVar();
                if (!vmap.Any())
                    Console.WriteLine("Expression does not contain variables\n");
                else foreach (var item in vmap)
                    Console.WriteLine($"      {item.Key} =  {item.Value}");
                Console.WriteLine();
            }

            static void SelfTest()
            {
                Console.WriteLine("-------------------------------------------------------------------------\n");
                Console.WriteLine("Running test suite:\n");

                //var pt = new ParserTester();
                //pt.Run();

                Console.WriteLine("-------------------------------------------------------------------------\n");
                Console.WriteLine("Special parser functions:");
                Console.WriteLine("  list_var()   - list parser variables and return the number of variables.");
                Console.WriteLine("  list_fun()   - list parser functions and return  the number of functions");
                Console.WriteLine("  list_const() - list all numeric parser constants");
                Console.WriteLine("Command line commands:");
                Console.WriteLine("  exprvar      - list all variables found in the last expression");
                Console.WriteLine("  rpn          - Dump reverse polish notation of the current expression");
                Console.WriteLine("  quit         - exits the parser");
                Console.WriteLine("Constants:");
                Console.WriteLine("  \"e\"   2.718281828459045235360287");
                Console.WriteLine("  \"pi\"  3.141592653589793238462643");
                Console.WriteLine("-------------------------------------------------------------------------\n");
            }

            static void DrawSplash()
            {
                Console.WriteLine(@"
##################################################################
#                ____                          _  _              #
#               |  __ \                      _| || |_            #
#          _   _| |__) |_ _ _ _ ________ _ _|_  __  _|           #
#         | | | |  ___/ _` | '__|_  / _ \ '__|| || |_            #
#         | |_| | |  | (_| | |   / /  __/ | |_  __  _|           #
#         | ._,_|_|   \__,_|_|  /___\___|_|   |_||_|             #
#         | |                                                    #
#         |_|                                                    #
#                                                                #
##################################################################
");
                Console.Write($"Welcome to μParzer#.\n\t(c) John Cleary, {ParserXBase.GetVersion()}\n");
#if DEBUG
                Console.WriteLine("\tDEBUG Build");
#else
            Console.WriteLine();
#endif
            }
        }
    }
}
