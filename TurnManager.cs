using Godot;
using System;
using System.Collections.Generic;

public partial class TurnManager : Node
{
	private List<Player> _players;
	private Player currentPlayer;
	
	public override void _Ready()
	{
		_players = new List<Player>();
		foreach (Node player in GetTree().GetNodesInGroup("Players"))
		{
			_players.Add(player as Player);
			
		}
		currentPlayer = _players[0];
		foreach (Player player in _players)
		{
			player.AllFormationsCompletedActions += EndTurn;
		}
	}

	private void EndTurn()
	{
		foreach (Formation formation in currentPlayer.Formations)
		{
			formation.ResetActionPoints();
		}
	}

	private void ExecuteAllFormations()
	{
		currentPlayer.ExecuteAllFormations();
	}
}
