using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using wylder.Scripts.pools;

namespace wylder.Scripts.relics;

[Pool(typeof(EventRelicPool))]
public class BoiledCrab : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool ShowCounter => DisplayAmount > -1;

    public override int DisplayAmount
    {
        get
        {
            if (!CombatManager.Instance.IsInProgress)
                return -1;
            int expireTurn = DynamicVars["ExpireTurn"].IntValue;
            int turnNumber = Owner.PlayerCombatState.TurnNumber;
            if (turnNumber >= expireTurn)
                return -1;
            return turnNumber;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("Dexterity", 2), new IntVar("ExpireTurn", 7)];

    public override string PackedIconPath => $"res://wylder/images/relics/{GetType().Name}.png";
    protected override string PackedIconOutlinePath => $"res://wylder/images/relics/{GetType().Name}.png";
    protected override string BigIconPath => $"res://wylder/images/relics/{GetType().Name}.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DexterityPower>()];

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is CombatRoom)
        {
            Status = RelicStatus.Normal;
            Flash();
            await PowerCmd.Apply<DexterityPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars["Dexterity"].IntValue, Owner.Creature, null);
            InvokeDisplayAmountChanged();
        }
    }

    public override Task AfterSideTurnStart(CombatSide side, IReadOnlyList<MegaCrit.Sts2.Core.Entities.Creatures.Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature))
            return Task.CompletedTask;
        if (Owner.PlayerCombatState.TurnNumber == DynamicVars["ExpireTurn"].IntValue)
        {
            Status = RelicStatus.Active;
        }
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<MegaCrit.Sts2.Core.Entities.Creatures.Creature> participants)
    {
        if (!participants.Contains(Owner.Creature))
            return;
        int expireTurn = DynamicVars["ExpireTurn"].IntValue;
        int turnNumber = Owner.PlayerCombatState.TurnNumber;
        Status = RelicStatus.Normal;
        if (turnNumber == expireTurn)
        {
            Flash();
            await PowerCmd.Apply<DexterityPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, -DynamicVars["Dexterity"].IntValue, Owner.Creature, null);
            InvokeDisplayAmountChanged();
        }
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        Status = RelicStatus.Normal;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
}
