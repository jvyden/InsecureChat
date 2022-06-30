using System.Text;

namespace InsecureChat.Packets.PacketData; 

public class ClientSendMessagePacket : IPacketData {
    public ClientSendMessagePacket(string message) {
        this.Message = message;
    }

    public ClientSendMessagePacket(byte[] data) {
        this.Message = Encoding.ASCII.GetString(data);
    }
    
    public string Message { get; init; }
    
    public PacketType Type => PacketType.Client_SendMessage;
    public byte[] GetBytes() {
        return Encoding.ASCII.GetBytes(this.Message);
    }
}