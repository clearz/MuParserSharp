using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MuParserSharp.Framework;
using MuParserSharp.Util;

namespace MuParserSharp.Parser
{
    public class Matrix
    {
       // ---------------------------------------------------------------------------------------------
        public Matrix()
        {
            m_nRows = 1;
            m_nCols = 1;
            m_eStorageScheme = EMatrixStorageScheme.mssROWS_FIRST;

            m_vData = new IValue[1];
        }

        //---------------------------------------------------------------------------------------------
        public Matrix(long nRows, IValue value)
        {
            m_nRows = nRows;
            m_nCols = 1;
            m_eStorageScheme = EMatrixStorageScheme.mssROWS_FIRST;
            m_vData = new IValue[nRows];
            for (long l = 0; l < nRows; l++)
                m_vData[l] = (IValue)value.Clone();
        }
        

        //---------------------------------------------------------------------------------------------
        /*  Constructs a Matrix object representing a scalar value
        */
        public Matrix(IValue value)
        {
            m_nRows = 1;
            m_nCols = 1;
            m_eStorageScheme = EMatrixStorageScheme.mssROWS_FIRST;
            m_vData = new [] {(IValue)value.Clone()};
        }
        //---------------------------------------------------------------------------------------------
        /*  Constructs a Matrix object representing a vector
        */
        public Matrix(IEnumerable<IValue> v)
        {
            m_nRows = v.Count();
            m_nCols = 1;
            m_eStorageScheme = EMatrixStorageScheme.mssROWS_FIRST;
            m_vData = v.Select(v1 => (IValue)v1.Clone()).ToArray();
        }

        //---------------------------------------------------------------------------------------------
        /*  Constructs a Matrix object representing a vector
        */
        public Matrix(IValue[] v)
        {
            m_nRows = v.Length;
            m_nCols = 1;
            m_eStorageScheme = EMatrixStorageScheme.mssROWS_FIRST;
            m_vData = v.Select(v1 => (IValue)v1.Clone()).ToArray();
        }

        //---------------------------------------------------------------------------------------------

        public Matrix(IValue[][] v)
        {
            m_nRows = v.Length;
            m_nCols = v[0].Length;
            m_eStorageScheme = EMatrixStorageScheme.mssROWS_FIRST;
            m_vData = new IValue[m_nRows*m_nCols];
            for(long m=0; m < m_nRows; m++)
                for(long n=0; n < m_nCols; n++)
                {
                    At(m, n) = (IValue)v[m][n].Clone();
                }
        }

        //---------------------------------------------------------------------------------------------
        public Matrix(long nRows, long nCols, IValue value)
        {
            m_nRows = nRows;
            m_nCols = nCols;
            m_eStorageScheme = EMatrixStorageScheme.mssROWS_FIRST;
            m_vData = new IValue[nRows * nCols];
                for(long l = 0; l < nRows * nCols; l++)
                    m_vData[l] = (IValue)value.Clone();
        }    

        public Matrix(Matrix m)
        {
            Assign(m);
        }
        
        //---------------------------------------------------------------------------------------------
        public static implicit operator Matrix(IValue v)
        {
            return new Matrix(v);
        }

        //---------------------------------------------------------------------------------------------
        public static Matrix operator +(Matrix m1, Matrix m2)
        {
            if (m1.m_nRows != m2.m_nRows || m1.m_nCols != m2.m_nCols)
                throw new MatrixError("Matrix dimension mismatch");
            var m3 = new Matrix(m1.m_nRows, m1.m_nCols, (IValue)0);
            for (long i = 0; i < m1.m_nRows; ++i)
            {
                for (long j = 0; j < m1.m_nCols; ++j)
                {
                    m3.At(i, j) = m1.At(i, j) + m2.At(i, j);
                }
            }

            return m3;
        }

        
        //---------------------------------------------------------------------------------------------
        public static Matrix operator -(Matrix m1, Matrix m2)
        {
            if (m1.m_nRows != m2.m_nRows || m1.m_nCols != m2.m_nCols)
                throw new MatrixError("Matrix dimension mismatch");
            var m3 = new Matrix(m1.m_nRows, m1.m_nCols, (IValue)0);
            for (long i = 0; i < m1.m_nRows; ++i)
            {
                for (long j = 0; j < m1.m_nCols; ++j)
                {
                    m3.At(i, j) = m1.At(i, j) - m2.At(i, j);
                }
            }

            return m3;
        }
        //---------------------------------------------------------------------------------------------
        public static Matrix operator *(Matrix m1, IValue v)
        {

            var m3 = new Matrix(m1.m_nRows, m1.m_nCols, (IValue)0);
            for (long i = 0; i < m1.m_nRows; ++i)
            {
                for (long j = 0; j < m1.m_nCols; ++j)
                {
                    m3.At(i, j) = m1.At(i, j) * v;
                }
            }

            return m3;
        }
        
        //---------------------------------------------------------------------------------------------
        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            Matrix m3;
            // Matrix x Matrix multiplication
            if (m2.GetRows() == 0)
            {
                IValue v = m2.At(0, 0);
                m3 = new Matrix(m1);
                m3 *= v;
            }
            else if (m1.GetRows() == 0)
            {
                IValue v = m1.At(0, 0);
                m3 = new Matrix(m2);
                m3 *= v;
            }
            else if (m1.m_nCols == m2.m_nRows)
            {
                m3 = new Matrix(m1.m_nRows, m2.m_nCols, 0L);

                // For each cell in the output matrix
                for (long m = 0; m < m1.m_nRows; ++m)
                {
                    for (long n = 0; n < m2.m_nCols; ++n)
                    {
                        IValue buf = (IValue)0l;
                        for (long i = 0; i < m1.m_nCols; ++i)
                        {
                            buf += (IValue)(m1.At(m, i) * m2.At(i, n));
                        }
                        m3.At(m, n) = (IValue)buf;
                    } // for all rows
                } // for all columns
            }
            else
                throw new MatrixError("Matrix dimensions don't allow multiplication");

            return m3;
        }
        //---------------------------------------------------------------------------------------------
        public void AsciiDump(string szTitle)
        {

            Console.WriteLine(szTitle);
            Console.WriteLine("------------------");
            
            Console.WriteLine("Cols: " + GetCols());
            Console.WriteLine("Rows: " + GetRows());
            Console.WriteLine("Dim:  " + GetDim());

            for (long i = 0; i < m_nRows; ++i)
            {
                for (long j = 0; j < m_nCols; ++j)
                {
                    Console.Write(At(i, j) + "  ");
                }
                Console.WriteLine();
            }

            Console.WriteLine("\n");
        }

        //---------------------------------------------------------------------------------------------
        
        public override string ToString()
        {
            var ss = new StringBuilder();
            for (long i = 0; i < m_nRows; ++i)
            {
                for (long j = 0; j < m_nCols; ++j)
                {
                    ss.Append(At(i, j)).Append("  ");
                }
                ss.AppendLine();
            }

            return ss.ToString();
        }

        //---------------------------------------------------------------------------------------------
        public long GetRows()
        {
            return m_nRows;
        }

        //---------------------------------------------------------------------------------------------
        public long GetCols()
        {
            return m_nCols;
        }
    
        //---------------------------------------------------------------------------------------------
        internal long GetDim()
        {
            if (m_nCols == 1)
            {
                return (m_nRows == 1) ? 0 : 1;
            }
            else
                return 2;
        
        }
	    public ref IValue At(long nRow, long nCol = 0)
        {
            long i;
            if (m_eStorageScheme == EMatrixStorageScheme.mssROWS_FIRST)
            {
                i = nRow * m_nCols + nCol;
            }
            else
            {
                i = nCol * m_nRows + nRow;
            }

            Global.MUP_VERIFY(() => i < (long)m_vData.Length);
            return ref m_vData[i];
        }

        //---------------------------------------------------------------------------------------------
        public IValue GetData()
        {
            Global.MUP_VERIFY(() => m_vData.Length > 0);
            return m_vData[0];
        }

        public void SetStorageScheme(EMatrixStorageScheme eScheme)
        {
            m_eStorageScheme = eScheme;
        }

        //---------------------------------------------------------------------------------------------
        public EMatrixStorageScheme GetStorageScheme()
        {
            return m_eStorageScheme;
        }

        //---------------------------------------------------------------------------------------------
        public Matrix Transpose()
        {
            if (GetDim() == 0)
                return this;

            m_eStorageScheme = (m_eStorageScheme == EMatrixStorageScheme.mssROWS_FIRST) ? EMatrixStorageScheme.mssCOLS_FIRST : EMatrixStorageScheme.mssROWS_FIRST;
            var tmp = m_nRows;
            m_nRows = m_nCols;
            m_nCols = tmp;

            return this;
        }

        //---------------------------------------------------------------------------------------------
        public void Fill(IValue v)
        {
            for (long i = 0; i < m_vData.Length; i++)
            {
                m_vData[i] = v;
            }
        }

        void Assign(Matrix m)
        {
            m_nCols = m.m_nCols;
            m_nRows = m.m_nRows;
            m_eStorageScheme = m.m_eStorageScheme;
            m_vData = m.m_vData.Select(v1 => (IValue)v1.Clone()).ToArray();
        }

        public Matrix CastTo(char t)
        {
            for (int i = 0; i < m_vData.Length; i++)
            {
                if (t == 'i')
                    m_vData[i] = m_vData[i].AsInteger();
                else if (t == 'f')
                    m_vData[i] = m_vData[i].AsFloat();
            }
            return this;
        }
        public long Length() => m_vData.LongLength;

        private long m_nRows;
	    private long m_nCols;
	    private EMatrixStorageScheme m_eStorageScheme;
	    protected internal IValue[] m_vData;
    }

    public enum EMatrixStorageScheme
    {
        mssROWS_FIRST,
        mssCOLS_FIRST
     }
}
