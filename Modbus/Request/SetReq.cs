﻿using TopPortLib.Interfaces;
using Utils;

namespace Modbus.Request
{
    class SetReq(byte deviceAddress, ushort registerAddress, byte[] data, bool isHighByteBefore = true, bool IsHighByteBefore_MBAP = true, ushort? transactionId = null) : IByteStream
    {
        //设备地址+功能码+寄存器地址+寄存器个数+字节数+数据+crc
        public byte[] ToBytes()
        {
            var dataLength = BitConverter.GetBytes(data.Length / 2);
            var byteCount = (byte)data.Length;
            var startAddr = StringByteUtils.GetBytes(registerAddress, isHighByteBefore);
            var head = new byte[]
            {
                deviceAddress,
                0x10,
                startAddr[0],
                startAddr[1],
                dataLength[1],
                dataLength[0],
                byteCount
            };
            var temp = StringByteUtils.ComibeByteArray(head, data);

            if (transactionId.HasValue)
            {
                return StringByteUtils.ComibeByteArray(StringByteUtils.GetBytes(transactionId.Value, IsHighByteBefore_MBAP), [0x00, 0x00], StringByteUtils.GetBytes((ushort)temp.Length, IsHighByteBefore_MBAP), temp);
            }
            else
            {
                var crc = CRC.Crc16(temp, temp.Length);
                return StringByteUtils.ComibeByteArray(temp, crc);
            }
        }
    }
}
