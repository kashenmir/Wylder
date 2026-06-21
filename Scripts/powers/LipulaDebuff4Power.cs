using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using wylder.Scripts.cards;

namespace wylder.Scripts.powers;

public class LipulaDebuff4Power : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override string? CustomPackedIconPath => "res://wylder/powers/demons_plating_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/demons_plating_power.png";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("count", 10)];
    
    public override int DisplayAmount => DynamicVars["count"].IntValue;
    
    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if (cardSource == null || cardSource is not LipulaChoose4)
        {
            return;
        }

        await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), Owner, Amount-1, true);
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), Owner, -2, Owner, null);
        await PowerCmd.Apply<DexterityPower>(new ThrowingPlayerChoiceContext(), Owner, -2, Owner, null);
    }
    
    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        if (player != Owner.Player ||  DynamicVars["count"].IntValue == 0)
        {
            return count;
        }
        return count - 1;
    }
    
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == CombatSide.Player && DynamicVars["count"].IntValue > 0)
        {
            if (DynamicVars["count"].IntValue <= 1)
            {
                await CreatureCmd.GainMaxHp(Owner, Amount*1.6m);
                await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, 4, Owner, null);
                await PowerCmd.Apply<DexterityPower>(choiceContext, Owner, 4, Owner, null);
            }
            DynamicVars["count"].UpgradeValueBy(-1);
            InvokeDisplayAmountChanged();
        }
    }
}