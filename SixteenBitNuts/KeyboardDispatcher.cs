using System;
using System.Windows;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SixteenBitNuts
{
    public interface IKeyboardSubscriber
    {
        void ReceiveTextInput(char inputChar);
        void ReceiveTextInput(string text);
        void ReceiveCommandInput(char command);
        void ReceiveSpecialInput(Keys key);

        bool Selected { get; set; }
    }

    public class KeyboardDispatcher
    {
        string pasteResult;
        IKeyboardSubscriber subscriber;

        public IKeyboardSubscriber Subscriber
        {
            get
            {
                return subscriber;
            }

            set
            {
                if (subscriber != null)
                {
                    subscriber.Selected = false;
                }
                subscriber = value;

                if (value != null)
                {
                    value.Selected = true;
                }
            }
        }

        public KeyboardDispatcher(GameWindow window)
        {
            EventInput.Initialize(window);
            EventInput.CharEntered += CharEntered;
            EventInput.KeyDown += KeyDown;
        }

        void KeyDown(object sender, KeyEventArgs args)
        {
            if (subscriber == null)
            {
                return;
            }

            subscriber.ReceiveSpecialInput(args.KeyCode);
        }

        void CharEntered(object sender, CharacterEventArgs args)
        {
            if (subscriber == null)
            {
                return;
            }

            if (char.IsControl(args.Character))
            {
                if (args.Character == 0x16)
                {
                    Thread thread = new Thread(PasteThread);
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();
                    thread.Join();
                    subscriber.ReceiveTextInput(pasteResult);
                }
                else
                {
                    subscriber.ReceiveCommandInput(args.Character);
                }
            }
            else
            {
                subscriber.ReceiveTextInput(args.Character);
            }
        }

        [STAThread]
        void PasteThread()
        {
            pasteResult = "";
#if WINDOWS
            if (Clipboard.ContainsText())
            {
                pasteResult = Clipboard.GetText();
            }
#endif
        }
    }
}
