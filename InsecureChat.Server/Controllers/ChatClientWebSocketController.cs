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
            ChatClient client = ClientManager.CreateClient(ws);
            client.SendPacket(new Packet(PacketType.Server_Hello));
            await Task.Delay(1000);
        }
        else {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
}