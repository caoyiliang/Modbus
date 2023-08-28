namespace Modbus.Parameter
{
    public class SetBlockInfo
    {
        public ushort RegisterAddress { get; set; }
        public byte[]? Data { get; set; }
    }
}
