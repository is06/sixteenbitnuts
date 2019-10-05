using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SixteenBitNuts
{
    public delegate void TextBoxEvent(TextBox sender);

    public class TextBox : IKeyboardSubscriber
    {
        private readonly SpriteBatch spriteBatch;
        private readonly Texture2D texture;
        private readonly SpriteFont font;

        public event TextBoxEvent OnEnterPressed;
        public event TextBoxEvent OnTabPressed;

        public Rectangle Bounds { get; set; }
        public string Text
        {
            get
            {
                return text;
            }

            set
            {
                text = value;
                if (text == null)
                {
                    text = "";
                }

                if (text != "")
                {
                    string filtered = "";

                    foreach (char c in value)
                    {
                        if (font.Characters.Contains(c))
                        {
                            filtered += c;
                        }
                    }

                    text = filtered;

                    while (font.MeasureString(text).X > Bounds.Width)
                    {
                        text = text.Substring(0, text.Length - 1);
                    }
                }
            }
        }

        bool IKeyboardSubscriber.Selected { get; set; }

        private string text;

        public TextBox(Game game)
        {
            Text = "";

            spriteBatch = new SpriteBatch(game.GraphicsDevice);
            texture = game.Content.Load<Texture2D>("sprites/pixel");
            font = game.Content.Load<SpriteFont>("fonts/console");
        }

        public void Update()
        {
        }

        public void Draw(GameTime gameTime)
        {
            bool caretIsVisible = true;
            if ((gameTime.TotalGameTime.TotalMilliseconds % 1000) < 500)
                caretIsVisible = false;

            spriteBatch.Begin();
            spriteBatch.Draw(texture, Bounds, new Rectangle(0, 0, 1, 1), Color.FromNonPremultiplied(20, 20, 20, 180));
            spriteBatch.DrawString(font, text, new Vector2(Bounds.X + 4, Bounds.Y + 4), Color.White);

            Vector2 size = font.MeasureString(text);

            if (caretIsVisible)
            {
                spriteBatch.Draw(
                    texture,
                    new Rectangle(Bounds.X + (int)size.X + 4, Bounds.Y + 4, 2, 16),
                    new Rectangle(0, 0, 1, 1),
                    Color.White
                );
            }

            spriteBatch.End();
        }

        void IKeyboardSubscriber.ReceiveTextInput(char inputChar)
        {
            Text += inputChar;
        }

        void IKeyboardSubscriber.ReceiveTextInput(string text)
        {
            Text += text;
        }

        void IKeyboardSubscriber.ReceiveCommandInput(char command)
        {
            switch (command)
            {
                case '\b':
                    if (Text.Length > 0)
                    {
                        Text = Text.Substring(0, Text.Length - 1);
                    }
                    break;
                case '\r':
                    OnEnterPressed?.Invoke(this);
                    Text = "";
                    break;
                case '\t':
                    OnTabPressed?.Invoke(this);
                    break;
                default:
                    break;
            }
        }

        void IKeyboardSubscriber.ReceiveSpecialInput(Keys key)
        {
            
        }
    }
}
