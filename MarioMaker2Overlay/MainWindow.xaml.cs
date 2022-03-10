using System;
using System.Collections.Generic;
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
        private Timer _timer;
        private GlobalKeyboardHook _globalKeyboardHook;
        private LevelData _levelData = new();
        private LevelDataRepository _levelDataRepository = new();
        private NintendoServiceClient _nintendoServiceClient = new(new HttpClient());

        public MainWindow()
        {
            InitializeComponent();
            SetupKeyboardHooks();

            WindowStyle = WindowStyle.None;

            _timer = new Timer(5000);
            _timer.Elapsed += TryUpdateLevelData;
        }

        private void TryUpdateLevelData(object? sender, ElapsedEventArgs e)
        {
            // Check to see if the data exists
            if(_levelData?.LevelDataId > 0)
            {
                // if it exist update it
                Persistence.LevelData levelData;

                levelData = LocalToDb();

                _levelDataRepository.Update(levelData);
            }
            else if(!string.IsNullOrWhiteSpace(_levelData?.Code))
            {
                // if not insert it
                Persistence.LevelData levelData;

                levelData = LocalToDb();

                _levelDataRepository.Insert(levelData);
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
            DeathCount.Content = $"Deaths: {_levelData.PlayerDeaths}";
            _levelData.Code = LevelCode.Text;
            WinRate();
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
            decimal attempts = _levelData.PlayerDeaths + 1;

            if(attempts > 0)
            {
                decimal winrate = 1 / attempts * 100;

                //Winrate.Content = $"WR: {winrate:f2}%";
            }
            else
            {
                //Winrate.Content = string.Empty;
            }
        }

        private void Button_ClickUpdate(object sender, RoutedEventArgs e)
        {
            UpdateUi();

            Persistence.LevelData levelData;

            levelData = LocalToDb();

            _levelDataRepository.Update(levelData);
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

            if (Regex.IsMatch(LevelCode.Text, "[0-9A-HJ-NP-Y][0-9A-HJ-NP-Y][0-9A-HJ-NP-Y]-[0-9A-HJ-NP-Y][0-9A-HJ-NP-Y][0-9A-HJ-NP-Y]-[0-9A-HJ-NP-Y][0-9A-HJ-NP-Y][0-9A-HJ-NP-Y]"))
            {
                _levelData.Code = LevelCode.Text;

                MarioMakerLevelData levelData = await _nintendoServiceClient.GetLevelInfo(levelCode.Replace("-", string.Empty));

                LabelClears.Content = $"{levelData.Attempts}/{levelData.Clears}({levelData.ClearRate})";
                LabelFirstTag.Content = levelData.TagsName.First() ?? "--";
                LabelSecondTag.Content = levelData.TagsName.Last() ?? "--";
                LabelLevelName.Content = $"{levelData.Name} ({levelData.DifficultyName})";;
                LabelLikes.Content = levelData.Likes;
                LabelBoos.Content = levelData.Boos;
                LabelWorldRecord.Content = levelData.WorldRecord;
            }
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
