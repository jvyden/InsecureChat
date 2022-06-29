using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;

namespace InsecureChat.Managers; 

public static class WorkerManager {
    [SuppressMessage("ReSharper", "FunctionNeverReturns")]
    public static void SpawnWorker() {
        Task.Factory.StartNew(async () => {
            while(true) {
                await work();
                Thread.Sleep(20);
            }
        });
    }

    private static async Task work() {
        if(!ClientManager.ClientQueue.TryDequeue(out ChatClient? chatClient)) return;

        chatClient.Statistics.TimesProcessed++;
        try {
            await chatClient.Process();
        }
        catch(WebSocketException) {
            chatClient.Disconnect();
        }

        ClientManager.ClientQueue.Enqueue(chatClient);
    }
}