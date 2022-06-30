namespace InsecureChat.Packets.PacketData; 

public class ServerClientJoinedPacket : IPacketData {
    public ServerClientJoinedPacket(int clientId, string clientUsername) {
        this.ClientId = clientId;
        this.ClientUsername = clientUsername;
    }

    public ServerClientJoinedPacket(byte[] data) {
        using MemoryStream ms = new(data);
        using BinaryReader reader = new(ms);

        this.ClientId = reader.ReadInt32();
        this.ClientUsername = reader.ReadString();
    }

    public int ClientId { get; init; }
    public string ClientUsername { get; init; }
    
    public PacketType Type => PacketType.Server_ClientJoined;
    
    public byte[] GetBytes() {
        using MemoryStream ms = new();
        using BinaryWriter writer = new(ms);
        
        writer.Write(this.ClientId);
        writer.Write(this.ClientUsername);
        
        writer.Flush();

        return ms.ToArray();
    }
}