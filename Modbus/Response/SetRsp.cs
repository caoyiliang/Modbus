using Utils;

namespace Modbus.Response
{
    internal class SetRsp
    {
        public SetRsp(byte[] reqBytes, byte[] rspBytes, bool? IsHighByteBefore_MBAP = null)
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
        }
    }
}
