using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SixteenBitNuts
{
    public class ButtonInterface
    {
        public ButtonInterface()
        {

        }

        public int GetAxis(PlayerIndex playerIndex, Buttons negative, Buttons positive)
        {
            if (GamePad.GetState(playerIndex).IsButtonDown(negative))
            {
                return -1;
            }
            else if (GamePad.GetState(playerIndex).IsButtonDown(positive))
            {
                return 1;
            }
            return 0;
        }
    }
}
