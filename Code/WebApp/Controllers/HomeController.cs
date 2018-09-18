using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // application insights must be enabled, connected, and set to write logs!
            // logging level of atleast information must be enabled!
            System.Diagnostics.Trace.TraceInformation("Index controller action requested.");

            // on load of index we'll push a new message into the storage que, the webjob will pick that up
            var storageConnection = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=learningsa123jg;AccountKey=E7PD2oBMKYg4liD9gkMm2isuhyEXAy7LisQjLIlnSuVq9m6lncEsVaC5jf5olLinhkKfJmQTwAuOhtN1rYws0A==;EndpointSuffix=core.windows.net");
            var client = storageConnection.CreateCloudQueueClient();
            var que = client.GetQueueReference("testque");
            que.CreateIfNotExists();
            que.AddMessage(new CloudQueueMessage("this is a message!"));

            return View();
        }

        public ActionResult About()
        {
            // application insights must be enabled, connected, and set to write logs!
            // logging level of atleast information must be enabled!
            System.Diagnostics.Trace.TraceInformation("About controller action requested.");
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            // application insights must be enabled, connected, and set to write logs!
            // logging level of atleast information must be enabled!
            System.Diagnostics.Trace.TraceInformation("Contact controller action requested.");
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}