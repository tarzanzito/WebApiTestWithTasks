using Serilog;

namespace WebApplication1
{
    //https://csharpindepth.com/Articles/Singleton
    public interface IStatisticsInfo
    {
        int InstancesActives { get; set; }
        int InstancesCount { get; set; }
        int MethodActives { get; set; }
        int MethodsCount { get; set; }
    }

    public class StatisticsInfo : IStatisticsInfo
    {
        //private static readonly object _padlock = new();

        private int _instancesCount = 0;
        private int _instancesActives = 0;
        private int _methodsCount = 0;
        private int _methodsActive = 0;

        public int InstancesCount
        {
            get
            {
                //lock (_padlock)
                {
                    return _instancesCount;
                }
            }

            set
            {
                //lock (_padlock)
                {
                    _instancesCount = value;
                }
            }
        }

        public int InstancesActives
        {
            get
            {
                //lock (_padlock)
                {
                    return _instancesActives;
                }
            }

            set
            {
                //lock (_padlock)
                {
                    _instancesActives = value;
                }
            }
        }

        public int MethodsCount
        {
            get
            {
                //lock (_padlock)
                {
                    return _methodsCount;
                }
            }

            set
            {
                //lock (_padlock)
                {
                    _methodsCount = value;
                }
            }
        }

        public int MethodActives
        {
            get
            {
                //lock (_padlock)
                {
                    return _methodsActive;
                }
            }

            set
            {
                //lock (_padlock)
                {
                    _methodsActive = value;
                }
            }
        }

        public StatisticsInfo()
        {
            Log.Information($"New StatisticsInfo");
        }
    }
}
