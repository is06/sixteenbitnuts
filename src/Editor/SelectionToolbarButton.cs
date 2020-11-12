namespace SixteenBitNuts.Editor
{
    public class SelectionToolbarButton : ToolbarButton
    {
        public SelectionToolbarButton(Toolbar toolbar) : base(toolbar)
        {
            Type = ToolbarButtonType.Selection;
        }
    }
}
