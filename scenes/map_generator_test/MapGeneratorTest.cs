using System.Threading.Tasks;
using Godot;

public class MapGeneratorTest : Node2D
{
    [Signal]
    public delegate void TimeLeftUpdated(float timeLeft);

    Player player;
    public GeneratedTileMap tilemap;
    RichTextLabel debugLabel;

    int generationToleranceOffset = 10;
    int renderToleranceOffset = 10;
    private static float timeLeft = 30F;

    public float TimeLeft
    {
        set
        {
            timeLeft = Mathf.Max(0F, value);
            EmitSignal("TimeLeftUpdated", timeLeft);
            if (timeLeft <= 0F)
            {
                GetTree().ReloadCurrentScene();
            }
        }

        get { return timeLeft; }
    }

    public override void _Ready()
    {
        Global.MainScene = this;
        if (Global.IsDebug)
        {
            TimeLeft = Global.INITIAL_DEBUG_TIME;
        }
        else
        {
            TimeLeft = Global.INITIAL_TIME;
        }
        player = GetNode<Player>("Player");
        tilemap = GetNode<GeneratedTileMap>("GeneratedTileMap");
        debugLabel = GetNode<RichTextLabel>("DebugUI/DebugLabel");
        generationToleranceOffset = Global.GeneticWidth;
        renderToleranceOffset = Global.RenderWidth / 4; // 25% offset tolerance 
    }

    float lastDebugTime = 0F;
    public override void _PhysicsProcess(float delta)
    {

        CheckWorldUpdate();
        lastDebugTime += delta;
        if (lastDebugTime > 1F)
        {
            lastDebugTime = 0F;
            debugLabel.BbcodeText = tilemap.mMapGenerator.ToRichTextString();
        }

        TimeLeft -= delta;

    }

    private void CheckWorldUpdate()
    {
        var mappedPlayerPosX = (int)tilemap.WorldToMap(player.GlobalPosition).x;

        if (!Global.IsLeftGenBusy &&
            (mappedPlayerPosX < (tilemap.mMapGenerator.LeftmostGlobalX + generationToleranceOffset)))
        {
            Global.IsLeftGenBusy = true;
            Task.Factory.StartNew(() =>
            {
                tilemap.mMapGenerator.GenerateChunksOnLeft(2);
                Global.IsLeftGenBusy = false;
            });
        }

        if (!Global.IsRightGenBusy &&
            (mappedPlayerPosX > (tilemap.mMapGenerator.RightmostGlobalX - generationToleranceOffset)))
        {
            Global.IsRightGenBusy = true;
            Task.Factory.StartNew(() =>
            {
                tilemap.mMapGenerator.GenerateChunksOnRight(2);
                Global.IsRightGenBusy = false;
            });
        }

        if (mappedPlayerPosX < (tilemap.LeftmostGlobalX + renderToleranceOffset) ||
            mappedPlayerPosX > (tilemap.RightmostGlobalX - renderToleranceOffset))
        {
            // TODO: Possibly make signals for these
            tilemap.UpdateWorldAroundX(mappedPlayerPosX);
        }

    }

    public override void _Input(InputEvent e)
    {
        if (e.IsActionPressed("restart"))
        {
            GetTree().ReloadCurrentScene();
        }
    }

    private void OnPlayerExitedScreen()
    {
        GetTree().ReloadCurrentScene();
    }
}
