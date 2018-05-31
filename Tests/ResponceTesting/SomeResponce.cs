using ServiceInterfaces;
using System.Collections.Generic;

namespace ResponceTesting
{
    internal class SomeResponce : Responce<List<int>>
    {
        public override List<int> Cast()
        {
            return new List<int>();
        }
    }
}
