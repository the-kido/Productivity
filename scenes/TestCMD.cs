using System;
using System.Threading.Tasks;
using OllamaSharp;
using Godot;
using System.Linq;
using OllamaSharp.Models.Chat;

public partial class TestCMD : Node
{
	public override void _Ready()
	{
		var uri = new Uri("http://localhost:11434");
        var ollama = new OllamaApiClient(uri)
        {
            // select a model which should be used for further operations
            SelectedModel = "Modelfile"
        };

		ongoingChat = new Chat(ollama);
	}


	public async void SendChatMessage(AIMessage message, string msg)
	{
        void Call() => UpdateMessage(message, msg);
        
		await Task.Run(Call);
	}

	public void RemoveMessage(string content)
	{
		var msgs = ongoingChat.Messages.ToList();
		Message delted = msgs.Find( (msg) => msg.Content == content);
		msgs.Remove(delted);

		ongoingChat.SetMessages(msgs);
	}

	Chat ongoingChat;

	async void UpdateMessage(AIMessage message, string msg)
	{
		string built = "";
		await foreach (var answerToken in ongoingChat.Send(msg))
		{
			built += answerToken;
			CallDeferred("UpdateText", message.text, built);
		}
		CallDeferred("MakeDeleteButtonVisible", message);
	}

	public void UpdateText(Label label, string text)
	{
		label.GetParent<Control>().Visible = true;
		label.Text = text;
	}
	public void MakeDeleteButtonVisible(AIMessage message)
	{
		message.delete.Visible = true;
	}
}