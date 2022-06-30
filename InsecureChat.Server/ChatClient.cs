using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using InsecureChat.Managers;
using InsecureChat.Packets;
using InsecureChat.Packets.PacketData;

namespace InsecureChat; 

public class ChatClient {
    public ChatClient(int clientId, WebSocket webSocket, TaskCompletionSource<object> completionSource) {
        this.ClientId = clientId;
        this.WebSocket = webSocket;
        this.CompletionSource = completionSource;
    }
    
    public readonly int ClientId;
    public string? Username;
    public readonly WebSocket? WebSocket;

    private bool didHandshake;
    public bool Registered => this.Username != null;

    public ClientStatistics Statistics = new();

    public TaskCompletionSource<object> CompletionSource { get; init; }

    public void SendPacket(Packet packet) {
        if(this.WebSocket == null) return;

        if(this.WebSocket.State != WebSocketState.Open) {
            this.Disconnect();
            return;
        }
        
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

        CancellationTokenSource cts = new();
        cts.CancelAfter(TimeSpan.FromMilliseconds(100));

        try {
            byte[] buffer = new byte[1024];
            WebSocketReceiveResult res = await this.WebSocket.ReceiveAsync(buffer, cts.Token);
            Debug.Assert(res.MessageType == WebSocketMessageType.Binary);

            Packet packet = Packet.FromBuffer(buffer);
            processPacket(packet);
        }
        catch {
            // ignored
        }
    }

    public void Disconnect() {
        this.CompletionSource.SetResult(null!);
        ClientManager.RemoveClient(this);
        
        // inform all users this client is leaving
        ClientManager.BroadcastPacketExceptForSender(this.ClientId, new Packet(new ServerClientLeftPacket(this.ClientId)));

        if(this.WebSocket?.State != WebSocketState.Open) return;
        
        this.WebSocket.CloseAsync(WebSocketCloseStatus.Empty, null, CancellationToken.None).Wait();
        this.WebSocket.Dispose();
    }

    private void processPacket(Packet packet) {
        if(this.WebSocket == null) return;
        if(!this.didHandshake && packet.PacketType != PacketType.Client_Hello) return; // don't care + l + ratio
        
        Console.WriteLine("Got packet " + packet.PacketType);
        
        switch(packet.PacketType) {
            case PacketType.None:
                this.Disconnect(); // probably a bullshit client lol
                break;
            case PacketType.Client_Hello:
                this.didHandshake = true;
                break;
            case PacketType.Client_Register:
                this.Username = new ClientRegisterPacket(packet.Data).Username;
                Console.WriteLine("username: " + this.Username);
                
                // Inform everyone user has registered
                Packet joinPacket = new(new ServerClientJoinedPacket(this.ClientId, this.Username));
                ClientManager.BroadcastPacket(joinPacket);
                
                foreach(ChatClient client in ClientManager.Clients.Where(c => c.Registered && c.ClientId != this.ClientId)) {
                    this.SendPacket(new Packet(new ServerClientJoinedPacket(client.ClientId, client.Username!)));
                }
                break;
            case PacketType.Client_SendMessage:
                if(!this.Registered) break;
                ClientSendMessagePacket message = new(packet.Data);
                Console.WriteLine($"message from client {this.ClientId}: {message.Message}");
                
                ClientManager.BroadcastPacket(new Packet(new ServerSendMessagePacket(this.ClientId, message.Message)));
                break;
            case PacketType.Client_Quit:
                this.Disconnect();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}