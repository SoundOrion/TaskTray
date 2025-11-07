using ImageMagick;

class Program
{
    // ICOに含めるサイズ（必要に応じて削ってOK）
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

                // 必要ならここで拡張子フィルタ
                if (ext is not ".png" and not ".jpg" and not ".jpeg" and not ".bmp" and not ".gif")
                {
                    Console.WriteLine($"[SKIP] 対応していない拡張子: {fullPath}");
                    errorCount++;
                    continue;
                }

                var dir = Path.GetDirectoryName(fullPath)!;
                var name = Path.GetFileNameWithoutExtension(fullPath);
                var output = Path.Combine(dir, $"{name}.ico");

                // 複数サイズ入りICO生成
                using (var icoImages = new MagickImageCollection())
                {
                    foreach (var size in IconSizes)
                    {
                        using var image = new MagickImage(fullPath);

                        // 正方形に収まるようにリサイズ（比率維持）
                        image.Resize((uint)size, (uint)size);

                        // 32bit (透過あり) を想定
                        image.Depth = 8; // チャンネルあたりビット数 (8bit x 4 = 32bpp 相当)
                        image.Format = MagickFormat.Png; // ICO内でPNG形式として埋め込める

                        // コレクションに追加（Cloneして渡す）
                        icoImages.Add(image.Clone());
                    }

                    // ICOとして書き出し
                    icoImages.Write(output, MagickFormat.Ico);
                }

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
}
