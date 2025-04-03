using System;
using System.Collections.Generic;
using Fleck;
using Godot;

partial class ChromeTabDetector 
{
    public bool OnYoutube { get; private set; }

    readonly WebSocketServer server;
    readonly List<IWebSocketConnection> connections = new();
    public ChromeTabDetector()
    {
        server = new("ws://0.0.0.0:8080");

        server.Start(
            connection => 
            {
                connections.Add(connection);
                connection.OnMessage = OnMessage;
                connection.OnMessage += (msg) => 
                {
                    if (msg == "Reset Cooldown") 
                    {
                        connections.ForEach(conection => conection.Send("Reset Cooldown"));
                    }
                };
            }
        );
    }

    private void OnMessage(string msg) 
    {
        GD.Print(msg);
        if (msg == "On Youtube") OnYoutube = true;
        if (msg == "Not On Youtube") OnYoutube = false;
        OnYoutube = msg == "On Youtube";
    }
}