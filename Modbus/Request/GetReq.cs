using Modbus.Parameter;
using TopPortLib.Interfaces;
using Utils;

namespace Modbus.Request
{
    class GetReq(byte deviceAddress, BlockInfo blockInfo, bool isHighByteBefore = true) : IByteStream
    {
        private readonly ushort _registerAddress = (ushort)blockInfo.StartRegisterAddress!;
        private readonly ushort _registerCount = (ushort)(blockInfo.EndRegisterAddress - blockInfo.StartRegisterAddress + 1);

        //设备地址+功能码+寄存器地址+寄存器个数+crc
        public byte[] ToBytes()
        {
            var bytes = StringByteUtils.ComibeByteArray([deviceAddress, 0x03], StringByteUtils.GetBytes(_registerAddress, isHighByteBefore), StringByteUtils.GetBytes(_registerCount, isHighByteBefore));
            var crc = CRC.Crc16(bytes, bytes.Length);
            return StringByteUtils.ComibeByteArray(bytes, crc);
        }
    }
}
