using System.Collections.Generic;
using MuParserSharp.Framework;

namespace MuParserSharp
{


    class token_buf_type : List<IToken>
    {
        public token_buf_type(IEnumerable<IToken> type) : base(type)
        {
        }
  }
}


