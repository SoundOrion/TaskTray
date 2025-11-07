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
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize(); // ← .NET 8 新方式
            Application.Run(new TrayAppContext());

            // .net frameworkはこっち
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new TrayAppContext());
        }
    }

    public class TrayAppContext : ApplicationContext
    {
        private readonly NotifyIcon _notifyIcon;
        private readonly ContextMenuStrip _menu;

        public TrayAppContext()
        {
            // ======== メインメニュー構築 ========
            _menu = new ContextMenuStrip();

            // 通常メニュー項目
            _menu.Items.Add("今すぐ処理実行", null, OnRunNowClicked);
            _menu.Items.Add("ログフォルダを開く", null, OnOpenLogClicked);

            // === サブメニュー（ツール） ===
            var toolsMenu = new ToolStripMenuItem("ツール");

            var tool1 = new ToolStripMenuItem("メニュー1", null, OnTool1Clicked);
            var tool2 = new ToolStripMenuItem("メニュー2", null, OnTool2Clicked);
            var tool3 = new ToolStripMenuItem("メニュー3", null, OnTool3Clicked);

            toolsMenu.DropDownItems.Add(tool1);
            toolsMenu.DropDownItems.Add(tool2);
            toolsMenu.DropDownItems.Add(new ToolStripSeparator());
            toolsMenu.DropDownItems.Add(tool3);

            _menu.Items.Add(toolsMenu);
            _menu.Items.Add(new ToolStripSeparator());
            _menu.Items.Add("終了", null, OnExitClicked);

            // ======== NotifyIcon 設定 ========
            _notifyIcon = new NotifyIcon
            {
                Icon = new Icon("app.ico"),           // .ico ファイルを指定
                Text = "サブメニュー付きタスクトレイ",
                ContextMenuStrip = _menu,
                Visible = true
            };

            _notifyIcon.DoubleClick += OnRunNowClicked;

            _notifyIcon.ShowBalloonTip(
                1000,
                "起動しました",
                "右クリックメニューから操作できます。",
                ToolTipIcon.Info
            );
        }

        // ---- 通常メニュー ----
        private void OnRunNowClicked(object sender, EventArgs e)
        {
            File.AppendAllText("log.txt", $"{DateTime.Now} 処理実行\n");
            _notifyIcon.ShowBalloonTip(1000, "処理完了", "今すぐ実行しました。", ToolTipIcon.Info);
        }

        private void OnOpenLogClicked(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = AppDomain.CurrentDomain.BaseDirectory,
                UseShellExecute = true
            });
        }

        // ---- サブメニューの動作 ----
        private void OnTool1Clicked(object sender, EventArgs e)
        {
            _notifyIcon.ShowBalloonTip(800, "メニュー1", "ツール1を実行しました。", ToolTipIcon.Info);
        }

        private void OnTool2Clicked(object sender, EventArgs e)
        {
            _notifyIcon.ShowBalloonTip(800, "メニュー2", "ツール2を実行しました。", ToolTipIcon.Info);
        }

        private void OnTool3Clicked(object sender, EventArgs e)
        {
            _notifyIcon.ShowBalloonTip(800, "メニュー3", "ツール3を実行しました。", ToolTipIcon.Info);
        }

        // ---- 終了 ----
        private void OnExitClicked(object sender, EventArgs e)
        {
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
            _menu.Dispose();
            ExitThread();
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