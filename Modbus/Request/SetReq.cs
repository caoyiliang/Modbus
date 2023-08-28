using TopPortLib.Interfaces;
using Utils;

namespace Modbus.Request
{
    class SetReq : IByteStream
    {
        private readonly byte _deviceAddress;
        private readonly ushort _registerAddress;
        private readonly byte[] _data;

        public SetReq(byte deviceAddress, ushort registerAddress, byte[] data)
        {
            _deviceAddress = deviceAddress;
            _registerAddress = registerAddress;
            _data = data;
        }

        //设备地址+功能码+寄存器地址+寄存器个数+字节数+数据+crc
        public byte[] ToBytes()
        {
            var dataLength = BitConverter.GetBytes(_data.Length / 2);
            var byteCount = (byte)_data.Length;
            var startAddr = StringByteUtils.GetBytes(_registerAddress, true);
            var head = new byte[]
            {
                _deviceAddress,
                0x10,
                startAddr[0],
                startAddr[1],
                dataLength[1],
                dataLength[0],
                byteCount
            };
            var temp = StringByteUtils.ComibeByteArray(head, _data);
            var crc = CRC.Crc16(temp, temp.Length);
            return StringByteUtils.ComibeByteArray(temp, crc);
        }
    }
}
