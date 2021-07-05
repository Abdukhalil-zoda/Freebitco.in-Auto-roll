using System.IO;
using Telegram.Bot;

namespace Chromium
{
    public class TGBot
    {
        public TelegramBotClient bot;
        public TGBot(string api)
        { 
            bot = new TelegramBotClient(api);
            bot.OnMessage += Bot_OnMessage;
        }

        public void Start()
        {
            bot.StartReceiving();
        }

        public void Stop()
        {
            bot.StopReceiving();
        }

        public delegate void MethodContainer();
        public event MethodContainer OnStop;
        public event MethodContainer OnStart;
        
        private void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            
            
            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
            {
                File.WriteAllText("MESS", e.Message.Text);
            }
            if (e.Message.Text == "0")
            {
                OnStop();
                bot.SendTextMessageAsync(e.Message.From.Id, "Bot stoped send any message to run Auto-roll", replyToMessageId: e.Message.MessageId);
            }
            else if (File.ReadAllText("MESS") == "0")
            {
                OnStart();
                bot.SendTextMessageAsync(e.Message.From.Id, "Bot started send 0 to stop Auto-roll", replyToMessageId: e.Message.MessageId);
            }
            bot.SendTextMessageAsync(e.Message.From.Id, "Send 0 to stop Auto-roll");
        }
    }
}
