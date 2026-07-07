using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using wylder.Scripts.cards;
using wylder.Scripts.cards.recluse;
using wylder.Scripts.powers;
using wylder.Scripts.powers.recluse;

namespace wylder.Scripts.patchs;

public partial class NMagicUi : Control
{
    public Player Player { get; private set; }

    private TextureRect _icon;
    private Label _text;
    private List<TextureRect> _magicNodes; 
    private List<MagicPower>  _magicPowers;

    private bool _hover;
    
    private bool _running;
    
    private Creature? _target;

    private String _magicPowerPic = "res://wylder/powers/pure_magic_power.png";
    
    private String _firePowerPic = "res://wylder/powers/pure_fire_power.png";
    
    private String _lightingPowerPic =  "res://wylder/powers/pure_lighting_power.png";
    
    private String _holyPowerPic =  "res://wylder/powers/pure_holy_power.png";

    private String _nonePowerPic = "res://wylder/scenes/pure_null_power.png";
    
    

    public override void _Ready()
    {
        _icon = GetNode<TextureRect>("Icon");
        _text = GetNode<Label>("Label");
        _magicNodes = new List<TextureRect>()
        {
            GetNode<TextureRect>("Magic1"),
            GetNode<TextureRect>("Magic2"),
            GetNode<TextureRect>("Magic3"),
        };
        _magicPowers = new List<MagicPower>()
        {
            new MagicPower(1, MagicPowerOption.None),
            new MagicPower(1, MagicPowerOption.None),
            new MagicPower(1, MagicPowerOption.None),
        };

        _text.Visible = false;

        _icon.MouseFilter = MouseFilterEnum.Stop;
        _icon.GuiInput += OnIconInput;
    }

    public override void _Process(double delta)
    {
        Vector2 mouse = _icon.GetGlobalMousePosition();
        bool nowHover = _icon.GetGlobalRect().HasPoint(mouse);

        if (nowHover && !_hover)
        {
            _hover = true;
            _text.Visible = true;
        }
        else if (!nowHover && _hover)
        {
            _hover = false;
            _text.Visible = false;
        }
    }

    private async void OnIconInput(InputEvent e)
    {
        // 悬停（用 motion 判断）
        if (e is InputEventMouseMotion)
        {
            if (_icon.GetGlobalRect().HasPoint(_icon.GetGlobalMousePosition()) && !_running)
                _text.Visible = true;
            else
                _text.Visible = false;
        }

        // 点击
        if (e is InputEventMouseButton mb && mb.Pressed)
        {
            if (mb.ButtonIndex == MouseButton.Left)
            {
                await Click();
            }

            if (mb.ButtonIndex == MouseButton.Right)
            {
                GD.Print("Icon Right Click, Player:" + Player.Creature.CurrentHp);
                if (_running) return;

                _running = true;
                try
                {
                    await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), Player.Creature, 1,
                        Player.Creature, null);
                }
                finally
                {
                    _running = false;
                }
            }
        }
    }
    
    private async Task Click()
    {
        try
        {
            if (_running) return;

            _running = true;

            GD.Print("Icon Left Click, Player:" + Player.Creature.CurrentHp);
            if (!IsMagicReady())
            {
                _target = null;
                await SingleCreatureTargeting();
                if (_target != null && _target.IsEnemy)
                {
                    bool isGetMagic = false;
                    foreach (PowerModel powerModel in _target.Powers.ToList())
                    {
                        if (powerModel is PureMagicPower)
                        {
                            InsertMagicPower(new MagicPower(1, MagicPowerOption.Magic));
                            await PowerCmd.Remove(powerModel);
                            isGetMagic = true;
                            break;
                        } else if (powerModel is PureFirePower)
                        {
                            InsertMagicPower(new MagicPower(1, MagicPowerOption.Fire));
                            await PowerCmd.Remove(powerModel);
                            isGetMagic = true;
                            break;
                        } else if (powerModel is PureLightingPower)
                        {
                            InsertMagicPower(new MagicPower(1, MagicPowerOption.Lightning));
                            await PowerCmd.Remove(powerModel);
                            isGetMagic = true;
                            break;
                        } else if (powerModel is PureHolyPower)
                        {
                            InsertMagicPower(new MagicPower(1, MagicPowerOption.Holy));
                            await PowerCmd.Remove(powerModel);
                            isGetMagic = true;
                            break;
                        }
                    }

                    if (isGetMagic)
                    {
                        CardModel? cardModel = (await CardSelectCmd.FromHandForDiscard(new BlockingPlayerChoiceContext(), Player, new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 1), null, Player.Character)).FirstOrDefault();
                        if (cardModel != null)
                        {
                            await CardCmd.Discard(new ThrowingPlayerChoiceContext(), cardModel);
                        }
                    }
                    else
                    {
                        TalkCmd.Play(new LocString("characters", "WYLDER-RECLUSE.quitGetMagic"), Player.Creature, VfxColor.White);
                    }
                }
            }
            else
            {
                Type? cardType = ResolveMagicCardType();
                if (cardType != null)
                {
                    await (Task)typeof(CardPileCmd)
                        .GetMethod("AddToCombatAndPreview", new[] { typeof(Creature), typeof(PileType), typeof(int), typeof(Player), typeof(CardPilePosition) })!
                        .MakeGenericMethod(cardType)
                        .Invoke(null, new object?[] { Player.Creature, PileType.Hand, 1, Player, CardPilePosition.Bottom });
                }
                CleanMagicPowers();
            }
            _running = false;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[MagicUi Click Error] {ex}");
            _running = false;
        }
    }

    private async Task SingleCreatureTargeting()
    {
        var cts = new CancellationTokenSource();
        NTargetManager tm = NTargetManager.Instance;
        tm.StartTargeting(
            TargetType.AnyEnemy,
            this, // 从你的按钮节点出发
            TargetMode.ReleaseMouseToTarget,
            () => cts.IsCancellationRequested, // 外部可取消
            node => node is NCreature
        );
        Node? result = await tm.SelectionFinished();
        if (result is NCreature creature)
        {
            _target = ((NCreature)result).Entity;
        }
        else
        {
            _target = null;
        }
    }

    private Type? ResolveMagicCardType()
    {
        HashSet<MagicPowerOption> elements = new();
        foreach (MagicPower mp in _magicPowers)
        {
            if (mp.Option != MagicPowerOption.None)
                elements.Add(mp.Option);
        }

        return elements.Count switch
        {
            1 => ResolveSingle(elements),
            2 => ResolveDouble(elements),
            3 => ResolveTriple(elements),
            _ => null
        };
    }

    private static Type? ResolveSingle(HashSet<MagicPowerOption> elements)
    {
        MagicPowerOption e = elements.First();
        return e switch
        {
            MagicPowerOption.Magic => typeof(PureMagic),
            MagicPowerOption.Fire => typeof(PureFire),
            MagicPowerOption.Lightning => typeof(PureLightning),
            MagicPowerOption.Holy => typeof(PureHoly),
            _ => null
        };
    }

    private static Type? ResolveDouble(HashSet<MagicPowerOption> elements)
    {
        bool m = elements.Contains(MagicPowerOption.Magic);
        bool f = elements.Contains(MagicPowerOption.Fire);
        bool l = elements.Contains(MagicPowerOption.Lightning);
        bool h = elements.Contains(MagicPowerOption.Holy);

        if (m && f) return typeof(MagicFire);
        if (m && l) return typeof(MagicLightning);
        if (m && h) return typeof(MagicHoly);
        if (f && l) return typeof(FireLightning);
        if (f && h) return typeof(FireHoly);
        if (l && h) return typeof(LightningHoly);
        return null;
    }

    private static Type? ResolveTriple(HashSet<MagicPowerOption> elements)
    {
        bool m = elements.Contains(MagicPowerOption.Magic);
        bool f = elements.Contains(MagicPowerOption.Fire);
        bool l = elements.Contains(MagicPowerOption.Lightning);
        bool h = elements.Contains(MagicPowerOption.Holy);

        if (m && f && l) return typeof(MagicFireLightning);
        if (m && f && h) return typeof(MagicFireHoly);
        if (f && l && h) return typeof(FireLightningHoly);
        if (m && l && h) return typeof(MagicLightningHoly);
        return null;
    }

    private Boolean HasMagicPower(Creature creature)
    {
        foreach (PowerModel powerModel in creature.Powers.ToList())
        {
            if (powerModel is BasicMaicPower)
            {
                return true;
            }
        }
        return false;
    }

    public void Initialize(Player player)
    {
        Player = player;
    }

    public void InsertMagicPower(MagicPower power)
    {
        if (power.Option == MagicPowerOption.None)
        {
            return;
        }
        for (int i = 0; i < _magicPowers.Count; i++)
        {
            if (_magicPowers[i].Option == MagicPowerOption.None)
            {
                _magicPowers[i].Option = power.Option;
                _magicPowers[i].Count = power.Count;
                break;
            }
        }   
        UpdateMagicNodes();
    }

    private bool IsMagicReady()
    {
        foreach (MagicPower magicPower in _magicPowers)
        {
            if (magicPower.Option == MagicPowerOption.None)
            {
                return false;
            }
        }
        return true;
    }

    public void CleanMagicPowers()
    {
        _magicPowers = new List<MagicPower>()
        {
            new MagicPower(1, MagicPowerOption.None),
            new MagicPower(1, MagicPowerOption.None),
            new MagicPower(1, MagicPowerOption.None),
        };
        UpdateMagicNodes();
    }

    public void UpdateMagicNodes()
    {
        for (int i = 0; i < _magicNodes.Count; i++)
        {
            switch (_magicPowers[i].Option)
            {
                case MagicPowerOption.None:
                    _magicNodes[i].Texture = GD.Load<Texture2D>(_nonePowerPic);
                    break;
                case MagicPowerOption.Magic:
                    _magicNodes[i].Texture = GD.Load<Texture2D>(_magicPowerPic);
                    break;
                case MagicPowerOption.Fire:
                    _magicNodes[i].Texture = GD.Load<Texture2D>(_firePowerPic);
                    break;
                case MagicPowerOption.Lightning:
                    _magicNodes[i].Texture = GD.Load<Texture2D>(_lightingPowerPic);
                    break;
                case MagicPowerOption.Holy:
                    _magicNodes[i].Texture = GD.Load<Texture2D>(_holyPowerPic);
                    break;
                default:
                    _magicNodes[i].Texture = GD.Load<Texture2D>(_nonePowerPic);
                    break;
            }
        }

        if (IsMagicReady())
        {
            _text.Text = "单击发动混合魔法";
        }
        else
        {
            _text.Text = "单击以丢弃一张手牌\n来获取属性痕迹";
        }
    }
}