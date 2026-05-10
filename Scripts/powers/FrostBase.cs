using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Rooms;

namespace wylder.Scripts.powers;

using BaseLib.Abstracts;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

public class FrostBase : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override string? CustomPackedIconPath => "res://wylder/powers/frost_base.png";
    public override string? CustomBigIconPath => "res://wylder/powers/frost_base.png";
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(15, ValueProp.Unpowered), new IntVar("max", 10)];

    private static readonly Dictionary<Creature, int> _globalData = new Dictionary<Creature, int>();

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        // 暂时移除冰龙锁冰霜爆发上限的能力
        // foreach (Creature item in Owner.CombatState.Allies.ToList())
        // {
        //     ColdMiragePower? coldMiragePower = item.GetPower<ColdMiragePower>();
        //     if (coldMiragePower != null)
        //     {
        //         DynamicVars["max"].UpgradeValueBy(10-DynamicVars["max"].IntValue);
        //         return;
        //     }
        // }
        if (!_globalData.ContainsKey(Owner))
        {
            return;
        }
        int increase = _globalData[Owner];
        DynamicVars["max"].UpgradeValueBy(increase);
    }
    
    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power is not FrostBase || Amount < DynamicVars["max"].IntValue || amount <= 0 || power.Owner != Owner)
        {
            return;
        }
        Flash();
        //抗性提高
        if (!_globalData.TryAdd(Owner, 10))
        {
            _globalData[Owner] += 10; // 在原有数值上 +10
        }

        //检测是否有生物要因为异常亲和能力获得格挡
        foreach (Creature item in Owner.CombatState.Allies.ToList())
        {
            DebuffHealPower? debuffHealPower = item.GetPower<DebuffHealPower>();
            if (debuffHealPower != null)
            {
                await CreatureCmd.GainBlock(item, debuffHealPower.Amount, ValueProp.Unpowered, null, false);
            }
        }
   
        //检测拥有者是否可以免疫伤害
        ColdMiragePower? coldMiragePower = Owner.GetPower<ColdMiragePower>();
        if (coldMiragePower == null) {
            NCreature? nCreature = NCombatRoom.Instance?.GetCreatureNode(Owner);
            if (nCreature != null)
            {
                NGaseousImpactVfx child = NGaseousImpactVfx.Create(nCreature.VfxSpawnPosition, new Color("#84A7A9"));
                NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(child);
            }
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner,
            (int)(base.DynamicVars.Damage.IntValue + Owner.MaxHp * 0.05m), ValueProp.Unblockable | ValueProp.Unpowered,
            null, null);
        }
        if (Owner.IsAlive)
        {
            await PowerCmd.Remove(this);
            await PowerCmd.Apply<Frost>(Owner, 3, Owner, null);
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