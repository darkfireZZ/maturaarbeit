namespace Cubing
{
    public class SyncedIntWrapper
    {
        private static object _lockObject = new object();

        private int _value = default;

        public int Value
        {
            get
            {
                lock (_lockObject)
                    return _value;
            }
            set
            {
                lock (_lockObject)
                    _value = value;
            }
        }
    }

    public class SyncedFloatWrapper
    {
        private static object _lockObject = new object();

        private float _value = default;

        public float Value
        {
            get
            {
                lock (_lockObject)
                    return _value;
            }
            set
            {
                lock (_lockObject)
                    _value = value;
            }
        } 
    }

    public class SyncedBoolWrapper
    {
        private static object _lockObject = new object();

        private bool _value = default;

        public bool Value
        {
            get
            {
                lock (_lockObject)
                    return _value;
            }
            set
            {
                lock (_lockObject)
                    _value = value;
            }
        }
    }
}