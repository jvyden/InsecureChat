using System.Diagnostics.CodeAnalysis;

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
        await chatClient.Process();
        
        ClientManager.ClientQueue.Enqueue(chatClient);
    }
}