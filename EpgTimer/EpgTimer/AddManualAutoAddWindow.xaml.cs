﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using CtrlCmdCLI;
using CtrlCmdCLI.Def;

namespace EpgTimer
{
    /// <summary>
    /// AddManualAutoAddWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class AddManualAutoAddWindow : Window
    {
        private ManualAutoAddData defKey = null;
        private CtrlCmdUtil cmd = CommonManager.Instance.CtrlCmd;

        public AddManualAutoAddWindow()
        {
            InitializeComponent();

            try
            {
                if (Settings.Instance.NoStyle == 0)
                {
                    ResourceDictionary rd = new ResourceDictionary();
                    rd.MergedDictionaries.Add(
                        Application.LoadComponent(new Uri("/PresentationFramework.Aero, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35;component/themes/aero.normalcolor.xaml", UriKind.Relative)) as ResourceDictionary
                        //Application.LoadComponent(new Uri("/PresentationFramework.Classic, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, ProcessorArchitecture=MSIL;component/themes/Classic.xaml", UriKind.Relative)) as ResourceDictionary
                        );
                    this.Resources = rd;
                }
                else
                {
                    button_add.Style = null;
                    button_chg.Style = null;
                    button_cancel.Style = null;
                }

                comboBox_startHH.DataContext = CommonManager.Instance.HourDictionary.Values;
                comboBox_startHH.SelectedIndex = 0;
                comboBox_startMM.DataContext = CommonManager.Instance.MinDictionary.Values;
                comboBox_startMM.SelectedIndex = 0;
                comboBox_startSS.DataContext = CommonManager.Instance.MinDictionary.Values;
                comboBox_startSS.SelectedIndex = 0;
                comboBox_endHH.DataContext = CommonManager.Instance.HourDictionary.Values;
                comboBox_endHH.SelectedIndex = 0;
                comboBox_endMM.DataContext = CommonManager.Instance.MinDictionary.Values;
                comboBox_endMM.SelectedIndex = 0;
                comboBox_endSS.DataContext = CommonManager.Instance.MinDictionary.Values;
                comboBox_endSS.SelectedIndex = 0;

                comboBox_service.ItemsSource = ChSet5.Instance.ChList.Values;
                comboBox_service.SelectedIndex = 0;

                recSettingView.SetViewMode(false);
                SetChangeMode(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        public void SetChangeMode(bool chgFlag)
        {
            button_chg.Visibility = (chgFlag == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden);
        }

        public void SetDefaultSetting(ManualAutoAddData item)
        {
            defKey = item;
        }

        private bool CheckExistAutoAddItem()
        {
            bool retval = CommonManager.Instance.DB.ManualAutoAddList.ContainsKey(this.defKey.dataID);
            if (retval == false)
            {
                MessageBox.Show("項目がありません。\r\n" +
                    "既に削除されています。\r\n" +
                    "(別のEpgtimerによる操作など)");

                //追加モードに変更
                SetChangeMode(false);
                defKey = null;
            }
            return retval;
        }

        private void button_add_Click(object sender, RoutedEventArgs e)
        {
            button_add_chg(false);
        }

        private void button_chg_Click(object sender, RoutedEventArgs e)
        {
            button_add_chg(true);
        }
        
        private void button_add_chg(bool chgFlag)
        {
            if (chgFlag == true && CheckExistAutoAddItem() == false) return;

            if (defKey == null)
            {
                defKey = new ManualAutoAddData();
            }
            defKey.dayOfWeekFlag = 0;
            if (checkBox_week0.IsChecked == true)
            {
                defKey.dayOfWeekFlag |= 0x01;
            }
            if (checkBox_week1.IsChecked == true)
            {
                defKey.dayOfWeekFlag |= 0x02;
            }
            if (checkBox_week2.IsChecked == true)
            {
                defKey.dayOfWeekFlag |= 0x04;
            }
            if (checkBox_week3.IsChecked == true)
            {
                defKey.dayOfWeekFlag |= 0x08;
            }
            if (checkBox_week4.IsChecked == true)
            {
                defKey.dayOfWeekFlag |= 0x10;
            }
            if (checkBox_week5.IsChecked == true)
            {
                defKey.dayOfWeekFlag |= 0x20;
            }
            if (checkBox_week6.IsChecked == true)
            {
                defKey.dayOfWeekFlag |= 0x40;
            }

            defKey.startTime = ((UInt32)comboBox_startHH.SelectedIndex * 60 * 60) + ((UInt32)comboBox_startMM.SelectedIndex * 60) + (UInt32)comboBox_startSS.SelectedIndex;
            UInt32 endTime = ((UInt32)comboBox_endHH.SelectedIndex * 60 * 60) + ((UInt32)comboBox_endMM.SelectedIndex * 60) + (UInt32)comboBox_endSS.SelectedIndex;
            if (endTime < defKey.startTime)
            {
                defKey.durationSecond = (24 * 60 * 60 + endTime) - defKey.startTime;
            }
            else
            {
                defKey.durationSecond = endTime - defKey.startTime;
            }

            defKey.title = textBox_title.Text;

            ChSet5Item chItem = comboBox_service.SelectedItem as ChSet5Item;
            defKey.stationName = chItem.ServiceName;
            defKey.originalNetworkID = chItem.ONID;
            defKey.transportStreamID = chItem.TSID;
            defKey.serviceID = chItem.SID;

            RecSettingData recSet = new RecSettingData();
            recSettingView.GetRecSetting(ref recSet);
            defKey.recSetting = recSet;

            List<ManualAutoAddData> val = new List<ManualAutoAddData>();
            val.Add(defKey);

            if (chgFlag == true)
            {
                cmd.SendChgManualAdd(val);
            }
            else
            {
                cmd.SendAddManualAdd(val);
            }
            DialogResult = true;
        }

        private void button_cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.None)
            {
                switch (e.Key)
                {
                    case Key.Escape:
                        this.button_cancel.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                        break;
                }
            }
            base.OnKeyDown(e);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (defKey != null)
            {
                if ((defKey.dayOfWeekFlag & 0x01) != 0)
                {
                    checkBox_week0.IsChecked = true;
                }
                if ((defKey.dayOfWeekFlag & 0x02) != 0)
                {
                    checkBox_week1.IsChecked = true;
                }
                if ((defKey.dayOfWeekFlag & 0x04) != 0)
                {
                    checkBox_week2.IsChecked = true;
                }
                if ((defKey.dayOfWeekFlag & 0x08) != 0)
                {
                    checkBox_week3.IsChecked = true;
                }
                if ((defKey.dayOfWeekFlag & 0x10) != 0)
                {
                    checkBox_week4.IsChecked = true;
                }
                if ((defKey.dayOfWeekFlag & 0x20) != 0)
                {
                    checkBox_week5.IsChecked = true;
                }
                if ((defKey.dayOfWeekFlag & 0x40) != 0)
                {
                    checkBox_week6.IsChecked = true;
                }

                UInt32 hh = defKey.startTime / (60 * 60);
                UInt32 mm = (defKey.startTime % (60 * 60)) / 60;
                UInt32 ss = defKey.startTime % 60;

                comboBox_startHH.SelectedIndex = (int)hh;
                comboBox_startMM.SelectedIndex = (int)mm;
                comboBox_startSS.SelectedIndex = (int)ss;

                UInt32 endTime = defKey.startTime + defKey.durationSecond;
                if (endTime > 24 * 60 * 60)
                {
                    endTime -= 24 * 60 * 60;
                }
                hh = endTime / (60 * 60);
                mm = (endTime % (60 * 60)) / 60;
                ss = endTime % 60;

                comboBox_endHH.SelectedIndex = (int)hh;
                comboBox_endMM.SelectedIndex = (int)mm;
                comboBox_endSS.SelectedIndex = (int)ss;

                textBox_title.Text = defKey.title;

                UInt64 key = defKey.Create64Key();

                if (ChSet5.Instance.ChList.ContainsKey(key) == true)
                {
                    comboBox_service.SelectedItem = ChSet5.Instance.ChList[key];
                }
                defKey.recSetting.PittariFlag = 0;
                defKey.recSetting.TuijyuuFlag = 0;
                recSettingView.SetDefSetting(defKey.recSetting);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.ListFoucsOnVisibleChanged();
        }
    }
}
