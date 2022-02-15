using System;
using System.Runtime.InteropServices;
using System.Windows;
using SnagFree.TrayApp.Core;

namespace MarioMaker2Overlay
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SetupKeyboardHooks();
            WindowStyle = WindowStyle.None;
        }

        int deaths = 0;

        private GlobalKeyboardHook _globalKeyboardHook;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };
        public static Point GetMousePosition()
        {
            var w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);

            return new Point(w32Mouse.X, w32Mouse.Y);
        }

        public void SetupKeyboardHooks()
        {
            _globalKeyboardHook = new GlobalKeyboardHook();
            _globalKeyboardHook.KeyboardPressed += OnKeyPressed;
        }

        private void OnKeyPressed(object sender, GlobalKeyboardHookEventArgs e)
        {
            if (e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyUp)
            {
                switch (e.KeyboardData.VirtualCode)
                {
                    case GlobalKeyboardHook.VkUp:
                        deaths += 1;
                        DeathCount.Text = $"Deaths: {deaths}";
                        WinRate();
                        break;
                    case GlobalKeyboardHook.VkDown:
                        deaths -= 1;
                        DeathCount.Text = $"Deaths: {deaths}";
                        WinRate();
                        break;
                }

            }
        }
        public void Dispose()
        {
            _globalKeyboardHook?.Dispose();
        }

        bool _trackMouse = false;
        Point? _myPoint = null;

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                _myPoint = e.GetPosition(Window);
                _trackMouse = true;
            }
        }

        public void WinRate()
        {
            decimal attempts = deaths + 1;

            decimal winrate = 1 / attempts * 100;

            Winrate.Text = $"WR: {winrate:f2}%";
        }

        private void Window_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Released)
            {
                _trackMouse = false;
            }
        }

        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_trackMouse)
            {
                if (_myPoint.HasValue)
                {
                    Point myPoint = GetMousePosition();
                    Left = myPoint.X - _myPoint.Value.X;
                    Top = myPoint.Y - _myPoint.Value.Y;
                }
            }
        }
    }
}
