using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using InsecureChat.Packets;

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

    private bool fullyConnected = false;

    public ClientStatistics Statistics = new();

    public TaskCompletionSource<object> CompletionSource { get; init; }

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

        CancellationTokenSource cts = new();
        cts.CancelAfter(TimeSpan.FromSeconds(1));

        byte[] buffer = new byte[1024];
        WebSocketReceiveResult res = await this.WebSocket.ReceiveAsync(buffer, cts.Token);
        Debug.Assert(res.MessageType == WebSocketMessageType.Binary);

        Packet packet = Packet.FromBuffer(buffer);
        processPacket(packet);
    }

    public void Disconnect() {
        this.CompletionSource.SetResult(null!);
        
        if(this.WebSocket?.State != WebSocketState.Open) return;
        
        this.WebSocket.CloseAsync(WebSocketCloseStatus.Empty, null, CancellationToken.None).Wait();
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
//                this.SendPacket(new Packet(PacketType.Server_Hello)); // i like money
                this.fullyConnected = true;
                break;
            case PacketType.Client_Register:
                break;
            case PacketType.Client_SendMessage:
                break;
            case PacketType.Client_Quit:
                this.Disconnect();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}