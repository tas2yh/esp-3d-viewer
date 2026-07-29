using System;
using System.Net;
using System.IO;

var listener = new HttpListener();
listener.Prefixes.Add("http://localhost:8080/");
listener.Start();
Console.WriteLine("Server running on http://localhost:8080");

var directory = Directory.GetCurrentDirectory();

while (true)
{
    var context = listener.GetContext();
    var request = context.Request;
    var response = context.Response;
    
    var filePath = request.Url!.LocalPath;
    if (filePath == "/") filePath = "/index.html";
    
    var fullPath = Path.Combine(directory, filePath.TrimStart('/'));
    Console.WriteLine($"Request: {filePath} -> {fullPath} (exists: {File.Exists(fullPath)})");
    
    if (File.Exists(fullPath))
    {
        var bytes = File.ReadAllBytes(fullPath);
        response.ContentType = fullPath switch
        {
            var p when p.EndsWith(".html") => "text/html; charset=utf-8",
            var p when p.EndsWith(".js") => "application/javascript",
            var p when p.EndsWith(".css") => "text/css",
            var p when p.EndsWith(".json") => "application/json",
            _ => "text/plain"
        };
        response.OutputStream.Write(bytes, 0, bytes.Length);
    }
    else
    {
        response.StatusCode = 404;
    }
    
    response.Close();
}
