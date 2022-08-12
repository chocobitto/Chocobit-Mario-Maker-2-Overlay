using System;
using System.IO;
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
        public Action<MarioMaker2OcrModel>? OnLevelCodeChanged;
        public Action<MarioMaker2OcrModel>? OnMarioDeath;
        public Action<MarioMaker2OcrModel>? OnStartOver;
        public Action<MarioMaker2OcrModel>? OnClear;
        public Action? OnExitOrQuit;
        public Action? OnSkip;
        public Action? OnWorldRecord;
        public Action? OnFirstClear;

        public async Task RunAsync()
        {
            CancellationTokenSource source = new CancellationTokenSource();

            using (var webSocketClient = new ClientWebSocket())
            {
                await webSocketClient.ConnectAsync(new Uri("ws://localhost:3000/wss"), CancellationToken.None);
                while (webSocketClient.State == WebSocketState.Open)
                {

                    ArraySegment<Byte> buffer = new ArraySegment<byte>(new Byte[8192]);
                    WebSocketReceiveResult? result = null;

                    using (var memoryStream = new MemoryStream())
                    {
                        try
                        {
                            do
                            {
                                result = await webSocketClient.ReceiveAsync(buffer, CancellationToken.None);
                                memoryStream.Write(buffer.Array, buffer.Offset, result.Count);
                            }
                            while (!result.EndOfMessage);

                            memoryStream.Seek(0, SeekOrigin.Begin);

                            if (result.MessageType == WebSocketMessageType.Close)
                            {
                                await webSocketClient.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                            }
                            else
                            {
                                HandleMessage(memoryStream.GetBuffer());
                            }
                        }
                        catch (Exception ex)
                        {
                            //throw;
                        }
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

                if (OnLevelCodeChanged != null && dataFromService.Level != null)
                {
                    OnLevelCodeChanged(dataFromService);
                }
                else if (OnMarioDeath != null && (dataFromService?.Type.Equals("death", StringComparison.OrdinalIgnoreCase) ?? false))
                {
                    OnMarioDeath(dataFromService);
                }
                else if (OnStartOver != null && (dataFromService?.Type.Equals("restart", StringComparison.OrdinalIgnoreCase) ?? false))
                {
                    OnStartOver(dataFromService);
                }
                else if (OnClear != null && (dataFromService?.Type.Equals("clear", StringComparison.OrdinalIgnoreCase) ?? false))
                {
                    OnClear(dataFromService);
                }
                else if (OnExitOrQuit != null && (dataFromService?.Type.Equals("exit", StringComparison.OrdinalIgnoreCase) ?? false))
                {
                    OnExitOrQuit();
                }
                else if (OnSkip != null && (dataFromService?.Type.Equals("skip", StringComparison.OrdinalIgnoreCase) ?? false))
                {
                    OnSkip();
                }
                else if (OnWorldRecord != null && (dataFromService?.Type.Equals("worldrecord", StringComparison.OrdinalIgnoreCase) ?? false))
                {
                    OnWorldRecord();
                }
                else if (OnFirstClear != null && (dataFromService?.Type.Equals("firstclear", StringComparison.OrdinalIgnoreCase) ?? false))
                {
                    OnFirstClear();
                }
            }
            catch (Exception ex)
            {
                //throw;
            }
        }
    }
}
