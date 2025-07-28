using Godot;
using System.Collections.Generic;
using System.Net.Http;

public partial class TutorialStuff : MarginContainer
{
    private AudioStreamPlayer TextBubbleSFX;
    private Control _textbox;
    private RichTextLabel _objlabel;

    private int _step = 0;
    private int language = 0;
    public ConfigFile CF = new ConfigFile();

    private string[][] _messages = [];
    private string[][] _Emessages =
    [
        ["Welcome! Move around using [b]WASD[/b], and you can change the camera angle with [b]Q/E[/b].",
        "Try heading over to the glowing blue beacon over there!"],
        ["By using [b][color=misty_rose]Left Click[/color][/b], you can vacuum trash.",
            "Try filling any of the four tanks to 50% by vacuuming trash!"],
        ["Great job! Now, let's learn how to toss trash.",
            "Use [b][color=misty_rose]1-4[/color][/b] to select the type of trash you want to toss.",
            "Then, once your tank is at 50% capacity or more, hold [b][color=misty_rose]Right Click[/color][/b] to charge up a throw.",
            "Finally, release [b]Right Click[/b] to toss a bag of the selected trash!",
            "Your tank holds [b][color=#ff4e4a]Combustibles[/color][/b], [b][color=#57e792]Plastics[/color][/b], [b][color=#e9c261]PET Bottles[/color][/b], and [b][color=#5a98fa]Cans & Glass Bottles[/color][/b].",
            "Try throwing a bag of [b][color=#ff4e4a]Combustibles[/color][/b] into the correct trash bin!",
            "Trash bins appear on the [b]Minimap[/b] as a colored circle with an outline."],
        ["Amazing! Try tossing trash into one of each colored bin."],
        ["Head to the flashing symbol on the map to continue your cleanup journey."],
        ["This object represents a major hazard that you need to alert the authorities about.",
            "Open your phone with [b]F[/b] to learn more about it."],
        ["Find the [b]agency[/b] that can deal with this hazard, quickly!"],
        ["The agency's phone number is listed at the bottom. Dial the inputs using [b]WASD[/b] and press [b]SPACE[/b] or click \"DIAL\" to dial them."],
        ["You now have a flare that can alert the agency to the hazard's location.",
            "Throw it at the hazard just like trash, using [b]Right Click[/b]."],
        ["Great job! You now know how to deal with hazards!",
                "The minimap at the bottom-left shows the location of hazards as a large warning sign.",
                "Try to fully clean the area. Your job will be complete once the bar at the bottom right reaches 100%."],
        ["Thanks for playing our game!"]
    ];
    private string[][] _Jmessages =
    [
        ["危機解決協会へようこそ！[b]WASD[/b]で移動してみ？", "あそこの青いビーコンに向かってみてください！"],
        ["[b][color=misty_rose]左クリック[/color][/b]でゴミをパパっと吸い上げる！", "ほら、タンク四つあるでしょ？ ゴミをバキュームで吸って一個でも50パーセントにしよう。"],
        ["次は捨て方を叩き込もう。",
            "[b][color=misty_rose]１～４[/color][/b]でゴミの種類を選んで。",
            "タンクの容量が50パーセント以上になったら、[b][color=misty_orange]右クリック[/color][/b]を長押して勢いを調整して。。。",
            "放ってゴミ袋を投げよう！",
            "タンクには[b][color=#ff4e4a]燃えるゴミ[/color][/b]、[b][color=#57e792]プラスチックごみ[/color][/b]、[b][color=#e9c261]ペットボトル[/color][/b]、[b][color=#5a98fa]缶[/color][/b]・瓶の四種類がある。",
            "さあ、燃えるゴミを正しいゴミ箱に捨ててみ？",
            "ゴミ箱は[b]ミニマップ[/b]上に輪郭の付いた色付きの円として表示されます。"],
        ["ゴミをちゃんと分別すればするほどゲームが進行する！"],
        ["清掃の旅を続けるには、地図上で点滅しているシンボルに向かいます。"],
        ["この地域が結構危険やね。", "[b]F[/b]で携帯をとって情報を確認しよう。"],
        ["この危険に対処できる機関をすぐに見つけてください。"],
        ["代理店の電話番号は下部に記載されています。[b]WASD[/b]キーを押しながら[b]SPACE[/b]キーを押すか、\"ダイヤル\" をクリックしてダイヤルしてください。"],
        ["これで、危険物の位置を当局に警告できる信号弾が手に入りました。[b]右クリック[/b]を使用して、ゴミと同じように危険物に投げてください。"],
        ["よくできました！危険に対処する方法がわかりました！", "左下のミニマップには、危険の場所が大きな警告サインとして表示されます。"],
        ["プロトタイプをプレイしていただきありがとうございます!"]
    ];

    private string[] _objs =
    [
        "Head over to the blue beacon near the park using WASD",
        "Vacuum 20 (50% of a tank) trash of a single type",
        "Toss a bag of Combustibles in a red bin by holding and releasing Right Click",
        "Toss trash into one of each colored bin",
        "Head towards the flashing symbol in the park",
        "Open your phone with F",
        "Find the agency that can deal with the hazard",
        "Dial the number of the Fire Department using WASD and SPACE",
        "Throw the flare at the hazard using Right Click",
        "Clean the area to 100% completion!",
        "Thanks for playing our game!"
    ];


    public override void _Ready()
    {
        var JFont = GD.Load<FontFile>("res://assets/menu/Futehodo-MaruGothic.ttf");
        var EFont = GD.Load<FontFile>("res://assets/menu/Atop.ttf");

        TextBubbleSFX = GetParent().GetNode<AudioStreamPlayer>("TextBubbleSFX");
        _textbox = GetParent().GetNode<Control>("Textbox");
        _objlabel = GetParent().GetNode<RichTextLabel>("Minimap/PanelContainer/CenterContainer/ObjLabel");

        if (CF.Load(OS.GetUserDataDir() + "/" + "PlayerSettings.cfg") != Error.Ok)
        {
            _messages = _Emessages;
            _textbox.AddThemeFontOverride("font", EFont);
        }
        else
        {
            language = (int)CF.GetValue("playersettings", "lang");
            if (language == 1)
            {
                _messages = _Jmessages;
                _textbox.AddThemeFontOverride("font", JFont);

            }
            else
            {
                _messages = _Emessages;
                _textbox.AddThemeFontOverride("font", EFont);
            }
        }
        ShowStep(_step);
    }

    public void ShowStep(int step)
    {
        if (step < _messages.Length)
        {
            _textbox.Call("PopUp", _messages[step], 1);
            _objlabel.Text = _objs[step];
        }
    }

    public void NextStep()
    {
        _step++;
        ShowStep(_step);
    }

    public int GetTutorialStep()
    {
        return _step;
    }
}
