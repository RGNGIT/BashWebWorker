using BashWebWorker.Classes;
using BashWebWorker.Tools;
using Newtonsoft.Json;
using System.Net;

var httpListener = new HttpListener();
httpListener.Prefixes.Add("http://localhost:5928/");
httpListener.Start();

Logger.Debug("Application started");

while (true) 
{
    var context = httpListener.GetContext();
    var request = context.Request;
    var response = context.Response;

    if (request.HttpMethod == "POST" && request.Url?.AbsolutePath == "/cmd/smbdeletefile")
    {
        using var reader = new StreamReader(request.InputStream);
        var body = await reader.ReadToEndAsync();

        var parsedBody = JsonConvert.DeserializeObject<SambaDeleteFileRequest>(body);

        var responseText = $"Received: {body}";
        var buffer = System.Text.Encoding.UTF8.GetBytes(responseText);
        response.ContentLength64 = buffer.Length;
        await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
    }
}