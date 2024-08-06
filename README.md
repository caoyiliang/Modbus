# Modbus协议：

``` C#
IModBusRtu modBusRtu = new ModBusRtu(new SerialPort("COM2"));

//01 03 00 01 00 05 D4 09 -> 01 03 0A 41 A0 00 00 00 15 41 B0 00 00 97 B8(20、21、22)
var block = new Modbus.Parameter.BlockInfo();

block.AddChannelInfos(4, 300, RegisterValueType.UInt16);

block.ChannelInfos.Add(new() { ChannelId = "1", RegisterAddress = 1, ValueType = RegisterValueType.Float });
block.ChannelInfos.Add(new() { ChannelId = "2", RegisterAddress = 3, ValueType = RegisterValueType.UInt16 });
block.ChannelInfos.Add(new() { ChannelId = "3", RegisterAddress = 4, ValueType = RegisterValueType.Float });

modBusRtu.BlockInfos.Add(block);

//01 03 00 09 00 06 15 CA -> 01 03 0C 41 F0 00 00 42 20 00 00 42 48 00 00 5C CA(30、40、50)
block = new Modbus.Parameter.BlockInfo();
block.ChannelInfos.Add(new() { RegisterAddress = 9, ValueType = RegisterValueType.Float });
block.ChannelInfos.Add(new() { RegisterAddress = 11, ValueType = RegisterValueType.Float });
block.ChannelInfos.Add(new() { RegisterAddress = 13, ValueType = RegisterValueType.Float });

modBusRtu.BlockInfos.Add(block);

await modBusRtu.OpenAsync();

var rs = await modBusRtu.GetAsync("01");

await modBusRtu.SetAsync("01",
    [
    new Modbus.Parameter.SetBlockInfo() { RegisterAddress = 01, Data = [0x3F, 0x80, 0x00, 0x00] },
    new Modbus.Parameter.SetBlockInfo() { RegisterAddress = 03, Data = [0x3F, 0x80, 0x00, 0x00] }
    ]);
```
