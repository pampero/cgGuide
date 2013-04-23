using System.Threading;
using System.Web.Mvc;
using ServiceStack.Redis;
using log4net;

namespace Common.Base
{
    public class BaseController : Controller
    {
        public ILog Logger { get; set; }
        public IRedisClientsManager Cache { get; set; }
    }
}
