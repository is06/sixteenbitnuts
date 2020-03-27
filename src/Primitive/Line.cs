namespace SixteenBitNuts
{
    public class Line : Primitive
    {
        public Line(Game game) : base(game)
        {

        }

        public override void Draw()
        {
            base.Draw();

            game.SpriteBatch?.Begin();



            game.SpriteBatch?.End();
        }
    }
}
