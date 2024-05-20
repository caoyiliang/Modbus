// See https://aka.ms/new-console-template for more information
using Communication.Bus.PhysicalPort;
using Modbus;
using ProtocolInterface;

Console.WriteLine("Hello, World!");
IModBusRtu modBusRtu = new ModBusRtu(new SerialPort("COM2"));

var block = new Modbus.Parameter.BlockInfo();
block.ChannelInfos.Add(new() { ChannelId = 1, RegisterAddress = 1, ValueType = RegisterValueType.Float });
block.ChannelInfos.Add(new() { ChannelId = 2, RegisterAddress = 3, ValueType = RegisterValueType.Float });
block.ChannelInfos.Add(new() { ChannelId = 3, RegisterAddress = 5, ValueType = RegisterValueType.Float });

modBusRtu.BlockInfos.Add(block);

block = new Modbus.Parameter.BlockInfo();
block.ChannelInfos.Add(new() { ChannelId = 4, RegisterAddress = 9, ValueType = RegisterValueType.Float });
block.ChannelInfos.Add(new() { ChannelId = 5, RegisterAddress = 11, ValueType = RegisterValueType.Float });
block.ChannelInfos.Add(new() { ChannelId = 6, RegisterAddress = 13, ValueType = RegisterValueType.Float });

await modBusRtu.OpenAsync();
#pragma warning disable IDE0059 // 不需要赋值
var rs = await modBusRtu.GetAsync("01");
#pragma warning restore IDE0059 // 不需要赋值

Console.ReadKey();