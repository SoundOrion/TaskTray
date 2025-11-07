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
Console.WriteLine($"SystemRoot:           {Environment.GetEnvironmentVariable("SystemRoot")}");
Console.WriteLine($"UserProfile:          {Environment.GetEnvironmentVariable("UserProfile")}");
Console.WriteLine($"Temp (TMP):           {Environment.GetEnvironmentVariable("TMP")}");
Console.WriteLine($"Temp (TEMP):          {Environment.GetEnvironmentVariable("TEMP")}");

Console.WriteLine();
Console.WriteLine("Press any key to exit...");
Console.ReadKey();