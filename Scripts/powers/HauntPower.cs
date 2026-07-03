using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace wylder.Scripts.powers;

public class HauntPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override string? CustomPackedIconPath => "res://wylder/powers/haunt_power.png";
    public override string? CustomBigIconPath => "res://wylder/powers/haunt_power.png";

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Player)
        {
            foreach (Player player in CombatState.Players.ToList())
            {
                IReadOnlyList<CardModel> cards = PileType.Hand.GetPile(player).Cards;
                if (cards.Count != 0)
                {
                    int num = (int)((decimal)cards.Count * Amount);
                    Flash();
                    await CreatureCmd.Damage(choiceContext, player.Creature, num, ValueProp.Unpowered, null, null);
                    VfxCmd.PlayOnCreatureCenter(player.Creature, "vfx/vfx_attack_blunt");
                }   
            }
        }
    }
}