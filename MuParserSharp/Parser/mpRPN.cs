using System;
using System.Collections.Generic;
using System.Linq;
using MuParserSharp.Framework;
using MuParserSharp.Util;

namespace MuParserSharp.Parser
{
    class RPN
    {
        public RPN()
        {
            m_vRPN = new List<IToken>();
            m_nStackPos = -1;

        }

        public void Add(IToken tok)
        {
            m_vRPN.Add(tok);
            if (tok.AsIValue() != null)
            {
                m_nStackPos++;
            }
            else if (tok.AsICallback() != null)
            {
                ICallback pFun = tok.AsICallback();
                Global.MUP_VERIFY(pFun != null);
                m_nStackPos -= pFun.GetArgsPresent() - 1;
            }

            Global.MUP_VERIFY(m_nStackPos >= 0);
            m_nMaxStackPos = Math.Max(m_nStackPos, m_nMaxStackPos);
        }



        public void AddNewline(IToken tok, int n)
        {
            ((TokenNewline)tok).SetStackOffset(n);
            m_vRPN.Add(tok);
            m_nStackPos -= n;
            m_nLine++;
        }

        public void Pop(int num)
        {
            if (!m_vRPN.Any())
                return;

            for (int i = 0; i < num; ++i)
            {
                IToken tok = m_vRPN.Last();

                if (tok.AsIValue() != null)
                    m_nStackPos--;

                m_vRPN.RemoveAt(m_vRPN.Count - 1);
            }
        }

        public void Reset()
        {
            m_vRPN.Clear();
            m_nStackPos = -1;
            m_nMaxStackPos = 0;
            m_nLine = 0;
        }


        public void Finalise()
        {
            // Determine the if-then-else jump offsets
            var stIf = new Stack<int>();
            var stElse = new Stack<int>();
            int idx;
            for (var i = 0; i < m_vRPN.Count; ++i)
            {
                switch (m_vRPN[i].GetCode())
                {
                    case ECmdCode.cmIF:
                        stIf.Push(i);
                        break;

                    case ECmdCode.cmELSE:
                        stElse.Push(i);
                        idx = stIf.Pop();
                        ((TokenIfThenElse)m_vRPN[idx]).SetOffset(i - idx);
                        break;

                    case ECmdCode.cmENDIF:
                        idx = stElse.Pop();
                        ((TokenIfThenElse)m_vRPN[idx]).SetOffset(i - idx);
                        break;

                    default:
                        continue;
                }
            }
        }
        internal void EnableOptimizer(bool bStat) => m_bEnableOptimizer = bStat;

        public int GetSize() => m_vRPN.Count;

        public IList<IToken> GetData() => m_vRPN;

        public int GetRequiredStackSize() => m_nMaxStackPos + 1;

        internal void AsciiDump()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n   Number of tokens: " + m_vRPN.Count);
            Console.WriteLine("   MaxStackPos:      " + m_vRPN.Count);
            for (var i = 0; i < m_vRPN.Count; ++i)
            {
                var pTok = m_vRPN[i];
                Console.WriteLine($"    {i,2} : {pTok.GetExprPos(),2} : {pTok.AsciiDump()}");
            }
            Console.ResetColor();
        }

        private readonly List<IToken> m_vRPN;
        private int m_nStackPos;
        private int m_nLine;
        private int m_nMaxStackPos;
        private bool m_bEnableOptimizer;
    }
}
