using System;
using MuParserSharp.Framework;

namespace MuParserSharp.Parser
{
    public class ValueCache
    {
        public ValueCache(int size = 10)
        {
            m_nIdx = -1;
            m_vCache = new IValue[size];

        }
        
        //------------------------------------------------------------------------------
        public void ReleaseAll()
        {
            for(int i=0; i< m_vCache.Length;i++)
            {
                m_vCache[i] = null;
            }
            m_nIdx = -1;
        }

        //------------------------------------------------------------------------------
        public void ReleaseToCache(ref IValue pValue)
        {
            if(pValue == null)return;

            if(m_nIdx < (m_vCache.Length - 1))
            {
                m_nIdx++;
                m_vCache[m_nIdx] = pValue;
            }

        }
        
        //------------------------------------------------------------------------------
        public bool CreateFromCache(out IValue pValue)
        {
            if(m_nIdx >= 0)
            {
                pValue = m_vCache[m_nIdx];
                m_vCache[m_nIdx] = null;
                m_nIdx--;
                return true;
            }
            var val = new Value();
            val.BindToCache(this);
            pValue = val;
            return false;
        }

        private int m_nIdx;
        readonly IValue[] m_vCache;
    }
}
