using System;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using MuParserSharp.Framework;
using MuParserSharp.Util;

namespace MuParserSharp.Parser
{


    public class Value : IValue
    {
        private IValue self;

        internal Value()
        {
            self = this;
        }

        public Value(long val) : this()
        {
            Assign(val);
        }

        public Value(char val) : this()
        {
            Assign(val);
        }

        public Value(double val) : this()
        {
            Assign(val);
        }

        public Value(bool val) : this()
        {
            Assign(val);
        }

        public Value(string val) : this()
        {
            Assign(val);
        }

        public Value(Complex val) : this()
        {
            Assign(val);
        }

        public Value(Matrix val) : this()
        {
            Assign(val);
        }

        public Value(long m, double v) : this()
        {
            m_cType = 'm';
            matrixVal = new Matrix(m, v);
        }

        public Value(long m, long n, double v) : this()
        {
            m_cType = 'm';
            matrixVal = new Matrix(m, n, v);
        }

        public Value(long m, long v) : this()
        {
            m_cType = 'm';
            matrixVal = new Matrix(m, v);
        }

        public Value(long m, long n, long v) : this()
        {
            m_cType = 'm';
            matrixVal = new Matrix(m, n, v);
        }

        public IValue this[int m, int n = 0] {
            get => self.At(m, n);
            set => self.At(m, n).Assign(value);
        }


        public Value(Value val) : this()
        {
            Assign(val);
        }

        public Value(IValue val) : this()
        {
            Assign(val);
        }

        public override IToken Clone() => new Value(self);


        public void BindToCache(ValueCache pCache)
        {
            m_pCache = pCache;
        }

        public override Value AsValue() => (Value)self;
        public override Complex AsComplex()
        {
            switch (m_cType)
            {
                case 'i':
                case 'c':
                    return new Complex((double)v.intVal, 0);
                case 'f':
                    return new Complex(v.floatVal, 0);
                case 'z':
                    return v.complexVal;
                case 'b':
                    return new Complex(Convert.ToInt32(v.boolVal), 0);
                default:
                    var err = new ErrorContext();
                    err.Errc = EErrorCodes.ecINVALID_TYPECAST;
                    err.Type1 = GetValueType();
                    err.Type2 = 'z';
                    throw new ParserError(err);
            }
        }

        public override IValue Assign(long val)
        {
            m_cType = 'i';
            v.intVal = val;

            return self;
        }

        public override IValue Assign(char val)
        {
            m_cType = 'c';
            v.charVal = val;
            return self;
        }

        public override IValue Assign(double val)
        {
            //m_cType = val == (long)val ? 'i' : 'f';
            //if (m_cType == 'i')
            //    v.intVal = (long)val;
            //else
            m_cType = 'f';
            v.floatVal = val;
            return self;
        }

        public override IValue Assign(string val)
        {
            m_cType = 's';
            stringVal = val;
            return self;
        }

        public override IValue Assign(bool val)
        {
            m_cType = 'b';
            v.boolVal = val;
            return self;
        }

        public override IValue Assign(Complex val)
        {
            m_cType = val.Imaginary == 0 ? (val.Real == (long)val.Real) ? 'i' : 'f' : 'z';
            if (m_cType == 'i')
                v.intVal = (long)val.Real;
            else
                v.complexVal = val;
            return self;
        }

        public override IValue Assign(Matrix val)
        {
            if (val.m_vData.Length == 1)
                return Assign(val.m_vData[0]);
            m_cType = 'm';
            matrixVal = val;
            return self;
        }

        public override long AsInteger()
        {
            switch (m_cType)
            {
                case 'i':
                    return v.intVal;
                case 'c':
                    return (long)v.charVal;
                case 'f':
                case 'z':
                    return (long)v.floatVal;
                case 'b':
                    return Convert.ToInt64(v.boolVal);
                default:
                    var err = new ErrorContext();
                    err.Errc = EErrorCodes.ecINVALID_TYPECAST;
                    err.Type1 = GetValueType();
                    err.Type2 = 'i';
                    throw new ParserError(err);
            }
        }

        public override double AsFloat()
        {
            switch (GetValueType())
            {
                case 'i':
                    return (double)v.intVal;
                case 'c':
                    return (double)v.charVal;
                case 'z':
                case 'f':
                    return v.floatVal;
                case 'b':
                    return Convert.ToDouble(v.boolVal);
                default:
                    var err = new ErrorContext();
                    err.Errc = EErrorCodes.ecINVALID_TYPECAST;
                    err.Type1 = GetValueType();
                    err.Type2 = 'i';
                    throw new ParserError(err);
            }
        }
        public override IValue Assign(IValue val)
        {
            switch (val.GetValueType())
            {
                case 'i':
                case 'c':
                case 'f':
                case 'b':
                case 'z':
                    v.complexVal = val.GetComplex(false);
                    break;
                case 's':
                    stringVal = val.GetString();
                    break;
                case 'm':
                    matrixVal = val.GetArray();
                    break;
                case 'v': break;
                default:
                    throw new ParserError("INVALID TYPE CODE");
            }
            m_cType = val.GetValueType();
            return self;
        }

        public override IValue Assign(Value val)
        {

            Assign(val.AsIValue());
            return self;
        }

        public override ref IValue At(int nRow, int nCol = 0)
        {
            if (IsMatrix())
            {
                if (nRow >= matrixVal.GetRows() || nCol >= matrixVal.GetCols() || nRow < 0 || nCol < 0)
                    throw new ParserError(new ErrorContext(EErrorCodes.ecINDEX_OUT_OF_BOUNDS, -1, GetIdent()));

                return ref matrixVal.At(nRow, nCol);
            }
            else if (nRow == 0 && nCol == 0)
            {
                return ref self;
            }
            else
                throw new ParserError(new ErrorContext(EErrorCodes.ecINDEX_OUT_OF_BOUNDS));
        }

        public override ref IValue At(IValue row, IValue col)
        {
            if (!row.IsInteger() || !col.IsInteger())
            {
                var errc = new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_IDX, GetExprPos());
                errc.Type1 = (!row.IsInteger()) ? row.GetValueType() : col.GetValueType();
                errc.Type2 = 'i';
                throw new ParserError(errc);
            }

            int nRow = (int)row.GetInteger(),
                nCol = (int)col.GetInteger();
            return ref At(nRow, nCol);
        }

        public override long GetInteger()
        {
            Global.MUP_VERIFY(m_cType == 'i');
            return v.intVal;
        }

        public override double GetFloat()
        {
            Global.MUP_VERIFY(IsNonComplexScalar());
            return m_cType == 'f' ? v.floatVal : v.intVal;
        }

        public override bool GetBool()
        {
            Global.MUP_VERIFY(m_cType == 'b');
            return v.boolVal;
        }

        public override double GetReal()
        {
            if (IsNonComplexScalar()) return GetFloat();
            Global.MUP_VERIFY(m_cType == 'z');
            return v.complexVal.Real;
        }

        public override double GetImag()
        {
            if (IsNonComplexScalar()) return 0;
            Global.MUP_VERIFY(m_cType == 'z');
            return v.complexVal.Imaginary;
        }

        public override Complex GetComplex(bool assert = true)
        {
            if (assert) Global.MUP_VERIFY(m_cType == 'z' || m_cType == 'f');
            return v.complexVal;
        }

        public override string GetString()
        {
            Global.MUP_VERIFY(m_cType == 's');
            return stringVal;
        }

        public override char GetChar()
        {
            Global.MUP_VERIFY(m_cType == 'c');
            return v.charVal;
        }

        public override Matrix GetArray()
        {
            Global.MUP_VERIFY(m_cType == 'm');
            return matrixVal;
        }

        public override char GetValueType()
        {
            return m_cType;
        }

        public override long GetRows()
        {
            return (m_cType != 'm') ? 1 : GetArray().GetRows();
        }

        public override long GetCols()
        {
            return (m_cType != 'm') ? 1 : GetArray().GetCols();
        }

        void CheckType(char a_cType)
        {
            if (m_cType != a_cType)
            {
                var err = new ErrorContext();
                err.Errc = EErrorCodes.ecTYPE_CONFLICT;
                err.Type1 = m_cType;
                err.Type2 = a_cType;

                if (GetIdent().Length > 0)
                {
                    err.Ident = GetIdent();
                }
                else
                {
                    err.Ident = self.ToString();
                }

                throw new ParserError(err);
            }


        }

        //---------------------------------------------------------------------------
        public void Reset()
        {
            v = default(ValueHandle);
            stringVal = null;
            matrixVal = null;

            m_cType = 'f';
            m_iFlags = EFlags.flNONE;
        }

        public override bool IsScalarMatrix()
        {
            if (m_cType != 'm') return false;
            Global.MUP_VERIFY(matrixVal != null);
            if (matrixVal.Length() > 0)
                return matrixVal.m_vData.All(v => v.IsScalar());
            Console.WriteLine("Maybe shouldn't be here");
            return false;
        }

        public override bool IsVariable() => false;

        public static implicit operator Value(int i)
        {
            return new Value(i);
        }
        public static implicit operator Value(char c)
        {
            return new Value(c);
        }
        public static implicit operator Value(long i)
        {
            return new Value(i);
        }
        public static implicit operator Value(double i)
        {
            return new Value(i);
        }
        public static implicit operator Value(bool i)
        {
            return new Value(i);
        }
        public static implicit operator Value(string i)
        {
            return new Value(i);
        }
        public static implicit operator Value(Complex i)
        {
            return new Value(i);
        }
        public static implicit operator Value(Matrix i)
        {
            return new Value(i);
        }

        internal override void Release()
        {
            m_pCache?.ReleaseToCache(ref self);
        }

        private Matrix matrixVal;
        private string stringVal;
        private ValueHandle v;

        char m_cType;  // A byte indicating the type os the represented value
        EFlags m_iFlags = EFlags.flNONE; // Additional flags
        ValueCache m_pCache; // Pointer to the Value Cache
    }

    [StructLayout(LayoutKind.Explicit, Size = 256, CharSet = CharSet.Unicode)]
    public unsafe struct ValueHandle
    {
        [FieldOffset(0)]
        public Complex complexVal;
        [FieldOffset(0)]
        public double floatVal;
        [FieldOffset(0)]
        public char charVal;
        [FieldOffset(0)]
        public bool boolVal;
        [FieldOffset(0)]
        public long intVal;
        [FieldOffset(0)]
        public IntPtr stringVal;
    }
}
