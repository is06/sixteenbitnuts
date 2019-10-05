using System;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SixteenBitNuts
{
    public class KeyboardLayout
    {
        const int KL_NAMELENGTH = 9;

        [DllImport("user32.dll")]
        private static extern long LoadKeyboardLayout(string pwszKLID, uint Flags);

        [DllImport("user32.dll")]
        private static extern long GetKeyboardLayoutName(StringBuilder pwszKLID);

        public static string GetName()
        {
            StringBuilder name = new StringBuilder(KL_NAMELENGTH);
            GetKeyboardLayoutName(name);
            return name.ToString();
        }
    }

    public class CharacterEventArgs : EventArgs
    {
        public char Character { get; }
        public int Param { get; }
        public int RepeatCount
        {
            get
            {
                return Param & 0xffff;
            }
        }
        public bool ExtendedKey
        {
            get
            {
                return (Param & (1 << 24)) > 0;
            }
        }
        public bool AltPressed
        {
            get
            {
                return (Param & (1 << 29)) > 0;
            }
        }
        public bool PreviousState
        {
            get
            {
                return (Param & (1 << 30)) > 0;
            }
        }
        public bool TransitionState
        {
            get
            {
                return (Param & (1 << 31)) > 0;
            }
        }

        public CharacterEventArgs(char character, int param)
        {
            Character = character;
            Param = param;
        }
    }

    public class KeyEventArgs : EventArgs
    {
        public Keys KeyCode { get; }

        public KeyEventArgs(Keys keyCode)
        {
            KeyCode = keyCode;
        }
    }

    public delegate void CharEnteredHandler(object sender, CharacterEventArgs args);
    public delegate void KeyEventHandler(object sender, KeyEventArgs args);

    public static class EventInput
    {
        public static event CharEnteredHandler CharEntered;
        public static event KeyEventHandler KeyDown;
        public static event KeyEventHandler KeyUp;

        delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        static bool initialized;
        static IntPtr prevWndProc;
        static WndProc hookProcDelegate;
        static IntPtr hIMC;

        const int GWL_WNDPROC = -4;
        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;
        const int WM_CHAR = 0x102;
        const int WM_IME_SETCONTEXT = 0x0281;
        const int WM_INPUTLANGCHANGE = 0x51;
        const int WM_GETDLGCODE = 0x87;
        const int DLGC_WANTALLKEYS = 4;

        [DllImport("Imm32.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr ImmGetContext(IntPtr hWnd);

        [DllImport("Imm32.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr ImmAssociateContext(IntPtr hWnd, IntPtr hIMC);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        static IntPtr HookProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            IntPtr returnCode = CallWindowProc(prevWndProc, hWnd, msg, wParam, lParam);

            switch (msg)
            {
                case WM_GETDLGCODE:
                    returnCode = (IntPtr)(returnCode.ToInt32() | DLGC_WANTALLKEYS);
                    break;
                case WM_KEYDOWN:
                    KeyDown?.Invoke(null, new KeyEventArgs((Keys)wParam));
                    break;
                case WM_KEYUP:
                    KeyUp?.Invoke(null, new KeyEventArgs((Keys)wParam));
                    break;
                case WM_CHAR:
                    CharEntered?.Invoke(null, new CharacterEventArgs((char)wParam, lParam.ToInt32()));
                    break;
                case WM_IME_SETCONTEXT:
                    if (wParam.ToInt32() == 1)
                        ImmAssociateContext(hWnd, hIMC);
                    break;
                case WM_INPUTLANGCHANGE:
                    ImmAssociateContext(hWnd, hIMC);
                    returnCode = (IntPtr)1;
                    break;
            }

            return returnCode;
        }

        public static void Initialize(GameWindow window)
        {
            if (initialized)
            {
                throw new InvalidOperationException("TextInput.Initialize can only be called once!");
            }

            hookProcDelegate = new WndProc(HookProc);
            prevWndProc = (IntPtr)SetWindowLong(
                window.Handle,
                GWL_WNDPROC,
                (int)Marshal.GetFunctionPointerForDelegate(hookProcDelegate)
            );

            hIMC = ImmGetContext(window.Handle);
            initialized = true;
        }
    }
}
