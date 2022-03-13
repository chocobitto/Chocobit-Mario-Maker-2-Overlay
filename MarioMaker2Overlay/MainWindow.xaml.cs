using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using MarioMaker2Overlay.Persistence;
using MarioMaker2Overlay.Services;
using SnagFree.TrayApp.Core;

namespace MarioMaker2Overlay
{
    public partial class MainWindow : Window
    {
        private Timer _updateDatabaseTimer;
        private GlobalKeyboardHook _globalKeyboardHook;
        private LevelData _levelData = new();
        private LevelDataRepository _levelDataRepository = new();
        private NintendoServiceClient _nintendoServiceClient = new(new HttpClient());
        private Timer _gameTimer;
        public decimal _time;
        private Stopwatch _stopwatch = new();


        public MainWindow()
        {
            InitializeComponent();
            SetupKeyboardHooks();

            WindowStyle = WindowStyle.None;

            _gameTimer = new Timer(20);
            _gameTimer.Elapsed += CurrentTimeElapsed;
            _gameTimer.Enabled = true;

            _stopwatch.Start();

            _updateDatabaseTimer = new Timer(5000);
            _updateDatabaseTimer.Elapsed += TryUpdateLevelData;
            _updateDatabaseTimer.Enabled = true;

            InitializeAllFieldsToDefaults();
        }
        
        private void TryUpdateLevelData(object? sender, ElapsedEventArgs e)
        {
            // Check to see if the data exists
            if(_levelData?.LevelDataId > 0 || !string.IsNullOrWhiteSpace(_levelData?.Code))
            {
                // if it exists update it
                Persistence.LevelData levelData;

                levelData = LocalToDb();

                _levelDataRepository.Upsert(levelData);
            }
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
            LabelDeathCount.Content = $"{_levelData.PlayerDeaths}";
            _levelData.Code = LevelCode.Text;
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

            data.Code = _levelData.Code;
            data.PlayerDeaths = _levelData.PlayerDeaths;
            data.TotalGlobalAttempts = _levelData.TotalGlobalAttempts;
            data.TotalGlobalClears = _levelData.TotalGlobalClears;

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

        private void Button_ClickGetData(object sender, RoutedEventArgs e)
        {
            UpdateUi();

            Persistence.LevelData levelData;

            levelData = _levelDataRepository.GetByLevelCode(LevelCode.Text);

            _levelData.Code = levelData?.Code;
            _levelData.PlayerDeaths = levelData.PlayerDeaths;
            _levelData.TotalGlobalAttempts = levelData.TotalGlobalAttempts;
            _levelData.TotalGlobalClears = levelData.TotalGlobalClears;

            UpdateUi();
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

        private async void LevelCode_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            string levelCode = LevelCode.Text;

            if (IsValidLevelCode(levelCode))
            {

                try
                {
                    MarioMakerLevelData levelData = await _nintendoServiceClient.GetLevelInfo(levelCode.Replace("-", string.Empty));

                    LabelClears.Content = $"{levelData.Clears}/{levelData.Attempts} ({levelData.ClearRate})";
                    LabelTags.Content = string.Join(", ", levelData.TagsName) ?? "--";
                    LabelLevelName.Content = $"{levelData.Name}";
                    LabelDifficultyName.Content = $"({levelData.DifficultyName})";
                    LabelLikes.Content = levelData.Likes;
                    LabelBoos.Content = levelData.Boos;
                    LabelWorldRecord.Content = $"{levelData.WorldRecord}";

                    // reset deaths and timer
                    _levelData.Code = LevelCode.Text;
                    _levelData.TotalGlobalAttempts = levelData.Attempts;
                    _levelData.TotalGlobalClears = levelData.Clears;
                }
                catch (Exception ex) 
                {
                    InitializeAllFieldsToDefaults();
                }

                // reset deaths and timer for now until we're getting this
                // from the DB
                _stopwatch.Restart();
                _levelData.PlayerDeaths = 0;

                UpdateUi();
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
            bool result = Regex.IsMatch(levelCode, "[0-9A-HJ-NP-Y][0-9A-HJ-NP-Y][0-9A-HJ-NP-Y]-[0-9A-HJ-NP-Y][0-9A-HJ-NP-Y][0-9A-HJ-NP-Y]-[0-9A-HJ-NP-Y][0-9A-HJ-NP-Y][0-9A-HJ-NP-Y]");

            return result;
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
