using System;
using Fleck;
using Godot;

partial class ChromeTabDetector 
{
    public bool OnYoutube { get; private set; }

    readonly WebSocketServer server;
    public ChromeTabDetector()
    {
        server = new("ws://0.0.0.0:8080");

        server.Start(
            connection => {
            connection.OnMessage = OnMessage;
            }
        );
    }

    private void OnMessage(string msg) => OnYoutube = msg.Substr(0,23) == "https://www.youtube.com";
}