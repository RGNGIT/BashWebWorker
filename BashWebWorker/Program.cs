using BashWebWorker.Classes;
using BashWebWorker.Cmd;
using BashWebWorker.Cmd.Interfaces;
using BashWebWorker.Tools;
using Newtonsoft.Json;
using System.Net;

public class Program
{
    public static async Task Main(string[] args)
    {
        await RunApp();
    }

    public static async Task RunApp()
    {
        var httpListener = new HttpListener();
        httpListener.Prefixes.Add("http://localhost:5928/");
        httpListener.Start();

        Logger.Debug("Application started");

        while (true)
        {
            var context = await httpListener.GetContextAsync();
            var request = context.Request;
            var response = context.Response;

            if (request.HttpMethod == "POST" && request.Url?.AbsolutePath == "/cmd/smbdeletefile")
            {
                Logger.Debug($"Call smbdeletefile");
                using var reader = new StreamReader(request.InputStream);
                var body = await reader.ReadToEndAsync();

                var parsedBody = JsonConvert.DeserializeObject<SambaDeleteFileRequest>(body);
                ICommand command = new CmdSambaDeleteFile();
                command.Run(parsedBody.FolderPath, parsedBody.FileName);

                var responseText = $"Received: {body}. OK";
                var buffer = System.Text.Encoding.UTF8.GetBytes(responseText);
                response.ContentLength64 = buffer.Length;
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            }
        }
    }
}