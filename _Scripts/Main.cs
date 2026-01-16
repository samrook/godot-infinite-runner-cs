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
	
	// NEW: Track space between spikes
	private int _chunksSinceLastSpike = 0;
	
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
	
	private void SpawnFloor(Vector2? startPos = null, bool forceSafe = false)
	{
		FloorSegment newFloor = FloorScene.Instantiate<FloorSegment>();
		
		// 1. Position Logic (Same as before)
		if (startPos != null)
		{
			newFloor.Position = startPos.Value;
		}
		else
		{
			Marker2D prevMarker = _lastFloor.GetNode<Marker2D>("EndMarker");
			newFloor.Position = prevMarker.GlobalPosition - new Vector2(12, 0);
		}

		AddChild(newFloor);

		// 2. OBSTACLE LOGIC
		bool spawnSpike = false;

		if (forceSafe)  
		{
			spawnSpike = false;
			_chunksSinceLastSpike++; // Count this as a safe chunk
		}
		else
		{
			// RULE: If we had a spike recently (less than 2 chunks ago), FORCE SAFE.
			if (_chunksSinceLastSpike < 1) 
			{
				spawnSpike = false;
				_chunksSinceLastSpike++;
			}
			else
			{
				// We are allowed to spawn. Roll the dice.
				if (GD.Randf() > 0.5f) 
				{
					spawnSpike = true;
					_chunksSinceLastSpike = 0; // Reset counter!
				}
				else
				{
					spawnSpike = false;
					_chunksSinceLastSpike++;
				}
			}
		}

		// 3. Command the floor
		newFloor.Initialize(spawnSpike);

		_lastFloor = newFloor;
	}
}
