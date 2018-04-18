using System;

namespace MuParserSharp.Util
{
    public unsafe struct scoped_setter : IDisposable
    {
        private readonly bool* m_ref;
        private readonly bool m_buf;
        public scoped_setter(bool* ref_val, bool new_val)
        {
            m_ref = ref_val;
            m_buf = *ref_val;
            *ref_val = new_val;

        }
        public void Dispose()
        {
            *m_ref = m_buf;
        }
    }
}
