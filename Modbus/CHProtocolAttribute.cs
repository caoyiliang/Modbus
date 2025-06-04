using System.Runtime.CompilerServices;


/// <summary>  
/// �Ĵ�����Ϣ  
/// </summary>  
[AttributeUsage(AttributeTargets.Property)]
public class CHProtocolAttribute : Attribute
{
    /// <summary>  
    /// ��ȡͨ�� ID��  
    /// </summary>  
    public string ChannelId { get; }

    /// <summary>  
    /// ��ȡ�Ĵ�����ַ��  
    /// </summary>  
    public ushort RegisterAddress { get; }

    /// <summary>  
    /// ��ʼ�� <see cref="CHProtocolAttribute"/> �����ʵ����  
    /// </summary>  
    /// <param name="channelId">ͨ�� ID��</param>  
    /// <param name="registerAddress">�Ĵ�����ַ��</param>  
    public CHProtocolAttribute(ushort registerAddress, [CallerMemberName] string channelId = "")
    {
        ChannelId = channelId;
        RegisterAddress = registerAddress;
    }
}
