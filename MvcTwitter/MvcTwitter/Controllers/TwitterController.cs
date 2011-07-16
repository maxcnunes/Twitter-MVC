using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcTwitter.Controllers
{
    public class TwitterController : Controller
    {
        //
        // GET: /Twitter/

        public ActionResult Index()
        {
            //Passa como parametro o valor da pesquisa
            Models.TwitterSearch tw = new Models.TwitterSearch();
            var x = tw.AllTheseWords;
            tw.AllTheseWords = "nome";
            x = tw.AllTheseWords;

            ViewBag.Twitter = tw.GetTwitter();
            return View();
        }

    } 
}
