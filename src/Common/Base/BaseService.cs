using ServiceStack.Logging;

namespace Common.Base
{
    public abstract class BaseService
    {
        public ILog Logger { get; set; }
    }
}

   