using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Chocobit.Shared.Logic;
using MarioMaker2Overlay.Models;
using MarioMaker2Overlay.Persistence;
using MarioMaker2Overlay.Utility;
using SnagFree.TrayApp.Core;

namespace ChocobitAwesomenessIndex
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Timers.Timer _showIndexTimeout = new System.Timers.Timer(5000);
        private System.Timers.Timer _hideIndexTimeout = new System.Timers.Timer(30000);
        private WebsocketClientHelper _websocketClientHelper = new WebsocketClientHelper();
        private AwesomenessIndexLogic _awesomenessIndexLogic = new();
        private PlayerRepository _playerRepository = new();
        private int _playersActiveIndex = 0;
        private List<Player> _players = new();
        private KeyboardHook _keyboardHook = new KeyboardHook(true);

        public MainWindow()
        {
            InitializeComponent();

            Action<MarioMaker2OcrModel> refreshAwesomenessIndex = (response) =>
            {
                Dispatcher.Invoke(async () =>
                {
                    await RefreshAwesomenessIndex();
                });
            };

            _websocketClientHelper.OnClear = refreshAwesomenessIndex;

            //_showIndexTimeout.Elapsed += (sender, args) =>
            //{
            //    HideWindow();
            //};

            _keyboardHook.KeyUp += OnKeyPressed;

            _players = _playerRepository.GetPlayers();
         
            Task.Run(() => ShowActivePlayer());
        }

        private async void OnKeyPressed(Keys key, bool shift, bool ctrl, bool alt)
        {
            if ((key == Keys.Right || key == Keys.Left) && ctrl && shift && alt)
            {
                if (key == Keys.Right)
                {
                    await ChangePlayer();
                }
                else
                {
                    await ChangePlayer(false);
                }

            }
        }

        private async Task ChangePlayer(bool forward = true)
        {
            if (forward)
            {
                if (_players.Count > (_playersActiveIndex + 1))
                {
                    _playersActiveIndex++;
                }
                else
                {
                    _playersActiveIndex = 0;
                }
            }
            else
            {
                if (_playersActiveIndex > 0)
                {
                    _playersActiveIndex--;
                }
                else
                {
                    _playersActiveIndex = _players.Count - 1;
                }
            }

            await ShowActivePlayer();
        }

        private async Task ShowActivePlayer()
        {
            Dispatcher.Invoke(() =>
            {
                LabelPlayer.Content = _players[_playersActiveIndex].PlayerName;
            });

            await RefreshAwesomenessIndex();
        }

        //private void HideWindow()
        //{
        //    Hide();
        //}

        //private void ShowWindow()
        //{
        //    Show();

        //    _showIndexTimeout.Stop();
        //    _showIndexTimeout.Start();
        //}

        private async Task RefreshAwesomenessIndex()
        {
            LabelAwesomenessIndex.Content = await _awesomenessIndexLogic.GetAwesomenessIndex(_players[_playersActiveIndex].PlayerId);
        }
    }
}
