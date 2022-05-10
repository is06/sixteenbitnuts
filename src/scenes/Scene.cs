namespace SixteenBitNuts
{
    public abstract class Scene
    {
        private readonly Game game;

        public Scene(Game game)
        {
            this.game = game;
        }

        public virtual void Initialize()
        {
            
        }

        public virtual void LoadContent()
        {
            
        }

        public virtual void Update()
        {
            
        }

        public virtual void Draw()
        {
            
        }

        public virtual void UnloadContent()
        {
            
        }
    }
}
