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
            //Neste exemplo:
            //from -> Filtra as mensagens por usuário
            //rpp  -> Retorna apenas os últimos 15 resultados
            //ViewBag.Twitter = Models.Twitter.GetTwitter("from=twitterapi&rpp=15");
            Models.Twitter tw = new Models.Twitter();
            var x = tw.AllTheseWords;
            tw.AllTheseWords = "nome";
            x = tw.AllTheseWords;
            return View();
        }

    } 
}
