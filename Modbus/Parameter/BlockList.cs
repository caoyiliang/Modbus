using ProtocolInterface;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Channels;

namespace Modbus.Parameter
{
    public class BlockList
    {
        internal ObservableCollection<Block> Blocks { get; set; }

        public BlockList()
        {
            Blocks = [];
            Blocks.CollectionChanged += Blocks_CollectionChanged;
        }

        private void Blocks_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Block item in e.NewItems)
                {
                    Change();
                    item.PropertyChanged += Item_PropertyChanged;
                }
            }
        }

        private void Change()
        {
            for (int i = Blocks.Count - 1; i >= 0; i--)
            {
                for (int j = Blocks.Count - 1; j > i; j--)
                {
                    if (Blocks[i].EndRegisterAddress + 1 == Blocks[j].StartRegisterAddress)
                    {
                        for (int k = 0; k < Blocks[j].Channels.Count; k++)
                        {
                            Blocks[i].Channels.Add(Blocks[j].Channels[k]);
                        }

                        Blocks.RemoveAt(j);
                    }
                    else if (Blocks[j].EndRegisterAddress + 1 == Blocks[i].StartRegisterAddress)
                    {
                        for (int k = 0; k < Blocks[i].Channels.Count; k++)
                        {
                            Blocks[j].Channels.Add(Blocks[i].Channels[k]);
                        }
                        Blocks.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            Change();
        }

        public void Add(Channel channelInfo)
        {
            var newBlock = new Block();
            newBlock.Channels.Add(channelInfo);
            Blocks.Add(newBlock);
        }

        public void Add(Block block)
        {
            Blocks.Add(block);
        }

        public void Add<T>(T obj)
        {
            var properties = typeof(T).GetProperties();
            bool hasAttribute = false;

            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute<CHProtocolAttribute>();
                if (attribute != null)
                {
                    hasAttribute = true;
                    var channel = new Channel
                    {
                        ChannelId = attribute.ChannelId,
                        RegisterAddress = attribute.RegisterAddress,
                        ValueType = BlockList.GetRegisterValueType(property.PropertyType)
                    };
                    Add(channel);
                }
            }

            if (!hasAttribute)
            {
                throw new InvalidOperationException("The provided object does not contain any properties with the CHProtocolAttribute.");
            }
        }

        private static RegisterValueType GetRegisterValueType(Type propertyType)
        {
            if (propertyType == typeof(float))
            {
                return RegisterValueType.Float;
            }
            else if (propertyType == typeof(ushort))
            {
                return RegisterValueType.UInt16;
            }
            else if (propertyType == typeof(uint))
            {
                return RegisterValueType.UInt32;
            }
            else if (propertyType == typeof(short))
            {
                return RegisterValueType.Int16;
            }
            else if (propertyType == typeof(int))
            {
                return RegisterValueType.Int32;
            }
            else if (propertyType == typeof(string))
            {
                return RegisterValueType.String;
            }
            else
            {
                throw new InvalidOperationException($"Unsupported property type: {propertyType}");
            }
        }
    }
}
