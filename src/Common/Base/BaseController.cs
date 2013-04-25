using ServiceStack.Logging;
using ServiceStack.Mvc;
using ServiceStack.Mvc.MiniProfiler;

namespace Common.Base
{
    // Mini Profiler
    [ProfilingActionFilter]
    public class BaseController : ServiceStackController
    {
        public ILog Logger { get; set; }
    }
}
