using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using SnagFree.TrayApp.Core;

namespace MarioMaker2Overlay
{
    public partial class MainWindow : Window
    {
        private GlobalKeyboardHook _globalKeyboardHook;
        private LevelData _levelData = new();

        public MainWindow()
        {
            InitializeComponent();
            SetupKeyboardHooks();
            WindowStyle = WindowStyle.None;
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

                Winrate.Content = $"WR: {winrate:f2}%";
            }
            else
            {
                Winrate.Content = string.Empty;
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
