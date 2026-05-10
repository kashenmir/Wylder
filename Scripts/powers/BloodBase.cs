using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using wylder.Scripts.cards;
using wylder.Scripts.relics;

namespace wylder.Scripts.powers;

public class BloodBase : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override string? CustomPackedIconPath => "res://wylder/powers/blood_base.png";
    public override string? CustomBigIconPath => "res://wylder/powers/blood_base.png";
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(15, ValueProp.Unpowered), new IntVar("max", 10)];
    
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
        if (power is not BloodBase || Amount < DynamicVars["max"].IntValue || amount<=0 || power.Owner != Owner)
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

            if (item.IsPlayer && item.Player != null)
            {
                foreach (RelicModel relicModel in item.Player.Relics.ToList())
                {
                    if (relicModel is LordOfBloodsExultation)
                    {
                        await PowerCmd.Apply<StrengthPower>(item, 2, item, null);
                    }
                }
            }
        }
        // 如果不存在，直接设为 10
        VfxCmd.PlayOnCreatureCenter(base.Owner, "vfx/vfx_bloody_impact");
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner,
            (int)(base.DynamicVars.Damage.IntValue + Owner.MaxHp * 0.05m), ValueProp.Unblockable | ValueProp.Unpowered,
            null, null);
        if (Owner.IsAlive)
        {
            await PowerCmd.Remove(this);
        }
        else
        {
            await Cmd.CustomScaledWait(0.1f, 0.25f);
        }

        if (applier != null && cardSource != null)
        {
            if (cardSource is BloodBoonRitual)
            {
                await PowerCmd.Apply<StrengthPower>(applier, 3, applier, null);
            }
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