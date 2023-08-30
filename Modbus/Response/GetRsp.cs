using Modbus.Parameter;
using ProtocolInterface;

namespace Modbus.Response
{
    class GetRsp
    {
        public List<ChannelRsp> RecData { get; set; }

        public GetRsp(byte[] rspBytes, BlockInfo blockInfo, bool isHighByteBefore = true)
        {
            if (rspBytes.Length < 3)
            {
                throw new Exception("长度不够");
            }
            var crc = Utils.CRC.Crc16(rspBytes, rspBytes.Length - 2);
            if (!(crc[0] == rspBytes[rspBytes.Length - 2] && crc[1] == rspBytes[rspBytes.Length - 1]))
            {
                throw new Exception("CRC校验失败");
            }
            RecData = new List<ChannelRsp>();
            foreach (var channelInfo in blockInfo.ChannelInfos)
            {
                var index = (channelInfo.RegisterAddress - blockInfo.StartRegisterAddress) * 2 + 3;
                var value = channelInfo.ValueType switch
                {
                    RegisterValueType.Float => Convert.ToDecimal(Utils.StringByteUtils.ToSingle(rspBytes, index, isHighByteBefore)),
                    RegisterValueType.UInt16 => Convert.ToDecimal(Utils.StringByteUtils.ToUInt16(rspBytes, index, isHighByteBefore)),
                    RegisterValueType.UInt32 => Convert.ToDecimal(Utils.StringByteUtils.ToUInt32(rspBytes, index, isHighByteBefore)),
                    RegisterValueType.bIntA => rspBytes[index],
                    RegisterValueType.bIntB => rspBytes[index + 1],
                    _ => throw new ArgumentException("RegisterValueType Error"),
                };
                RecData.Add(new ChannelRsp { ChannelId = channelInfo.ChannelId, Value = value });
            }
        }
    }
}
