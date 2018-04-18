using Microsoft.VisualStudio.TestTools.UnitTesting;
using MuParserSharp.Parser;

namespace MuParserSharp.Tests
{
    [TestClass]
    public class VectorTests
    {

        //[DataRow("va==9", EErrorCodes.ecEVAL);
        //[DataRow("va==a", EErrorCodes.ecEVAL);
        //[DataRow("a==va", EErrorCodes.ecEVAL);
        //[DataRow("9==va", EErrorCodes.ecEVAL);
        [TestMethod]
        [DataRow("10+2*va", EErrorCodes.ecEVAL)]   // fail: number + vector
        [DataRow("10+va*2", EErrorCodes.ecEVAL)]   // fail: number + vector
        public void test_vector_operations(string s, EErrorCodes e) => Tester.ThrowTest(s, e);

        [TestMethod]
        [DataRow("va+vc", EErrorCodes.ecMATRIX_DIMENSION_MISMATCH)]   // fail: vectors of different size
        [DataRow("va-vc", EErrorCodes.ecMATRIX_DIMENSION_MISMATCH)]   // fail: vectors of different size
        [DataRow("va*vc", EErrorCodes.ecMATRIX_DIMENSION_MISMATCH)]   // fail: vectors of different size
        [DataRow("va*vb", EErrorCodes.ecMATRIX_DIMENSION_MISMATCH)]   // fail: matrix dimension mismatch
        [DataRow("va*va", EErrorCodes.ecMATRIX_DIMENSION_MISMATCH)]   // fail: matrix dimension mismatch
        [DataRow("(va*vb)*b", EErrorCodes.ecMATRIX_DIMENSION_MISMATCH)]   // fail: matrix dimension mismatch
        public void test_vector_dimension_mismatch(string s, EErrorCodes e) => Tester.ThrowTest(s, e);


        [TestMethod]
        [DataRow("va[-1]", EErrorCodes.ecINDEX_OUT_OF_BOUNDS)] // fail: negative value used as an index
        [DataRow("va[c]", EErrorCodes.ecINDEX_OUT_OF_BOUNDS)]
        [DataRow("va[(3)]", EErrorCodes.ecINDEX_OUT_OF_BOUNDS)]
        [DataRow("a[1]", EErrorCodes.ecINDEX_OUT_OF_BOUNDS)] // indexing a scalar is ok, but this index is out of bounds (0 would be ok...)
        public void test_vector_index_bounds(string s, EErrorCodes e) => Tester.ThrowTest(s, e);


        [TestMethod]
        [DataRow("va[1.23]", EErrorCodes.ecTYPE_CONFLICT_IDX, -1)]   // fail: float value used as index
        [DataRow("va[sin(8)]", EErrorCodes.ecTYPE_CONFLICT_IDX, -1)]   // fail: float value used as index
        public void test_vector_index_type_conflict(string s, EErrorCodes e, int i = -1) => Tester.ThrowTest(s, e, i);

        [TestMethod]
        [DataRow("va[1", EErrorCodes.ecMISSING_SQR_BRACKET)]
        [DataRow("va[1]]", EErrorCodes.ecUNEXPECTED_SQR_BRACKET)]
        public void test_vector_malformed_brackets(string s, EErrorCodes e) => Tester.ThrowTest(s, e);

        [TestMethod]
        public void test_vector_addition()
        {
            var v = new Value(3, 0);
            v.At(0) = 5;
            v.At(1) = 5;
            v.At(2) = 5;
            Tester.EqnTest("va+vb", v, true);
            v.At(2) = 6;
            Tester.EqnTest("va+vb", v, false);
            v.At(0) = -1;
            v.At(1) = -2;
            v.At(2) = -3;
            Tester.EqnTest("-va", v, true);

        }

        [TestMethod]
        [DataRow("sizeof(va+vb)", 3, true)]
        [DataRow("sizeof(va-vb)", 3, true)]

        [DataRow("va==vb", false, true)]
        [DataRow("va!=vb", true, true)]
        //[DataRow("va<vb",  false, true)]
        //[DataRow("va>vb",  true, true)]
        //[DataRow("va<=vb", false, true)]
        //[DataRow("va>=vb", true, true)]

        [DataRow("vb[va[0]]", 3, true)]
        [DataRow("m1[0,0]+m1[1,1]+m1[2,2]", 3, true)]
        [DataRow("vb[m1[0,0]]", 3, true)]

        [DataRow("va[(int)sin(8)+1]", 2, true)]
        [DataRow("m1[0,0]=2", 2, true)]
        [DataRow("m1[1,1]=2", 2, true)]
        [DataRow("m1[2,2]=2", 2, true)]
        [DataRow("va[0]=12.3", 12.3, true)]
        [DataRow("va[1]=12.3", 12.3, true)]
        [DataRow("va[2]=12.3", 12.3, true)]

        [DataRow("va[0]", 1, true)]
        [DataRow("va[1]", 2, true)]
        [DataRow("va[2]", 3, true)]
        [DataRow("(va[2])", 3, true)]
        [DataRow("va[a]", 2, true)]
        [DataRow("(va[a])", 2, true)]
        [DataRow("va[b]", 3, true)]
        [DataRow("va[(2)]", 3, true)]
        [DataRow("va[-(-2)]", 3, true)]
        [DataRow("(va[(2)])", 3, true)]
        [DataRow("(va[-(-2)])", 3, true)]
        [DataRow("va[1+1]", 3, true)]

        [DataRow("va[2]+4", 7, true)]
        [DataRow("4+va[2]", 7, true)]
        [DataRow("va[2]*4", 12, true)]
        [DataRow("4*va[2]", 12, true)]
        [DataRow("va[2]+a", 4, true)]
        [DataRow("a+va[2]", 4, true)]
        [DataRow("va[2]*b", 6, true)]
        [DataRow("b*va[2]", 6, true)]
        public void test_vector_indexing(string s1, object v1, bool t) => Tester.EqnTest(s1, v1, t);


    }
}