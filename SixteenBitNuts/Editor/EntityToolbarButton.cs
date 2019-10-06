namespace SixteenBitNuts.Editor
{
    public class EntityToolbarButton : ToolbarButton
    {
        private readonly string type;

        public EntityToolbarButton(Toolbar bar, string type) : base(bar)
        {
            this.type = type;
        }

        public override void Draw()
        {
            base.Draw();
        }
    }
}
