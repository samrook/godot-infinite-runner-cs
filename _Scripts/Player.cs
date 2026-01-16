using Godot;
using System;

public partial class Player : CharacterBody2D
{
	// Forces
	[Export]
	public float JumpVelocity { get; set; } = -400.0f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	public override void _Ready()
	{
		//GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("run");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Move
		Velocity = velocity;
		MoveAndSlide();
	}
}
