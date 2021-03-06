﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace FindChangeInContainsRandomTXT
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 改变数据的记录
        /// </summary>
        public class ChangeTextLineNumber
        {
            /// <summary>
            /// 基础随机A行号
            /// </summary>
            public int LineNumberRandomA { set; get; }
            /// <summary>
            /// 基础随机B行号
            /// </summary>
            public int LineNumberRandomB { set; get; }
            /// <summary>
            /// 比较数据行号
            /// </summary>
            public int LineNumberCompareData { set; get; }
            /// <summary>
            /// 随机文本
            /// </summary>
            public string TextRandom { set; get; }
            /// <summary>
            /// 比较文本
            /// </summary>
            public string TextCompareData { set; get; }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="numberA">行号A</param>
            /// <param name="numberB">行号B</param>
            /// <param name="numberC">行号数据</param>
            public ChangeTextLineNumber(int numberA, int numberB, int numberC, string randomText, string compareText)
            {
                LineNumberRandomA = numberA;
                LineNumberRandomB = numberB;
                LineNumberCompareData = numberC;
                TextRandom = randomText;
                TextCompareData = compareText;
            }
        }
        /// <summary>
        /// 比较数据行号字典
        /// </summary>
        public Dictionary<ListBoxItem, ChangeTextLineNumber> DictionaryChangeText { set; get; }
        const string LogFile = "Log.log";
        const string RandomAFile = "RandomA.txt";
        const string RandomBFile = "RandomB.txt";
        const string CompareFile = "Compare.txt";

        /// <summary>
        /// 构造函数
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            DictionaryChangeText = new Dictionary<ListBoxItem, ChangeTextLineNumber>();
            StreamReader sr;
            if (File.Exists(RandomAFile))
            {
                sr = new StreamReader(RandomAFile);
                TextEditor_BaseRandomA.Text = sr.ReadToEnd();
                sr.Close();
            }
            if (File.Exists(RandomBFile))
            {
                sr = new StreamReader(RandomBFile);
                TextEditor_BaseRandomB.Text = sr.ReadToEnd();
                sr.Close();
            }
            if (File.Exists(CompareFile))
            {
                sr = new StreamReader(CompareFile);
                TextEditor_CompareData.Text = sr.ReadToEnd();
                sr.Close();
            }
        }

        /// <summary>
        /// 清理变化值列表
        /// </summary>
        /// <param name="sender">响应空间</param>
        /// <param name="e">响应事件参数</param>
        private void Button_ClearChange_Click(object sender, RoutedEventArgs e)
        {
            ListBox_FindList.Items.Clear();
            DictionaryChangeText.Clear();
            Label_RandomSameCount.Content = "基础共同计数：0";
            Label_CompareSameCount.Content = "变化结果计数：0";
        }

        /// <summary>
        /// 查找变化值并填充列表
        /// </summary>
        /// <param name="sender">响应空间</param>
        /// <param name="e">响应事件参数</param>
        private void Button_GetChange_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, ChangeTextLineNumber> randomAList = new Dictionary<string, ChangeTextLineNumber>();
            int i = 1;
            foreach (string select in TextEditor_BaseRandomA.Text.Split('\n'))
            {
                randomAList[select] = new ChangeTextLineNumber(i++, 0, 0, select, "");
            }
            Dictionary<string, ChangeTextLineNumber> randomBList = new Dictionary<string, ChangeTextLineNumber>();
            i = 1;
            foreach (string select in TextEditor_BaseRandomB.Text.Split('\n'))
            {
                randomBList[select] = new ChangeTextLineNumber(0, i++, 0, select, "");
            }
            Dictionary<string, ChangeTextLineNumber> compareList = new Dictionary<string, ChangeTextLineNumber>();
            i = 1;
            foreach (string select in TextEditor_CompareData.Text.Split('\n'))
            {
                compareList[select] = new ChangeTextLineNumber(0, 0, i++, "", select);
            }
            Dictionary<string, ChangeTextLineNumber> randomStringList = new Dictionary<string, ChangeTextLineNumber>();
            Dictionary<int, ChangeTextLineNumber> randomIntList = new Dictionary<int, ChangeTextLineNumber>();
            foreach (var select in randomAList)
            {
                if (randomBList.Keys.Contains(select.Key))
                {
                    randomStringList[select.Key] = new ChangeTextLineNumber(select.Value.LineNumberRandomA, randomBList[select.Key].LineNumberRandomB, 0, select.Key, "");
                    randomIntList[select.Value.LineNumberRandomA] = randomStringList[select.Key];
                }
            }

            DictionaryChangeText.Clear();
            ListBox_FindList.Items.Clear();
            foreach (var select in compareList)
            {
                if (!randomStringList.Keys.Contains(select.Key))
                {
                    ListBoxItem item = new ListBoxItem();
                    item.Content = "[" + select.Value.LineNumberCompareData + "]:" + select.Key;
                    if (randomIntList.Keys.Contains(select.Value.LineNumberCompareData))
                    {
                        ChangeTextLineNumber temp = randomIntList[select.Value.LineNumberCompareData];
                        DictionaryChangeText[item] = new ChangeTextLineNumber(temp.LineNumberRandomA, temp.LineNumberRandomB, select.Value.LineNumberCompareData, temp.TextRandom, select.Value.TextCompareData);
                        ListBox_FindList.Items.Add(item);
                    }
                }
            }
            Label_RandomSameCount.Content = "基础共同计数：" + randomStringList.Count();
            Label_CompareSameCount.Content = "变化结果计数：" + DictionaryChangeText.Count();
        }

        /// <summary>
        /// 改变列表选择变化
        /// </summary>
        /// <param name="sender">响应空间</param>
        /// <param name="e">响应事件参数</param>
        private void ListBox_FindList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem item = ListBox_FindList.SelectedItem as ListBoxItem;
            if (item == null || !DictionaryChangeText.Keys.Contains(item)) return;
            ChangeTextLineNumber select = DictionaryChangeText[ListBox_FindList.SelectedItem as ListBoxItem];
            TextEditor_BaseRandomA.ScrollTo(select.LineNumberRandomA, 0);
            TextEditor_BaseRandomB.ScrollTo(select.LineNumberRandomB, 0);
            TextEditor_CompareData.ScrollTo(select.LineNumberCompareData, 0);
        }

        /// <summary>
        /// 保存日志
        /// </summary>
        /// <param name="sender">响应空间</param>
        /// <param name="e">响应事件参数</param>
        private void Button_SaveLog_Click(object sender, RoutedEventArgs e)
        {
            StreamWriter sw = new StreamWriter(LogFile, true);
            sw.WriteLine("写日志于:" + DateTime.Now.ToLocalTime());
            foreach (var select in ListBox_FindList.Items)
            {
                sw.WriteLine((select as ListBoxItem).Content);
            }
            sw.Close();
        }
        /// <summary>
        /// 清理日志
        /// </summary>
        /// <param name="sender">响应空间</param>
        /// <param name="e">响应事件参数</param>
        private void Button_ClearLog_Click(object sender, RoutedEventArgs e)
        {
            StreamWriter sw = new StreamWriter(LogFile, false);
            sw.Write("");
            sw.Close();
        }
        /// <summary>
        /// 打开日志
        /// </summary>
        /// <param name="sender">响应空间</param>
        /// <param name="e">响应事件参数</param>
        private void Button_OpenLog_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(LogFile))
            {
                System.Diagnostics.Process.Start(LogFile);
            }
            else
            {
                MessageBox.Show("日志文件不存在！");
            }
        }
        /// <summary>
        /// 保存基础随机AB的文本
        /// </summary>
        /// <param name="sender">响应空间</param>
        /// <param name="e">响应事件参数</param>
        private void Button_SaveRandomAB_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("是否保存基础随机文本A和B？同意将覆盖旧文件。", "注意", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes) != MessageBoxResult.Yes) return;
            StreamWriter sw = new StreamWriter(RandomAFile, false);
            sw.Write(TextEditor_BaseRandomA.Text);
            sw.Close();
            sw = new StreamWriter(RandomBFile, false);
            sw.Write(TextEditor_BaseRandomB.Text);
            sw.Close();
        }
        /// <summary>
        /// 读取基础随机AB的文本
        /// </summary>
        /// <param name="sender">响应空间</param>
        /// <param name="e">响应事件参数</param>
        private void Button_LoadRandomAB_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("是否加载基础随机文本A和B？同意将覆盖当前文本。", "注意", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes) != MessageBoxResult.Yes) return;
            StreamReader sr;
            if (File.Exists(RandomAFile))
            {
                sr = new StreamReader(RandomAFile);
                TextEditor_BaseRandomA.Text = sr.ReadToEnd();
                sr.Close();
            }
            else
            {
                if (MessageBox.Show("当前随机文本A记录文件不存在，是否将当前文本清除？", "注意", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes) != MessageBoxResult.Yes)
                    TextEditor_BaseRandomA.Text = "";
            }
            if (File.Exists(RandomAFile))
            {
                sr = new StreamReader(RandomBFile);
                TextEditor_BaseRandomB.Text = sr.ReadToEnd();
                sr.Close();
            }
            else
            {
                if (MessageBox.Show("当前随机文本B记录文件不存在，是否将当前文本清除？", "注意", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes) != MessageBoxResult.Yes)
                    TextEditor_BaseRandomB.Text = "";
            }
        }
        /// <summary>
        /// 保存比较B的文本
        /// </summary>
        /// <param name="sender">响应空间</param>
        /// <param name="e">响应事件参数</param>
        private void Button_SaveCompare_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("是否保存比较文本？同意将覆盖旧文件。", "注意", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes) != MessageBoxResult.Yes) return;
            StreamWriter sw = new StreamWriter(CompareFile, false);
            sw.Write(TextEditor_CompareData.Text);
            sw.Close();
        }

        /// <summary>
        /// 加载比较B的文本
        /// </summary>
        /// <param name="sender">响应空间</param>
        /// <param name="e">响应事件参数</param>
        private void Button_LoadCompare_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("是否加载比较文本？同意将覆盖当前文本。", "注意", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes) != MessageBoxResult.Yes) return;
            StreamReader sr;
            if (File.Exists(CompareFile))
            {
                sr = new StreamReader(CompareFile);
                TextEditor_CompareData.Text = sr.ReadToEnd();
                sr.Close();
            }
            else
            {
                if (MessageBox.Show("当前比较文件不存在，是否将当前文本清除？", "注意", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes) != MessageBoxResult.Yes)
                    TextEditor_CompareData.Text = "";
            }
        }
    }
} 
