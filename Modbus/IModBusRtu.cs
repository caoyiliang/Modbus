using Modbus.Parameter;
using ProtocolInterface;

namespace Modbus
{
    public interface IModBusRtu : IProtocol
    {
        bool IsHighByteBefore { get; set; }

        List<BlockInfo> BlockInfos { get; set; }

        Task<List<ChannelRsp>> GetAsync(string address, BlockInfo blockInfo);

        Task<List<ChannelRsp>> GetAsync(string address, List<BlockInfo> blockInfos);

        Task<List<ChannelRsp>> GetAsync(string address);

        Task SetAsync(string address, List<SetBlockInfo> ts);
    }
}
