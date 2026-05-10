using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace wylder.Scripts.powers;

public class PoisonBase : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override string? CustomPackedIconPath => "res://wylder/powers/poison_base.png";
    public override string? CustomBigIconPath => "res://wylder/powers/poison_base.png";
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("max", 10)];

    private static readonly Dictionary<Creature, int> _globalData = new Dictionary<Creature, int>();

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if (!_globalData.ContainsKey(Owner))
        {
            return;
        }
        int increase = _globalData[Owner];
        DynamicVars["max"].UpgradeValueBy(increase);
    }

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power is not PoisonBase || Amount < DynamicVars["max"].IntValue || amount <= 0 || power.Owner != Owner)
        {
            return;
        }
        Flash();
        if (!_globalData.TryAdd(Owner, 10))
        {
            _globalData[Owner] += 10; // 在原有数值上 +10
        }
        
        foreach (Creature item in Owner.CombatState.Allies.ToList())
        {
            DebuffHealPower? debuffHealPower = item.GetPower<DebuffHealPower>();
            if (debuffHealPower != null)
            {
                await CreatureCmd.GainBlock(item, debuffHealPower.Amount, ValueProp.Unpowered, null, false);
            }
        }
        
        NCreature? nCreature = NCombatRoom.Instance?.GetCreatureNode(Owner);
        if (nCreature != null)
        {
            NGaseousImpactVfx child = NGaseousImpactVfx.Create(nCreature.VfxSpawnPosition, new Color("#83A165"));
            NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(child);
        }
        if (Owner.IsAlive)
        {
            await PowerCmd.Remove(this);
            await PowerCmd.Apply<Poison>(Owner, 10, Owner, null);
        }
        else
        {
            await Cmd.CustomScaledWait(0.1f, 0.25f);
        }
    }


    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == CombatSide.Enemy)
        {
            await PowerCmd.TickDownDuration(this);
        }
    }
    
    public override async Task AfterCombatEnd(CombatRoom _)
    {
        _globalData.Clear();
    }
}