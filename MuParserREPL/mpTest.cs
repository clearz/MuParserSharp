using System;
using System.Collections.Generic;
using System.IO;
using MuParserSharp.Parser;

namespace MuParserREPL
{
 
    class ParserTester
    {
        public ParserTester()
        {
            m_stream = new StreamWriter(Console.OpenStandardOutput());
            AddTest(TestParserValue);
            AddTest(TestUndefVar);
            AddTest(TestErrorCodes);
            AddTest(TestEqn);
            AddTest(TestIfElse);
            AddTest(TestStringFun);
            AddTest(TestMatrix);
            AddTest(TestComplex);
            AddTest(TestVector);
            AddTest(TestBinOp);
            AddTest(TestPostfix);
            AddTest(TestInfix);
            AddTest(TestMultiArg);
            AddTest(TestScript);
            AddTest(TestValReader);
            AddTest(TestIssueReports);

            c_iCount = 0;
        }

        public void Run()
        {
            int iStat = 0;
            try
            {
                foreach (var func in m_vTestFun)
                    iStat += func();
            }
            catch (ParserError e)
            {
                m_stream.WriteLine(e.GetMsg());
                m_stream.WriteLine(e.GetToken());
                Abort();
            }

            catch (Exception e)
            {
                m_stream.WriteLine(e.Message);
                Abort();
            }

            if (iStat == 0)
            {
                m_stream.WriteLine($"Test passed ({c_iCount} expressions)");
            }
            else
            {
                m_stream.WriteLine("Test failed with {iStat} errors ({c_iCount} expressions)");
            }

            c_iCount = 0;
        }

        int TestParserValue()
        {
            return 0;
        }
        int TestErrorCodes()
        {
            return 0;
        }
        int TestStringFun()
        {
            return 0;
        }
        int TestVector()
        {
            return 0;
        }
        int TestBinOp()
        {
            return 0;
        }
        int TestPostfix()
        {
            return 0;
        }
        int TestInfix()
        {
            return 0;
        }
        int TestEqn()
        {
            return 0;
        }
        int TestMultiArg()
        {
            return 0;
        }
        int TestUndefVar()
        {
            return 0;
        }
        int TestIfElse()
        {
            return 0;
        }
        int TestMatrix()
        {
            return 0;
        }
        int TestComplex()
        {
            return 0;
        }
        int TestScript()
        {
            return 0;
        }
        int TestValReader()
        {
            return 0;
        }
        int TestIssueReports()
        {
            return 0;
        }
        private void AddTest(testfun_type a_pFun)
        {
            m_vTestFun.Add(a_pFun);
        }

        int EqnTest(string a_str, Value a_val, bool a_fPass, int nExprVar = -1)
        {
            return 0;
        }

        int ThrowTest(string a_str, int a_nErrc, int a_nPos = -1, string a_sIdent = "")
        {
            return 0;
        }
        private void Abort()
        {
            m_stream.Write("\nTest failed (internal error in test class)\n");
            Console.ReadKey();
            Environment.Exit(-1);
        }
        void Assessment(int a_iNumErr)
        {
            if (a_iNumErr == 0)
                m_stream.Write("passed\n");
            else
                m_stream.Write($"\n  failed with {a_iNumErr} errors\n");
        }

        private static int c_iCount;
        private delegate int testfun_type();
        private readonly StreamWriter m_stream;
        private List<testfun_type> m_vTestFun;
    }
}
