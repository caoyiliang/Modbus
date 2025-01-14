using ProtocolInterface;
using System.ComponentModel;

namespace Modbus.Parameter
{
    /// <summary>
    /// 存储通道信息
    /// </summary>
    public class Channel
    {
        private ushort registerAddress;

        /// <summary>
        /// 存储通道ID
        /// </summary>
        public string? ChannelId { get; set; }

        /// <summary>
        /// 寄存器地址
        /// </summary>
        public ushort RegisterAddress
        {
            get => registerAddress; 
            set
            {
                registerAddress = value;
                PropertyChanged?.Invoke(value, new PropertyChangedEventArgs(nameof(RegisterAddress)));
            }
        }

        /// <summary>
        /// 存储值类型
        /// </summary>
        public RegisterValueType ValueType { get; set; }

        /// <summary>
        /// 当选string类型时的寄存器个数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 是否高字节在前
        /// </summary>
        public bool? IsHighByteBefore { get; set; }

        /// <summary>
        /// 寄存器地址变化事件
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}