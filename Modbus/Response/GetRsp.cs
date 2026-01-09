using Modbus.Parameter;
using ProtocolInterface;
using Utils;

namespace Modbus.Response
{
    class GetRsp
    {
        public List<ChannelRsp> RecData { get; set; }

        public GetRsp(byte[] reqBytes, byte[] rspBytes, Block blockInfo, bool isHighByteBefore = true, bool? IsHighByteBefore_MBAP = null)
        {
            if (IsHighByteBefore_MBAP.HasValue)
            {
                if (rspBytes.Length < 6)
                {
                    throw new Exception("长度不够");
                }
                if (rspBytes.Length < StringByteUtils.ToInt16(rspBytes, 4, IsHighByteBefore_MBAP.Value) + 6)
                {
                    throw new Exception("数据长度不够");
                }
                if (reqBytes[0] != rspBytes[0] || reqBytes[1] != rspBytes[1])
                {
                    throw new Exception("返回事务ID不匹配");
                }
            }
            else
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
            }
            var data = rspBytes;
            if (IsHighByteBefore_MBAP.HasValue) data = data.Skip(6).ToArray();
            RecData = [];
            foreach (var channelInfo in blockInfo.Channels)
            {
                var index = (channelInfo.RegisterAddress - (ushort)blockInfo.StartRegisterAddress!) * 2 + 3;
                object value = channelInfo.ValueType switch
                {
                    RegisterValueType.Float => StringByteUtils.ToSingle(data, index, channelInfo.IsHighByteBefore ?? isHighByteBefore),
                    RegisterValueType.Double => StringByteUtils.ToDouble(data, index, channelInfo.IsHighByteBefore ?? isHighByteBefore),
                    RegisterValueType.UInt16 => StringByteUtils.ToUInt16(data, index, channelInfo.IsHighByteBefore ?? isHighByteBefore),
                    RegisterValueType.UInt32 => StringByteUtils.ToUInt32(data, index, channelInfo.IsHighByteBefore ?? isHighByteBefore),
                    RegisterValueType.Int16 => StringByteUtils.ToInt16(data, index, channelInfo.IsHighByteBefore ?? isHighByteBefore),
                    RegisterValueType.Int32 => StringByteUtils.ToInt32(data, index, channelInfo.IsHighByteBefore ?? isHighByteBefore),
                    RegisterValueType.sbyteA => data[index],
                    RegisterValueType.sbyteB => data[index + 1],
                    RegisterValueType.String => GetArray(data, index, channelInfo.Count * 2),
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
