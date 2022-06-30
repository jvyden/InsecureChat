using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;

namespace InsecureChat.Managers; 

public static class WorkerManager {
    [SuppressMessage("ReSharper", "FunctionNeverReturns")]
    public static void SpawnWorker(int n = 0) {
        Task.Factory.StartNew(async () => {
            Thread.CurrentThread.Name = "InsecureChat Worker " + n;
            while(true) {
                try {
                    await work();
                }
                catch(Exception e) {
                    Console.WriteLine($"{Thread.CurrentThread.Name} experienced an error: \n{e}");
                }
                Thread.Sleep(20);
            }
        });
    }

    private static async Task work() {
        if(!ClientManager.ClientQueue.TryDequeue(out ChatClient? chatClient)) {
//            Console.WriteLine("couldn't dequeue, clientQueue len: " + ClientManager.ClientQueue.Count);
            return;
        }

        if(!ClientManager.Clients.Contains(chatClient)) return; // return before we add to queue
        
        chatClient.Statistics.TimesProcessed++;
        try {
            await chatClient.Process();
        }
        catch(WebSocketException) {
            chatClient.Disconnect();
        }
        catch(Exception e) {
            Console.WriteLine($"Error while processing {chatClient.ClientId}:\n{e}");
        }

        ClientManager.ClientQueue.Enqueue(chatClient);
    }
}