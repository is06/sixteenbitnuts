namespace SixteenBitNuts
{
    public struct CounterConfig
    {
        public int CurrentValue;
        public int MinValue;
        public int MaxValue;
    }

    public delegate void CounterValueChangedHandler(int value, int previousValue);

    abstract public class Counter : HudElement, ICounter
    {
        private int currentValue;
        private int maxValue;
        private int minValue;

        public int MinValue
        {
            get
            {
                return minValue;
            }
            set
            {
                var previousValue = minValue;
                minValue = value;

                OnMinValueChanged?.Invoke(minValue, previousValue);
            }
        }
        public int MaxValue
        {
            get
            {
                return maxValue;
            }
            set
            {
                var previousValue = maxValue;
                maxValue = value;

                OnMaxValueChanged?.Invoke(maxValue, previousValue);
            }
        }
        public int Value
        {
            get
            {
                return currentValue;
            }
            set
            {
                var previousValue = currentValue;

                currentValue = value;
                if (currentValue < MinValue)
                    currentValue = MinValue;
                if (currentValue > MaxValue)
                    currentValue = MaxValue;

                OnValueChanged?.Invoke(currentValue, previousValue);
            }
        }

        public event CounterValueChangedHandler? OnValueChanged;
        public event CounterValueChangedHandler? OnMinValueChanged;
        public event CounterValueChangedHandler? OnMaxValueChanged;

        public Counter(Hud hud, CounterConfig config) : base(hud)
        {
            currentValue = config.CurrentValue;
            minValue = config.MinValue;
            maxValue = config.MaxValue;
        }
    }
}
