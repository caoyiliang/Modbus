using Modbus.Parameter;
using TopPortLib.Interfaces;
using Utils;

namespace Modbus.Request
{
    class GetReq : IByteStream
    {
        private readonly byte _deviceAddress;
        private readonly ushort _registerAddress;
        private readonly ushort _registerCount;
        private readonly bool _isHighByteBefore;

        public GetReq(byte deviceAddress, BlockInfo blockInfo, bool isHighByteBefore = true)
        {
            _deviceAddress = deviceAddress;
            _registerAddress = blockInfo.StartRegisterAddress;
            _registerCount = (ushort)(blockInfo.EndRegisterAddress - blockInfo.StartRegisterAddress + 1);
            _isHighByteBefore = isHighByteBefore;
        }

        //设备地址+功能码+寄存器地址+寄存器个数+crc
        public byte[] ToBytes()
        {
            var bytes = StringByteUtils.ComibeByteArray(new byte[] { _deviceAddress, 0x03 }, StringByteUtils.GetBytes(_registerAddress, _isHighByteBefore), StringByteUtils.GetBytes(_registerCount, _isHighByteBefore));
            var crc = CRC.Crc16(bytes, bytes.Length);
            return StringByteUtils.ComibeByteArray(bytes, crc);
        }
    }
}
