using CommandProcessing.DTO;
using ServiceInterfaces;
using System;

namespace CommandProcessing.Responces
{
    public class IdentityResponce : Responce<Identity>
    {
        public override Identity Cast()
        {
            throw new NotImplementedException();
        }
    }
}
