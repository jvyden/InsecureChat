using System.Net.WebSockets;
using InsecureChat.Managers;
using InsecureChat.Packets;
using Microsoft.AspNetCore.Mvc;

namespace InsecureChat.Controllers; 

[ApiController]
public class ChatClientWebSocketController : ControllerBase {
    [HttpGet("/ws")]
    public async Task Get() {
        if(HttpContext.WebSockets.IsWebSocketRequest) {
            WebSocket ws = await this.HttpContext.WebSockets.AcceptWebSocketAsync();
            TaskCompletionSource<object> socketClosed = new();
            
            ChatClient client = ClientManager.CreateClient(ws, socketClosed);
            client.SendPacket(new Packet(PacketType.Server_Hello));

            await socketClosed.Task;
            Console.WriteLine("Done with client " + client.ClientId);
        }
        else {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
}