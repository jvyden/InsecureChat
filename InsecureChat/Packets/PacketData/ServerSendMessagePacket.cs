namespace InsecureChat.Packets.PacketData; 

public class ServerSendMessagePacket : IPacketData {
    public ServerSendMessagePacket(int sendingClientId, string message) {
        this.SendingClientId = sendingClientId;
        this.Message = message;
    }

    public ServerSendMessagePacket(byte[] data) {
        using MemoryStream ms = new(data);
        using BinaryReader reader = new(ms);

        this.SendingClientId = reader.ReadInt32();
        this.Message = reader.ReadString();
    }

    public int SendingClientId { get; init; }
    public string Message { get; init; }
    
    public PacketType Type => PacketType.Server_SendMessage;
    public byte[] GetBytes() {
        using MemoryStream ms = new();
        using BinaryWriter writer = new(ms);
        
        writer.Write(this.SendingClientId);
        writer.Write(this.Message);
        
        writer.Flush();

        return ms.ToArray();
    }
}