using Modbus.Parameter;
using ProtocolInterface;

namespace Modbus
{
    public static class BlockHelper
    {
        public static List<ChannelInfo> CreateBlock(ushort startRegisterAddress, int Count, RegisterValueType valueType)
        {
            var channelInfos = new List<ChannelInfo>();
            for (int i = 0; i < Count; i++)
            {
                if (valueType == RegisterValueType.Int32 || valueType == RegisterValueType.UInt32 || valueType == RegisterValueType.Float)
                {
                    channelInfos.Add(new() { RegisterAddress = (ushort)(startRegisterAddress + 2 * i), ValueType = valueType });
                }
                else
                {
                    channelInfos.Add(new() { RegisterAddress = (ushort)(startRegisterAddress + i), ValueType = valueType });
                }
            }
            return channelInfos;
        }
    }
}
