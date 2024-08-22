using Modbus.Parameter;
using ProtocolInterface;
using Utils;

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
            var crc = CRC.Crc16(rspBytes, rspBytes.Length - 2);
            if (!(crc[0] == rspBytes[rspBytes.Length - 2] && crc[1] == rspBytes[rspBytes.Length - 1]))
            {
                throw new Exception("CRC校验失败");
            }
            RecData = [];
            foreach (var channelInfo in blockInfo.ChannelInfos)
            {
                var index = (channelInfo.RegisterAddress - (ushort)blockInfo.StartRegisterAddress!) * 2 + 3;
                object value = channelInfo.ValueType switch
                {
                    RegisterValueType.Float => StringByteUtils.ToSingle(rspBytes, index, channelInfo.IsHighByteBefore ?? isHighByteBefore),
                    RegisterValueType.UInt16 => StringByteUtils.ToUInt16(rspBytes, index, channelInfo.IsHighByteBefore ?? isHighByteBefore),
                    RegisterValueType.UInt32 => StringByteUtils.ToUInt32(rspBytes, index, channelInfo.IsHighByteBefore ?? isHighByteBefore),
                    RegisterValueType.Int16 => StringByteUtils.ToInt16(rspBytes, index, channelInfo.IsHighByteBefore ?? isHighByteBefore),
                    RegisterValueType.Int32 => StringByteUtils.ToInt32(rspBytes, index, channelInfo.IsHighByteBefore ?? isHighByteBefore),
                    RegisterValueType.sbyteA => rspBytes[index],
                    RegisterValueType.sbyteB => rspBytes[index + 1],
                    RegisterValueType.String => GetArray(rspBytes, index, channelInfo.Count * 2),
                    _ => throw new ArgumentException("RegisterValueType Error"),
                };
                RecData.Add(new ChannelRsp { ChannelId = channelInfo.ChannelId, Value = value });
            }
        }

        private byte[] GetArray(byte[] rspBytes, int index, int count)
        {
            byte[] result = new byte[count];
            Array.Copy(rspBytes, index, result, 0, count);
            return result;
        }
    }
}
