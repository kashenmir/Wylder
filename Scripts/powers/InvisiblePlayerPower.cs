using Godot;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace wylder.Scripts.powers;

public class InvisiblePlayerPower : LipulaCurseBasePower
{
    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        foreach (Player player in CombatState.Players)
        {
            if (LocalContext.IsMe(player))
            {
                continue;
            }
            NCreature? nCreature = NCombatRoom.Instance?.GetCreatureNode(player.Creature);
            if (nCreature != null)
            {
                nCreature.Visible = false;
            }
        }

        foreach (Node node in NRun.Instance?.GlobalUi.MultiplayerPlayerContainer.GetChildren())
        {
            if (node is NMultiplayerPlayerState)
            {
                if (!LocalContext.IsMe(((NMultiplayerPlayerState)node).Player))
                {
                    ((NMultiplayerPlayerState)node).Visible = false;
                }
            }
        }

        NMultiplayerVoteContainer? voteContainer = NCombatRoom.Instance?.Ui.EndTurnButton.GetNode<NMultiplayerVoteContainer>("PlayerIconContainer");
        if (voteContainer != null)
        {
            voteContainer.Visible = false;
        }
        return Task.CompletedTask;
    }

    public override Task AfterRemoved(Creature oldOwner)
    {
        foreach (Player player in CombatState.Players)
        {
            if (player == Owner.Player)
            {
                continue;
            }
            NCreature? nCreature = NCombatRoom.Instance?.GetCreatureNode(player.Creature);
            if (nCreature != null)
            {
                nCreature.Visible = true;
            }
        }
        foreach (Node node in NRun.Instance?.GlobalUi.MultiplayerPlayerContainer.GetChildren())
        {
            if (node is NMultiplayerPlayerState)
            {
                if (!LocalContext.IsMe(((NMultiplayerPlayerState)node).Player))
                {
                    ((NMultiplayerPlayerState)node).Visible = true;
                }
            }
        }
        NMultiplayerVoteContainer? voteContainer = NCombatRoom.Instance?.Ui.EndTurnButton.GetNode<NMultiplayerVoteContainer>("PlayerIconContainer");
        if (voteContainer != null)
        {
            voteContainer.Visible = true;
        }
        return Task.CompletedTask;
    }
}