using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace SixteenBitNuts
{
    public enum ControllerCommand
    {
        Button1,
        Button2,
        Button3,
        Button4,
        ButtonL,
        ButtonR,
        ButtonL3,
        ButtonR3,
        ButtonStart,
        ButtonBack,
    }
    public struct ControllerKeyboardMapping
    {
        public Dictionary<ControllerCommand, Keys> GameplayButtons;
        public Dictionary<Direction, Keys> GameplayDirections;
    }

    public struct ControllerGamePadMapping
    {
        public Dictionary<ControllerCommand, Buttons> GameplayButtons;
        public Dictionary<Direction, Buttons> GameplayDirections;
        public Dictionary<Direction, Buttons> GameplayJoystick;
    }

    public struct ControllerMapping
    {
        public ControllerKeyboardMapping Keyboard;
        public ControllerGamePadMapping GamePad;
    }

    public static class Controller
    {
        public static ControllerMapping Mapping = new ControllerMapping()
        {
            Keyboard = new ControllerKeyboardMapping()
            {
                GameplayButtons = new Dictionary<ControllerCommand, Keys>()
                {
                    [ControllerCommand.Button1] = Keys.C,
                    [ControllerCommand.Button2] = Keys.V,
                    [ControllerCommand.Button3] = Keys.X,
                    [ControllerCommand.Button4] = Keys.D,
                    [ControllerCommand.ButtonL] = Keys.S,
                    [ControllerCommand.ButtonR] = Keys.F,
                    [ControllerCommand.ButtonStart] = Keys.Enter,
                    [ControllerCommand.ButtonBack] = Keys.Escape,
                },
                GameplayDirections = new Dictionary<Direction, Keys>()
                {
                    [Direction.Right] = Keys.Right,
                    [Direction.Top] = Keys.Up,
                    [Direction.Left] = Keys.Left,
                    [Direction.Bottom] = Keys.Down,
                },
            },
            GamePad = new ControllerGamePadMapping()
            {
                GameplayButtons = new Dictionary<ControllerCommand, Buttons>()
                {
                    [ControllerCommand.Button1] = Buttons.A,
                    [ControllerCommand.Button2] = Buttons.B,
                    [ControllerCommand.Button3] = Buttons.X,
                    [ControllerCommand.Button4] = Buttons.Y,
                    [ControllerCommand.ButtonL] = Buttons.LeftTrigger,
                    [ControllerCommand.ButtonR] = Buttons.RightTrigger,
                    [ControllerCommand.ButtonStart] = Buttons.Start,
                    [ControllerCommand.ButtonBack] = Buttons.Back,
                },
                GameplayDirections = new Dictionary<Direction, Buttons>()
                {
                    [Direction.Right] = Buttons.DPadRight,
                    [Direction.Top] = Buttons.DPadUp,
                    [Direction.Left] = Buttons.DPadLeft,
                    [Direction.Bottom] = Buttons.DPadDown,
                },
                GameplayJoystick = new Dictionary<Direction, Buttons>()
                {
                    [Direction.Right] = Buttons.LeftThumbstickRight,
                    [Direction.Top] = Buttons.LeftThumbstickUp,
                    [Direction.Left] = Buttons.LeftThumbstickLeft,
                    [Direction.Bottom] = Buttons.LeftThumbstickDown,
                },
            }
        };

        public static bool IsDirectionPressed(PlayerIndex playerIndex, Direction direction)
        {
            if (Mapping.Keyboard.GameplayDirections.ContainsKey(direction)
                && Keyboard.GetState().IsKeyDown(Mapping.Keyboard.GameplayDirections[direction]))
            {
                return true;
            }
            if (Mapping.GamePad.GameplayDirections.ContainsKey(direction)
                && GamePad.GetState(playerIndex).IsButtonDown(Mapping.GamePad.GameplayDirections[direction]))
            {
                return true;
            }
            if (Mapping.GamePad.GameplayJoystick.ContainsKey(direction)
                && GamePad.GetState(playerIndex).IsButtonDown(Mapping.GamePad.GameplayJoystick[direction]))
            {
                return true;
            }

            return false;
        }

        public static bool IsDirectionReleased(PlayerIndex playerIndex, Direction direction)
        {
            if (Mapping.Keyboard.GameplayDirections.ContainsKey(direction)
                && Keyboard.GetState().IsKeyUp(Mapping.Keyboard.GameplayDirections[direction]))
            {
                if (Mapping.GamePad.GameplayDirections.ContainsKey(direction)
                    && GamePad.GetState(playerIndex).IsButtonUp(Mapping.GamePad.GameplayDirections[direction]))
                {
                    if (Mapping.GamePad.GameplayJoystick.ContainsKey(direction)
                        && GamePad.GetState(playerIndex).IsButtonUp(Mapping.GamePad.GameplayJoystick[direction]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool IsCommandPressed(PlayerIndex playerIndex, ControllerCommand command)
        {
            if (Mapping.Keyboard.GameplayButtons.ContainsKey(command)
                && Keyboard.GetState().IsKeyDown(Mapping.Keyboard.GameplayButtons[command]))
            {
                return true;
            }
            if (Mapping.GamePad.GameplayButtons.ContainsKey(command)
                && GamePad.GetState(playerIndex).IsButtonDown(Mapping.GamePad.GameplayButtons[command]))
            {
                return true;
            }

            return false;
        }

        public static bool IsCommandReleased(PlayerIndex playerIndex, ControllerCommand command)
        {
            if (Mapping.Keyboard.GameplayButtons.ContainsKey(command)
                && Keyboard.GetState().IsKeyUp(Mapping.Keyboard.GameplayButtons[command]))
            {
                if (Mapping.GamePad.GameplayButtons.ContainsKey(command)
                    && GamePad.GetState(playerIndex).IsButtonUp(Mapping.GamePad.GameplayButtons[command]))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
