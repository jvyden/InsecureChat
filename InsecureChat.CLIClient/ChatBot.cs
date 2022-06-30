using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using InsecureChat.Packets;
using InsecureChat.Packets.PacketData;
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

    public int ClientId = -1;
    public string Username = "username";

    private bool didHandshake = false;
    private bool registered = false;

    private WebsocketClient client { get; }

    private void handleData(byte[] buffer) {
        Packet packet = Packet.FromBuffer(buffer);
        this.processPacket(packet);
    }

    private void processPacket(Packet packet) {
        Console.WriteLine("Got packet " + packet.PacketType);
        
        switch(packet.PacketType) {
            case PacketType.None:
                break;
            case PacketType.Server_Hello:
                this.SendPacket(new Packet(PacketType.Client_Hello));
                this.SendPacket(new Packet(new ClientRegisterPacket(this.Username)));
                this.didHandshake = true;
                break;
            case PacketType.Server_SendMessage:
                break;
            case PacketType.Server_ClientJoined:
                ServerClientJoinedPacket clientJoined = new(packet.Data);
                // if unregistered or server is updating us
                if(!this.registered || clientJoined.ClientId == this.ClientId) {
                    this.ClientId = clientJoined.ClientId;
                    this.Username = clientJoined.ClientUsername; // intentional, allows server to rename user
                    
                    Console.WriteLine($"Registered, id:{this.ClientId}, name:{this.Username}");
                    this.registered = true;
                    
                    this.SendPacket(new Packet(new ClientSendMessagePacket("ayo")));
                }
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