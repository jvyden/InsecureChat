namespace InsecureChat.Packets.PacketData;

public class ServerClientLeftPacket : IPacketData {
    public ServerClientLeftPacket(int clientId) {
        this.ClientId = clientId;
    }

    public ServerClientLeftPacket(byte[] data) {
        using MemoryStream ms = new(data);
        using BinaryReader reader = new(ms);

        this.ClientId = reader.ReadInt32();
    }

    public int ClientId { get; init; }

    public PacketType Type => PacketType.Server_ClientLeft;

    public byte[] GetBytes() {
        using MemoryStream ms = new();
        using BinaryWriter writer = new(ms);

        writer.Write(this.ClientId);

        writer.Flush();

        return ms.ToArray();
    }
}