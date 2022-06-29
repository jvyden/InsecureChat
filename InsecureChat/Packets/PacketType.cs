namespace InsecureChat.Packets; 

public enum PacketType : byte {
    None = 0,
    Client_Hello = 1,
    Server_Hello = 2,
    Client_Register = 3,
    Server_SendMessage = 4,
    Client_SendMessage = 5,
    Server_ClientJoined = 6,
    Server_ClientLeft = 7,
    Client_Quit = 8,
}