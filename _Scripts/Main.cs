using Godot;
using System;

public partial class Main : Node2D
{
	// Assign your FloorSegment.tscn here in the Inspector
	[Export]
	public PackedScene FloorScene { get; set; }

	// Track the newest floor piece so we know where to attach the next one
	private Node2D _lastFloor;
	
	private float _score = 0.0f;
	private Label _scoreLabel;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_scoreLabel = GetNode<Label>("HUD/ScoreLabel");
		
		// 1. Spawn the first floor right under the player
		SpawnFloor(new Vector2(0, 300), true); 
		
		// 2. Spawn a few more immediately so the screen isn't empty
		SpawnFloor(null, true); 
		SpawnFloor(null, true);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// 3. The Infinite Loop Check
		// If we have a floor, check its position
		if (_lastFloor != null)
		{
			// Get the "EndMarker" from the last floor piece
			Marker2D endMarker = _lastFloor.GetNode<Marker2D>("EndMarker");

			// LOGIC: If the end of the floor is getting close to the screen edge...
			// (GetViewportRect().Size.X is the screen width, usually 640)
			if (endMarker.GlobalPosition.X < GetViewportRect().Size.X)
			{
				SpawnFloor();
			}
		}
		
		_score += (float)delta * 10.0f; 

		// Update Text (No decimals)
		_scoreLabel.Text = $"Score: {(int)_score}";
	}
	
	private void SpawnFloor(Vector2? startPos = null, bool isSafe = false)
	{
		// Create the new floor
		FloorSegment newFloor = FloorScene.Instantiate<FloorSegment>();
		
		// Set the safe flag BEFORE adding it to the tree (so _Ready sees it)
		newFloor.IsSafe = isSafe;
	
		// Figure out where to put it
		if (startPos != null)
		{
			// This is the very first floor (Manual position)
			newFloor.Position = startPos.Value;
		}
		else
		{
			// Attach to the end of the previous floor
			Marker2D prevMarker = _lastFloor.GetNode<Marker2D>("EndMarker");
			newFloor.Position = prevMarker.GlobalPosition;
		}

		// Add it to the game
		AddChild(newFloor);

		// Update our tracker
		_lastFloor = newFloor;
	}
}
