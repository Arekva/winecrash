namespace Winecrash.Entities
{
    public class PlayerEntity : Entity
    {
        public Player Player { get; private set; }

        protected override void OnDelete()
        {
            Player = null;
            base.OnDelete();
        }
    }
}