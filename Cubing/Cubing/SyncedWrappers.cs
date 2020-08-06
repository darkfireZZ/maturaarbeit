using System;
using System.Collections.Generic;
using System.Text;

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

    public class SyncedDoubleWrapper
    {
        private static object _lockObject = new object();

        private double _value = default;

        public double Value
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