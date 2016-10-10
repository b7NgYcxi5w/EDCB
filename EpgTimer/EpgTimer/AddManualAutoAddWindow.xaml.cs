﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EpgTimer
{
    /// <summary>
    /// AddManualAutoAddWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class AddManualAutoAddWindow : Window
    {
        private ManualAutoAddData defKey = null;
        private static CtrlCmdUtil cmd { get { return CommonManager.Instance.CtrlCmd; } }
        private MenuBinds mBinds = new MenuBinds();

        private bool chgMode = false;
        private List<CheckBox> chbxList;

        public AddManualAutoAddWindow()
        {
            InitializeComponent();

            try
            {
                //コマンドの登録
                this.CommandBindings.Add(new CommandBinding(EpgCmds.Cancel, (sender, e) => DialogResult = false));
                this.CommandBindings.Add(new CommandBinding(EpgCmds.AddInDialog, button_add_click));
                this.CommandBindings.Add(new CommandBinding(EpgCmds.ChangeInDialog, button_chg_click, (sender, e) => e.CanExecute = chgMode));
                this.CommandBindings.Add(new CommandBinding(EpgCmds.DeleteInDialog, button_del_click, (sender, e) => e.CanExecute = chgMode));
                this.CommandBindings.Add(new CommandBinding(EpgCmds.Delete2InDialog, button_del2_click, (sender, e) => e.CanExecute = chgMode));

                //ボタンの設定
                mBinds.SetCommandToButton(button_cancel, EpgCmds.Cancel);
                mBinds.SetCommandToButton(button_chg, EpgCmds.ChangeInDialog);
                mBinds.SetCommandToButton(button_add, EpgCmds.AddInDialog);
                mBinds.SetCommandToButton(button_del, EpgCmds.DeleteInDialog);
                mBinds.SetCommandToButton(button_del2, EpgCmds.Delete2InDialog);
                mBinds.ResetInputBindings(this);

                //その他設定
                chbxList = new List<CheckBox>(new string[] { "日", "月", "火", "水", "木", "金", "土" }
                    .Select(wd => new CheckBox { Content = wd, Margin = new Thickness(0, 0, 6, 0) }));
                chbxList.ForEach(chbx => stackPanel_week.Children.Add(chbx));

                comboBox_startHH.DataContext = CommonManager.CustomHourList;
                comboBox_startHH.SelectedIndex = 0;
                comboBox_startMM.DataContext = Enumerable.Range(0, 60);
                comboBox_startMM.SelectedIndex = 0;
                comboBox_startSS.DataContext = Enumerable.Range(0, 60);
                comboBox_startSS.SelectedIndex = 0;
                comboBox_endHH.DataContext = CommonManager.CustomHourList;
                comboBox_endHH.SelectedIndex = 0;
                comboBox_endMM.DataContext = Enumerable.Range(0, 60);
                comboBox_endMM.SelectedIndex = 0;
                comboBox_endSS.DataContext = Enumerable.Range(0, 60);
                comboBox_endSS.SelectedIndex = 0;

                comboBox_service.ItemsSource = ChSet5.ChList.Values;
                comboBox_service.SelectedIndex = 0;

                recSettingView.SetViewMode(false);
                SetChangeMode(false);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace); }
        }

        public void SetChangeMode(bool chgFlag)
        {
            chgMode = chgFlag;
            button_chg.Visibility = (chgFlag == true ? Visibility.Visible : Visibility.Hidden);
            button_del.Visibility = button_chg.Visibility;
            button_del2.Visibility = button_chg.Visibility;
        }

        public void SetDefaultSetting(ManualAutoAddData item)
        {
            defKey = item.Clone();
        }

        //proc 0:追加、1:変更、2:削除、3:予約ごと削除
        private bool CheckAutoAddChange(ExecutedRoutedEventArgs e, int proc)
        {
            if (proc != 3)
            {
                if (CmdExeUtil.IsDisplayKgMessage(e) == true)
                {
                    var strMode = new string[] { "追加", "変更", "削除" }[proc];
                    if (MessageBox.Show("プログラム予約登録を" + strMode + "します。\r\nよろしいですか？", strMode + "の確認", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                    { return false; }
                }
            }
            else
            {
                if (CmdExeUtil.CheckAllProcCancel(e, CommonUtil.ToList(defKey), true) == true)
                { return false; }
            }

            if (proc != 0)
            {
                if (CommonManager.Instance.DB.ManualAutoAddList.ContainsKey(this.defKey.dataID) == false)
                {
                    MessageBox.Show("項目がありません。\r\n" + "既に削除されています。\r\n" + "(別のEpgtimerによる操作など)", "データエラー", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                    //追加モードに変更
                    SetChangeMode(false);
                    defKey = null;
                    return false;
                }
            }

            return true;
        }

        private void button_add_click(object sender, ExecutedRoutedEventArgs e)
        {
            button_add_chg(sender, e, false);
        }
        private void button_chg_click(object sender, ExecutedRoutedEventArgs e)
        {
            button_add_chg(sender, e, true);
        }
        private void button_add_chg(object sender, ExecutedRoutedEventArgs e, bool chgFlag)
        {
            try
            {
                UInt32 startTime = ((UInt32)comboBox_startHH.SelectedIndex * 60 * 60) + ((UInt32)comboBox_startMM.SelectedIndex * 60) + (UInt32)comboBox_startSS.SelectedIndex;
                UInt32 endTime = ((UInt32)comboBox_endHH.SelectedIndex * 60 * 60) + ((UInt32)comboBox_endMM.SelectedIndex * 60) + (UInt32)comboBox_endSS.SelectedIndex;
                while (endTime < startTime) endTime += 24 * 60 * 60;
                UInt32 duration = endTime - startTime;
                if (duration >= 24 * 60 * 60)
                {
                    //深夜時間帯の処理の関係で、不可条件が新たに発生しているため、その対応。
                    MessageBox.Show("24時間以上の録画時間は設定出来ません。", "録画時間長の確認", MessageBoxButton.OK);
                    return;
                }

                if (CheckAutoAddChange(e, chgFlag == false ? 0 : 1) == false) return;
                //
                if (defKey == null)
                {
                    defKey = new ManualAutoAddData();
                }

                defKey.startTime = startTime;
                defKey.durationSecond = duration;

                //曜日の処理、0～6bit目:日～土
                defKey.dayOfWeekFlag = 0;
                int val = 0;
                chbxList.ForEach(chbx => defKey.dayOfWeekFlag |= (byte)((chbx.IsChecked == true ? 0x01 : 0x00) << val++));

                //開始時刻を0～24時に調整する。
                defKey.RegulateData();
                
                defKey.IsEnabled = checkBox_keyDisabled.IsChecked != true;

                defKey.title = textBox_title.Text;

                ChSet5Item chItem = comboBox_service.SelectedItem as ChSet5Item;
                defKey.stationName = chItem.ServiceName;
                defKey.originalNetworkID = chItem.ONID;
                defKey.transportStreamID = chItem.TSID;
                defKey.serviceID = chItem.SID;
                defKey.recSetting = recSettingView.GetRecSetting();

                if (chgFlag == true)
                {
                    bool ret = MenuUtil.AutoAddChange(CommonUtil.ToList(defKey));
                    StatusManager.StatusNotifySet(ret, "プログラム予約登録を変更");
                }
                else
                {
                    bool ret = MenuUtil.AutoAddAdd(CommonUtil.ToList(defKey));
                    StatusManager.StatusNotifySet(ret, "プログラム予約登録を追加");
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace); }
            DialogResult = true;
        }

        private void button_del_click(object sender, ExecutedRoutedEventArgs e)
        {
            if (CheckAutoAddChange(e, 2) == false) return;
            //
            bool ret = MenuUtil.AutoAddDelete(CommonUtil.ToList(defKey));
            StatusManager.StatusNotifySet(ret, "プログラム予約登録を削除");
            DialogResult = true;
        }

        private void button_del2_click(object sender, ExecutedRoutedEventArgs e)
        {
            if (CheckAutoAddChange(e, 3) == false) return;
            //
            bool ret = MenuUtil.AutoAddDelete(CommonUtil.ToList(defKey), true, true);
            StatusManager.StatusNotifySet(ret, "プログラム予約登録を予約ごと削除");
            DialogResult = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (defKey != null)
            {
                //深夜時間帯の処理
                if (Settings.Instance.LaterTimeUse == true && DateTime28.IsLateHour(defKey.PgStartTime.Hour) == true)
                {
                    defKey.ShiftRecDay(-1);
                }

                //曜日の処理、0～6bit目:日～土
                int val = 0;
                chbxList.ForEach(chbx => chbx.IsChecked = (defKey.dayOfWeekFlag & (0x01 << val++)) != 0);

                checkBox_keyDisabled.IsChecked = defKey.IsEnabled == false;

                UInt32 hh = defKey.startTime / (60 * 60);
                UInt32 mm = (defKey.startTime % (60 * 60)) / 60;
                UInt32 ss = defKey.startTime % 60;

                comboBox_startHH.SelectedIndex = (int)hh;
                comboBox_startMM.SelectedIndex = (int)mm;
                comboBox_startSS.SelectedIndex = (int)ss;

                //深夜時間帯の処理も含む
                UInt32 endTime = defKey.startTime + defKey.durationSecond;
                if (endTime >= comboBox_endHH.Items.Count * 60 * 60 || endTime >= 24 * 60 * 60
                    && DateTime28.JudgeLateHour(defKey.PgStartTime.AddSeconds(defKey.durationSecond), defKey.PgStartTime) == false)
                {
                    //正規のデータであれば、必ず0～23時台かつstartTimeより小さくなる。
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

                if (ChSet5.ChList.ContainsKey(key) == true)
                {
                    comboBox_service.SelectedItem = ChSet5.ChList[key];
                }
                recSettingView.SetDefSetting(defKey.recSetting, true);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            ViewUtil.MainWindow.ListFoucsOnVisibleChanged();
        }
    }
}
