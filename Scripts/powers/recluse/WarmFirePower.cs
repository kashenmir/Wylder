using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace wylder.Scripts.powers.recluse;

public class WarmFirePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://wylder/powers/warm_fire_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/warm_fire_power.png";

    private async Task RevertTempMaxHp(int count)
    {
        Log.Warn("触发消除");
        int hpLoss = count;
        Log.Warn("触发消除"+hpLoss);
        if (Owner.CurrentHp <= hpLoss)
        {
            hpLoss = Owner.CurrentHp - 1;
        }
        if (hpLoss > 0)
        {
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner, hpLoss, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
            await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), Owner, hpLoss, true);
        }
    }

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power is not WarmFirePower || amount >= 0 || power.Owner != Owner)
        {
            return;
        }
        Log.Warn("触发消除");
        await RevertTempMaxHp(-(int)amount);
    }

    public override async Task AfterRemoved(Creature oldOwner)
    {
        Log.Warn("触发消除");
        await RevertTempMaxHp(Amount);
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        Log.Warn("触发消除");
        if (!Owner.IsDead)
        {
            Log.Warn("触发消除");
            Flash();
            await RevertTempMaxHp(Amount);
        }
    }
}
