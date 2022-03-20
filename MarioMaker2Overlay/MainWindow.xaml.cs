using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using MarioMaker2Overlay.Models;
using MarioMaker2Overlay.Persistence;
using MarioMaker2Overlay.Utility;
using SnagFree.TrayApp.Core;

namespace MarioMaker2Overlay
{
    public partial class MainWindow : Window
    {
        private Timer _updateDatabaseTimer = new Timer(5000);
        private GlobalKeyboardHook _globalKeyboardHook;
        private OverlayLevelData _levelData = new();
        private LevelDataRepository _levelDataRepository = new();
        private NintendoServiceClient _nintendoServiceClient = new(new HttpClient());
        private Timer _gameTimer = new Timer(20);
        public decimal _time;
        private StopwatchWithOffset _stopwatch = new();
        private WebsocketClientHelper _websocketClientHelper = new WebsocketClientHelper();

        public MainWindow()
        {
            InitializeComponent();
            SetupKeyboardHooks();
            InitializeAllFieldsToDefaults();

            Topmost = true;

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

            _websocketClientHelper.OnMarioDeath = (response) =>
            {
                _levelData.PlayerDeaths++;
                UpdateUi();
            };

            Task.Run(async () => await _websocketClientHelper.RunAsync());

            WindowStyle = WindowStyle.None;

            _gameTimer.Elapsed += CurrentTimeElapsed;
            _gameTimer.Enabled = true;

            _updateDatabaseTimer.Elapsed += TryUpsertLevel;
            _updateDatabaseTimer.Enabled = true;
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
                Persistence.LevelData levelData;

                levelData = LocalToDb();

                _levelDataRepository.Upsert(levelData);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _gameTimer.Stop();
            _updateDatabaseTimer.Stop();

            Upsert();

            base.OnClosing(e);
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
                        _levelData.PlayerDeaths++;
                        UpdateUi();
                        break;
                    case GlobalKeyboardHook.VkDown:
                        if(_levelData.PlayerDeaths > 0)
                        {
                            _levelData.PlayerDeaths--;
                            UpdateUi();
                        }
                        break;
                }
            }
        }

        private void UpdateUi()
        {
            //_levelData.Code = LevelCode.Text;

            LabelDeathCount.Content = $"{_levelData.PlayerDeaths}";
            LabelClears.Content = $"{_levelData.Clears}/{_levelData.Attempts} ({_levelData.ClearRate})";
            LabelTags.Content = string.Join(", ", _levelData.TagsName) ?? "--";
            LabelLevelName.Content = $"{_levelData.Name}";
            LabelDifficultyName.Content = $"({_levelData.DifficultyName})";
            LabelLikes.Content = _levelData.Likes;
            LabelBoos.Content = _levelData.Boos;
            LabelWorldRecord.Content = $"{_levelData.WorldRecord}";

            CalculateWinRate();
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

            Persistence.LevelData levelData;

            levelData = LocalToDb();

            _levelDataRepository.Upsert(levelData);
        }

        private void Button_ClickInsert(object sender, RoutedEventArgs e)
        {
            UpdateUi();

            Persistence.LevelData levelData;

            levelData = LocalToDb();

            _levelDataRepository.Insert(levelData);
        }

        private Persistence.LevelData LocalToDb()
        {
            Persistence.LevelData data = new();

            data.Code = _levelData.Code?.Replace("-", string.Empty);
            data.PlayerDeaths = _levelData.PlayerDeaths;
            data.TotalGlobalAttempts = _levelData.Attempts;
            data.TotalGlobalClears = _levelData.Clears;
            data.TimeElapsed = _stopwatch.ElapsedTicks;

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
                Persistence.LevelData newLevelData = new Persistence.LevelData { Code = myRandom.Next(100000000, 999999999).ToString(), PlayerDeaths = myRandom.Next(5, 300), TotalGlobalAttempts = myRandom.Next(100, 10000), TotalGlobalClears = myRandom.Next(10, 600) };

                _levelDataRepository.Insert(newLevelData);
            }
        }

        private void InitializeAllFieldsToDefaults()
        {
            LabelLevelName.Content = "---";
            LabelDifficultyName.Content = "(---)";
            LabelLikes.Content = "-";
            LabelBoos.Content = "-";
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

        private async void LevelCode_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            
        }

        private async void LevelCode_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string levelCode = LevelCode.Text?.Replace("-", string.Empty) ?? string.Empty;

            if (IsValidLevelCode(levelCode))
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
                }
                finally
                {
                    _updateDatabaseTimer.Start();
                }
            }
        }

        private async Task<OverlayLevelData> JoinMarioMakerApiAndDatabaseData(string levelCode)
        {
            OverlayLevelData overlayLevelData = new OverlayLevelData();

            List<Task> tasks = new List<Task>();
            
            Task<MarioMakerLevelData> apiTask = _nintendoServiceClient.GetLevelInfo(levelCode);
            Task<LevelData?> databaseTask = _levelDataRepository.GetByLevelCode(levelCode);

            await Task.WhenAll(apiTask, databaseTask);

            MarioMakerLevelData apiData = await apiTask;
            LevelData? levelData = await databaseTask;

            overlayLevelData = new OverlayLevelData
            {
                Attempts = apiData.Attempts,
                Boos = apiData.Boos,
                WorldRecord = apiData.WorldRecord,
                ClearRate = apiData.ClearRate,
                Clears = apiData.Clears,
                Code = levelCode,
                DifficultyName = apiData.DifficultyName,
                LevelDataId = levelData.LevelDataId,
                Likes = apiData.Likes,
                Name = apiData.Name,
                TagsName = apiData.TagsName,
                PlayerDeaths = levelData.PlayerDeaths,
                ClearCheckTime = apiData.ClearCheckTime
            };

            return overlayLevelData;
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{

        //    List<LevelData> existingFileData = GetLevelDataFromFile();

        //    using (FileStream myFile = File.OpenWrite("LevelData.json"))
        //    {
        //        existingFileData.Add(_levelData);

        //        BinaryFormatter binaryFormatter = new();

        //        binaryFormatter.Serialize(myFile, existingFileData);


        //        XmlSerializer serializer = new XmlSerializer(typeof(List<LevelData>));
        //        XmlWriter myWriter = XmlWriter.Create(myFile);
        //        serializer.Serialize(myWriter, existingFileData);

        //        JsonSerializer.Serialize(myFile, new List<LevelData> { _levelData });
        //    }
        //}

        //private void Button_Click_1(object sender, RoutedEventArgs e)
        //{
        //    _levelData = GetLevelDataFromFile().First();

        //    UpdateUi();
        //}

        //private List<LevelData> GetLevelDataFromFile()
        //{
        //    List<LevelData> result = new();

        //    using (FileStream myFile = File.OpenRead("LevelData.json"))
        //    {
        //        BinaryFormatter binaryFormatter = new();

        //        result = (List<LevelData>)binaryFormatter.Deserialize(myFile);

        //        XmlSerializer serializer = new XmlSerializer(typeof(List<LevelData>));

        //        result = (List<LevelData>)serializer.Deserialize(myFile);

        //        result = JsonSerializer.Deserialize<List<LevelData>>(myFile) ?? new List<LevelData>();
        //    }

        //    return result;
        //}
    }
}
