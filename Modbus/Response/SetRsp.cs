namespace Modbus.Response
{
    internal class SetRsp
    {
        public SetRsp(byte[] rspBytes)
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
        }
    }
}
