using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECom.Utility.Interface
{
    public interface IWebSocketManager
    {
        Task HandleConnection(HttpContext context);
        Task BroadcastMessageAsync(string message);
    }
}
