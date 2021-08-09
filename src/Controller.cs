using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace SixteenBitNuts
{
    public enum ControllerCommand
    {
        ButtonDown,
        ButtonRight,
        ButtonLeft,
        ButtonUp,
        ButtonStart,
        ButtonBack,
        ShoulderL,
        ShoulderR,
        TriggerL,
        TriggerR,
        ClickL,
        ClickR,
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
                    [ControllerCommand.ButtonDown] = Keys.C,
                    [ControllerCommand.ButtonRight] = Keys.V,
                    [ControllerCommand.ButtonLeft] = Keys.X,
                    [ControllerCommand.ButtonUp] = Keys.D,
                    [ControllerCommand.ShoulderL] = Keys.S,
                    [ControllerCommand.ShoulderR] = Keys.F,
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
                    [ControllerCommand.ButtonDown] = Buttons.A,
                    [ControllerCommand.ButtonRight] = Buttons.B,
                    [ControllerCommand.ButtonLeft] = Buttons.X,
                    [ControllerCommand.ButtonUp] = Buttons.Y,
                    [ControllerCommand.ShoulderL] = Buttons.LeftShoulder,
                    [ControllerCommand.ShoulderR] = Buttons.RightShoulder,
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
