using Godot;

public partial class SystemPopupTest : PanelContainer {
    [Export] private Vector2 originalSize;
    [Export] private Vector2 shrinkedSize;
    [Export] private float animationDuration = 0.2f;
    private MarginContainer content;
    private Vector2 originalPosition;
    private Vector2 shrinkPosition;
    Vector2 screenSize;


    public override void _Ready() {
        screenSize = DisplayServer.ScreenGetSize();
        originalPosition = new Vector2(screenSize.X / 2 - originalSize.X / 2, screenSize.Y / 2 - originalSize.Y / 2);
        shrinkPosition = new Vector2(screenSize.X / 2 - shrinkedSize.X / 2, screenSize.Y / 2 - shrinkedSize.Y / 2);
        content = GetNode<MarginContainer>("%Content");
        GetTree().CreateTimer(2).Timeout += ShowPopup;
        GetTree().CreateTimer(4).Timeout += HidePopup;
    }


    public void ShowPopup() {
        Position = shrinkPosition;
        Visible = true;
        Size = shrinkedSize;
        Tween tween = CreateTween();
        tween.TweenProperty(this, "size", originalSize, animationDuration).SetEase(Tween.EaseType.Out);
        tween.Parallel().TweenProperty(this, "position", originalPosition, animationDuration).SetEase(Tween.EaseType.Out);
        tween.TweenProperty(content, "visible", true, animationDuration).SetEase(Tween.EaseType.Out);
    }


    public void HidePopup() {
        Position = originalPosition;
        content.Visible = false;
        Tween tween = CreateTween();
        tween.TweenProperty(this, "position", shrinkPosition, animationDuration).SetEase(Tween.EaseType.Out);
        tween.Parallel().TweenProperty(this, "size", shrinkedSize, animationDuration).SetEase(Tween.EaseType.Out);
        tween.TweenProperty(this, "visible", false, animationDuration).SetEase(Tween.EaseType.Out);
    }
}
