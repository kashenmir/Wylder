using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace wylder.Scripts.powers.recluse;

public class KnightsLightningSpearPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override string? CustomPackedIconPath => "res://wylder/powers/knights_lightning_spear_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/knights_lightning_spear_power.png";

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player || !Owner.IsAlive)
        {
            return;
        }

        Player player = CombatState.Players.First();
        int handCount = PileType.Hand.GetPile(player).Cards.Count;
        if (handCount > 0)
        {
            for (int i=0;i<handCount;i++) {
                VfxCmd.PlayOnCreature(Owner, "vfx/vfx_attack_lightning");
                SfxCmd.Play("event:/sfx/characters/defect/defect_lightning_evoke");
                await CreatureCmd.Damage(
                    new ThrowingPlayerChoiceContext(),
                    Owner,
                    Amount,
                    ValueProp.Unpowered,
                    null,
                    null
                );
            }
        }

        await PowerCmd.Remove(this);
    }
}
