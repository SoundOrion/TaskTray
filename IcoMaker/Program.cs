using System;
using System.IO;
using ImageMagick;

class Program
{
    // ICOに含めるサイズ（必要に応じて増減OK）
    private static readonly int[] IconSizes = { 16, 24, 32, 48, 64, 128, 256 };

    static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("画像ファイルをこの exe にドラッグ＆ドロップすると .ico を生成します。");
            Console.WriteLine("対応: PNG / JPG / JPEG / BMP / GIF など");
            return 0;
        }

        int errorCount = 0;

        foreach (var input in args)
        {
            try
            {
                if (!File.Exists(input))
                {
                    Console.WriteLine($"[SKIP] ファイルが見つかりません: {input}");
                    errorCount++;
                    continue;
                }

                var fullPath = Path.GetFullPath(input);
                var ext = Path.GetExtension(fullPath).ToLowerInvariant();

                // 必要なら対応拡張子を絞る
                if (ext is not ".png" and not ".jpg" and not ".jpeg" and not ".bmp" and not ".gif")
                {
                    Console.WriteLine($"[SKIP] 対応していない拡張子: {fullPath}");
                    errorCount++;
                    continue;
                }

                var dir = Path.GetDirectoryName(fullPath)!;
                var name = Path.GetFileNameWithoutExtension(fullPath);
                var output = Path.Combine(dir, $"{name}.ico");

                CreateIco(fullPath, output);

                Console.WriteLine($"[OK] {output}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERR] {input}: {ex.Message}");
                errorCount++;
            }
        }

        return errorCount == 0 ? 0 : 1;
    }

    //private static void CreateIco(string srcPath, string outputPath)
    //{
    //    // 複数サイズ入りICO生成
    //    using (var icoImages = new MagickImageCollection())
    //    {
    //        foreach (var size in IconSizes)
    //        {
    //            using var image = new MagickImage(srcPath);

    //            // 正方形に収まるようにリサイズ（比率維持）
    //            image.Resize((uint)size, (uint)size);

    //            // 32bit (透過あり) を想定
    //            image.Depth = 8; // チャンネルあたりビット数 (8bit x 4 = 32bpp 相当)
    //            image.Format = MagickFormat.Png; // ICO内でPNG形式として埋め込める

    //            // コレクションに追加（Cloneして渡す）
    //            icoImages.Add(image.Clone());
    //        }

    //        // ICOとして書き出し
    //        icoImages.Write(outputPath, MagickFormat.Ico);
    //    }
    //}

    /// <summary>
    /// 比率を維持しつつ余白を黒透明にする処理追加バージョン
    /// </summary>
    private static void CreateIco(string srcPath, string outputPath)
    {
        using var icoImages = new MagickImageCollection();

        foreach (var size in IconSizes)
        {
            using var src = new MagickImage(srcPath);

            // カラースペース・アルファ有効化（透過維持用）
            src.ColorSpace = ColorSpace.sRGB;
            src.Alpha(AlphaOption.On);

            // アスペクト比を維持したまま、指定サイズ内に収まるよう計算
            double scale = Math.Min(
                (double)size / src.Width,
                (double)size / src.Height
            );

            var targetW = Math.Max(1, (int)Math.Round(src.Width * scale));
            var targetH = Math.Max(1, (int)Math.Round(src.Height * scale));

            // 元を壊さないようクローンして加工
            using var frame = src.Clone();

            // 比率維持でリサイズ
            frame.Resize((uint)targetW, (uint)targetH);

            // キャンバスを size×size にして中央配置、余白は透明
            frame.Extent(
                (uint)size,
                (uint)size,
                Gravity.Center,
                MagickColors.Transparent
            );

            // ICO内ではPNG形式（32bit）で埋め込む
            frame.Format = MagickFormat.Png;
            frame.Depth = 8; // 8bit/チャンネル (32bpp相当)

            // コレクションに追加（クローンして渡す）
            icoImages.Add(frame.Clone());
        }

        // ICOとして書き出し
        icoImages.Write(outputPath, MagickFormat.Ico);
    }
}
