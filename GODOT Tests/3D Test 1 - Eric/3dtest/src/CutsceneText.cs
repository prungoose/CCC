using Godot;
using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

public partial class CutsceneText : RichTextLabel
{
    private Tween _tween;
    private int textPos = 0;
    private bool typeStart = false;
    private AudioStreamPlayer CutsceneSFX;
    private RichTextLabel BottomTip;

    public ConfigFile CF = new ConfigFile();
    private float val;
    private int sfx_index;
    [Export] SceneTransition _transitionscene;

    private TextureRect MatIMG; // d2a245   (0.824f, 0.635f, 0.271f, 1.0f)
    private TextureRect InuIMG; // b09af4   (0.69f, 0.604f, 0.957f, 1.0f)
    private TextureRect MizIMG; // 58bfb4   (0.345f, 0.749f, 0.706f, 1.0f)
    private TextureRect NekIMG; // 5694cd (0.335f, 0.58f, 0.802f, 1.0f)
    private TextureRect ShiIMG; // d780c0 (0.841f, 0.503f, 0.752f, 1.0f)
    private RichTextLabel CharaName;
    int charactersIntroduced = 1;
    int language = 0;

    public FontFile JFont = GD.Load<FontFile>("res://assets/menu/Futehodo-MaruGothic.ttf");
	public FontFile EFont = GD.Load<FontFile>("res://assets/menu/Atop.ttf");

    float _acc = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _transitionscene = GetNode<SceneTransition>("/root/SceneTransition");

        sfx_index = AudioServer.GetBusIndex("SFX");
        MatIMG = GetParent().GetParent().GetNode<Control>("Character Images").GetNode<TextureRect>("Matsumoto");
        InuIMG = GetParent().GetParent().GetNode<Control>("Character Images").GetNode<TextureRect>("Inumaru");
        MizIMG = GetParent().GetParent().GetNode<Control>("Character Images").GetNode<TextureRect>("Mizuki");
        NekIMG = GetParent().GetParent().GetNode<Control>("Character Images").GetNode<TextureRect>("Nekota");
        ShiIMG = GetParent().GetParent().GetNode<Control>("Character Images").GetNode<TextureRect>("Shinroku");
        CharaName = GetParent().GetParent().GetNode<RichTextLabel>("Character Name");

        BottomTip = GetParent().GetNode<RichTextLabel>("Bottom Tip");
        CutsceneSFX = GetParent().GetParent().GetNode<AudioStreamPlayer>("CutsceneSFX");
        GD.Print("preSequenceText ready: ", Name, " | In tree: ", IsInsideTree());
        // Set the text to be displayed
        // Show the text
        Show();
        BottomTip.Modulate = new Color(1, 1, 1, 0);

        if (CF.Load(OS.GetUserDataDir() + "/" + "PlayerSettings.cfg") != Error.Ok)
        {
            AudioServer.SetBusVolumeDb(sfx_index, Mathf.LinearToDb(1f));
            BottomTip.Text = "Press Space/Click to Continue...";
            BottomTip.AddThemeFontOverride("font", EFont);
            text = Englishtext;
            CharaName.Text = "Matsumoto";
            CharaName.AddThemeFontOverride("font", EFont);


            this.AddThemeFontOverride("font", EFont);
        }
        else
        {
            val = (float)CF.GetValue("playersettings", "sfx");
            language = (int)CF.GetValue("playersettings", "lang");
            AudioServer.SetBusVolumeDb(sfx_index, Mathf.LinearToDb(val));
            if (language == 1)
            {
                BottomTip.Text = "スペースキーまたはクリックして続行します...";
                BottomTip.AddThemeFontOverride("font", JFont);
                text = JapaneseText;
                CharaName.Text = "松本";
                CharaName.AddThemeFontOverride("font", JFont);

                this.AddThemeFontOverride("font", JFont);
            }
            else
            {
                BottomTip.Text = "Press Space/Click to Continue...";
                BottomTip.AddThemeFontOverride("font", EFont);
                text = Englishtext;
                CharaName.Text = "Matsumoto";
                CharaName.AddThemeFontOverride("font", EFont);

                this.AddThemeFontOverride("font", EFont);
            }
        }

    }

    // Text to be displayed in the visual novel.
    private string[] text = { };
    private string[] Englishtext = {
        "Welcome to Crisis Cleanup Crew! This is our test playground where we’ll teach you everything we know before assigning you to your first job!",
        "Cleaning up after a natural disaster is very important! It helps create room for the city to rebuild and prevents further issues in plumbing and roadways, and it keeps the citizens safe.",
        "Your job is the reason that the city continues to remain safe and operable after a typhoon, so do your best!",
    };
    private string[] JapaneseText = {
        "クライシス・クリーンアップ・クルーへようこそ！ここは私たちのテストの場です。最初の仕事に配属される前に、私たちが知っていることの全てをお教えします！",
        "自然災害後の清掃は非常に重要です！街の復興のためのスペースを確保し、水道や道路のさらなる問題を防ぐのに役立ちます。そして、住民の安全を守ることにもつながります。",
        "台風後も街が安全に機能し続けるのはあなたの仕事のおかげです。頑張ってください！"
    };

    private string[] IMEnglishtext = {
        "Oh hey, you're the person on cleanup duty, right?",
        "I used to live in the area, but I had to evacuate since the typhoon was looking pretty bad...",
    };
    private string[] IMJapanesetext = {
        "ああ、あなたは掃除当番ですよね？",
        "以前、この地域に住んでいましたが、台風がかなりひどくなりそうだったので避難しなければなりませんでした...",
    };
    private string[] MZEnglishtext = {
        "Me too, I had to leave to my grandma's house an hour over...",
        "I know your guys' jobs are tough, so do your best!"
    };
    private string[] MZJapanesetext = {
        "私も、おばあちゃんの家に行くのに1時間も遅れて出発しなければならなかったので...",
        "皆さんの仕事は大変だとは思いますが、頑張ってください！",
    };

    // private string[] NTEnglishtext = {
    //     "...",
    // };
    // private string[] NTJapanesetext = {
    //     "...",
    // };
    // private string[] SREnglishtext = {
    //     "...",
    // };
    // private string[] SRJapanesetext = {
    //     "...",
    // };


    //Have space change text
    public override void _Process(double delta)
    {
        _acc += (float)delta;
        // Check if the player pressed the space key or left mouse button
        if (Input.IsActionJustPressed("jump") || Input.IsActionJustPressed("m1"))
        {
            if (text.Length > 1)
            {
                //Text = text[0];
                //text = text[1..];
                text = text[1..];
                this.Text = "";
                textPos = 0;
                CutsceneSFX.Play();
                BottomTip.Modulate = new Color(1, 1, 1, 0);
            }
            else if ((Input.IsActionJustPressed("jump") || Input.IsActionJustPressed("m1")) && text.Length == 1)
            {
                if (charactersIntroduced == 1)
                {
                    if (language == 1)
                    {
                        changeSpeaker("犬丸");
                    }
                    else
                    {
                        changeSpeaker("Inumaru");
                    }
                    charactersIntroduced += 1;
                }
                else if (charactersIntroduced == 2)
                {
                    if (language == 1)
                    {
                        changeSpeaker("海月");
                    }
                    else
                    {
                        changeSpeaker("Mizuki");
                    }
                    charactersIntroduced += 1;
                }
                // else if (charactersIntroduced == 3)
                // {
                //     if (language == 1)
                //     {
                //         changeSpeaker("猫田");
                //     }
                //     else
                //     {
                //         changeSpeaker("Nekota");
                //     }
                //     charactersIntroduced += 1;
                // }
                // else if (charactersIntroduced == 4)
                // {
                //     if (language == 1)
                //     {
                //         changeSpeaker("神鹿");
                //     }
                //     else
                //     {
                //         changeSpeaker("Shinroku");
                //     }
                //     charactersIntroduced += 1;
                // }
                else // ADD MORE ELSE IFS IF MORE NPCS ARE ADDED
                {
                    _transitionscene.Call("ChangeScene", "res://assets/level/SceneLoader.tscn");
                    //res://assets/level/SceneLoader.tscn res://assets/level/testscene.tscn
                }
            }
        }
        while (_acc > 0.01f)
        {
            TypeText(text[0]);
            _acc -= 0.01f;
        }

    }

    public void TypeText(string text)
    {
        if (textPos >= 0 && textPos < text.Length) // checks if texting position is correct and so it doesn't go over the amount that needs to be typed
        {

            this.Text += text[textPos];
            textPos++; // moves to the next char in the str
        }
        else if (textPos >= text.Length) // go here once it goes over the amount typed (when it's done typing)
        {
            textPos = -1; // blocks typing until the person presses m1 or jump
            FadeIn();
        }
    }

    public void FadeIn()
    {
        _tween?.Kill();
        _tween = GetTree().CreateTween();
        _tween.TweenProperty(BottomTip, "modulate:a", 1, 1f);
        _tween.Play();
    }

    public void changeSpeaker(string newNPC)
    {
        CharaName.Text = newNPC;
        StyleBoxFlat charaColor = new();
        textPos = 0;
        this.Text = "";
        CutsceneSFX.Play();
        BottomTip.Modulate = new Color(1, 1, 1, 0);
        charaColor.BorderColor = new Color(1f, 1f, 1f, 1f);
        charaColor.BorderBlend = false;
        charaColor.CornerRadiusBottomLeft = 20;
        charaColor.CornerRadiusBottomRight = 20;
        charaColor.CornerRadiusTopLeft = 20;
        charaColor.CornerRadiusTopRight = 20;
        charaColor.BorderWidthLeft = 4;
        charaColor.BorderWidthRight = 4;
        charaColor.BorderWidthTop = 4;
        charaColor.BorderWidthBottom = 4;
        MatIMG.Hide();
        InuIMG.Hide();
        MizIMG.Hide();
        NekIMG.Hide();
        ShiIMG.Hide();

        if (newNPC == "Inumaru" || newNPC == "犬丸")
        {
            InuIMG.Show();

            charaColor.BgColor = new Color(0.69f, 0.604f, 0.957f, 1.0f);

            CharaName.AddThemeStyleboxOverride("normal", charaColor);

            if (language == 1)
            {
                text = IMJapanesetext;
            }
            else
            {
                text = IMEnglishtext;
            }

        }
        else if (newNPC == "Mizuki" || newNPC == "海月")
        {
            MizIMG.Show();

            charaColor.BgColor = new Color(0.345f, 0.749f, 0.706f, 1.0f);

            CharaName.AddThemeStyleboxOverride("normal", charaColor);

            if (language == 1)
            {
                text = MZJapanesetext;
            }
            else
            {
                text = MZEnglishtext;
            }
        }
        // else if (newNPC == "Nekota" || newNPC == "猫田")
        // {
        //     NekIMG.Show();

        //     charaColor.BgColor = new Color(0.335f, 0.58f, 0.802f, 1.0f);

        //     CharaName.AddThemeStyleboxOverride("normal", charaColor);

        //     if (language == 1)
        //     {
        //         text = NTJapanesetext;
        //     }
        //     else
        //     {
        //         text = NTEnglishtext;
        //     }
        // }
        // else if (newNPC == "Shinroku" || newNPC == "神鹿")
        // {
        //     ShiIMG.Show();

        //     charaColor.BgColor = new Color(0.841f, 0.503f, 0.752f, 1.0f);

        //     CharaName.AddThemeStyleboxOverride("normal", charaColor);

        //     if (language == 1)
        //     {
        //         text = SRJapanesetext;
        //     }
        //     else
        //     {
        //         text = SREnglishtext;
        //     }
        // }
    }
}