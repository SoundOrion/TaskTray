using System.Diagnostics;

namespace TaskTray
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TrayAppContext());
        }
    }

    public class TrayAppContext : ApplicationContext
    {
        private readonly NotifyIcon _notifyIcon;
        private readonly ContextMenuStrip _menu;

        public TrayAppContext()
        {
            // メニュー作成
            _menu = new ContextMenuStrip();
            _menu.Items.Add("今すぐ処理実行", null, OnRunNowClicked);
            _menu.Items.Add("ログフォルダを開く", null, OnOpenLogClicked);
            _menu.Items.Add(new ToolStripSeparator());
            _menu.Items.Add("終了", null, OnExitClicked);

            // NotifyIcon 作成
            _notifyIcon = new NotifyIcon
            {
                Icon = new Icon("app.ico"),           // プロジェクトに追加した .ico
                Text = "タスクトレイ常駐ツール",
                ContextMenuStrip = _menu,
                Visible = true
            };

            // 左ダブルクリックで「今すぐ処理実行」にしてもOK
            _notifyIcon.DoubleClick += OnRunNowClicked;

            // 起動通知（任意）
            _notifyIcon.ShowBalloonTip(
                1000,
                "起動しました",
                "右クリックメニューから操作できます。",
                ToolTipIcon.Info
            );
        }

        /// <summary>
        /// 「今すぐ処理実行」が押されたときの処理
        /// </summary>
        private void OnRunNowClicked(object sender, EventArgs e)
        {
            try
            {
                // ★ここに実行したい本処理を書く ★
                // 例: 簡単なログを書き出す
                var logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
                Directory.CreateDirectory(logDir);
                var logPath = Path.Combine(logDir, "log.txt");
                File.AppendAllText(logPath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} 実行しました{Environment.NewLine}");

                _notifyIcon.ShowBalloonTip(
                    800,
                    "処理完了",
                    "タスクが正常に実行されました。",
                    ToolTipIcon.Info
                );
            }
            catch (Exception ex)
            {
                _notifyIcon.ShowBalloonTip(
                    1500,
                    "エラー",
                    ex.Message,
                    ToolTipIcon.Error
                );
            }
        }

        /// <summary>
        /// 「ログフォルダを開く」が押されたとき
        /// </summary>
        private void OnOpenLogClicked(object sender, EventArgs e)
        {
            var logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = logDir,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                _notifyIcon.ShowBalloonTip(
                    1500,
                    "フォルダを開けませんでした",
                    ex.Message,
                    ToolTipIcon.Error
                );
            }
        }

        /// <summary>
        /// 「終了」が押されたとき
        /// </summary>
        private void OnExitClicked(object sender, EventArgs e)
        {
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
            _menu.Dispose();
            ExitThread(); // Application.ExitThread と同等
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _notifyIcon?.Dispose();
                _menu?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}