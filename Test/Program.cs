// See https://aka.ms/new-console-template for more information
using Communication.Bus.PhysicalPort;
using Modbus;
using ProtocolInterface;

Console.WriteLine("Hello, World!");
IModBusRtu modBusRtu = new ModBusRtu(new SerialPort("COM2"));
var block = new Modbus.Parameter.BlockInfo() { StartRegisterAddress = 1, EndRegisterAddress = 1 };
block.ChannelInfos.Add(new Modbus.Parameter.ChannelInfo() { ChannelId = 1, RegisterAddress = 1, ValueType = RegisterValueType.sbyteB });
modBusRtu.BlockInfos.Add(block);
await modBusRtu.OpenAsync();
#pragma warning disable IDE0059 // 不需要赋值
var rs = await modBusRtu.GetAsync("01");
#pragma warning restore IDE0059 // 不需要赋值

Console.ReadKey();