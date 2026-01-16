using Godot;
using System;

public partial class FloorSegment : StaticBody2D
{
	// Speed of the world moving left (pixels per second)
	[Export]
	public float MoveSpeed { get; set; } = 150.0f;
	
	// Assign the Obstacle.tscn here in the Inspector
	[Export]
	public PackedScene ObstacleScene { get; set; }
	
	// New Property: Default to FALSE (Dangerous), but can be set to TRUE (Safe)
	public bool IsSafe { get; set; } = false;
	
	public override void _Ready()
	{
		GD.Print(IsSafe ? "Segment Safe" : "Segment Not Safe");
		
		// Simple Random Logic: 50% chance to spawn a spike
		// GD.Randf() returns a float between 0.0 and 1.0
		if (!IsSafe && ObstacleScene != null && GD.Randf() > 0.5f)
		{
			SpawnObstacle();
		}
	}

	private void SpawnObstacle()
	{
		Node2D obstacle = ObstacleScene.Instantiate<Node2D>();
		
		// Add it as a child of THIS floor segment.
		// This is crucial: If the floor moves, the spike moves with it automatically!
		AddChild(obstacle);

		// Position it somewhere on the floor
		// Let's put it slightly to the right, sitting on top of the tiles.
		// (You might need to tweak the Y value depending on your tile height)
		obstacle.Position = new Vector2(0, -18); 
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Move to the left
		Position += new Vector2(-MoveSpeed * (float)delta, 0);
	}
	
	// This function runs automatically when the object leaves the screen
	private void OnVisibleOnScreenNotifier2DScreenExited()
	{
		QueueFree(); // Delete this object from memory
		
		GD.Print("OBject deleted");
	}
}
