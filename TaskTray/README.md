## 🟢 1. チェックマーク（Check）

✅ **最もよく使う ON/OFF 表示**

```csharp
item.CheckOnClick = true;
item.Checked = true; // 初期状態
```

→ 自動で ✔ マークが付きます。

---

## 🟣 2. ラジオボタン（RadioCheck）

🔘 **複数の項目のうち一つだけ選択したい場合** に使えます。
複数アイテムの中で排他的にチェックマークを付けられます。

```csharp
var item1 = new ToolStripMenuItem("モードA") { RadioCheck = true };
var item2 = new ToolStripMenuItem("モードB") { RadioCheck = true };
var item3 = new ToolStripMenuItem("モードC") { RadioCheck = true };

// 同じグループ内で1つだけONにしたい場合、自前で切り替える
item1.Click += (s, e) => SetMode(item1, item2, item3);
item2.Click += (s, e) => SetMode(item2, item1, item3);
item3.Click += (s, e) => SetMode(item3, item1, item2);

void SetMode(params ToolStripMenuItem[] items)
{
    foreach (var i in items)
        i.Checked = (i == items[0]); // 最初のアイテムを選択扱いに
}
```

これで「モードA / モードB / モードC」がラジオボタンのように排他選択できます。

---

## 🟠 3. イメージ（Image）

🎨 アイコン付きメニューも可能です。

```csharp
item.Image = Image.FromFile("run.png");
```

→ メニュー左にアイコン画像が出ます。
（チェックマークの代わりにも使えます）

---

## 🔵 4. Enabled / Visible

メニューの**無効化・非表示**も簡単です。

```csharp
item.Enabled = false;  // グレーアウト
item.Visible = false;  // 完全に非表示
```

動的に切り替えることもできるので、「処理中はボタン無効」みたいな制御もOK。

---

## 🟤 5. ShortcutKeys（ショートカットキー表示）

ショートカットキーをメニュー右側に表示できます（押しても反応しないけど表示はされる）。

```csharp
item.ShortcutKeys = Keys.Control | Keys.R;
```

→ メニュー右側に「Ctrl+R」と表示されます。

---

## ⚫ 6. OwnerDraw（カスタム描画）

もっと凝ったUI（色つきメニュー、進捗バー付きなど）を作るときに使います。

```csharp
item.OwnerDraw = true;
item.Paint += (s, e) => { /* 独自描画 */ };
```

ただしこれはWinFormsの中でも上級テクです。

---

## 🟢 7. ToolTipText

右クリックメニューでは標準では表示されませんが、`NotifyIcon`のヒントとして流用可能。

```csharp
item.ToolTipText = "機能Aを有効/無効にします";
```

---

## 🧩 まとめ表

| 機能          | プロパティ / イベント              | 用途           |
| ----------- | ------------------------- | ------------ |
| ✅ チェック      | `CheckOnClick`, `Checked` | ON/OFFトグル    |
| 🔘 ラジオ      | `RadioCheck`              | 排他選択（1つだけ選ぶ） |
| 🖼 アイコン     | `Image`                   | アイコン付きメニュー   |
| 🚫 無効化      | `Enabled = false`         | 実行中は押せない状態   |
| 👁 非表示      | `Visible = false`         | 条件付きで隠す      |
| ⌨ ショートカット表示 | `ShortcutKeys`            | Ctrl+Xなどの表示  |
| 🎨 カスタム描画   | `OwnerDraw`               | デザインを自由に描く   |
| 💬 ヒントテキスト  | `ToolTipText`             | 説明を表示（主に内部用） |
