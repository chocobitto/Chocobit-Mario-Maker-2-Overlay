using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MarioMaker2Overlay.Models;

namespace MarioMaker2Overlay.Utility
{
    internal class WebsocketClientHelper
    {
        public Action<MarioMaker2OcrModel>? LevelCodeChanged;

        public async Task RunAsync()
        {
            CancellationTokenSource source = new CancellationTokenSource();
            using (var ws = new ClientWebSocket())
            {
                await ws.ConnectAsync(new Uri("ws://localhost:3000/wss"), CancellationToken.None);
                byte[] buffer = new byte[256];
                while (ws.State == WebSocketState.Open)
                {
                    var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                    }
                    else
                    {
                        HandleMessage(buffer);
                    } 
                }
            }
        }

        private void HandleMessage(byte[] buffer)
        {
            try
            {
                // if the level code has changed, fire the event
                string levelCode = string.Empty;

                string response = ASCIIEncoding.UTF8.GetString(buffer);

                response = response.TrimEnd('\0');

                MarioMaker2OcrModel? dataFromService = JsonSerializer.Deserialize<MarioMaker2OcrModel>(response);

                if (LevelCodeChanged != null && dataFromService.Level != null)
                {
                    LevelCodeChanged(dataFromService);
                }
            }
            catch (Exception ex)
            {
                //throw;
            }
        }
    }
}
