using Communication;
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
        /// 发送数据的事件
        /// </summary>
        public event SentDataEventHandler<byte[]> OnSentData;

        /// <summary>
        /// 接收数据的事件
        /// </summary>
        public event Crow.ReceivedDataEventHandler<byte[]> OnReceivedData;

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
        BlockList BlockInfos { get; set; }

        /// <summary>
        /// 获取寄存器数据
        /// </summary>
        /// <param name="address">设备地址</param>
        /// <param name="blockInfo">需要获取的块</param>
        Task<List<ChannelRsp>> GetAsync(string address, Block blockInfo);

        /// <summary>
        /// 获取寄存器数据
        /// </summary>
        /// <param name="address">设备地址</param>
        /// <param name="blockInfos">需要获取的块</param>
        Task<List<ChannelRsp>> GetAsync(string address, BlockList blockInfos);

        /// <summary>
        /// 获取寄存器数据
        /// </summary>
        /// <param name="address">设备地址</param>
        Task<List<ChannelRsp>> GetAsync(string address);

        /// <summary>
        /// 获取寄存器数据（支持泛型）
        /// </summary>
        /// <typeparam name="T">返回的类型</typeparam>
        /// <param name="address">设备地址</param>
        /// <param name="blockInfos">需要获取的块</param>
        Task<T> GetAsync<T>(string address, BlockList blockInfos) where T : new();

        /// <summary>
        /// 设置寄存器数据
        /// </summary>
        /// <param name="address">设备地址</param>
        /// <param name="ts">设置块数据</param>
        Task SetAsync(string address, List<SetBlockInfo> ts);
    }
}
