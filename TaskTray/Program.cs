using System.Diagnostics;
using System.Reflection;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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
            Application.Run(new TrayAppContext4());

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

    public class TrayAppContext2 : ApplicationContext
    {
        private readonly NotifyIcon _notifyIcon;
        private readonly ContextMenuStrip _menu;

        // 環境種別
        private enum AppEnvironment
        {
            Prod,
            Staging,
            Uat,
            Dev
        }

        // 現在選択中の環境（初期値は UAT にしておく例）
        private AppEnvironment _selectedEnv = AppEnvironment.Uat;

        // 環境メニュー項目
        private ToolStripMenuItem _envProdItem;
        private ToolStripMenuItem _envStgItem;
        private ToolStripMenuItem _envUatItem;
        private ToolStripMenuItem _envDevItem;

        // 「この環境で起動」メニュー
        private ToolStripMenuItem _launchCurrentEnvItem;
        private ToolStripMenuItem _launcherMenu;

        public TrayAppContext2()
        {
            _menu = new ContextMenuStrip();

            // アプリ名ヘッダ（飾り用：クリック不可）
            var appTitleItem = new ToolStripMenuItem("★ MyBusinessApp Launcher");
            appTitleItem.Enabled = false;
            _menu.Items.Add(appTitleItem);

            // ===== 起動ランチャー サブメニュー =====
            _launcherMenu = new ToolStripMenuItem("起動ランチャー");

            // 「この環境で起動」項目（選択中の環境に応じてテキスト更新）
            _launchCurrentEnvItem = new ToolStripMenuItem();
            _launchCurrentEnvItem.Click += OnLaunchClicked;
            _launcherMenu.DropDownItems.Add(_launchCurrentEnvItem);
            _launcherMenu.DropDownItems.Add(new ToolStripSeparator());

            // 環境選択メニューたち（1つだけチェックされるようにする）
            _envProdItem = CreateEnvItem("本番環境", AppEnvironment.Prod);
            _envStgItem = CreateEnvItem("ステージング環境", AppEnvironment.Staging);
            _envUatItem = CreateEnvItem("UAT環境", AppEnvironment.Uat);
            _envDevItem = CreateEnvItem("開発環境", AppEnvironment.Dev);

            _launcherMenu.DropDownItems.Add(_envProdItem);
            _launcherMenu.DropDownItems.Add(_envStgItem);
            _launcherMenu.DropDownItems.Add(_envUatItem);
            _launcherMenu.DropDownItems.Add(_envDevItem);

            _menu.Items.Add(_launcherMenu);
            _menu.Items.Add(new ToolStripSeparator());
            _menu.Items.Add("終了", null, OnExitClicked);

            // ===== NotifyIcon 設定 =====
            _notifyIcon = new NotifyIcon
            {
                Icon = new Icon("app.ico"), // 任意のアイコン
                Text = "MyBusinessApp Launcher",
                ContextMenuStrip = _menu,
                Visible = true
            };

            // ダブルクリックで「この環境で起動」してもかっこいい
            _notifyIcon.DoubleClick += OnLaunchClicked;

            // 初期選択を反映
            ApplyEnvSelection(_selectedEnv);

            // 起動案内（任意）
            _notifyIcon.ShowBalloonTip(
                1000,
                "起動しました",
                "起動ランチャーから環境を選んで起動できます。",
                ToolTipIcon.Info
            );
        }

        // 環境メニュー生成（チェック付き）
        private ToolStripMenuItem CreateEnvItem(string text, AppEnvironment env)
        {
            var item = new ToolStripMenuItem(text)
            {
                CheckOnClick = true,
                Tag = env,
                // ラジオっぽくしたければこれもアリ（見た目が●になる）
                // RadioCheck = true
            };

            item.Click += OnEnvMenuClick;
            return item;
        }

        // 環境メニュークリック時：1つだけチェック状態にする
        private void OnEnvMenuClick(object sender, EventArgs e)
        {
            if (sender is not ToolStripMenuItem clicked) return;

            var env = (AppEnvironment)clicked.Tag;
            ApplyEnvSelection(env);
        }

        // 選択環境の反映（チェック状態＋表示テキスト更新）
        private void ApplyEnvSelection(AppEnvironment env)
        {


            _selectedEnv = env;

            // 全解除 → 選択だけON
            _envProdItem.Checked = (env == AppEnvironment.Prod);
            _envStgItem.Checked = (env == AppEnvironment.Staging);
            _envUatItem.Checked = (env == AppEnvironment.Uat);
            _envDevItem.Checked = (env == AppEnvironment.Dev);

            // 「この環境で起動」メニューの表示更新
            _launchCurrentEnvItem.Text = $"この環境で起動（現在：{GetEnvLabel(env)}）";
        }

        private string GetEnvLabel(AppEnvironment env) => env switch
        {
            AppEnvironment.Prod => "本番",
            AppEnvironment.Staging => "ステージング",
            AppEnvironment.Uat => "UAT",
            AppEnvironment.Dev => "開発",
            _ => env.ToString()
        };

        // 実際の起動処理
        private void OnLaunchClicked(object sender, EventArgs e)
        {
            try
            {
                // ここで環境ごとの URL / EXE を振り分ける
                var target = _selectedEnv switch
                {
                    AppEnvironment.Prod => "https://prod.example.com",
                    AppEnvironment.Staging => "https://stg.example.com",
                    AppEnvironment.Uat => "https://uat.example.com",
                    AppEnvironment.Dev => "https://dev.example.com",
                    _ => "https://dev.example.com"
                };

                Process.Start(new ProcessStartInfo
                {
                    FileName = target,
                    UseShellExecute = true
                });

                _notifyIcon.ShowBalloonTip(
                    800,
                    "起動しました",
                    $"{GetEnvLabel(_selectedEnv)}環境を開きました。",
                    ToolTipIcon.Info
                );
            }
            catch (Exception ex)
            {
                _notifyIcon.ShowBalloonTip(
                    1500,
                    "起動エラー",
                    ex.Message,
                    ToolTipIcon.Error
                );
            }
            finally
            {
                _menu.AutoClose = true;
                _launcherMenu.DropDown.AutoClose = true;
            }
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

    public class TrayAppContext3 : ApplicationContext
    {
        private readonly NotifyIcon _notifyIcon;
        private readonly ContextMenuStrip _menu;

        private enum AppEnvironment
        {
            Prod,
            Staging,
            Uat,
            Dev
        }

        private AppEnvironment _selectedEnv = AppEnvironment.Uat;

        private ToolStripMenuItem _envProdItem;
        private ToolStripMenuItem _envStgItem;
        private ToolStripMenuItem _envUatItem;
        private ToolStripMenuItem _envDevItem;
        private ToolStripMenuItem _launchCurrentEnvItem;
        private ToolStripMenuItem _launcherMenu;

        public TrayAppContext3()
        {
            _menu = new ContextMenuStrip();

            // アプリ名ヘッダ（飾り）
            var appTitleItem = new ToolStripMenuItem("★ MyBusinessApp Launcher");
            appTitleItem.Enabled = false;
            _menu.Items.Add(appTitleItem);

            // ================= 起動ランチャー =================
            _launcherMenu = new ToolStripMenuItem("Launcher");

            _launchCurrentEnvItem = new ToolStripMenuItem();
            _launchCurrentEnvItem.Click += OnLaunchClicked;
            _launcherMenu.DropDownItems.Add(_launchCurrentEnvItem);
            _launcherMenu.DropDownItems.Add(new ToolStripSeparator());

            _envProdItem = CreateEnvItem("Prod", AppEnvironment.Prod);
            _envStgItem = CreateEnvItem("Staging", AppEnvironment.Staging);
            _envUatItem = CreateEnvItem("UAT", AppEnvironment.Uat);
            _envDevItem = CreateEnvItem("Dev", AppEnvironment.Dev);

            _launcherMenu.DropDownItems.Add(_envProdItem);
            _launcherMenu.DropDownItems.Add(_envStgItem);
            _launcherMenu.DropDownItems.Add(_envUatItem);
            _launcherMenu.DropDownItems.Add(_envDevItem);

            _menu.Items.Add(_launcherMenu);

            // ================= ドキュメント =================
            var docsMenu = new ToolStripMenuItem("ドキュメント");

            // ここに好きなだけ追加（URLでもローカルパスでもOK）
            docsMenu.DropDownItems.Add(new ToolStripMenuItem("利用者向けマニュアル", null,
                (s, e) => OpenDoc("https://wiki.example.com/user-manual")));

            docsMenu.DropDownItems.Add(new ToolStripMenuItem("運用手順書", null,
                (s, e) => OpenDoc(@"C:\Docs\operations.pdf")));

            docsMenu.DropDownItems.Add(new ToolStripMenuItem("システム仕様書", null,
                (s, e) => OpenDoc(@"C:\Docs\system-spec.pdf")));

            // 区切りと「ドキュメントフォルダを開く」みたいなのもアリ
            docsMenu.DropDownItems.Add(new ToolStripSeparator());
            docsMenu.DropDownItems.Add(new ToolStripMenuItem("ドキュメントフォルダを開く", null,
                (s, e) => OpenDoc(@"C:\Docs")));

            //docsMenu.DragOver += OnTrue;
            _menu.Items.Add(docsMenu);

            // ================= 終了 =================
            _menu.Items.Add(new ToolStripSeparator());
            _menu.Items.Add("終了", null, OnExitClicked);

            // ================= NotifyIcon =================
            _notifyIcon = new NotifyIcon
            {
                Icon = new Icon("app.ico"),
                Text = "MyBusinessApp Launcher",
                ContextMenuStrip = _menu,
                Visible = true
            };

            _notifyIcon.DoubleClick += OnLaunchClicked;

            ApplyEnvSelection(_selectedEnv);

            //_notifyIcon.ShowBalloonTip(
            //    1000,
            //    "起動しました",
            //    "環境を選んで起動 / ドキュメント参照が可能です。",
            //    ToolTipIcon.Info
            //);
        }

        // 環境メニュー
        private ToolStripMenuItem CreateEnvItem(string text, AppEnvironment env)
        {
            var item = new ToolStripMenuItem(text)
            {
                CheckOnClick = true,
                // RadioCheck = true; // ●で見せたいなら有効化
                Tag = env
            };
            item.Click += OnEnvMenuClick;
            return item;
        }

        private void OnEnvMenuClick(object sender, EventArgs e)
        {
            if (sender is not ToolStripMenuItem clicked) return;
            var env = (AppEnvironment)clicked.Tag;
            ApplyEnvSelection(env);
        }

        private void ApplyEnvSelection(AppEnvironment env)
        {
            _selectedEnv = env;

            _envProdItem.Checked = (env == AppEnvironment.Prod);
            _envStgItem.Checked = (env == AppEnvironment.Staging);
            _envUatItem.Checked = (env == AppEnvironment.Uat);
            _envDevItem.Checked = (env == AppEnvironment.Dev);

            _launchCurrentEnvItem.Text = $"Launch in {GetEnvLabel(env)} Environment";
        }

        private string GetEnvLabel(AppEnvironment env) => env switch
        {
            AppEnvironment.Prod => "Prod",
            AppEnvironment.Staging => "Staging",
            AppEnvironment.Uat => "UAT",
            AppEnvironment.Dev => "Dev",
            _ => env.ToString()
        };

        // 選択中の環境で起動
        private void OnLaunchClicked(object sender, EventArgs e)
        {
            try
            {
                var target = _selectedEnv switch
                {
                    AppEnvironment.Prod => "https://prod.example.com",
                    AppEnvironment.Staging => "https://stg.example.com",
                    AppEnvironment.Uat => "https://uat.example.com",
                    AppEnvironment.Dev => "https://dev.example.com",
                    _ => "https://dev.example.com"
                };

                Process.Start(new ProcessStartInfo
                {
                    FileName = target,
                    UseShellExecute = true
                });

                _notifyIcon.ShowBalloonTip(
                    800,
                    "起動しました",
                    $"{GetEnvLabel(_selectedEnv)}環境を開きました。",
                    ToolTipIcon.Info
                );
            }
            catch (Exception ex)
            {
                _notifyIcon.ShowBalloonTip(
                    1500,
                    "起動エラー",
                    ex.Message,
                    ToolTipIcon.Error
                );
            }
            finally
            {
                _menu.AutoClose = true;
                _launcherMenu.DropDown.AutoClose = true;
                _launcherMenu.DropDown.Close();
                _menu.Close();
            }
        }

        // ドキュメントオープン共通
        private void OpenDoc(string pathOrUrl)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = pathOrUrl,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                _notifyIcon.ShowBalloonTip(
                    1500,
                    "ドキュメントを開けませんでした",
                    ex.Message,
                    ToolTipIcon.Error
                );
            }
        }

        private void OnExitClicked(object sender, EventArgs e)
        {
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
            _launcherMenu.Dispose();
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

    public class TrayAppContext4 : ApplicationContext
    {
        private readonly NotifyIcon _notifyIcon;
        private readonly ContextMenuStrip _menu;

        private enum AppEnvironment
        {
            Prod,
            Staging,
            Uat,
            Dev
        }

        private AppEnvironment _selectedEnv = AppEnvironment.Uat;

        private ToolStripMenuItem _envProdItem;
        private ToolStripMenuItem _envStgItem;
        private ToolStripMenuItem _envUatItem;
        private ToolStripMenuItem _envDevItem;
        private ToolStripMenuItem _launchCurrentEnvItem;

        public TrayAppContext4()
        {
            // ================= ContextMenuStrip 本体 =================
            _menu = new ContextMenuStrip();

            //// アプリ名ヘッダ（飾り）
            //var appTitleItem = new ToolStripMenuItem("★ MyBusinessApp Launcher")
            //{
            //    Enabled = false
            //};
            //_menu.Items.Add(appTitleItem);

            // ================= 起動ランチャー =================
            var launcherMenu = new ToolStripMenuItem("Launcher");

            // 「現在選択中の環境で起動」メニュー
            _launchCurrentEnvItem = new ToolStripMenuItem();
            _launchCurrentEnvItem.Click += OnLaunchClicked;
            launcherMenu.DropDownItems.Add(_launchCurrentEnvItem);
            launcherMenu.DropDownItems.Add(new ToolStripSeparator());

            // 環境選択メニュー（クリックしてもメニューは閉じない）
            _envProdItem = CreateEnvItem("Prod", AppEnvironment.Prod);
            _envStgItem = CreateEnvItem("Staging", AppEnvironment.Staging);
            _envUatItem = CreateEnvItem("UAT", AppEnvironment.Uat);
            _envDevItem = CreateEnvItem("Dev", AppEnvironment.Dev);

            launcherMenu.DropDownItems.Add(_envProdItem);
            launcherMenu.DropDownItems.Add(_envStgItem);
            launcherMenu.DropDownItems.Add(_envUatItem);
            launcherMenu.DropDownItems.Add(_envDevItem);

            // 「環境メニューは項目クリックでは閉じない」制御
            launcherMenu.DropDown.Closing += LauncherDropDown_Closing;

            _menu.Items.Add(launcherMenu);

            // ================= ドキュメント =================
            var docsMenu = new ToolStripMenuItem("ドキュメント");

            docsMenu.DropDownItems.Add(new ToolStripMenuItem("利用者向けマニュアル", null,
                (s, e) => OpenDoc("https://wiki.example.com/user-manual")));

            docsMenu.DropDownItems.Add(new ToolStripMenuItem("運用手順書", null,
                (s, e) => OpenDoc(@"C:\Docs\operations.pdf")));

            docsMenu.DropDownItems.Add(new ToolStripMenuItem("システム仕様書", null,
                (s, e) => OpenDoc(@"C:\Docs\system-spec.pdf")));

            docsMenu.DropDownItems.Add(new ToolStripSeparator());
            docsMenu.DropDownItems.Add(new ToolStripMenuItem("ドキュメントフォルダを開く", null,
                (s, e) => OpenDoc(@"C:\Docs")));

            _menu.Items.Add(docsMenu);

            // ================= 終了 =================
            _menu.Items.Add(new ToolStripSeparator());
            _menu.Items.Add("終了", null, OnExitClicked);

            // ================= NotifyIcon =================
            var trayIcon = GetEmbeddedIconOrDefault("TaskTray.app.ico");

            _notifyIcon = new NotifyIcon
            {
                Icon = trayIcon, //Icon.ExtractAssociatedIcon(Application.ExecutablePath), //new Icon("app.ico"),
                Text = "MyBusinessApp Launcher",
                ContextMenuStrip = _menu,
                Visible = true
            };

            // アイコンのダブルクリックで現在環境を起動
            _notifyIcon.DoubleClick += OnLaunchClicked;

            // 初期環境を反映
            ApplyEnvSelection(_selectedEnv);

            // 初回バルーンを出したい場合はコメントアウト解除
            /*
            _notifyIcon.ShowBalloonTip(
                1000,
                "起動しました",
                "環境を選んで起動 / ドキュメント参照が可能です。",
                ToolTipIcon.Info
            );
            */
        }

        /// <summary>
        /// 埋め込みリソースの読み込み
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        private Icon GetEmbeddedIconOrDefault(string resourceName)
        {
            try
            {
                var asm = Assembly.GetExecutingAssembly();
                using Stream? iconStream = asm.GetManifestResourceStream(resourceName);
                if (iconStream != null)
                {
                    return new Icon(iconStream);
                }
            }
            catch (Exception ex)
            {
                // ログなどに出しても良い
                Console.WriteLine($"アイコン読み込み失敗: {ex.Message}");
            }

            // 埋め込みが見つからなかった場合は既定のシステムアイコン
            return SystemIcons.Application;
        }


        // 環境メニュー項目作成
        private ToolStripMenuItem CreateEnvItem(string text, AppEnvironment env)
        {
            var item = new ToolStripMenuItem(text)
            {
                CheckOnClick = true,
                Tag = env
            };
            item.Click += OnEnvMenuClick;
            return item;
        }

        // Launcher配下のドロップダウンが閉じようとするときの制御
        private void LauncherDropDown_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            // 環境メニューなど「項目クリック」の場合は閉じない
            if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
            {
                e.Cancel = true;
            }
            // それ以外（外側クリック、Esc、アプリ切り替えなど）は通常通り閉じる
        }

        // 環境メニュークリック
        private void OnEnvMenuClick(object sender, EventArgs e)
        {
            if (sender is not ToolStripMenuItem clicked) return;
            var env = (AppEnvironment)clicked.Tag;
            ApplyEnvSelection(env);
        }

        // チェック状態と「Launch in ...」表示更新
        private void ApplyEnvSelection(AppEnvironment env)
        {
            _selectedEnv = env;

            _envProdItem.Checked = env == AppEnvironment.Prod;
            _envStgItem.Checked = env == AppEnvironment.Staging;
            _envUatItem.Checked = env == AppEnvironment.Uat;
            _envDevItem.Checked = env == AppEnvironment.Dev;

            _launchCurrentEnvItem.Text = $"Launch in {GetEnvLabel(env)} Environment";
        }

        private string GetEnvLabel(AppEnvironment env) => env switch
        {
            AppEnvironment.Prod => "Prod",
            AppEnvironment.Staging => "Staging",
            AppEnvironment.Uat => "UAT",
            AppEnvironment.Dev => "Dev",
            _ => env.ToString()
        };

        // 選択中の環境で起動
        private void OnLaunchClicked(object sender, EventArgs e)
        {
            try
            {
                var target = _selectedEnv switch
                {
                    AppEnvironment.Prod => "https://prod.example.com",
                    AppEnvironment.Staging => "https://stg.example.com",
                    AppEnvironment.Uat => "https://uat.example.com",
                    AppEnvironment.Dev => "https://dev.example.com",
                    _ => "https://dev.example.com"
                };

                Process.Start(new ProcessStartInfo
                {
                    FileName = target,
                    UseShellExecute = true
                });

                _notifyIcon.ShowBalloonTip(
                    800,
                    "起動しました",
                    $"{GetEnvLabel(_selectedEnv)}環境を開きました。",
                    ToolTipIcon.Info
                );
            }
            catch (Exception ex)
            {
                _notifyIcon.ShowBalloonTip(
                    1500,
                    "起動エラー",
                    ex.Message,
                    ToolTipIcon.Error
                );
            }
            finally
            {
                // 起動メニューをきちんと閉じておく
                _menu.Close();
            }
        }

        // ドキュメントオープン共通
        private void OpenDoc(string pathOrUrl)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = pathOrUrl,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                _notifyIcon.ShowBalloonTip(
                    1500,
                    "ドキュメントを開けませんでした",
                    ex.Message,
                    ToolTipIcon.Error
                );
            }
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