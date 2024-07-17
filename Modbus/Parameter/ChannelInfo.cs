using ProtocolInterface;
using System.ComponentModel;

namespace Modbus.Parameter
{
    public class ChannelInfo
    {
        private ushort registerAddress;

        public string ChannelId { get; set; }
        public ushort RegisterAddress
        {
            get => registerAddress; set
            {
                registerAddress = value;
                PropertyChanged?.Invoke(value, new PropertyChangedEventArgs(nameof(RegisterAddress)));
            }
        }
        public RegisterValueType ValueType { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}