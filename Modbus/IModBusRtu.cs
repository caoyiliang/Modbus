using Modbus.Parameter;
using ProtocolInterface;

namespace Modbus
{
    /// <summary>
    /// ModBusRtu
    /// </summary>
    public interface IModBusRtu : IProtocol
    {
        /// <summary>
        /// 请求是否高字节在前
        /// </summary>
        bool IsHighByteBefore_Req { get; set; }

        /// <summary>
        /// 响应是否高字节在前
        /// </summary>
        bool IsHighByteBefore_Rsp { get; set; }

        /// <summary>
        /// 块信息
        /// </summary>
        List<BlockInfo> BlockInfos { get; set; }

        /// <summary>
        /// 获取寄存器数据
        /// </summary>
        /// <param name="address">设备地址</param>
        /// <param name="blockInfo">需要获取的块</param>
        Task<List<ChannelRsp>> GetAsync(string address, BlockInfo blockInfo);

        /// <summary>
        /// 获取寄存器数据
        /// </summary>
        /// <param name="address">设备地址</param>
        /// <param name="blockInfos">需要获取的块</param>
        Task<List<ChannelRsp>> GetAsync(string address, List<BlockInfo> blockInfos);

        /// <summary>
        /// 获取寄存器数据
        /// </summary>
        /// <param name="address">设备地址</param>
        Task<List<ChannelRsp>> GetAsync(string address);

        /// <summary>
        /// 设置寄存器数据
        /// </summary>
        /// <param name="address">设备地址</param>
        /// <param name="ts">设置块数据</param>
        Task SetAsync(string address, List<SetBlockInfo> ts);
    }
}
