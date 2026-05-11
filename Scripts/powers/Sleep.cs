using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.ValueProps;

namespace wylder.Scripts.powers;

public class Sleep : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override string? CustomPackedIconPath => "res://wylder/powers/sleep.png";
    public override string? CustomBigIconPath => "res://wylder/powers/sleep.png";

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if (Owner.IsAlive && Owner.IsMonster && applier == Owner)
        {
            Flash();
            if (Owner.Monster == null)
            {
                throw new InvalidOperationException("Can't stun a player.");
            }
            if (CombatState != null && !Owner.IsDead)
            {
                List<MonsterState> stateLog = Owner.Monster.MoveStateMachine.StateLog;
                string nextMoveId = stateLog.Last().Id;
                MoveState state = new MoveState("SLEEP", Wrapper, new SleepIntent())
                {
                    FollowUpStateId = nextMoveId,
                    MustPerformOnceBeforeTransitioning = false
                };
                Owner.Monster.SetMoveImmediate(state);
            }
            async Task Wrapper(IReadOnlyList<Creature> c)
            {
                // NStunnedVfx? vfx = NStunnedVfx.Create(Owner.Monster.Creature);
                // if (vfx != null)
                // {
                //     Callable.From(delegate
                //     {
                //         NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(vfx);
                //     }).CallDeferred();
                // }
                await Task.CompletedTask;
            }
        }
        else
        {
            await Cmd.CustomScaledWait(0.1f, 0.25f);
        }
    }
    
    public override bool TryModifyPowerAmountReceived(PowerModel canonicalPower, Creature target, decimal amount, Creature? applier, out decimal modifiedAmount)
    {
        // 1. 先假设我们不拦截，数值保持不变
        modifiedAmount = amount;
        
        if (target != Owner)
        {
            modifiedAmount = amount;
            return false; // 不拦截，使用原本的数值
        }

        // 2. 安全检查：确保 canonicalPower 是 FrostbitePower 的原型
        //    并且当前正在处理的 target (怪物) 身上确实有 HeatPower
        if (canonicalPower is SleepBase 
            && target.HasPower<Sleep>()) // <-- 这里换成你实际的“另一种状态”类名
        {
            // 3. 拦截逻辑：把要增加的层数强制归零
            modifiedAmount = 0;

            // 4. 返回 true，告诉系统："我修改了数值，请用我修改后的 (0) "
            return true;
        }

        // 5. 如果没有 HeatPower，返回 false，表示不修改数值，正常施加
        return false;
    }
    
    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == Owner && props.IsPoweredAttack() && result.UnblockedDamage != 0 && Owner.IsAlive)
        {
            Flash();
            await CreatureCmd.Stun(Owner,(IReadOnlyList<Creature> _) => Task.CompletedTask, null);
            await PowerCmd.Remove(this);
        }
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == CombatSide.Enemy)
        {
            await PowerCmd.TickDownDuration(this);
            if (Amount>0 && Owner.IsAlive && Owner.IsMonster)
            {
                Flash();
                if (Owner.Monster == null)
                {
                    throw new InvalidOperationException("Can't stun a player.");
                }
                if (CombatState != null && !Owner.IsDead)
                {
                    List<MonsterState> stateLog = Owner.Monster.MoveStateMachine.StateLog;
                    string nextMoveId = stateLog.Last().Id;
                    MoveState state = new MoveState("SLEEP", Wrapper, new SleepIntent())
                    {
                        FollowUpStateId = nextMoveId,
                        MustPerformOnceBeforeTransitioning = false
                    };
                    Owner.Monster.SetMoveImmediate(state);
                }
                async Task Wrapper(IReadOnlyList<Creature> c)
                {
                    // NStunnedVfx? vfx = NStunnedVfx.Create(Owner.Monster.Creature);
                    // if (vfx != null)
                    // {
                    //     Callable.From(delegate
                    //     {
                    //         NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(vfx);
                    //     }).CallDeferred();
                    // }
                    await Task.CompletedTask;
                }
            }
            else
            {
                await Cmd.CustomScaledWait(0.1f, 0.25f);
            }
        }
    }
}