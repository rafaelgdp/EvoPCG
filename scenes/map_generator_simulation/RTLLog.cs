using Godot;
using System;
public class RTLLog : RichTextLabel {

    public override void _Ready() {
        BbcodeEnabled = true;
        BbcodeText = "";
    }
    public void LogLine(string s) {
        string currentTime = DateTime.Now.ToString();
        string bbLog = $"[color=yellow]{currentTime}: [/color]" + s + "\n";
        string log = $"{currentTime}: " + s + "\n";
        BbcodeText += bbLog;
        GD.Print(log);
    }
}