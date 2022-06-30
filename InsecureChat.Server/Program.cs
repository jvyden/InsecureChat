using InsecureChat.Managers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

WebApplication app = builder.Build();

Console.WriteLine($"Spawning {Environment.ProcessorCount} worker threads");

AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) => {
    Console.WriteLine(eventArgs.ExceptionObject);
};

for(int i = 0; i < Environment.ProcessorCount; i++) {
    WorkerManager.SpawnWorker(i);
}

app.UseStaticFiles();
app.UseRouting();
app.UseWebSockets();
app.MapControllers();
app.MapRazorPages();

app.Run();