namespace InsecureChat.Packets; 

public interface IPacketData {
    public PacketType Type { get; }
    public byte[] GetBytes();
}