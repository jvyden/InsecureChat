using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace InsecureChat.Managers; 

public static class ClientManager {
    private static readonly List<ChatClient> clients = new();
    public static IReadOnlyList<ChatClient> Clients => clients.AsReadOnly();

    public static readonly ConcurrentQueue<ChatClient> ClientQueue = new();

    public static ChatClient CreateClient(WebSocket webSocket, TaskCompletionSource<object> completionSource) {
        ChatClient client = new(clients.Count, webSocket, completionSource);

        clients.Add(client);
        ClientQueue.Enqueue(client);

        return client;
    }


}