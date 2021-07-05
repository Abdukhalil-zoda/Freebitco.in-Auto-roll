using System;
using System.Windows;
using CefSharp;
using CefSharp.Wpf;
using System.IO;

namespace Chromium
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Бот для управление
        /// </summary>
        static TGBot bot;
        public MainWindow()
        {
            var api = File.Exists("api.txt");
            if (!api)
            {
                MessageBox.Show("\"api.txt\" not found Telegram bot function disabled", "Warning");
            }
            if (api)
            {
                bot = new TGBot(File.ReadAllText("api.txt"));
                bot.Start();
                bot.OnStart += Bot_OnStart;
            }
            InitializeComponent();
        }
        /// <summary>
        /// Основной браузер
        /// </summary>
        ChromiumWebBrowser chrome;
        /// <summary>
        /// Для скрытие
        /// </summary>
        System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            
            CefSettings settings = new CefSettings();
            settings.LogSeverity = LogSeverity.Verbose;
            settings.CachePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache");
            Cef.Initialize(settings);
            chrome = new ChromiumWebBrowser("https://freebitco.in/?r=37959627");
            grid.Children.Add(chrome);
            ni.Icon = new System.Drawing.Icon("./favicon.ico");
            ni.Visible = true;

            
            chrome.FrameLoadEnd += Chrome_FrameLoadEnd;
            ni.DoubleClick += delegate (object sender1, EventArgs args)
            {
                Show();
                WindowState = WindowState.Normal;
            };
        }
        /// <summary>
        /// Обновляем страницу призапуске Auto-roll
        /// </summary>
        private void Bot_OnStart()
        {
            Refresh();
        }
        /// <summary>
        /// Пороверка прикаждом обнавление
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Chrome_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            var mess = "";
            if (File.Exists("MESS"))
            {
                mess = File.ReadAllText("MESS");
            }
            bool check = mess != "0";
            string script = string.Format("document.getElementById('free_play_form_button').style.display;");
            
            await chrome.EvaluateScriptAsync(script).ContinueWith(x =>
            {
                var response = x.Result;

                if (response.Success && response.Result != null && check)
                {
                    var id = "play_without_captchas_button";
                    var s = string.Format($"document.getElementById('{id}').style.display;");
                    chrome.EvaluateScriptAsync(script).ContinueWith(xx =>
                    {
                        var res = xx.Result;
                        if (res.Success && res.Result != null)
                        {
                            if (Convert.ToString(res.Result) != "none")
                            {
                                chrome.EvaluateScriptAsync($"document.getElementById('{id}').click();").Wait();
                            }
                        }
                    });
                    var resultres = Convert.ToString(response.Result);
                    if (resultres != "none")
                    {
                        chrome.EvaluateScriptAsync("document.getElementById('free_play_form_button').click();").Wait();
                    }
                    else
                    {
                        Console.WriteLine("+++++++++++++++WAITforUPDATE++++++++++++++++++++++++");
                        Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.ff"));
                    }
                }
            });
        }

        private void Refresh()
        {
            chrome.Reload();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var result = MessageBox.Show("Really close? Press no for background working", "Warning", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes)
            {
                e.Cancel = true;
                this.Hide();
                ni.Visible = true;
            }
        }
    }
}
