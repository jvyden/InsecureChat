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

    public static Packet FromBuffer(byte[] buffer) {
        using MemoryStream memoryStream = new(buffer);
        using BinaryReader reader = new(memoryStream);
        
        while(reader.BaseStream.Position != buffer.Length) {
            PacketType type = (PacketType)reader.ReadByte();
            ushort length = reader.ReadUInt16();
            byte[] data = new byte[length];

            for(int i = 0; i < length; i++) {
                data[i] = reader.ReadByte();
            }

            return new Packet(type, data, length);
        }
        
        return new Packet(PacketType.None, Array.Empty<byte>(), 0);
    }
}