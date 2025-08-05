namespace IngameScript
{
    public abstract class BaseSystem
    {
        private readonly string _systemName = string.Empty;

        protected string RefCustomData { get; set; } = nameof(ToString);

        protected abstract void Update();

        public override string ToString()
        {
            return string.IsNullOrEmpty(_systemName) ? GetType().Name : _systemName;
        }
    }
}