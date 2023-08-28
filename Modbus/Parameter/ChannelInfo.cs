namespace Modbus.Parameter
{
    public class ChannelInfo
    {
        public int ChannelId { get; set; }
        public ushort RegisterAddress { get; set; }
        public RegisterValueType ValueType { get; set; }
    }
}