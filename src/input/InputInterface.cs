namespace SixteenBitNuts
{
    public class InputInterface
    {
        public KeyboardInterface Keyboard;
        public ButtonInterface Buttons;

        public InputInterface()
        {
            Keyboard = new KeyboardInterface();
            Buttons = new ButtonInterface();
        }
    }
}
