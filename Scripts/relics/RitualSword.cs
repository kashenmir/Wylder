using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using wylder.Scripts.pools;

namespace wylder.Scripts.relics;

// 加入哪个遗物池，此处为通用
[Pool(typeof(WylderRelicPool))]
public class RitualSword : CustomRelicModel
{
    // 稀有度
    public override RelicRarity Rarity => RelicRarity.Common;
    
    public override bool ShowCounter => false;
    
    // 小图标（原版85x85）
    public override string PackedIconPath => $"res://wylder/images/relics/{GetType().Name}.png";
    // 轮廓图标（原版85x85）
    protected override string PackedIconOutlinePath => $"res://wylder/images/relics/{GetType().Name}.png";
    // 大图标（原版256x256）
    protected override string BigIconPath => $"res://wylder/images/relics/{GetType().Name}.png";

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (!props.IsPoweredAttack())
        {
            return 1m;
        }
        if (dealer != Owner.Creature)
        {
            return 1m;
        }
        if (Owner.Creature.CurrentHp < Owner.Creature.MaxHp)
        {
            return 1m;
        }
        return 1.2m;
    }
}