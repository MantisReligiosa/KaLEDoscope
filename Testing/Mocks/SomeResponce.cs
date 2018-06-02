using CommandProcessing;
using System.Collections.Generic;

namespace Testing
{
    internal class SomeResponce : Responce<List<int>>
    {
        public override byte ResponceID => 0xef;

        public override List<int> Cast()
        {
            return new List<int>();
        }
    }
}
