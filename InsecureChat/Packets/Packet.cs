namespace InsecureChat.Packets; 

public struct Packet {
    public readonly PacketType PacketType;
    public readonly ushort Length;
    public readonly byte[] Data;

    public Packet(PacketType type) {
        this.PacketType = type;
        this.Data = Array.Empty<byte>();
        this.Length = 0;
    }

    public Packet(PacketType type, byte[] data) {
        this.PacketType = type;
        this.Data = data;
        this.Length = (ushort)data.Length;
    }

    public Packet(PacketType type, byte[] data, ushort length) {
        this.PacketType = type;
        this.Data = data;
        this.Length = length;
    }
}