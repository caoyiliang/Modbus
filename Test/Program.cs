// See https://aka.ms/new-console-template for more information
using Communication.Bus.PhysicalPort;
using Modbus;

Console.WriteLine("Hello, World!");
IModBusRtu modBusRtu = new ModBusRtu(new SerialPort("COM2"));
var block = new Modbus.Parameter.BlockInfo() { StartRegisterAddress = 1, EndRegisterAddress = 1 };
block.ChannelInfos.Add(new Modbus.Parameter.ChannelInfo() { ChannelId = 1, RegisterAddress = 1, ValueType = RegisterValueType.bInt });
modBusRtu.BlockInfos.Add(block);
await modBusRtu.OpenAsync();
var rs = await modBusRtu.GetAsync("01");

Console.ReadKey();