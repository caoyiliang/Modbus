namespace Modbus.Parameter
{
    public class BlockInfo
    {
        public ushort StartRegisterAddress { get; set; }
        public ushort EndRegisterAddress { get; set; }
        public List<ChannelInfo> ChannelInfos { get; set; }
        public BlockInfo() => ChannelInfos = [];
    }
}
