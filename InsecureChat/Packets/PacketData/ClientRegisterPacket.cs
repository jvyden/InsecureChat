using System.Text;

namespace InsecureChat.Packets.PacketData; 

public class ClientRegisterPacket : IPacketData {
    public ClientRegisterPacket(string username) {
        this.Username = username;
    }

    public ClientRegisterPacket(byte[] data) {
        this.Username = Encoding.ASCII.GetString(data);
    }
    
    public string Username { get; init; }

    public PacketType Type => PacketType.Client_Register;
    public byte[] GetBytes() {
        return Encoding.ASCII.GetBytes(this.Username);
    }
}