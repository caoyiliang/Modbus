using Communication;
using Communication.Interfaces;
using LogInterface;
using Modbus.Parameter;
using Modbus.Request;
using Modbus.Response;
using Parser.Parsers;
using ProtocolInterface;
using System.Reflection;
using TopPortLib;
using TopPortLib.Interfaces;
using Utils;

namespace Modbus
{
    /// <inheritdoc/>
    public class ModBusMaster : IModBusMaster, IProtocol
    {
        private static readonly ILogger _logger = Logs.LogFactory.GetLogger<ModBusMaster>();
        private readonly ICrowPort _crowPort;
        private readonly ModbusType _modbusType;
        private ushort _transactionId = 0;

        /// <inheritdoc/>
        public bool IsConnect { get; private set; }
        /// <inheritdoc/>
        public bool IsHighByteBefore_Req { get; set; } = true;
        /// <inheritdoc/>
        public bool IsHighByteBefore_Rsp { get; set; } = true;
        /// <inheritdoc/>
        public bool IsHighByteBefore_MBAP { get; set; } = true;
        /// <inheritdoc/>
        public BlockList BlockInfos { get; set; } = new();

        /// <inheritdoc/>
        public event DisconnectEventHandler? OnDisconnect { add => _crowPort.OnDisconnect += value; remove => _crowPort.OnDisconnect -= value; }
        /// <inheritdoc/>
        public event ConnectEventHandler? OnConnect { add => _crowPort.OnConnect += value; remove => _crowPort.OnConnect -= value; }
        /// <inheritdoc/>
        public event SentDataEventHandler<byte[]>? OnSentData { add => _crowPort.OnSentData += value; remove => _crowPort.OnSentData -= value; }
        /// <inheritdoc/>
        public event Crow.ReceivedDataEventHandler<byte[]>? OnReceivedData { add => _crowPort.OnReceivedData += value; remove => _crowPort.OnReceivedData -= value; }

        /// <inheritdoc/>
        public ModBusMaster(IPhysicalPort physicalPort, ModbusType modbusType, int defaultTimeout = 5000)
        {
            _modbusType = modbusType;
            _crowPort = new CrowPort(new TopPort(physicalPort, new TimeParser(200)), defaultTimeout);
            _crowPort.OnSentData += CrowPort_OnSentData;
            _crowPort.OnReceivedData += CrowPort_OnReceivedData;
            _crowPort.OnConnect += CrowPort_OnConnect;
            _crowPort.OnDisconnect += CrowPort_OnDisconnect;
        }

        /// <inheritdoc/>
        public ModBusMaster(ICrowPort crowPort)
        {
            _crowPort = crowPort;
            _crowPort.OnConnect += CrowPort_OnConnect;
            _crowPort.OnDisconnect += CrowPort_OnDisconnect;
        }

        private Task CrowPort_OnDisconnect()
        {
            IsConnect = false;
            return Task.CompletedTask;
        }

        private Task CrowPort_OnConnect()
        {
            IsConnect = true;
            return Task.CompletedTask;
        }

        private Task CrowPort_OnReceivedData(byte[] data)
        {
            _logger.Trace($"ModBusRtu Rec:<-- {StringByteUtils.BytesToString(data)}");
            return Task.CompletedTask;
        }

        private Task CrowPort_OnSentData(byte[] data)
        {
            _logger.Trace($"ModBusRtu Send:--> {StringByteUtils.BytesToString(data)}");
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task OpenAsync()
        {
            IsConnect = _crowPort.PhysicalPort.IsOpen;
            return _crowPort.OpenAsync();
        }

        /// <inheritdoc/>
        public Task CloseAsync(bool closePhysicalPort)
        {
            return _crowPort.CloseAsync();
        }

        /// <inheritdoc/>
        public async Task<List<ChannelRsp>> GetAsync(string address, Block blockInfo)
        {

            var req = new GetReq(Convert.ToByte(address), blockInfo, IsHighByteBefore_Req, IsHighByteBefore_MBAP, _modbusType == ModbusType.RTU ? null : _transactionId++);
            return (await _crowPort.RequestAsync(req, (byte[] reqBytes, byte[] rspBytes) => new GetRsp(reqBytes, rspBytes, blockInfo, IsHighByteBefore_Rsp, _modbusType == ModbusType.RTU ? null : IsHighByteBefore_MBAP))).RecData;
        }

        /// <inheritdoc/>
        public async Task<List<ChannelRsp>> GetAsync(string address, BlockList blockInfos)
        {
            return await GetFromBlocksAsync(address, blockInfos.Blocks);
        }

        /// <inheritdoc/>
        public async Task<List<ChannelRsp>> GetAsync(string address)
        {
            return await GetFromBlocksAsync(address, BlockInfos.Blocks);
        }

        private async Task<List<ChannelRsp>> GetFromBlocksAsync(string address, IEnumerable<Block> blocks)
        {
            var result = new List<ChannelRsp>();
            foreach (var blockInfo in blocks)
            {
                result.AddRange(await GetAsync(address, blockInfo));
            }
            return result;
        }

        /// <inheritdoc/>
        public async Task<T> GetAsync<T>(string address, BlockList blockInfos) where T : new()
        {
            var result = new T();
            var propertyMap = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttribute<CHProtocolAttribute>() != null)
                .ToDictionary(p => p.GetCustomAttribute<CHProtocolAttribute>()!.ChannelId, p => p);

            foreach (var blockInfo in blockInfos.Blocks)
            {
                var channelResponses = await GetAsync(address, blockInfo);
                foreach (var channelRsp in channelResponses)
                {
                    if (channelRsp.ChannelId != null && propertyMap.TryGetValue(channelRsp.ChannelId, out var property) && property.CanWrite)
                    {
                        property.SetValue(result, Convert.ChangeType(channelRsp.Value, property.PropertyType));
                    }
                }
            }
            return result;
        }

        /// <inheritdoc/>
        public async Task SetAsync(string address, List<SetBlockInfo> BlockInfos)
        {
            byte addressByte = Convert.ToByte(address);
            foreach (var block in BlockInfos)
            {
                if (block.Data is null) continue;
                var req = new SetReq(addressByte, block.RegisterAddress, block.Data, IsHighByteBefore_Req);
                await _crowPort.RequestAsync(req, (reqBytes, rspBytes) => new SetRsp(reqBytes, rspBytes, _modbusType == ModbusType.RTU ? null : IsHighByteBefore_MBAP));
            }
        }
    }
}