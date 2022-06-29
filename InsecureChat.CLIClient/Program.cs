namespace InsecureChat.CLIClient;

public static class Program {
    public static async Task Main() {
        ChatBot client = new(new Uri("ws://localhost:5056/ws"));

        await client.Start();

        while(true) {
            await Task.Delay(50);
        }
    }
}