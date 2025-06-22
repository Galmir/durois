using Godot;

public partial class SystemPopupTest : PanelContainer {
    [Export] private Vector2 originalSize;
    [Export] private Vector2 shrinkedSize;
    private MarginContainer content;

    public override void _Ready() {
        content = GetNode<MarginContainer>("%Content");
        Visible = false;
    }


    public void ShowPopup() {
        Visible = true;
        Tween tween = CreateTween();
        tween.TweenProperty(this, "size", originalSize, 0.3).SetEase(Tween.EaseType.Out);
        tween.TweenProperty(content, "visible", true, 0.2);
    }


    public void HidePopup() {
        Tween tween = CreateTween();
        //tween.TweenProperty(this, "size", shrinkedSize, 0.3).SetEase(Tween.EaseType.Out);
        tween.TweenProperty(content, "visible", false, 0.2);
        tween.TweenProperty(this, "size", shrinkedSize, 0.3).SetEase(Tween.EaseType.Out);
        tween.TweenProperty(this, "visible", false, 0.2);
    }
}
