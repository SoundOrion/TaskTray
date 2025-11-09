using System.Diagnostics;
using System.Reflection;

namespace TaskTray
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            Text = "About MyBusinessApp Launcher";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(420, 320);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.White;

            // =================== アイコン（埋め込みリソースから） ===================
            Icon? appIcon = GetEmbeddedIconOrDefault("TaskTray.app.ico");

            var pictureBox = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.CenterImage,
                Size = new Size(64, 64),
                Location = new Point(25, 25),
                Image = appIcon?.ToBitmap()
            };

            // =================== タイトル・概要 ===================
            var titleLabel = new Label
            {
                Text = "MyBusinessApp Launcher",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(110, 35)
            };

            var versionLabel = new Label
            {
                Text = $"Version {Application.ProductVersion}",
                Font = new Font("Segoe UI", 9),
                AutoSize = true,
                Location = new Point(110, 65)
            };

            var descLabel = new Label
            {
                Text = "タスクトレイ常駐型の業務アプリランチャーです。\n" +
                       "複数環境の起動やドキュメント参照を簡単に行えます。",
                Font = new Font("Segoe UI", 9),
                AutoSize = false,
                Location = new Point(25, 110),
                Size = new Size(360, 60)
            };

            // =================== サポートリンク ===================
            var link = new LinkLabel
            {
                Text = "サポートサイトを開く",
                AutoSize = true,
                Location = new Point(25, 180)
            };
            link.LinkClicked += (s, e) =>
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://wiki.example.com/mybusinessapp",
                    UseShellExecute = true
                });

            // =================== 閉じるボタン ===================
            var closeBtn = new Button
            {
                Text = "閉じる",
                DialogResult = DialogResult.OK,
                Size = new Size(80, 30),
                Location = new Point(300, 230)
            };

            Controls.AddRange(new Control[] { pictureBox, titleLabel, versionLabel, descLabel, link, closeBtn });
        }

        /// <summary>
        /// 埋め込みリソースのアイコンを取得。見つからなければ既定のシステムアイコン。
        /// </summary>
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
                Debug.WriteLine($"アイコン読み込み失敗: {ex.Message}");
            }

            return SystemIcons.Application;
        }
    }
}
