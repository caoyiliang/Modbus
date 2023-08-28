using Communication;
using Communication.Bus.PhysicalPort;
using LogInterface;
using Modbus.Parameter;
using Modbus.Request;
using Modbus.Response;
using Parser.Parsers;
using ProtocolInterface;
using TopPortLib;
using TopPortLib.Interfaces;
using Utils;

namespace Modbus
{
    public class ModBusRtu : IModBusRtu, IProtocol
    {
        private static readonly ILogger _logger = Logs.LogFactory.GetLogger<ModBusRtu>();
        private readonly ICrowPort _crowPort;
        private readonly bool _isHighByteBefore;

        private bool _isConnect = false;
        public bool IsConnect => _isConnect;

        public List<BlockInfo> BlockInfos { get; set; } = new List<BlockInfo>();

        /// <inheritdoc/>
        public event DisconnectEventHandler? OnDisconnect { add => _crowPort.OnDisconnect += value; remove => _crowPort.OnDisconnect -= value; }
        /// <inheritdoc/>
        public event ConnectEventHandler? OnConnect { add => _crowPort.OnConnect += value; remove => _crowPort.OnConnect -= value; }

        public ModBusRtu(SerialPort serialPort, bool isHighByteBefore = true, int defaultTimeout = 200)
        {
            _crowPort = new CrowPort(new TopPort(serialPort, new TimeParser(60)), defaultTimeout);
            _crowPort.OnSentData += CrowPort_OnSentData;
            _crowPort.OnReceivedData += CrowPort_OnReceivedData;
            _crowPort.OnConnect += CrowPort_OnConnect;
            _crowPort.OnDisconnect += CrowPort_OnDisconnect;
            _isHighByteBefore = isHighByteBefore;
        }

        private async Task CrowPort_OnDisconnect()
        {
            _isConnect = false;
            await Task.CompletedTask;
        }

        private async Task CrowPort_OnConnect()
        {
            _isConnect = true;
            await Task.CompletedTask;
        }

        private async Task CrowPort_OnReceivedData(byte[] data)
        {
            _logger.Trace($"ModBusRtu Rec:<-- {StringByteUtils.BytesToString(data)}");
            await Task.CompletedTask;
        }

        private async Task CrowPort_OnSentData(byte[] data)
        {
            _logger.Trace($"ModBusRtu Send:--> {StringByteUtils.BytesToString(data)}");
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task OpenAsync() => _crowPort.OpenAsync();

        /// <inheritdoc/>
        public Task CloseAsync() => _crowPort.CloseAsync();

        public async Task<List<ChannelRsp>> GetAsync(string address, BlockInfo blockInfo)
        {
            var req = new GetReq(Convert.ToByte(address), blockInfo);
            return (await _crowPort.RequestAsync(req, new Func<byte[], GetRsp>(rspByte => new GetRsp(rspByte, blockInfo, _isHighByteBefore)))).RecData;
        }

        public async Task<List<ChannelRsp>> GetAsync(string address, List<BlockInfo> blockInfos)
        {
            var result = new List<ChannelRsp>();
            foreach (var blockInfo in blockInfos)
            {
                result.AddRange(await GetAsync(address, blockInfo));
            }
            return result;
        }

        public async Task<List<ChannelRsp>> GetAsync(string address)
        {
            var result = new List<ChannelRsp>();
            foreach (var blockInfo in BlockInfos)
            {
                result.AddRange(await GetAsync(address, blockInfo));
            }
            return result;
        }

        public async Task SetAsync(string address, List<SetBlockInfo> BlockInfos)
        {
            byte addressByte = Convert.ToByte(address);
            foreach (var block in BlockInfos)
            {
                if (block.Data is null) continue;
                var req = new SetReq(addressByte, block.RegisterAddress, block.Data);
                await _crowPort.RequestAsync<SetReq, SetRsp>(req);
            }
        }
    }
}