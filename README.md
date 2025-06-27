# Modbus协议：

``` C#
IModBusMaster modBus = new ModBusMaster(new SerialPort("COM2"), ModbusType.TCP);
await modBus.OpenAsync();
var b = new BlockList();
b.Add(new ProtocolData());
//var rs = await modBus.GetAsync("01", b);
var rs = await modBus.GetAsync<ProtocolData>("01", b);

// rs 的类型为 List<ProtocolData>
Console.WriteLine($"A: {rs.A}, B: {rs.B}, C: {rs.C}, D: {rs.D}");

public class ProtocolData
{
    [CHProtocol(1, "a")]
    public float A { get; set; }

    [CHProtocol(3)]
    public UInt16 B { get; set; }

    [CHProtocol(4)]
    public float C { get; set; }

    [CHProtocol(9)]
    public float D { get; set; }
}
```

## 下边的都过时了
``` C#
IModBusMaster modBusRtu = new ModBusMaster(new SerialPort("COM2"));

//01 03 00 01 00 05 D4 09 -> 01 03 0A 41 A0 00 00 00 15 41 B0 00 00 97 B8(20、21、22)
var block = new Modbus.Parameter.BlockInfo();

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

block = new Modbus.Parameter.BlockInfo();
block.AddChannelInfos(15, 300, RegisterValueType.UInt16);
modBusRtu.BlockInfos.Add(block);

await modBusRtu.OpenAsync();

var rs = await modBusRtu.GetAsync("01");

//01 03 11 00 00 0A C0 F1->03 10 14 56 65 72 30 39 30 49 5F 32 34 30 35 32 31 5F 30 39 32 37 5F B2 F3
var b = new Modbus.Parameter.BlockInfo();
b.ChannelInfos.Add(new() { ChannelId = "1", RegisterAddress = 0x1100, ValueType = RegisterValueType.String, Count = 10 });
var rs = await modBusRtu.GetAsync("01", b);
var str = Encoding.ASCII.GetString((byte[])rs[0].Value);

//await modBusRtu.SetAsync("01",
//    [
//    new Modbus.Parameter.SetBlockInfo() { RegisterAddress = 01, Data = [0x3F, 0x80, 0x00, 0x00] },
//    new Modbus.Parameter.SetBlockInfo() { RegisterAddress = 03, Data = [0x3F, 0x80, 0x00, 0x00] }
//    ]);
```
