using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using InsecureChat.Packets;

namespace InsecureChat; 

public class ChatClient {
    public ChatClient(int clientId, WebSocket webSocket) {
        this.ClientId = clientId;
        this.WebSocket = webSocket;
    }
    
    public readonly int ClientId;
    public string? Username;
    public readonly WebSocket? WebSocket;

    private bool fullyConnected = false;

    public ClientStatistics Statistics = new();

    public void SendPacket(Packet packet) {
        if(this.WebSocket == null) return;
        
        using MemoryStream memoryStream = new();
        using BinaryWriter writer = new(memoryStream);
        
        Console.WriteLine("Sending packet " + packet.PacketType);

        writer.Write((byte)packet.PacketType);
        writer.Write(packet.Length);
        writer.Write(packet.Data);
        
        writer.Flush();

        this.WebSocket.SendAsync(memoryStream.ToArray(), WebSocketMessageType.Binary, true, CancellationToken.None).Wait();
    }

    public async Task Process() {
        if(this.WebSocket == null) return;
        Console.WriteLine("going");

        CancellationTokenSource cts = new();
        cts.CancelAfter(TimeSpan.FromSeconds(1));

        byte[] buffer = new byte[1024];
        WebSocketReceiveResult res = await this.WebSocket.ReceiveAsync(buffer, cts.Token);
        Debug.Assert(res.MessageType == WebSocketMessageType.Binary);

        using MemoryStream memoryStream = new(buffer);
        using BinaryReader reader = new(memoryStream);

        while(reader.BaseStream.Position != res.Count) {
            PacketType type = (PacketType)reader.ReadByte();
            ushort length = reader.ReadUInt16();
            byte[] data = new byte[length];

            for(int i = 0; i < length; i++) {
                data[i] = reader.ReadByte();
            }
            
            Packet packet = new(type, data, length);
            processPacket(packet);
        }
    }

    private void processPacket(Packet packet) {
        if(this.WebSocket == null) return;
        if(!this.fullyConnected && packet.PacketType != PacketType.Client_Hello) return; // don't care + l + ratio
        
        Console.WriteLine("Got packet " + packet.PacketType);
        
        switch(packet.PacketType) {
            case PacketType.None:
                // wat
                break;
            case PacketType.Client_Hello:
                this.SendPacket(new Packet(PacketType.Server_Hello)); // i like money
                this.fullyConnected = true;
                break;
            case PacketType.Server_Hello:
                break;
            case PacketType.Client_Register:
                break;
            case PacketType.Server_SendMessage:
                break;
            case PacketType.Client_SendMessage:
                break;
            case PacketType.Server_ClientJoined:
                break;
            case PacketType.Server_ClientLeft:
                break;
            case PacketType.Client_Quit:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}