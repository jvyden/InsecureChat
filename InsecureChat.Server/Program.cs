using InsecureChat;
using InsecureChat.Managers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

WebApplication app = builder.Build();

Console.WriteLine($"Spawning {Environment.ProcessorCount} worker threads");

for(int i = 0; i < Environment.ProcessorCount; i++) {
    WorkerManager.SpawnWorker();
}

app.UseStaticFiles();
app.UseRouting();
app.UseWebSockets();
app.MapControllers();
app.MapRazorPages();

app.Run();