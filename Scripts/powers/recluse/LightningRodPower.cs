using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace wylder.Scripts.powers.recluse;

public class LightningRodPower : CustomPowerModel
{
    private const string _countKey = "Countdown";

    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => DynamicVars[_countKey].IntValue;

    public override string? CustomPackedIconPath => "res://wylder/powers/lightning_rod_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/lightning_rod_power.png";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar(_countKey, 4m)];

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(Owner))
        {
            return;
        }

        DynamicVars[_countKey].BaseValue -= 1m;
        InvokeDisplayAmountChanged();

        if (DynamicVars[_countKey].BaseValue <= 0m)
        {
            foreach (Creature enemy in CombatState.HittableEnemies)
            {
                VfxCmd.PlayOnCreature(enemy, "vfx/vfx_attack_lightning");
                SfxCmd.Play("event:/sfx/characters/defect/defect_lightning_evoke");
                await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), enemy, Amount, ValueProp.Unpowered, null, null);
            }
            await PowerCmd.Remove(this);
        }
    }
}
