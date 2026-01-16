using Godot;
using System;

public partial class Obstacle : Area2D
{
	public override void _Ready()
	{
		// Wire up the signal via code (safest method!)
		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node2D body)
	{
		// Check if the object that hit us is actually the Player
		// (We don't want the Floor triggering this if we accidentally placed it too low)
		if (body is Player)
		{
			// For now, let's just reload the scene instantly to try again
			// OLD (Crashy): GetTree().ReloadCurrentScene();
		
			// NEW (Safe): Schedule the reload for the very next "idle" frame
			GetTree().CallDeferred(SceneTree.MethodName.ReloadCurrentScene);
		}
	}
} 
