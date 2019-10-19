using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SixteenBitNuts
{
    public delegate void ConsoleEvent(Console sender, ConsoleEventArgs args);

    public class ConsoleEventArgs : EventArgs
    {
        public string[] Parameters { get; }

        public ConsoleEventArgs(string[] parameters)
        {
            Parameters = parameters;
        }
    }

    public class Console
    {
        private bool enabled;
        private readonly TextBox textBox;
        private readonly KeyboardDispatcher keyboardDispatcher;

        public event ConsoleEvent OnLoadTitleScreen;
        public event ConsoleEvent OnLoadMap;
        public event ConsoleEvent OnExitGame;
        public event ConsoleEvent OnEditSection;
        public event ConsoleEvent OnEditMap;

        public bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                enabled = value;
                keyboardDispatcher.Subscriber = enabled ? textBox : null;
            }
        }

        public Console(Game game, SpriteBatch spriteBatch, KeyboardDispatcher keyboardDispatcher)
        {
            this.keyboardDispatcher = keyboardDispatcher;

            textBox = new TextBox(game, spriteBatch)
            {
                Bounds = new Rectangle(32, 754, 600, 24)
            };
            textBox.OnEnterPressed += CommandEntered;
        }

        public void Update()
        {
            if (Enabled)
            {
                textBox.Update();
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (Enabled)
            {
                textBox.Draw(gameTime);
            }
        }

        private void CommandEntered(TextBox sender)
        {
            string command = sender.Text;
            string[] components = command.Split(' ');

            switch (components[0])
            {
                case "titlescreen":
                    OnLoadTitleScreen?.Invoke(this, null);
                    break;
                case "map":
                    OnLoadMap?.Invoke(this, new ConsoleEventArgs(new string[] { components[1] }));
                    break;
                case "layout":
                    OnEditMap?.Invoke(this, null);
                    break;
                case "edit":
                    OnEditSection?.Invoke(this, null);
                    break;
                case "quit":
                case "exit":
                    OnExitGame?.Invoke(this, null);
                    break;
            }
        }
    }
}
