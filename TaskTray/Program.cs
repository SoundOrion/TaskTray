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

        // トグル状態（必要なら外部保存も可）
        private bool _featureAEnabled = true;
        private bool _featureBEnabled = false;

        public TrayAppContext()
        {
            _menu = new ContextMenuStrip();

            // 通常メニュー
            _menu.Items.Add("今すぐ処理実行", null, OnRunNowClicked);

            // ===== ツール（サブメニュー） =====
            var toolsMenu = new ToolStripMenuItem("ツール");

            // トグル項目A
            var toggleAItem = new ToolStripMenuItem("機能A 有効")
            {
                CheckOnClick = true,    //この設定をすると、クリックしたときに自動でチェック状態（✔）がトグルされます。
                Checked = _featureAEnabled  //初期状態（チェック付き or なし）を指定します。
            };
            toggleAItem.CheckedChanged += (s, e) =>
            {
                _featureAEnabled = toggleAItem.Checked;
                UpdateToggleAText(toggleAItem);

                _notifyIcon.ShowBalloonTip(
                    800,
                    "機能A",
                    _featureAEnabled ? "機能AをONにしました。" : "機能AをOFFにしました。",
                    ToolTipIcon.Info
                );
            };

            // トグル項目B
            var toggleBItem = new ToolStripMenuItem("機能B 有効")
            {
                CheckOnClick = true,
                Checked = _featureBEnabled
            };
            toggleBItem.CheckedChanged += (s, e) =>
            {
                _featureBEnabled = toggleBItem.Checked;
                UpdateToggleBText(toggleBItem);
            };

            toolsMenu.DropDownItems.Add(toggleAItem);
            toolsMenu.DropDownItems.Add(toggleBItem);

            _menu.Items.Add(toolsMenu);
            _menu.Items.Add(new ToolStripSeparator());
            _menu.Items.Add("終了", null, OnExitClicked);

            // ===== NotifyIcon =====
            _notifyIcon = new NotifyIcon
            {
                Icon = new Icon("app.ico"),
                Text = "トグル付きトレイツール",
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

            // 初期表示用にテキスト整える
            UpdateToggleAText(toggleAItem);
            UpdateToggleBText(toggleBItem);
        }

        private void UpdateToggleAText(ToolStripMenuItem item)
        {
            item.Text = _featureAEnabled ? "機能A 有効 (ON)" : "機能A 無効 (OFF)";
        }

        private void UpdateToggleBText(ToolStripMenuItem item)
        {
            item.Text = _featureBEnabled ? "機能B 有効 (ON)" : "機能B 無効 (OFF)";
        }

        // 「今すぐ処理実行」
        private void OnRunNowClicked(object sender, EventArgs e)
        {
            // トグル状態によって処理を分岐
            if (_featureAEnabled)
            {
                // 機能AがONのときの処理
            }

            if (_featureBEnabled)
            {
                // 機能BがONのときの処理
            }

            _notifyIcon.ShowBalloonTip(
                800,
                "実行",
                $"処理を実行しました。\nA={_featureAEnabled}, B={_featureBEnabled}",
                ToolTipIcon.Info
            );
        }

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