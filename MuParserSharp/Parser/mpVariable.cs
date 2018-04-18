using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using MuParserSharp.Framework;
using MuParserSharp.Util;

namespace MuParserSharp.Parser
{

    
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class Variable : IValue
    {
        private IValue m_pVal;

        public Variable(IValue value)
        {
            m_pVal = value;
            AddFlags(EFlags.flVOLATILE);
        }

        public Variable(Variable obj)
        {
            Assign(obj);
            AddFlags(EFlags.flVOLATILE);
        }

        public override double AsFloat()
        {
            return m_pVal.AsFloat();
        }

        public override long AsInteger()
        {
            return m_pVal.AsInteger();
        }

        public override IValue Assign(long val)
        {
            return m_pVal.Assign(val);
        }

        public override IValue Assign(double val)
        {
            m_pVal.Assign(val);
            return this;
        }

        public override IValue Assign(string val)
        {
            m_pVal.Assign(val);
            return this;
        }

        public override IValue Assign(char val)
        {
            m_pVal.Assign(val);
            return this;
        }

        public override IValue Assign(bool val)
        {
            m_pVal.Assign(val);
            return this;
        }

        public override IValue Assign(Complex val)
        {
            m_pVal.Assign(val);
            return this;
        }

        public override IValue Assign(Matrix val)
        {
            Global.MUP_VERIFY(val != null);
            m_pVal.Assign(val);
            return this;
        }

        public override IValue Assign(IValue val)
        {
            Global.MUP_VERIFY(val != null);
            m_pVal.Assign(val);
            return this;
        }

        public override IValue Assign(Value val)
        {
            Global.MUP_VERIFY(val != null);
            m_pVal.Assign(val);
            return this;
        }

        public void Assign(Variable obj)
        {
            if (ReferenceEquals(this, obj)) return;
            if (ReferenceEquals(null, obj)) return;
            m_pVal = obj.m_pVal;
        }

        public override ref IValue At(int nRow, int nCol = 0) => ref m_pVal.At(nRow, nCol);

        public override ref IValue At(IValue row, IValue col)
        {
            try
            {
                return ref m_pVal.At(row, col);
            }
            catch (ParserError exc)
            {
                // add the identifier to the error context
                exc.GetContext().Ident = GetIdent();
                throw;
            }
        }

        public IValue GetPtr() => m_pVal;

        public override long GetInteger()
        {
            try
            {
                return m_pVal.GetInteger();
            }
            catch (ParserError exc)
            {
                exc.GetContext().Ident = GetIdent();
                throw;
            }
        }

        public override Matrix GetArray()
        {
            try
            {
                return m_pVal.GetArray();
            }
            catch (ParserError exc)
            {
                exc.GetContext().Ident = GetIdent();
                throw;
            }
        }

        public override char GetChar()
        {
            try
            {
                return m_pVal.GetChar();
            }
            catch (ParserError exc)
            {
                exc.GetContext().Ident = GetIdent();
                throw;
            }
        }

        public override bool GetBool()
        {
            try
            {
                return m_pVal.GetBool();
            }
            catch (ParserError exc)
            {
                exc.GetContext().Ident = GetIdent();
                throw;
            }
        }

        public override long GetCols()
        {
            try
            {
                return m_pVal.GetCols();
            }
            catch (ParserError exc)
            {
                exc.GetContext().Ident = GetIdent();
                throw;
            }
        }

        public override Complex GetComplex(bool assert = true)
        {
            try
            {
                return m_pVal.GetComplex(assert);
            }
            catch (ParserError exc)
            {
                exc.GetContext().Ident = GetIdent();
                throw;
            }
        }

        public override double GetFloat()
        {
            try
            {
                return m_pVal.GetFloat();
            }
            catch (ParserError exc)
            {
                exc.GetContext().Ident = GetIdent();
                throw;
            }
        }

        public override double GetReal()
        {
            try
            {
                return m_pVal.GetReal();
            }
            catch (ParserError exc)
            {
                exc.GetContext().Ident = GetIdent();
                throw;
            }
        }

        public override double GetImag()
        {
            try
            {
                return m_pVal.GetImag();
            }
            catch (ParserError exc)
            {
                exc.GetContext().Ident = GetIdent();
                throw;
            }
        }

        public override long GetRows()
        {
            try
            {
                return m_pVal.GetRows();
            }
            catch (ParserError exc)
            {
                exc.GetContext().Ident = GetIdent();
                throw;
            }
        }

        public override string GetString()
        {
            try
            {
                return m_pVal.GetString();
            }
            catch (ParserError exc)
            {
                exc.GetContext().Ident = GetIdent();
                throw;
            }
        }

        public void SetInteger(long b)
        {
            Global.MUP_VERIFY(m_pVal != null);
            m_pVal.Assign(b);
        }

        public void SetChar(char b)
        {
            Global.MUP_VERIFY(m_pVal != null);
            m_pVal.Assign(b);
        }
        public void SetFloat(double b)
        {
            Global.MUP_VERIFY(m_pVal != null);
            m_pVal.Assign(b);
        }
        public void SetString(string b)
        {
            Global.MUP_VERIFY(m_pVal != null);
            m_pVal.Assign(b);
        }
        public void SetBool(bool b)
        {
            Global.MUP_VERIFY(m_pVal != null);
            m_pVal.Assign(b);
        }

        public void SetComplex(Complex b)
        {
            Global.MUP_VERIFY(m_pVal != null);
            m_pVal.Assign(b);
        }

        public void SetArray(Matrix b)
        {
            Global.MUP_VERIFY(m_pVal != null);
            m_pVal.Assign(b);
        }
        public override char GetValueType() => m_pVal?.GetValueType() ?? 'v';
        public void Bind(IValue pValue) => m_pVal = pValue;
        public override bool IsScalarMatrix()
        {
            return m_pVal.IsScalarMatrix();
        }

        public override bool IsVariable() => true;

        public override IToken Clone() => new Variable(this);

        public override Value AsValue() => m_pVal.AsValue();
        public override Complex AsComplex()
        {
            return m_pVal.AsComplex();
        }

        internal override string AsciiDump()
        {
            return "VAR" + base.AsciiDump().Substring(3);
        }

        public static implicit operator Variable(long i) => new Variable(new Value(i));
        public static implicit operator Variable(float i) => new Variable(new Value(i));
        public static implicit operator Variable(bool i) => new Variable(new Value(i));
        public static implicit operator Variable(string i) => new Variable(new Value(i));
        public static implicit operator Variable(Complex i) => new Variable(new Value(i));
        public static implicit operator Variable(Matrix i) => new Variable(new Value(i));

        public static IValue operator +(Variable v, IValue val)
        {
            Global.MUP_VERIFY(v.m_pVal != null);
            return v.m_pVal + val;
        }
        public static IValue operator -(Variable v, IValue val)
        {
            Global.MUP_VERIFY(v.m_pVal != null);
            return v.m_pVal - val;
        }
        public static IValue operator *(Variable v, IValue val)
        {
            Global.MUP_VERIFY(v.m_pVal != null);
            return v.m_pVal * val;
        }
        public static IValue operator /(Variable v, IValue val)
        {
            Global.MUP_VERIFY(v.m_pVal != null);
            return v.m_pVal / val;
        }
}
}
