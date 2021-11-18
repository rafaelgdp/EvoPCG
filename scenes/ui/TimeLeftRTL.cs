using Godot;
using System;

public class TimeLeftRTL : RichTextLabel
{
    public override void _Process(float delta)
    {
        var tl = Global.MainScene.TimeLeft;
        var bb = $"[center]Time left: {tl:0.00} s[/center]";
        if (tl <= 15F && tl > 10F) {
            bb = $"[color=yellow]{bb}[/color]";
        } else if (tl <= 10F) {
            bb = $"[color=red]{bb}[/color]";
        }
        BbcodeText = bb;
    }
}
