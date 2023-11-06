namespace OverlordVR.Player.Usable
{
    public interface IUsable
    {
        int NeededCoins { get; set; }

        void TryActive();
        void Active();
        void Cancel();
    }
}