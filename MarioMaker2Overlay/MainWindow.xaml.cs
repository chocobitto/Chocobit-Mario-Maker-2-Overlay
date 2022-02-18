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

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        public void WinRate()
        {
            decimal attempts = deaths + 1;

            decimal winrate = 1 / attempts * 100;

            Winrate.Text = $"WR: {winrate:f2}%";
        }
    }
}
