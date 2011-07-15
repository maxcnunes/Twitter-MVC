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
    public class TwitterSearch
    {

        #region Variables
        private const string _URL_Twitter_Search = @"http://search.twitter.com/search.atom?";
        private string _AllTheseWords;
        private string _ThisExactPhrase;
        private string _AnyTheseWords;
        private string _NoneTheseWords;
        private string _ThisHastag;
        private string _WrittenIn;
        private string _FromThisPerson;
        private string _ToThisPerson;
        private string _ReferencingThisPerson;
        private string _NearThisPlace;
        private string _WithinThisDistance;
        private string _KindDistance;
        private string _SinceThisDate;
        private string _UntilThisDate;
        private string _WithPositiveAttitude;
        private string _WithNegativeAttitude;
        private string _AskingQuestion;
        private string _ContainingLinks;
        private string _IncludeRetweets;
        private string _ResultPerPage;
        #endregion

        #region Properties
        public string AllTheseWords { get { return _AllTheseWords; } set { this._AllTheseWords = "ands=" + value; } }
        public string ThisExactPhrase { get { return _ThisExactPhrase; } set { this._ThisExactPhrase = "phrase=" + value; } }
        public string AnyTheseWords { get { return _AnyTheseWords; } set { this.AnyTheseWords = "ors=" + value; } }
        public string NoneTheseWords { get { return _NoneTheseWords; } set { this.NoneTheseWords = "nots" + value; } }
        public string ThisHastag { get { return _ThisHastag; } set { this.ThisHastag = "tag=" + value; } }
        public string WrittenIn { get { return _WrittenIn; } set { this.WrittenIn = "lang=ar" + value; } }
        public string FromThisPerson { get { return _FromThisPerson; } set { this.FromThisPerson = "from=" + value; } }
        public string ToThisPerson { get { return _ToThisPerson; } set { this.ToThisPerson = "to=" + value; } }
        public string ReferencingThisPerson { get { return _ReferencingThisPerson; } set { this.ReferencingThisPerson = "ref" + value; } }
        public string NearThisPlace { get { return _NearThisPlace; } set { this.NearThisPlace = "near" + value; } }
        public string WithinThisDistance { get { return _WithinThisDistance; } set { this.WithinThisDistance = "within" + value; } }
        public string KindDistance { get { return _KindDistance; } set { this.KindDistance = "units=mi" + value; } }
        public string SinceThisDate { get { return _SinceThisDate; } set { this.SinceThisDate = "since=2011-07-13" + value; } }
        public string UntilThisDate { get { return _UntilThisDate; } set { this.UntilThisDate = "until=2011-07-14" + value; } }
        public string WithPositiveAttitude { get { return _WithPositiveAttitude; } set { this.WithPositiveAttitude = "tude[]=%3A)" + value; } }
        public string WithNegativeAttitude { get { return _WithNegativeAttitude; } set { this.WithNegativeAttitude = "tude[]=%3A(" + value; } }
        public string AskingQuestion { get { return _AskingQuestion; } set { this.AskingQuestion = "tude[]=%3F" + value; } }
        public string ContainingLinks { get { return _ContainingLinks; } set { this.ContainingLinks = "filter[]=links" + value; } }
        public string IncludeRetweets { get { return _IncludeRetweets; } set { this.IncludeRetweets = "include[]=retweets" + value; } }
        public string ResultPerPage { get { return _ResultPerPage; } set { this.ResultPerPage = "rpp=" + value; } }
        #endregion

        #region Methods
        public List<Twitter> GetTwitter(string search)
        {
            //Faz a pesquisa no twitter de acordo com o filtro passado
            var request = WebRequest.Create(_URL_Twitter_Search + search) as HttpWebRequest;
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
        #endregion

        #region Helpers
        private static string Format_DateTime(string value)
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
        #endregion
        
    }

    public class Twitter
    {
        public string AuthorName { get; set; }
        public string AuthorUri { get; set; }
        public string Content { get; set; }
        public string Updated { get; set; }
        public string Link { get; set; }
    }
}