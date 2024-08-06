using ProtocolInterface;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Modbus.Parameter
{
    public class BlockInfo
    {
        public ushort? StartRegisterAddress { get; private set; }
        public ushort EndRegisterAddress { get; private set; }
        public ObservableCollection<ChannelInfo> ChannelInfos { get; set; }
        public BlockInfo()
        {
            ChannelInfos = [];
            ChannelInfos.CollectionChanged += ChannelInfos_CollectionChanged;
        }

        private void ChannelInfos_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ChannelInfo item in e.NewItems)
                {
                    Change(item);
                    item.PropertyChanged += Item_PropertyChanged;
                }
            }
        }

        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            StartRegisterAddress = null;
            foreach (var item in ChannelInfos)
            {
                Change(item);
            }
        }

        private void Change(ChannelInfo item)
        {
            var value = item.RegisterAddress;
            StartRegisterAddress ??= value;
            if (value < StartRegisterAddress)
            {
                StartRegisterAddress = value;
            }
            if (value > EndRegisterAddress)
            {
                EndRegisterAddress = value;
                switch (item.ValueType)
                {
                    case RegisterValueType.Float:
                    case RegisterValueType.UInt32:
                    case RegisterValueType.Int32:
                        EndRegisterAddress++;
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 添加连续的寄存器地址
        /// </summary>
        /// <param name="startRegisterAddress">起始地址</param>
        /// <param name="Count">数据个数</param>
        /// <param name="valueType">数据类型</param>
        public void AddChannelInfos(ushort startRegisterAddress, int Count, RegisterValueType valueType) => BlockHelper.CreateBlock(startRegisterAddress, Count, valueType).ForEach(item => ChannelInfos.Add(item));
    }
}
