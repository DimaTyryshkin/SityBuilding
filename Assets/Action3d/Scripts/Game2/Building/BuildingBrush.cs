namespace Game2.Building
{
    public abstract class BuildingBrush
    {
        public abstract void OnEnableBrush();
        public abstract void OnDisableBrush();
        public abstract void LateUpdate();
    }
}