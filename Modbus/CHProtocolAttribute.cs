using ProtocolInterface;

[AttributeUsage(AttributeTargets.Property)]
public class CHProtocolAttribute : Attribute
{
    public string ChannelId { get; }
    public ushort RegisterAddress { get; }

    public CHProtocolAttribute(string channelId, ushort registerAddress)
    {
        ChannelId = channelId;
        RegisterAddress = registerAddress;
    }
}
