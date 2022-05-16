using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace SixteenBitNuts
{
    public class VirtualButton
    {
        /// <summary>
        /// Represents the key mapping for this virtual button
        /// </summary>
        private class KeyMapping
        {
            public Keys Key;

            public bool IsPressed;
            public bool IsReleased;
            public bool IsPressedOnce;

            private bool isBufferPressed;

            /// <summary>
            /// Updates the statuses of this virtual button key mapping according to the keyboard state
            /// </summary>
            public void Update()
            {
                IsPressed = false;
                IsReleased = false;
                IsPressedOnce = false;

                if (Keyboard.GetState().IsKeyDown(Key))
                {
                    IsPressed = true;
                }
                if (Keyboard.GetState().IsKeyUp(Key))
                {
                    IsReleased = true;
                    isBufferPressed = false;
                }
                if (isBufferPressed == false)
                {
                    if (Keyboard.GetState().IsKeyDown(Key))
                    {
                        IsPressedOnce = true;
                        isBufferPressed = true;
                    }
                }
            }
        }

        private class ButtonMapping
        {
            public PlayerIndex PlayerIndex;
            public Buttons Button;

            public bool IsPressed;
            public bool IsReleased;
            public bool IsPressedOnce;

            private bool isBufferPressed;

            public void Update()
            {
                IsPressed = false;
                IsReleased = false;
                IsPressedOnce = false;

                if (GamePad.GetState(PlayerIndex).IsButtonDown(Button))
                {
                    IsPressed = true;
                }
                if (GamePad.GetState(PlayerIndex).IsButtonUp(Button))
                {
                    IsReleased = true;
                    isBufferPressed = false;
                }
                if (isBufferPressed == false)
                {
                    if (GamePad.GetState(PlayerIndex).IsButtonDown(Button))
                    {
                        IsPressedOnce = true;
                        isBufferPressed = true;
                    }
                }
            }
        }

        private readonly List<KeyMapping> keyMappings;
        private readonly List<ButtonMapping> buttonMappings;

        public VirtualButton()
        {
            keyMappings = new List<KeyMapping>();
            buttonMappings = new List<ButtonMapping>();
        }

        public VirtualButton AddKey(Keys key)
        {
            keyMappings.Add(new KeyMapping()
            {
                Key = key,
                IsPressed = false,
                IsReleased = false,
                IsPressedOnce = false,
            });
            return this;
        }

        public VirtualButton AddButton(PlayerIndex playerIndex, Buttons button)
        {
            buttonMappings.Add(new ButtonMapping()
            {
                PlayerIndex = playerIndex,
                Button = button,
                IsPressed = false,
                IsReleased = false,
                IsPressedOnce = false,
            });
            return this;
        }

        public void Update()
        {
            foreach (var mapping in keyMappings)
            {
                mapping.Update();
            }
            foreach (var mapping in buttonMappings)
            {
                mapping.Update();
            }
        }

        public bool IsPressed()
        {
            foreach (var mapping in keyMappings)
            {
                if (mapping.IsPressed)
                {
                    return true;
                }
            }
            foreach (var mapping in buttonMappings)
            {
                if (mapping.IsPressed)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsReleased()
        {
            foreach (var mapping in keyMappings)
            {
                if (!mapping.IsReleased)
                {
                    return false;
                }
            }
            foreach (var mapping in buttonMappings)
            {
                if (!mapping.IsReleased)
                {
                    return false;
                }
            }
            
            return true;
        }

        public bool IsPressedOnce()
        {
            foreach (var mapping in keyMappings)
            {
                if (mapping.IsPressedOnce)
                {
                    return true;
                }
            }
            foreach (var mapping in buttonMappings)
            {
                if (mapping.IsPressedOnce)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
