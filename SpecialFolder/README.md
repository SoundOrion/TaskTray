### プロジェクト概要

このプログラムは、Windows 環境で
`Environment.SpecialFolder` や環境変数 (`Environment.GetEnvironmentVariable`) を利用して
システムフォルダ（`C:\Program Files` など）の実際のパスを一覧表示するサンプルです。

---

### 💻 主な機能

* `Environment.GetFolderPath(Environment.SpecialFolder.◯◯)` により、
  一般的なシステムフォルダパスを取得
* `Environment.GetEnvironmentVariable()` により、
  64bit/32bit 双方の `Program Files` ディレクトリを明示的に取得
* 取得結果をコンソールに整形して出力

---

### 🧩 出力例（64bit OS 上の 32bit アプリ実行時）

```
=== Environment.SpecialFolder ===
ProgramFiles:         C:\Program Files (x86)
ProgramFilesX86:      C:\Program Files (x86)
System:               C:\Windows\System32
Desktop:              C:\Users\UserName\Desktop
MyDocuments:          C:\Users\UserName\Documents
ApplicationData:      C:\Users\UserName\AppData\Roaming
LocalApplicationData: C:\Users\UserName\AppData\Local

=== Environment Variables ===
ProgramW6432 (64bit): C:\Program Files
ProgramFiles(x86):    C:\Program Files (x86)
ProgramFiles:         C:\Program Files (x86)

Press any key to exit...
```

---

### 🛠️ 実行方法

1. Visual Studio または .NET CLI で新しいコンソールアプリを作成します。

   ```
   dotnet new console -n FolderPathSample
   ```

2. `Program.cs` に以下のコードを貼り付けます。

   ```csharp
   using System;

   class Program
   {
       static void Main()
       {
           Console.WriteLine("=== Environment.SpecialFolder ===");
           Console.WriteLine($"ProgramFiles:         {Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)}");
           Console.WriteLine($"ProgramFilesX86:      {Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)}");
           Console.WriteLine($"System:               {Environment.GetFolderPath(Environment.SpecialFolder.System)}");
           Console.WriteLine($"Desktop:              {Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}");
           Console.WriteLine($"MyDocuments:          {Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}");
           Console.WriteLine($"ApplicationData:      {Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}");
           Console.WriteLine($"LocalApplicationData: {Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}");

           Console.WriteLine();
           Console.WriteLine("=== Environment Variables ===");
           Console.WriteLine($"ProgramW6432 (64bit): {Environment.GetEnvironmentVariable("ProgramW6432")}");
           Console.WriteLine($"ProgramFiles(x86):    {Environment.GetEnvironmentVariable("ProgramFiles(x86)")}");
           Console.WriteLine($"ProgramFiles:         {Environment.GetEnvironmentVariable("ProgramFiles")}");

           Console.WriteLine();
           Console.WriteLine("Press any key to exit...");
           Console.ReadKey();
       }
   }
   ```

3. ビルドして実行します。

   ```
   dotnet run
   ```

---

### ⚙️ 注意点

* 64bit / 32bit アプリの違いにより、`ProgramFiles` の結果が変わります。
* `ProgramW6432` は 64bit OS でのみ定義されます。
* `C:\Program Files` にアクセスする場合、書き込みは管理者権限が必要です。

---

### 🧱 動作確認環境

* Windows 10 / Windows 11
* .NET 6 / .NET 7 / .NET 8
* Visual Studio 2022 / .NET CLI

---

### 📄 ライセンス
