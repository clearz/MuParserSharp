using System;
using System.Numerics;
using System.Text;
using MuParserSharp.Parser;
using MuParserSharp.Util;

namespace MuParserSharp.Framework
{
    
    public abstract class IValue : IToken, IEquatable<IValue>
    {
        protected IValue(string a_sIdent = "") : base(ECmdCode.cmVAL, a_sIdent){}
        
        public bool Equals(IValue val2)
        {

            char type1 = GetValueType(), type2 = val2.GetValueType();
            if (type1 == type2)
            {
                switch (GetValueType())
                {
                    case 'i': return GetInteger() == val2.GetInteger();
                    case 'f': return GetFloat() == val2.GetFloat();
                    case 'c': return GetChar() == val2.GetChar();
                    case 'z': return GetComplex() == val2.GetComplex();
                    case 's': return GetString() == val2.GetString();
                    case 'b': return GetBool() == val2.GetBool();
                    case 'v': return false;
                    case 'm': if (GetRows() != val2.GetRows() || GetCols() != val2.GetCols())
                        {
                            return false;
                        }
                        else
                        {
                            for (int i = 0; i < GetRows(); ++i)
                            {
                                if (this.At(i) != val2.At(i))
                                return false;
                            }

                            return true;
                        }
                    default:
                        var err = new ErrorContext();
                        err.Errc = EErrorCodes.ecINTERNAL_ERROR;
                        err.Pos = -1;
                        err.Type1 = type1;
                        err.Type2 = type2;
                        throw new ParserError(err);
                } // switch this type
            }
            else if(this.IsScalar() && val2.IsScalar())
                return this.AsFloat() == val2.AsFloat();

            return false;
        }
         
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IValue) obj);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }


        public override ICallback AsICallback() => null;

        public override IValue AsIValue() => this;
        
        public abstract Value AsValue();
        public abstract Complex AsComplex();
        public  string TidyString()
        {
            var rs = ToString();
            switch (GetValueType())
            {
                case 'c':
                    return $"'{rs}'";
                case 's':
                    return $"\"{rs}\"";
                default:
                    return rs;
            }
            
        }

        public override string ToString()
        {

            var ss = new StringBuilder();
            switch (GetValueType())
            {
                case 'm':
                    Matrix arr = GetArray();
                    if (arr.GetRows() > 1)
                        ss.Append('{');

                    for (int i = 0; i < arr.GetRows(); ++i)
                    {
                        if (arr.GetCols() > 1)
                            ss.Append('{');

                        for (int j = 0; j < arr.GetCols(); ++j)
                        {
                            ss.Append(arr.At(i, j).TidyString());
                            if (j != arr.GetCols() - 1)
                                ss.Append(", ");
                        }

                        if (arr.GetCols() > 1)
                            ss.Append('}');

                        if (i != arr.GetRows() - 1)
                            ss.Append(';');
                    }

                    if (arr.GetRows() > 1)
                        ss.Append('}');
                    break;
                case 'z':
                    {
                        double re = GetReal(),
                            im = GetImag();

                        // realteil nicht ausgeben, wenn es eine rein imaginäre Zahl ist
                        if (im == 0 || re != 0 || (im == 0 && re == 0))
                            ss.Append(re);

                        if (im != 0)
                        {
                            if (im > 0 && re != 0)
                                ss.Append('+');

                            if (im != 1)
                                ss.Append(im);

                            ss.Append('i');
                        }
                    }
                    break;
                case 'i':
                    ss.Append(GetInteger()); break;
                case 'f':
                    ss.Append(GetFloat()); break;
                case 'b':
                    ss.Append(GetBool()); break;
                case 'c':
                    ss.Append(GetChar()); break;
                case 's':
                    ss.Append(GetString()); break;
                case 'v':
                    ss.Append("void"); break;
                default:
                    ss.Append("internal error: unknown value type."); break;
            }

            return ss.ToString();
        }
        public static bool operator ==(IValue v1, IValue v2)
        {
            if ((object) v1 == null && (object) v2 == null) return true;
            if ((object) v1 == null || (object) v2 == null) return false;
            return v1.Equals(v2);
        }
        
        public static bool operator !=(IValue v1, IValue v2)
        {
            return !(v1 == v2);
        }
        internal override string AsciiDump()
        {
            var sb = new StringBuilder();
            switch (GetValueType())
            {
                case 'i':
                    sb.Append(GetInteger());
                    break;
                case 'f':
                    sb.Append(GetFloat());
                    break;
                case 'm':
                    sb.Append("(matrix)");
                    break;
                case 's':
                    sb.Append("\"").Append(GetString()).Append("\"");
                    break;
            }
            sb.Append(IsFlagSet(EFlags.flVOLATILE) ? ";" : "; not").Append(" volitile ]");
            return this.Dump("pos", GetExprPos(), "id", $"\"{GetIdent()}\"", "type", $"'{GetValueType()}'", "val", sb.ToString());
        }
        public static bool operator <(IValue v1, IValue v2){
            if ((object)v1 == null && (object)v2 == null) return true;
            if ((object)v1 == null || (object)v2 == null) return false;
            return !(v1.Equals(v2) || v1 > v2);
        }
        
        public static bool operator >(IValue v1, IValue v2){
            char type1 = v1.GetValueType();
            char type2 = v2.GetValueType();

            if (type1 == type2)
            {
                switch (v1.GetValueType())
                {
                    case 's': return String.Compare(v1.GetString(), v2.GetString(), StringComparison.Ordinal) > 0;
                    case 'i': return v1.GetInteger() > v2.GetInteger();
                    case 'f': return v1.GetFloat() > v2.GetFloat();
                    case 'c': return v1.GetChar() > v2.GetChar();
                    case 'z': return v1.AsFloat() > v2.AsFloat();
                    case 'b': return v1.GetBool().CompareTo(v2.GetBool()) > 0;

                    default:
                        var err = new ErrorContext();
                        err.Errc = EErrorCodes.ecINTERNAL_ERROR;
                        err.Pos = -1;
                        err.Type1 = type1;
                        err.Type2 = type2;
                        throw new ParserError(err);
                } // switch this type
            }
            else if (v1.IsScalar() && v2.IsScalar())
            {
                return v1.AsFloat() > v2.AsFloat();
            }
            else
            {
                var err = new ErrorContext();
                err.Errc = EErrorCodes.ecTYPE_CONFLICT_FUN;
                err.Arg = (type1 != 'f' && type1 != 'i') ? 1 : 2;
                err.Type1 = type2;
                err.Type2 = type1;
                throw new ParserError(err);
            }
        }
        
        public static bool operator >=(IValue v1, IValue v2)
        {
            if ((object) v1 == null && (object) v2 == null) return true;
            if ((object) v1 == null || (object) v2 == null) return false;
            return v1.Equals(v2) || v1 > v2;
        }
        
        public static bool operator <=(IValue v1, IValue v2)
        {
            if ((object) v1 == null && (object) v2 == null) return true;
            if ((object) v1 == null || (object) v2 == null) return false;
            return !(v1 > v2);
        }


        //---------------------------------------------------------------------------
        public abstract override IToken Clone();

        public abstract IValue Assign(long val);
        public abstract IValue Assign(double val);
        public abstract IValue Assign(string val);
        public abstract IValue Assign(char val);
        public abstract IValue Assign(bool val);
        public abstract IValue Assign(Complex val);
        public abstract IValue Assign(Matrix val);
        public abstract IValue Assign(Value val);
        public abstract IValue Assign(IValue val);


        public abstract ref IValue At(int nRow, int nCol = 0);
        public abstract ref IValue At(IValue nRows, IValue nCols);
        public abstract long GetInteger();
        public abstract double GetFloat();
        public abstract long AsInteger();
        public abstract double AsFloat();
        public abstract bool GetBool();
        public abstract double GetReal();
        public abstract double GetImag();
        public abstract Complex GetComplex(bool assert = true);
        public abstract string GetString();
        public abstract Matrix GetArray();
        public abstract char GetChar();
        public abstract char GetValueType();
        public abstract long GetRows();
        public abstract long GetCols();

        public static implicit operator IValue(long i) => new Value(i);
        public static implicit operator IValue(char i) => new Value(i);
        public static implicit operator IValue(bool i) => new Value(i);
        public static implicit operator IValue(double i) => new Value(i);
        public static implicit operator IValue(string i) => new Value(i);
        public static implicit operator IValue(Complex i) => new Value(i);
        public static implicit operator IValue(Matrix i) => new Value(i);


        public static explicit operator long(IValue v) => v.GetInteger();
        public static explicit operator double(IValue v) => v.GetFloat();
        public static explicit operator string(IValue v) => v.GetString();
        public static explicit operator bool(IValue v) => v.GetBool();
        public static explicit operator Complex(IValue v) => v.GetComplex();
        public static explicit operator Matrix(IValue v) => v.GetArray();
        public static explicit operator char(IValue v) => v.GetChar();

        //---------------------------------------------------------------------------
        /*  Returns the dimension of the value represented by a value object.
            
            The value represents the dimension of the object. Possible value are:
            <ul>
            <li>0 - scalar</li>
            <li>1 - vector</li>
            <li>2 - matrix</li>
            </ul>
        */
        public long GetDim()
        {
            return (IsMatrix()) ? GetArray().GetDim() : 0;
        }
        //---------------------------------------------------------------------------
        /*  Returns true if the type is either floating point or interger.
        */
        public bool IsNonComplexScalar()
        {
            char t = GetValueType();
            return t=='f' || t=='i';
        }

        //---------------------------------------------------------------------------
        /*  Returns true if the type is not a vector and not a string.
        */
        public bool IsScalar()
        {
            char t = GetValueType();
            return t=='f' || t=='i' || t=='z';
        }
        //---------------------------------------------------------------------------
        /*  Returns true if this value is a noncomplex integer.
        */
        public bool IsInteger()
        {
            char t = GetValueType();
            // checking the type is is insufficient. The integer could be disguised
            // as a float or a complex value
            return t=='i';//' || (t == 'z' && GetImag() == 0 || t == 'f') && GetFloat() == (int)GetFloat()
        }
        //---------------------------------------------------------------------------
        /*  Returns true if this value is an array.
        */  
        public bool IsMatrix() 
        {
            return GetValueType() == 'm';  
        }

        public abstract bool IsScalarMatrix();

        //---------------------------------------------------------------------------
        /*  Returns true if this value is a complex value.
        */
        public bool IsComplex()
        {
            return GetValueType() == 'z' && GetImag() !=0;
        }

        //---------------------------------------------------------------------------
        /*  Returns true if this value is a string value.
        */
        public bool IsString() 
        {
            return GetValueType() == 's';  
        }

        public static IValue operator +(IValue v1, IValue v2)
        {
            
            if (v1.IsComplex() && v2.IsComplex())
            {
                return new Value(v1.GetComplex() + v2.GetComplex());
            }

            else if (v1.IsComplex() && v2.IsNonComplexScalar())
            {
                return new Value(v1.GetComplex() + v2.AsFloat());
            }

            else if (v1.IsNonComplexScalar() && v2.IsComplex())
            {
                return new Value(v1.AsFloat() + v2.GetComplex());
            }
            else if (v1.IsScalar() && v2.IsScalar())
            {
                if (v1.IsInteger() && v2.IsInteger())
                    return new Value(v1.AsInteger() + v2.AsInteger());

                return new Value(v1.AsFloat() + v2.AsFloat());
            }
            else if (v1.IsMatrix() && v2.IsMatrix())
            {
                Matrix res = (Matrix)(v1.GetArray() + v2.GetArray());
                return new Value(res);
            }
            else if (v1.IsString() && v2.IsString())
            {
                return new Value(v1.GetString() + v2.GetString());
            }

            throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, -1, "+", v1.GetValueType(), v2.GetValueType(), 2));
        }

        public static IValue operator -(IValue v1, IValue v2)
        {
            if (v1.IsComplex() && v2.IsComplex())
            {
                return new Value(v1.GetComplex() - v2.GetComplex());
            }

            else if (v1.IsComplex() && v2.IsNonComplexScalar())
            {
                return new Value(v1.GetComplex() - v2.AsFloat());
            }

            else if (v1.IsNonComplexScalar() && v2.IsComplex())
            {
                return new Value(v1.AsFloat() - v2.GetComplex());
            }
            else if (v1.IsScalar() && v2.IsScalar())
            {
                if (v1.IsInteger() && v2.IsInteger())
                    return new Value(v1.AsInteger() - v2.AsInteger());

                return new Value(v1.AsFloat() + v2.AsFloat());
            }
            else if (v1.IsMatrix() && v2.IsMatrix())
            {
                Matrix res = v1.GetArray() - v2.GetArray();
                return new Value(res);
            }

            throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, -1, "-", v1.GetValueType(), v2.GetValueType(), 2));
        }

        public static IValue operator *(IValue v1, IValue v2)
        {
            if (v1.IsScalar() && v2.IsScalar())
            {
                if (v1.IsInteger() && v2.IsInteger())
                    return new Value(v1.AsInteger() * v2.AsInteger());

                return new Value(v1.AsFloat() * v2.AsFloat());
            }
            else if (v1.IsComplex() && v2.IsComplex())
            {
                return new Value(v1.GetComplex() * v2.GetComplex());
            }
            else if (v1.IsMatrix() && v2.IsMatrix())
            {
                Matrix res = (Matrix)(v1.GetArray() * v2.GetArray());
                return new Value(res);
            }

            else if (v1.IsMatrix() && v2.IsScalar())
            {
                Matrix res = (Matrix)(v1.GetArray() * v2);
                return new Value(res);
            }

            else if (v1.IsScalar() && v2.IsMatrix())
            {
                Matrix res = (Matrix)(v2.GetArray() * v1);
                return new Value(res);
            }

            throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, -1, "*", v1.GetValueType(), v2.GetValueType(), 2));
        }

        public static IValue operator /(IValue v1, IValue v2)
        {
            if (v1.IsScalar() && v2.IsScalar())
            {
                if (v1.IsInteger() && v2.IsInteger())
                    return new Value(v1.AsInteger() / v2.AsInteger());

                return new Value(v1.AsFloat() / v2.AsFloat());
            }
            else if (v1.IsComplex() && v2.IsComplex())
            {
                return new Value(v1.GetComplex() / v2.GetComplex());
            }

            throw new ParserError(new ErrorContext(EErrorCodes.ecTYPE_CONFLICT_FUN, -1, "/", v1.GetValueType(), v2.GetValueType(), 2));
        }

        public abstract bool IsVariable();
        
    }
}
