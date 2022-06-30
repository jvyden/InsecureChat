using System.Collections.Concurrent;
using System.Net.WebSockets;
using InsecureChat.Packets;

namespace InsecureChat.Managers; 

public static class ClientManager {
    private static readonly List<ChatClient> clients = new();
    public static IReadOnlyList<ChatClient> Clients => clients.AsReadOnly();

    public static readonly ConcurrentQueue<ChatClient> ClientQueue = new();

    public static ChatClient CreateClient(WebSocket webSocket, TaskCompletionSource<object> completionSource) {
        ChatClient client = new(clients.Count + 1, webSocket, completionSource);

        clients.Add(client);
        ClientQueue.Enqueue(client);

        return client;
    }

    public static void RemoveClient(ChatClient client) {
        clients.Remove(client);
        // client will be removed from queue by worker
    }

    public static void BroadcastPacket(Packet packet) {
        foreach(ChatClient client in clients) {
            client.SendPacket(packet);
        }
    }

    public static void BroadcastPacketExceptForSender(int senderId, Packet packet) {
        foreach(ChatClient client in clients.Where(client => client.ClientId != senderId)) {
            client.SendPacket(packet);
        }
    }
}