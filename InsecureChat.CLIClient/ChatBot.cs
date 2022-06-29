using System.Diagnostics;
using System.Net.WebSockets;
using InsecureChat.Packets;
using Websocket.Client;

namespace InsecureChat.CLIClient; 

public class ChatBot {
    public ChatBot(Uri url) {
        this.client = new WebsocketClient(url);

        client.MessageReceived.Subscribe(msg => {
            Debug.Assert(msg.MessageType == WebSocketMessageType.Binary);
            Console.WriteLine(BitConverter.ToString(msg.Binary));
            
            this.handleData(msg.Binary);
        });

        client.ReconnectionHappened.Subscribe(info => {
            Console.WriteLine($"reconnection, type: {info.Type}");
        });
    }

    private bool fullyConnected = false;

    private WebsocketClient client { get; }

    private void handleData(byte[] buffer) {
        Packet packet = Packet.FromBuffer(buffer);
        this.processPacket(packet);
    }

    private void processPacket(Packet packet) {
        switch(packet.PacketType) {
            case PacketType.None:
                break;
            case PacketType.Server_Hello:
                this.SendPacket(new Packet(PacketType.Client_Hello));
                this.fullyConnected = true;
                break;
            case PacketType.Server_SendMessage:
                break;
            case PacketType.Server_ClientJoined:
                break;
            case PacketType.Server_ClientLeft:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void SendPacket(Packet packet) {
        using MemoryStream memoryStream = new();
        using BinaryWriter writer = new(memoryStream);

        Console.WriteLine("Sending packet " + packet.PacketType);

        writer.Write((byte)packet.PacketType);
        writer.Write(packet.Length);
        writer.Write(packet.Data);

        writer.Flush();

        this.client.Send(memoryStream.ToArray());
    }

    public Task Start() {
        return client.Start();
    }
}