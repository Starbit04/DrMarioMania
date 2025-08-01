using System.Collections.Generic;
using Godot;
using Godot.Collections;
using static PowerUpEnums;

public partial class PillActive : Pill
{
    [Export] private Array<PowerUp> powerUps;
    [Export] private Array<Texture2D> powerUpPreviewTextures;
    [Export] private Sprite2D powerUpPreview;
    [Export] private Sprite2D ghostPowerUpPreview;
    public Vector2I LandPos { get; set; }
    public bool PowerUpHasPreview
    {
        get
        {
            return powerUps.Contains(CurrentPowerUp);
        }
    }
    public Color PowerUpPreviewColour
    {
        set
        {
            powerUpPreview.SelfModulate = value;
            ghostPowerUpPreview.SelfModulate = value;
        }
    }

    // Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

        powerUpPreview.Visible = false;
        ghostPowerUpPreview.Visible = false;
	}

    public void SetGhostPowerUpPreviewGlobalPosition(Vector2 pos)
    {
        ghostPowerUpPreview.GlobalPosition = pos;
    }

    public void UpdateGhostPowerUpPreviewVisibility()
    {
        ghostPowerUpPreview.Visible = GridPos != LandPos && powerUpPreview.Visible;
    }

    public void SetPowerUpPreviewVisibility(bool b)
    {
        powerUpPreview.Visible = b;

        UpdateGhostPowerUpPreviewVisibility();
    }

    public override void SetRandomSegmentColours(List<int> possibleColours, bool guaranteeSingleColour, RandomNumberGenerator rng)
	{
        SetPowerUpPreviewVisibility(false);
        
        base.SetRandomSegmentColours(possibleColours, guaranteeSingleColour, rng);
	}
	public override void SetSegmentColours(int centreColour, int secondaryColour)
	{
        SetPowerUpPreviewVisibility(false);

        base.SetSegmentColours(centreColour, secondaryColour);
	}

    public override void SetPowerUp(PowerUp powerUp, int colour)
	{
		base.SetPowerUp(powerUp, colour);

        if (powerUps.Contains(powerUp))
        {
            int previewIndex = powerUps.IndexOf(powerUp);
            SetPowerUpPreviewVisibility(true);
            powerUpPreview.Texture = powerUpPreviewTextures[previewIndex];
            ghostPowerUpPreview.Texture = powerUpPreviewTextures[previewIndex];
        }
        else
            SetPowerUpPreviewVisibility(false);
	}
}
