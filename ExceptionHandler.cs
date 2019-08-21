using Commom;
using Newtonsoft.Json;
using PDFHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Mvc;

namespace PDFHelper
{
    public class ExceptionHandler : ExceptionFilterAttribute
    {

        public override void OnException(HttpActionExecutedContext filterContext)
        {
            Log.Error("", filterContext.Exception);
            filterContext.Response = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent(JsonConvert.SerializeObject(new Result<string>
                {
                    Success = false,
                    Message = filterContext.Exception.Message,
                    Data = ""
                }))
            };
        }


    }
}