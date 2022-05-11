using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace SixteenBitNuts
{
    public class VirtualStick
    {
        private struct KeyMapping
        {
            public Keys Left;
            public Keys Right;
            public Keys Up;
            public Keys Down;
        }

        private struct ButtonMapping
        {
            public PlayerIndex PlayerIndex;
            public Buttons Left;
            public Buttons Right;
            public Buttons Up;
            public Buttons Down;
        }

        public Point Value;

        private readonly Game game;
        private readonly List<KeyMapping> keyMappings = new List<KeyMapping>();
        private readonly List<ButtonMapping> buttonMappings = new List<ButtonMapping>();

        public VirtualStick(Game game)
        {
            this.game = game;
        }

        public VirtualStick AddKeys(Keys left, Keys right, Keys up, Keys down)
        {
            keyMappings.Add(new KeyMapping
            {
                Left = left,
                Right = right,
                Up = up,
                Down = down,
            });

            return this;
        }

        public VirtualStick AddButtons(PlayerIndex playerIndex, Buttons left, Buttons right, Buttons up, Buttons down)
        {
            buttonMappings.Add(new ButtonMapping
            {
                PlayerIndex = playerIndex,
                Left = left,
                Right = right,
                Up = up,
                Down = down,
            });

            return this;
        }

        public VirtualStick AddAxes()
        {
            return this;
        }

        public void Update()
        {
            Value = Point.Zero;

            foreach (var mapping in keyMappings)
            {
                Value.X |= game.InputInterface.Keyboard.GetAxis(mapping.Left, mapping.Right);
                Value.Y |= game.InputInterface.Keyboard.GetAxis(mapping.Up, mapping.Down);
            }
            foreach (var mapping in buttonMappings)
            {
                Value.X |= game.InputInterface.Buttons.GetAxis(mapping.PlayerIndex, mapping.Left, mapping.Right);
                Value.Y |= game.InputInterface.Buttons.GetAxis(mapping.PlayerIndex, mapping.Up, mapping.Down);
            }
        }
    }
}
