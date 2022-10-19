using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Forms;
using MarioMaker2Overlay.Models;
using MarioMaker2Overlay.Persistence;
using MarioMaker2Overlay.Utility;
using Serilog;
using SnagFree.TrayApp.Core;

namespace MarioMaker2Overlay
{
    public partial class MainWindow : Window
    {
        private System.Timers.Timer _updateDatabaseTimer = new System.Timers.Timer(5000);
        private OverlayLevelData _levelData = new();
        private LevelDataRepository _levelDataRepository = new();
        private NintendoServiceClient _nintendoServiceClient = new(new HttpClient());
        private System.Timers.Timer _gameTimer = new System.Timers.Timer(20);
        public decimal _time;
        private StopwatchWithOffset _stopwatch = new();
        private WebsocketClientHelper _websocketClientHelper = new WebsocketClientHelper();
        private PlayerRepository _playerRepository = new();
        private List<Player> _players = new();
        private int _playersActiveIndex = 0;
        private KeyboardHook _keyboardHook = new KeyboardHook(true);
        private ILogger? _logger = null;

        public MainWindow()
        {
            InitializeComponent();

            _players = _playerRepository.GetPlayers();

            _keyboardHook.KeyUp += OnKeyPressed;

            InitializeAllFieldsToDefaults();

            _websocketClientHelper.OnLevelCodeChanged = (response) =>
            {
                if (response.Level != null)
                {
                    Dispatcher.Invoke(() =>
                    {
                        LevelCode.Text = response.Level.Code;
                    });
                }
            };

            Action<MarioMaker2OcrModel> marioDeath = (response) =>
            {
                _levelData.PlayerDeaths++;

                Dispatcher.Invoke(() =>
                {
                    UpdateUi();
                });
            };

            _websocketClientHelper.OnMarioDeath = marioDeath;
            _websocketClientHelper.OnStartOver = marioDeath;

            _websocketClientHelper.OnClear = (response) =>
            {
                Dispatcher.Invoke(() =>
                {
                    Regex timeParser = new Regex(@"(?<minutes>\d\d)'(?<seconds>\d\d)""(?<tenths>\d\d\d)");

                    Match match = timeParser.Match(response.Data);
                    
                    TimeSpan? clearTime = null;

                    if (match.Success && int.TryParse(match.Groups["minutes"].Value, out int minutes)
                        && int.TryParse(match.Groups["seconds"].Value, out int seconds)
                        && int.TryParse(match.Groups["tenths"].Value, out int tenths))
                    {
                        clearTime = new TimeSpan(0, 0, minutes, seconds, tenths);
                    }

                    LevelCleared(clearTime);
                });
            };

            _websocketClientHelper.OnWorldRecord = WorldRecord;
            _websocketClientHelper.OnFirstClear = FirstClear;

            Task.Run(async () => await _websocketClientHelper.RunAsync());

            WindowStyle = WindowStyle.None;

            _gameTimer.Elapsed += CurrentTimeElapsed;
            _gameTimer.Enabled = true;

            _updateDatabaseTimer.Elapsed += TryUpsertLevel;
            _updateDatabaseTimer.Enabled = true;

            // start logger
            _logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.File("log\\log.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 10)
                .CreateLogger();

            _logger.Information("Logger Initialized");
        }

        protected async override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            await ChangePlayer();
        }

        private void TryUpsertLevel(object? sender, ElapsedEventArgs e)
        {
            Upsert();
        }

        private void Upsert()
        {
            // Check to see if the data exists
            if (_levelData?.LevelDataId > 0 || !string.IsNullOrWhiteSpace(_levelData?.Code))
            {
                // if it exists update it
                LevelData levelData;

                levelData = LocalToDb();

                _levelDataRepository.Upsert(levelData);
            }
        }

        private async Task ChangePlayer(bool forward = true)
        {
            if (_players != null && _players.Count > 0)
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
        }

        private async Task ShowActivePlayer()
        {
            Dispatcher.Invoke(() =>
            {
                LabelPlayerName.Content = _players[_playersActiveIndex].PlayerName;
            });

            string levelCode = _levelData?.Code ?? string.Empty;

            if (IsValidLevelCode(levelCode))
            {
                await LoadLevelWithWaitAndCancel(levelCode);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            FinishOutLevel();

            base.OnClosing(e);
        }

        private void FinishOutLevel()
        {
            _stopwatch.Stop();
            _updateDatabaseTimer.Stop();

            Upsert();
        }

        private async void OnKeyPressed(Keys key, bool shift, bool ctrl, bool alt)
        {
            if (ctrl && shift && alt)
            {
                if (key == Keys.Right)
                {
                    await ChangePlayer();
                }
                else if (key == Keys.Left)
                {
                    await ChangePlayer(false);
                }
                else if (key == Keys.Up)
                {
                    _levelData.PlayerDeaths++;
                    UpdateUi();
                }
                else if (key == Keys.Down)
                {
                    if (_levelData.PlayerDeaths > 0)
                    {
                        _levelData.PlayerDeaths--;
                        UpdateUi();
                    }
                }
            }
        }

        private void UpdateUi()
        {
            if (!string.IsNullOrWhiteSpace(_levelData.Code) && !string.IsNullOrWhiteSpace(_levelData.Name))
            {
                LabelDeathCount.Content = $"{_levelData.PlayerDeaths}";
                LabelClears.Content = $"{_levelData.Clears}/{_levelData.Attempts} ({_levelData.ClearRate})";
                LabelTags.Content = string.Join(", ", _levelData.TagsName) ?? "--";
                LabelLevelName.Content = $"{_levelData.Name}";
                LabelDifficultyName.Content = $"({_levelData.DifficultyName})";
                LabelLikes.Content = _levelData.Likes;
                LabelBoos.Content = _levelData.Boos;
                LabelWorldRecord.Content = $"{_levelData.WorldRecord}";
                LabelLikeRatio.Content = $"({_levelData.LikeRatio}:1)";
                LabelClearCheckTime.Content = _levelData.ClearCheckTime;

                CalculateWinRate();
            }
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        public void CalculateWinRate()
        {
            decimal attempts = _levelData.PlayerDeaths + 1;

            if(attempts > 0)
            {
                decimal winrate = 1 / attempts * 100;

                LabelCalculatedWinRate.Content = $"{winrate:f2}%";
            }
            else
            {
                LabelCalculatedWinRate.Content = string.Empty;
            }
        }

        private void Button_ClickUpdate(object sender, RoutedEventArgs e)
        {
            UpdateUi();

            LevelData levelData;

            levelData = LocalToDb();

            _levelDataRepository.Upsert(levelData);
        }

        private void Button_ClickInsert(object sender, RoutedEventArgs e)
        {
            UpdateUi();

            LevelData levelData;

            levelData = LocalToDb();

            _levelDataRepository.Insert(levelData);
        }

        private LevelData LocalToDb()
        {
            LevelData data = new();

            data.Code = _levelData.Code?.Replace("-", string.Empty);
            data.PlayerDeaths = _levelData.PlayerDeaths;
            data.TotalGlobalAttempts = _levelData.Attempts;
            data.TotalGlobalClears = _levelData.Clears;
            data.TimeElapsed = _stopwatch.ElapsedTicks;
            data.ClearCondition = _levelData.ClearCondition;
            data.ClearConditionMagnitude = _levelData.ClearConditionMagnitude;
            data.ClearTime = _levelData.ClearTime;
            data.DateTimeUploaded = _levelData.DateTimeUploaded;
            data.Difficulty = _levelData.DifficultyName;
            data.GameStyle = _levelData.GameStyle;
            data.Tags = String.Join(",", _levelData.TagsName);
            data.Theme = _levelData.Theme;
            data.TotalGlobalAttempts = _levelData.TotalGlobalAttempts;
            data.TotalGlobalClears = _levelData.TotalGlobalClears;
            data.PlayerId = _players[_playersActiveIndex].PlayerId;

            return data;
        }

        private void CurrentTimeElapsed(object? sender, ElapsedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_levelData.Code) && IsValidLevelCode(_levelData.Code))
            {
                Dispatcher.Invoke(() =>
                {
                    LabelGameTime.Content = $"{_stopwatch.Elapsed.ToString("hh\\:mm\\:ss\\.ff")}";
                });
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Random myRandom = new Random((int)DateTime.Now.Ticks);

            for(int i = 0; i < 1000000; i++)
            {
                LevelData newLevelData = new LevelData { Code = myRandom.Next(100000000, 999999999).ToString(), PlayerDeaths = myRandom.Next(5, 300), TotalGlobalAttempts = myRandom.Next(100, 10000), TotalGlobalClears = myRandom.Next(10, 600) };

                _levelDataRepository.Insert(newLevelData);
            }
        }

        private void InitializeAllFieldsToDefaults()
        {
            LabelLevelName.Content = "---";
            LabelDifficultyName.Content = "(---)";
            LabelLikes.Content = "-";
            LabelBoos.Content = "-";
            LabelLikeRatio.Content = "(1.0)";
            LabelClears.Content = "---/--- (0%)";
            LabelWorldRecord.Content = "00:00:00";
            LabelTags.Content = "--, --";
            LabelGameTime.Content = "00:00:00";
            LabelCalculatedWinRate.Content = "(100%)";
            LabelDeathCount.Content = "0";
        }

        private bool IsValidLevelCode(string levelCode)
        {
            bool result = Regex.IsMatch(levelCode, "[0-9A-HJ-NP-Y][0-9A-HJ-NP-Y][0-9A-HJ-NP-Y]-?[0-9A-HJ-NP-Y][0-9A-HJ-NP-Y][0-9A-HJ-NP-Y]-?[0-9A-HJ-NP-Y][0-9A-HJ-NP-Y][0-9A-HJ-NP-Y]");

            return result;
        }

        CancellationTokenSource _loadLevelCancellationTokenSource = new CancellationTokenSource();

        private async void LevelCode_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string levelCode = LevelCode.Text?.Replace("-", string.Empty) ?? string.Empty;

            if (IsValidLevelCode(levelCode))
            {
                await LoadLevelWithWaitAndCancel(levelCode);

                //ThreadPool.QueueUserWorkItem(new WaitCallback(LoadLevel), _loadLevelCancellationTokenSource.Token);

                //Thread loadLevelThread = new ThreadStart(() => LoadLevel(levelCode));

                //await LoadLevel(levelCode);
            }
		}

        private async Task LoadLevelWithWaitAndCancel(string levelCode)
        {
            _loadLevelCancellationTokenSource.Cancel();
            _loadLevelCancellationTokenSource.Token.WaitHandle.WaitOne();
            _loadLevelCancellationTokenSource = new();

            await LoadLevel(levelCode);
        }

        private void LevelCleared(TimeSpan? clearTime)
        {
            _levelData.DateTimeCleared = DateTime.Now;
            FinishOutLevel();
            //_levelDataRepository.MarkLevelCleared(_levelData.Code, clearTime);
        }

        private void FirstClear()
        {
            _levelData.FirstClear = true;
            FinishOutLevel();
            //_levelDataRepository.MarkFirstClear(_levelData.Code);
        }

        private void WorldRecord()
        {
            FinishOutLevel();
            //_levelDataRepository.MarkWorldRecord(_levelData.Code);
        }

        private async Task LoadLevel(string levelCode)
		{
            if (_players != null && _players.Count > 0)
            {
                // disable our DB upsert for a bit
                _updateDatabaseTimer.Stop();

                try
                {
                    // store the most recent version of the current
                    // level to the DB first
                    Upsert();

                    _levelData = await JoinMarioMakerApiAndDatabaseData(levelCode);

                    UpdateUi();

                    _stopwatch.Restart();

                    _stopwatch.Start(new TimeSpan(_levelData.TimeElapsed));
                }
                finally
                {
                    _updateDatabaseTimer.Start();

                }
            }
		}

        private async Task<MarioMakerLevelData?> LoadLevelInfo(string levelCode)
		{
            MarioMakerLevelData? result = null;

            Stopwatch stopwatch = Stopwatch.StartNew();

            try
			{
                result = await _nintendoServiceClient.GetLevelInfo(levelCode, _loadLevelCancellationTokenSource.Token);
            }
			catch 
            {

            }
            finally
            {
                stopwatch.Stop();
                LabelApiLoadTime.Content = $"API Load wait time: {stopwatch.Elapsed.ToString("ss':'ffff")}";
            }

            return result;
		}

		private async Task<OverlayLevelData> JoinMarioMakerApiAndDatabaseData(string levelCode)
        {
            OverlayLevelData overlayLevelData = new();

            if (_players != null && _players.Count > 0)
            {
                MarioMakerLevelData? apiData = await LoadLevelInfo(levelCode);
                LevelData? levelData = await _levelDataRepository.GetByLevelCodeAndPlayer(levelCode, _players[_playersActiveIndex].PlayerId);

                if (apiData != null)
                {
                    overlayLevelData.Attempts = apiData.Attempts;
                    overlayLevelData.Boos = apiData.Boos;
                    overlayLevelData.WorldRecord = apiData.WorldRecord;
                    overlayLevelData.ClearRate = apiData.ClearRate;
                    overlayLevelData.Clears = apiData.Clears;
                    overlayLevelData.Code = levelCode;
                    overlayLevelData.DifficultyName = apiData.DifficultyName;
                    overlayLevelData.Likes = apiData.Likes;
                    overlayLevelData.Name = apiData.Name;
                    overlayLevelData.TagsName = apiData.TagsName;
                    overlayLevelData.ClearCheckTime = apiData.ClearCheckTime;
                    overlayLevelData.GameStyle = apiData.GameStyleName;
                    overlayLevelData.Theme = apiData.ThemeName;
                    overlayLevelData.TotalGlobalAttempts = apiData.Attempts;
                    overlayLevelData.TotalGlobalClears = apiData.Clears;

                    if (levelData?.LevelDataId > 0)
                    {
                        overlayLevelData.LevelDataId = levelData.LevelDataId;
                        overlayLevelData.PlayerDeaths = levelData.PlayerDeaths;
                        overlayLevelData.TimeElapsed = levelData.TimeElapsed;
                        overlayLevelData.ClearTime = levelData.ClearTime;
                        overlayLevelData.DateTimeCleared = levelData.DateTimeCleared;
                        overlayLevelData.DateTimeStarted = levelData.DateTimeStarted;
                        overlayLevelData.FirstClear = levelData.FirstClear;
                    }
                }
            }

            return overlayLevelData;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            WpfScreen myScreen = WpfScreen.GetScreenFrom(this);

            Left = (myScreen.DeviceBounds.Width / 2) - (Width / 2);
        }
    }
}
