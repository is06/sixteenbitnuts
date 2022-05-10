namespace SixteenBitNuts
{
    public abstract class Scene
    {
        public Game Game { get; private set; }

        public Scene(Game game)
        {
            Game = game;
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
