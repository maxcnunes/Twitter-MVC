using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Net;
using System.Globalization;
using System.IO;

namespace MvcTwitter.Models
{
    public class Twitter
    {
        public string AuthorName { get; set; }
        public string AuthorUri { get; set; }
        public string Content { get; set; }
        public string Updated { get; set; }
        public string Link { get; set; }

        private string _AllTheseWords;
        public string AllTheseWords { get { return _AllTheseWords; } set { this._AllTheseWords = "ands=" + value; } }
        //public string ThisExactPhrase { get; set { this.ThisExactPhrase = "phrase=" + value; } }
        //public string AnyTheseWords { get; set { this.AnyTheseWords = "ors=" + value; } }
        //public string NoneTheseWords { get; set { this.NoneTheseWords = "nots" + value; } }
        //public string ThisHastag { get; set { this.ThisHastag = "tag=" + value; } }
        //public string WrittenIn { get; set { this.WrittenIn = "lang=ar" + value; } }
        //public string FromThisPerson { get; set { this.FromThisPerson = "from=" + value; } }
        //public string ToThisPerson { get; set { this.ToThisPerson = "to=" + value; } }
        //public string ReferencingThisPerson { get; set { this.ReferencingThisPerson = "ref" + value; } }
        //public string NearThisPlace { get; set { this.NearThisPlace = "near" + value; } }
        //public string WithinThisDistance { get; set { this.WithinThisDistance = "within" + value; } }
        //public string KindDistance { get; set { this.KindDistance = "units=mi" + value; } }
        //public string SinceThisDate { get; set { this.SinceThisDate = "since=2011-07-13" + value; } }
        //public string UntilThisDate { get; set { this.UntilThisDate = "until=2011-07-14" + value; } }
        //public string WithPositiveAttitude { get; set { this.WithPositiveAttitude = "tude[]=%3A)" + value; } }
        //public string WithNegativeAttitude { get; set { this.WithNegativeAttitude = "tude[]=%3A(" + value; } }
        //public string AskingQuestion { get; set { this.AskingQuestion = "tude[]=%3F" + value; } }
        //public string ContainingLinks { get; set { this.ContainingLinks = "filter[]=links" + value; } }
        //public string IncludeRetweets { get; set { this.IncludeRetweets = "include[]=retweets" + value; } }
        //public string ResultPerPage { get; set { this.ResultPerPage = "rpp=" + value; } }

        public List<Twitter> GetTwitter(string search)
        {
            //Faz a pesquisa no twitter de acordo com o filtro passado
            var request = WebRequest.Create("http://search.twitter.com/search.atom?" + search) as HttpWebRequest;
            var twitterViewData = new List<Twitter>();

            if (request != null)
            {
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    //Lê o xml do resultado da pesquisa
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        var document = XDocument.Parse(reader.ReadToEnd());
                        XNamespace xmlns = "http://www.w3.org/2005/Atom";

                        //Cria uma lista do resultado
                        twitterViewData = (from entry in document.Descendants(xmlns + "entry")
                                           select new Twitter
                                           {
                                               Content = entry.Element(xmlns + "content").Value,
                                               Updated = Format_DateTime(entry.Element(xmlns + "updated").Value),
                                               AuthorName = entry.Element(xmlns + "author").Element(xmlns + "name").Value,
                                               AuthorUri = entry.Element(xmlns + "author").Element(xmlns + "uri").Value,
                                               Link = (from o in entry.Descendants(xmlns + "link")
                                                       where o.Attribute("rel").Value == "image"
                                                       select new { Val = o.Attribute("href").Value }).First().Val
                                           }).ToList();
                    }
                }
            }

            return twitterViewData;
        }

        public static string Format_DateTime(string value)
        {
            // Cria um CultureInfo para o Português.
            CultureInfo ci = new CultureInfo("pt-BR");

            value = value.Replace("T", " ").Replace("Z", string.Empty);

            DateTime _Date;
            if (!DateTime.TryParse(value, out _Date))
                throw new ArgumentException("Erro ao converter.");

            // Formata a data para o formato de tempo brasileiro.
            return _Date.ToString(ci);
        }
    }
}