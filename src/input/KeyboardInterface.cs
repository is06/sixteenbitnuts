using Microsoft.Xna.Framework.Input;

namespace SixteenBitNuts
{
    public class KeyboardInterface
    {
        public KeyboardInterface()
        {

        }

        public int GetAxis(Keys negative, Keys positive)
        {
            if (Keyboard.GetState().IsKeyDown(negative))
            {
                return -1;
            }
            else if (Keyboard.GetState().IsKeyDown(positive))
            {
                return 1;
            }
            return 0;
        }
    }
}
