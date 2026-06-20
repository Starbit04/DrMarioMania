using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class JarUIManager : Node
{
	// Handles the visuals of a spefific player's jar and its corrisponding UI
    [ExportGroup("Stat Label References")]
    [Export] private Label scoreLabel;
    [Export] private Label highScoreLabel;
    [Export] private Label levelLabel;
    [Export] private Label speedLabel;
    [Export] private Label virusLabel;
    [Export] private Label nextLabel;
    [Export] private Label holdLabel;
    [Export] private Label holdKeyLabel;

    [ExportGroup("Overlay References")]
    [Export] private Control overlay;
	[Export] private Label overlayTitleLabel;
	[Export] private AnimationPlayer overlayAniPlayer;
	[Export] private Button nextButton;
	[Export] private Button quitButton;

    [ExportGroup("Themeable References")]

	[ExportSubgroup("Jar")]
	[Export] private Godot.Collections.Array<NinePatchRect> jarRects;
	[Export] private NinePatchRect jarMainRect;
	[Export] private NinePatchRect jarTopLeftRect;
	[Export] private NinePatchRect jarTopRightRect;
	[Export] private NinePatchRect jarShadowRect;
	[Export] private NinePatchRect jarOverlayDarkenRect;
	[Export] private TileMapLayer jarTiles;
	[Export] private Node2D jarGroup;

	[ExportSubgroup("Pills")]
	[Export] private PillActive activePill;
	[Export] private Pill nextPill;
	[Export] private Pill holdPill;
	[Export] private Pill powerUpPill;

	[ExportSubgroup("Arrays")]
	[Export] private Godot.Collections.Array<NinePatchRect> uiBoxes;
	[Export] private Godot.Collections.Array<NinePatchRect> uiBoxesSmall;
	[Export] private Godot.Collections.Array<Label> uiLabels;
	[Export] private Godot.Collections.Array<Label> uiLightLabels;
	[Export] private Godot.Collections.Array<Button> uiButtons;
	[Export] private Godot.Collections.Array<Control> uiShadows;

	[ExportSubgroup("Other")]
    [Export] private PushUpWarningGroup pushUpWarningGroup;
    public PushUpWarningGroup PushUpWarningGroup { get { return pushUpWarningGroup; } }
	[Export] private Sprite2D powerUpIcon;
	[Export] private PowerUpMeter powerUpMeter;
	public PowerUpMeter PowerUpMeter { get { return powerUpMeter; } }
	[Export] private JarMario mario;
	public JarMario Mario { get { return mario; } }
    [Export] private VirusRing virusRing;
    public VirusRing VirusRing { get { return virusRing; } }
	[Export] private TextureRect docBoxTexRec;
	[Export] private NinePatchRect holdGroupRect;
	[Export] private WinIconContainer winIconContainer;

    [ExportGroup("Groups")]
    [Export] private Control hudGroup;
    [Export] private Control topLeftHud;
    [Export] private Control holdGroup;
    [Export] private Control rightHudGroup;
    [Export] private Control[] leftAlignedNodes;
    [Export] private Control[] rightAlignedNodes;
	[Export] private Pill[] leftAlignedPills;
    [Export] private Pill[] rightAlignedPills;

    [ExportGroup("Other Local References")]
    [Export] private Marker2D tlMarker;
    [Export] private Marker2D brMarker;

	public Vector2 TopLeftPos { get { return tlMarker.GlobalPosition; } }
	public Vector2 BottomRightPos { get { return brMarker.GlobalPosition; } }

    [ExportGroup("External References")]
    [Export] private JarManager jarMan;
	
	private Vector2I previousJarSize = Vector2I.Zero;
	private int origJarPositionY = -999;
	private int origTopLeftHudPositionY = -999;
	private int origTopLeftHudHeight = -999;
	private int origRightHudGroupPositionY = -999;
	private int origVirusRingPositionY = -999;
	private int origNextPillPosY = -999;
	private int origHoldPillPosY = -999;
	private const int multiplayerJarOffset = -19;
	private bool IsMultiplayer
	{
		get
		{
			return jarMan.CommonGameSettings.IsMultiplayer;
		}
	}

    // Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

    public void SetHighScoreLabel(int score) { highScoreLabel.Text = "" + score; }
    public void SetScoreLabel(int score) { scoreLabel.Text = "" + score; }
    public void SetLevelLabel(int level) { levelLabel.Text = level == -1 ? "-" : "" + level; }
    public void SetLevelLabel(int a, int b) { levelLabel.Text = a + "-" + b; }
    public void SetLevelLabel(string s) { levelLabel.Text = s; }
    public void SetVirusLabel(int virus) { virusLabel.Text = "" + virus; }
    public void SetSpeedLabel(int speed)
    {
        switch (speed)
        {
            case 0:
                speedLabel.Text = "LOW";
                break;
            case 1:
                speedLabel.Text = "MED";
                break;
            case 2:
                speedLabel.Text = "HI";
                break;
            default:
                speedLabel.Text = "???";
                break;
        }
    }

    public async void ShowWinOverlay()
    {		
		overlayTitleLabel.Text = (IsMultiplayer && jarMan.HasWonEnoughRounds) ? "YOU\nWIN" : "LEVEL\nCLEAR";
		
		overlayAniPlayer.Play("Show");

		await Task.Delay(400);

		quitButton.Visible = true;

		if (IsMultiplayer && jarMan.HasWonEnoughRounds)
		{
			nextButton.Text = "REMATCH";
			nextButton.Visible = true;
			nextButton.GrabFocus();
		}
		else
		{
			nextButton.Visible = true;

			if (jarMan.CommonGameSettings.IsCustomLevel && !IsMultiplayer)
			{
				nextButton.Text = "REPLAY";
				quitButton.GrabFocus();
			}
			else
			{
				nextButton.Text = "NEXT";
				nextButton.GrabFocus();
			}

		}
    }
    
    public async void ShowGameOverOverlay(bool hideButton = false)
	{
		overlayTitleLabel.Text = IsMultiplayer ? "LOSE" : "GAME\nOVER";
		overlayAniPlayer.Play("Show");

		await Task.Delay(400);

		nextButton.Visible = jarMan.CommonGameSettings.IsCustomLevel && !IsMultiplayer;
		
		if (hideButton)
			quitButton.Visible = false;
		else
		{
			quitButton.Visible = true;

			if (nextButton.Visible)
			{
				nextButton.Text = "REPLAY";
				nextButton.GrabFocus();
			}
			else
				quitButton.GrabFocus();
		}
	}

    public void HideOverlay()
    {
        overlayAniPlayer.Stop();

		overlay.Visible = false;
		nextButton.Visible = false;
		quitButton.Visible = false;
    }

	public void UpdateWinIconContainer(int wins)
	{
		if (winIconContainer != null)
			winIconContainer.SetWinAmount(wins);
	}

	public void SetHUDVisibility(bool b)
	{
		if (hudGroup != null)
			hudGroup.Visible = b;

		if (mario != null)
			mario.Visible = b;

		if (docBoxTexRec != null)
			docBoxTexRec.Visible = b;

		if (virusRing != null)
		{
			virusRing.Visible = b;
			if (!b)
				virusRing.DeleteAllViruses();
		}
	}

	public void UpdateHoldGroup(int theme, ThemeList themeList)
	{
		// top-left box height and hold visibility

		if (origTopLeftHudHeight == -999)
		{
			origTopLeftHudHeight = (int)topLeftHud.Size.Y;
		}

		int newHeight = origTopLeftHudHeight;
		bool useHold = jarMan.PlayerGameSettings.IsHoldEnabled;

		if (!useHold)
			newHeight -= 8;

		holdGroup.Visible = useHold;

		topLeftHud.Size = Vector2.Right * topLeftHud.Size.X + Vector2.Down * newHeight;

		// hold pill y pos

		if (origHoldPillPosY == -999)
		{
			origHoldPillPosY = (int)holdPill.OrigPos.Y;
		}
		
		holdPill.OrigPos = Vector2.Right * holdPill.OrigPos.X + Vector2.Down * (origHoldPillPosY + themeList.GetTopLeftHudOffset(theme));
		holdPill.Position = holdPill.OrigPos;
	}

	// updates the hold key/button indicator visible during multiplayer
	private void UpdateMultiHoldKeyLabel()
	{
		PlayerMultiInputSettings inputSettings = jarMan.PlayerMultiInputSettings;

		// controller
		if (inputSettings.MultiplayerIsUsingController)
			holdKeyLabel.Text = "";
		// full/exclusive keyboard
		else if (inputSettings.MultiplayerIsControlMethodExclusive)
			holdKeyLabel.Text = "C";
		// shared keyboard
		else
		{
			string holdAction = inputSettings.MultiplayerInputPrefix + "Hold";

			InputEventKey inputEventKey = InputMap.ActionGetEvents(holdAction)[0] as InputEventKey;

			string keyString = inputEventKey.AsTextPhysicalKeycode();

			if (keyString == "Slash")
				keyString = "/";

			holdKeyLabel.Text = keyString;
		}
	}

	public void UpdateJarTexture(int theme, ThemeList themeList)
	{
		Texture2D jarTex = themeList.GetJarTexture(jarMan.SpeedLevel, theme, IsMultiplayer, jarMan.CommonGameSettings.CurrentIsUsingCustomBgColour);
		foreach (NinePatchRect rect in jarRects)
		{
			rect.Texture = jarTex;
		}
	}

	public void UpdateMarioSprite(int theme, ThemeList themeList)
	{
		string texStr = jarMan.PlayerGameSettings.UseLuigiSprite ? "DrLuigi" : "TheDoc";
        mario.Texture = themeList.GetTexture(texStr, theme);
	}

    public void UpdateJarSpriteSize()
	{
        Vector2I baseSize = jarMan.BaseJarSize;

		if (previousJarSize == Vector2.Zero)
            previousJarSize = baseSize;

        Vector2I size = jarMan.JarSize;
        Vector2I offset = Vector2I.Right * (size - previousJarSize).X * 4;

        foreach (Control leftNode in leftAlignedNodes)
		{
            leftNode.Position -= offset;
        }

		foreach (Control rightNode in rightAlignedNodes)
		{
            rightNode.Position += offset;
		}

        foreach (Pill leftPill in leftAlignedPills)
		{
            leftPill.Position -= offset;
            leftPill.OffsetOrigPos(-offset);
        }

		foreach (Pill rightPill in rightAlignedPills)
		{
            rightPill.Position += offset;
            rightPill.OffsetOrigPos(offset);
		}

		powerUpIcon.Position += offset;

        jarTopLeftRect.Position -= offset;
        jarTopLeftRect.Size += offset;
		
        jarTopRightRect.Size += offset;

        jarMainRect.Position -= offset;
        jarMainRect.Size += offset * 2;

        jarShadowRect.Position -= offset;
        jarShadowRect.Size += offset * 2;

        jarOverlayDarkenRect.Position -= offset;
        jarOverlayDarkenRect.Size += offset * 2;

		tlMarker.Position -= offset;
		brMarker.Position += offset;

        pushUpWarningGroup.SetWarningWidth(size.X);

        previousJarSize = size;
    }

    public void UpdateJarVisuals(int theme, ThemeList themeList)
	{		
		if (origJarPositionY == -999)
			origJarPositionY = (int)jarGroup.Position.Y;

		// Get pill texture
		Texture2D pillTileTex = themeList.GetPillTileTexture(theme);
		Texture2D powerUpTileTex = themeList.GetPowerUpTileTexture(theme);

		// Get tilesets
		TileSetAtlasSource pillSource = (TileSetAtlasSource)jarTiles.TileSet.GetSource(GameConstants.pillSourceID);
		TileSetAtlasSource virusSource = (TileSetAtlasSource)jarTiles.TileSet.GetSource(GameConstants.virusSourceID);
		TileSetAtlasSource powerUpSource = (TileSetAtlasSource)jarTiles.TileSet.GetSource(GameConstants.powerUpSourceID);
		TileSetAtlasSource objectSource = (TileSetAtlasSource)jarTiles.TileSet.GetSource(GameConstants.objectSourceID);
		
		// Update visuals for...

		// Grid tilesets
		virusSource.Texture = themeList.GetVirusTileTexture(theme);
		pillSource.Texture = pillTileTex;
		powerUpSource.Texture = powerUpTileTex;
		objectSource.Texture = themeList.GetObjectTileTexture(theme);

		activePill.PowerUpPreviewColour = themeList.GetPowerUpPreviewColour(theme);

		// Arrays
		Texture2D uiBoxTex = themeList.GetUIBoxTexture(theme);
		if (uiBoxes != null)
		{
			foreach (NinePatchRect box in uiBoxes)
			{
				box.Texture = uiBoxTex;
			}
		}

		Texture2D uiBoxSmallTex = themeList.GetUIBoxSmallTexture(theme);
		if (uiBoxesSmall != null)
		{
			foreach (NinePatchRect box in uiBoxesSmall)
			{
				box.Texture = uiBoxSmallTex;
			}
		}

		if (uiShadows != null)
		{
			bool showUiShadows = themeList.GetUseUiShadow(theme);

			foreach (Control shadow in uiShadows)
			{
				shadow.Visible = showUiShadows;
			}
		}

		// Labels
		bool neverUseLightLabelColour = themeList.GetNeverUseLightLabelColour(theme);
		foreach (Label label in uiLabels)
		{
			label.AddThemeFontOverride("font", themeList.GetLabelFont(theme));

			if (label == overlayTitleLabel)
			{
				if (neverUseLightLabelColour)
				{
					label.AddThemeColorOverride("font_color", themeList.GetLabelColour(theme));
					label.AddThemeConstantOverride("shadow_outline_size", 4);
					label.AddThemeColorOverride("font_outline_color", themeList.GetLightColour(theme));
				}
				
				continue;
			}
			
			if (!neverUseLightLabelColour && uiLightLabels.Contains(label))
				label.AddThemeColorOverride("font_color", themeList.GetLightColour(theme));
			else
				label.AddThemeColorOverride("font_color", themeList.GetLabelColour(theme));

			if (themeList.GetUseLabelShadow(theme))
			{
                float opacity = 103.0f / 255.0f;

                label.AddThemeColorOverride("font_shadow_color", new Color(0, 0, 0, opacity));
				label.AddThemeConstantOverride("shadow_outline_size", 0);
				label.AddThemeConstantOverride("shadow_offset_x", 1);
				label.AddThemeConstantOverride("shadow_offset_y", 1);
			}
			else
			{
				label.AddThemeColorOverride("font_shadow_color", new Color(0, 0, 0, 0));
				label.RemoveThemeConstantOverride("shadow_outline_size");
				label.RemoveThemeConstantOverride("shadow_offset_x");
				label.RemoveThemeConstantOverride("shadow_offset_y");
			}
		}

		// Buttons
		foreach (Button button in uiButtons)
		{
			button.AddThemeFontOverride("font", themeList.GetLabelFont(theme));
		}

		// Other
		powerUpMeter.IconTexture = themeList.GetPowerUpIconTexture(theme);
		powerUpMeter.FillTexture = themeList.GetTexture("PowerUpMeterFill", theme);
		powerUpMeter.FillReadyTexture = themeList.GetTexture("PowerUpMeterFillReady", theme);
		powerUpMeter.EmptyTexture = themeList.GetTexture("PowerUpMeterEmpty", theme);

		if (holdGroupRect != null)
			holdGroupRect.Texture = themeList.GetTexture("HoldGroupBg", theme);

        UpdateJarTexture(theme, themeList);

        if (IsMultiplayer)
			jarGroup.Position = Vector2.Right * jarGroup.Position.X + Vector2.Down * (origJarPositionY + multiplayerJarOffset);
		else
			jarGroup.Position = Vector2.Right * jarGroup.Position.X + Vector2.Down * (origJarPositionY + themeList.GetJarOffset(theme));

		if (topLeftHud != null)
		{
			if (origTopLeftHudPositionY == -999)
			{
				origTopLeftHudPositionY = (int)topLeftHud.Position.Y;
			}
			
			int yOffset = themeList.GetTopLeftHudOffset(theme);
			
			topLeftHud.Position = Vector2.Right * topLeftHud.Position.X + Vector2.Down * (origTopLeftHudPositionY + yOffset);

			UpdateHoldGroup(theme, themeList);
		}

		if (rightHudGroup != null)
		{
			if (origRightHudGroupPositionY == -999)
			{
				origRightHudGroupPositionY = (int)rightHudGroup.Position.Y;
				origNextPillPosY = (int)nextPill.OrigPos.Y;
			}
			
			int yOffset = themeList.GetRightHudGroupOffset(theme);
			
			rightHudGroup.Position = Vector2.Right * rightHudGroup.Position.X + Vector2.Down * (origRightHudGroupPositionY + yOffset);
			nextPill.OrigPos = Vector2.Right * nextPill.OrigPos.X + Vector2.Down * (origNextPillPosY + yOffset);
			nextPill.Position = nextPill.OrigPos;
		}

		if (virusRing != null)
		{
			virusRing.RingBg.Texture = themeList.GetTexture("VirusRing", theme);
			virusRing.RingShadow.Texture = themeList.GetTexture("VirusRing", theme);
			virusRing.VirusTexture = themeList.GetTexture("VirusBig", theme);
			virusRing.VirusVanishTexture = themeList.GetTexture("VirusVanish", theme);

			if (themeList.GetHasRingOverlay(theme))
			{
				virusRing.RingOverlay.Texture = themeList.GetTexture("VirusRingOverlay", theme);
				virusRing.RingOverlay.Visible = true;
			}
			else
				virusRing.RingOverlay.Visible = false;

			if (origVirusRingPositionY == -999)
			{
				origVirusRingPositionY = (int)virusRing.Position.Y;
			}
			
			int yOffset = themeList.GetVirusRingOffset(theme);
			
			virusRing.Position = Vector2.Right * virusRing.Position.X + Vector2.Down * (origVirusRingPositionY + yOffset);
		}

		// If hold label present, in multiplayer and hold is disabled, hide hold label and centre next label and pill
		if (holdLabel != null && jarMan.CommonGameSettings.IsMultiplayer && !jarMan.PlayerGameSettings.IsHoldEnabled)
		{
			holdLabel.Visible = false;
			holdKeyLabel.Visible = false;

			nextLabel.HorizontalAlignment = HorizontalAlignment.Center;
			nextPill.Position = new Vector2(-4, nextPill.Position.Y);
		}
		
		if (mario != null)
		{
            UpdateMarioSprite(theme, themeList);
        }

		if (docBoxTexRec != null)
			docBoxTexRec.Texture = themeList.GetTexture("DocBox", theme);

		if (winIconContainer != null)
		{
			winIconContainer.WinIconTexture = themeList.GetTexture("WinIcons", theme);
			winIconContainer.WinIconSmallTexture = themeList.GetTexture("WinIconsSmall", theme);
		}

		if (holdKeyLabel != null)
			UpdateMultiHoldKeyLabel();
	}
}
