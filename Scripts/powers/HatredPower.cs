using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace wylder.Scripts.powers;

public class HatredPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;
    
    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override string? CustomPackedIconPath => "res://wylder/powers/hatred.png";
    public override string? CustomBigIconPath => "res://wylder/powers/hatred.png";

    public String TargetPlayer="";
    
    public override LocString Description
    { 
        get 
        {
            // 1. 把 Key 写成 SmartFormat 的占位符格式
            LocString fixedString = new LocString("powers", base.Id.Entry + ".description");
        
            // 2. 把真实的文本塞进变量中
            if (CombatState.Enemies.Count <= 1)
            {
                fixedString.Add("target", "本回合"+Owner.Monster.Title.GetFormattedText()+"的攻击会命中所有玩家！");
            } else if (TargetPlayer != "")
            {
                fixedString.Add("target", "本回合"+Owner.Monster.Title.GetFormattedText()+"被"+TargetPlayer+"吸引了仇恨，不会对其他玩家造成攻击伤害。");
            }
            else
            {
                fixedString.Add("target", "本回合"+Owner.Monster.Title.GetFormattedText()+"在观望，不会对任何玩家造成攻击伤害。");
            }

            return fixedString;
        } 
    }
    
    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        if (target == Owner && props.IsPoweredAttack() && result.UnblockedDamage > 0 && dealer!=null && dealer.IsPlayer && dealer.Player!=null)
        {
            TargetPlayer = dealer.Player.Creature.Name;
            InvokeDisplayAmountChanged();
        }
    }
    
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (CombatState.Enemies.Count <= 1)
        {
            return 1m;
        }
        if (dealer != Owner || !props.IsPoweredAttack())
        {
            return 1m;
        }
        if (target!=null && target.IsPlayer && target.Player!=null && target.Player.Creature.Name==TargetPlayer && dealer==Owner && props.IsPoweredAttack())
        {
            return 1m;
        }
        return 0m;
    }
    
    public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Enemy)
        {
            TargetPlayer = "";
        }
        return Task.CompletedTask;
    }
}