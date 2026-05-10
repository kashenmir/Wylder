using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using wylder.Scripts.pools;
using wylder.Scripts.powers;

namespace wylder.Scripts.relics;

// 加入哪个遗物池，此处为通用
[Pool(typeof(WylderRelicPool))]
public class CrucibleFeather : CustomRelicModel
{
    // 稀有度
    public override RelicRarity Rarity => RelicRarity.Rare;
    
    public override bool ShowCounter => false;
    
    // 小图标（原版85x85）
    public override string PackedIconPath => $"res://wylder/images/relics/{GetType().Name}.png";
    // 轮廓图标（原版85x85）
    protected override string PackedIconOutlinePath => $"res://wylder/images/relics/{GetType().Name}.png";
    // 大图标（原版256x256）
    protected override string BigIconPath => $"res://wylder/images/relics/{GetType().Name}.png";
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DexterityPower>()];
    
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == base.Owner && cardPlay.Card.Type == CardType.Skill)
        {
            Flash();
            await PowerCmd.Apply<CrucibleFeatherPower>(base.Owner.Creature, 1, base.Owner.Creature, null);
        }
    }
    
}