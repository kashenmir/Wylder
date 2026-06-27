using Godot;
using MegaCrit.Sts2.Core.Logging;

namespace wylder.Scripts.patchs;

public partial class NAct4Selector : Control
{
	private Button _noneButton;
	private Button _boss1Button;
	
	

	public override void _Ready()
	{
		_noneButton =
			GetNode<Button>(
                "PanelContainer/VBoxContainer/NoneButton"
			);

		_boss1Button =
			GetNode<Button>(
                "PanelContainer/VBoxContainer/BalanceLawMonsterButton"
			);

		_noneButton.Pressed += OnNonePressed;
		_boss1Button.Pressed += OnBoss1Pressed;

		Select(
			Act4SelectionState.SelectedBoss
		);
	}

	private void Select(
		Act4SelectionState.BossOption option
	)
	{
		Act4SelectionState.SelectedBoss =
			option;

		_noneButton.Modulate =
			option ==
			Act4SelectionState.BossOption.None
				? Colors.Gold
				: Colors.White;

		_boss1Button.Modulate =
			option ==
			Act4SelectionState.BossOption.BalanceLawMonster
				? Colors.Gold
				: Colors.White;
	}

	private void OnNonePressed()
	{
		Select(
			Act4SelectionState.BossOption.None
		);
		Log.Warn("act4boss:"+Act4SelectionState.SelectedBoss);
	}

	private void OnBoss1Pressed()
	{
		Select(
			Act4SelectionState.BossOption.BalanceLawMonster
		);
		Log.Warn("act4boss:"+Act4SelectionState.SelectedBoss);
	}
}
