using System.Runtime.CompilerServices;


/// <summary>  
/// 寄存器信息  
/// </summary>  
[AttributeUsage(AttributeTargets.Property)]
public class CHProtocolAttribute : Attribute
{
    /// <summary>  
    /// 获取通道 ID。  
    /// </summary>  
    public string ChannelId { get; }

    /// <summary>  
    /// 获取寄存器地址。  
    /// </summary>  
    public ushort RegisterAddress { get; }

    /// <summary>  
    /// 初始化 <see cref="CHProtocolAttribute"/> 类的新实例。  
    /// </summary>  
    /// <param name="channelId">通道 ID。</param>  
    /// <param name="registerAddress">寄存器地址。</param>  
    public CHProtocolAttribute(ushort registerAddress, [CallerMemberName] string channelId = "")
    {
        ChannelId = channelId;
        RegisterAddress = registerAddress;
    }
}
