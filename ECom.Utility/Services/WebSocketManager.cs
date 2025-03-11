using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ECom.Utility.Interface;
using Microsoft.AspNetCore.Http;

namespace ECom.Utility.Services
{
    public class WebSocketManager : IWebSocketManager
    {
        private readonly List<WebSocket> _sockets = new List<WebSocket>();

        public async Task HandleConnection(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                _sockets.Add(webSocket);

                await ListenForMessages(webSocket);
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }

        private async Task ListenForMessages(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];

            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
                    _sockets.Remove(webSocket);
                }
            }
        }

        public async Task BroadcastMessageAsync(string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            var buffer = new ArraySegment<byte>(messageBytes);

            foreach (var socket in _sockets)
            {
                if (socket.State == WebSocketState.Open)
                {
                    await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
}
