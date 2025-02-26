﻿using Communication;
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
    /// <inheritdoc/>
    public class ModBusRtu : IModBusRtu, IProtocol
    {
        private static readonly ILogger _logger = Logs.LogFactory.GetLogger<ModBusRtu>();
        private readonly ICrowPort _crowPort;
        /// <inheritdoc/>
        public bool IsConnect { get; private set; }
        /// <inheritdoc/>
        public bool IsHighByteBefore_Req { get; set; } = true;
        /// <inheritdoc/>
        public bool IsHighByteBefore_Rsp { get; set; } = true;
        /// <inheritdoc/>
        public BlockList BlockInfos { get; set; }

        /// <inheritdoc/>
        public event DisconnectEventHandler? OnDisconnect { add => _crowPort.OnDisconnect += value; remove => _crowPort.OnDisconnect -= value; }
        /// <inheritdoc/>
        public event ConnectEventHandler? OnConnect { add => _crowPort.OnConnect += value; remove => _crowPort.OnConnect -= value; }
        /// <inheritdoc/>
        public event SentDataEventHandler<byte[]>? OnSentData { add => _crowPort.OnSentData += value; remove => _crowPort.OnSentData -= value; }
        /// <inheritdoc/>
        public event Crow.ReceivedDataEventHandler<byte[]>? OnReceivedData { add => _crowPort.OnReceivedData += value; remove => _crowPort.OnReceivedData -= value; }
        /// <inheritdoc/>
        public ModBusRtu(SerialPort serialPort, int defaultTimeout = 5000)
        {
            _crowPort = new CrowPort(new TopPort(serialPort, new TimeParser(60)), defaultTimeout);
            _crowPort.OnSentData += CrowPort_OnSentData;
            _crowPort.OnReceivedData += CrowPort_OnReceivedData;
            _crowPort.OnConnect += CrowPort_OnConnect;
            _crowPort.OnDisconnect += CrowPort_OnDisconnect;
        }

        private async Task CrowPort_OnDisconnect()
        {
            IsConnect = false;
            await Task.CompletedTask;
        }

        private async Task CrowPort_OnConnect()
        {
            IsConnect = true;
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

        /// <inheritdoc/>
        public async Task<List<ChannelRsp>> GetAsync(string address, Block blockInfo)
        {
            var req = new GetReq(Convert.ToByte(address), blockInfo, IsHighByteBefore_Req);
            return (await _crowPort.RequestAsync(req, new Func<byte[], GetRsp>(rspByte => new GetRsp(rspByte, blockInfo, IsHighByteBefore_Rsp)))).RecData;
        }

        /// <inheritdoc/>
        public async Task<List<ChannelRsp>> GetAsync(string address, BlockList blockInfos)
        {
            var result = new List<ChannelRsp>();
            foreach (var blockInfo in blockInfos.Blocks)
            {
                result.AddRange(await GetAsync(address, blockInfo));
            }
            return result;
        }

        /// <inheritdoc/>
        public async Task<List<ChannelRsp>> GetAsync(string address)
        {
            var result = new List<ChannelRsp>();
            foreach (var blockInfo in BlockInfos.Blocks)
            {
                result.AddRange(await GetAsync(address, blockInfo));
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
                await _crowPort.RequestAsync<SetReq, SetRsp>(req);
            }
        }
    }
}