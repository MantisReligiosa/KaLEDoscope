namespace CommandProcessing.Responces
{
    public class AcceptanceResponce : Responce<object>
    {
        public override byte ResponceID => 0xf0;

        public override object Cast()
        {
            return new object();
        }
    }
}
