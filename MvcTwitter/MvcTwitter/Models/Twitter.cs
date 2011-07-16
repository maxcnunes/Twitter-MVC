using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Net;
using System.Globalization;
using System.IO;
using System.Web.Script.Serialization;

namespace MvcTwitter.Models
{
    public class TwitterSearch
    {

        #region Variables
        public const string URL_Twitter_Search = @"http://search.twitter.com/search.atom?";
        public string AllTheseWords;
        public string ThisExactPhrase;
        public string AnyTheseWords;
        public string NoneTheseWords;
        public string ThisHastag;
        public string WrittenIn;
        public string FromThisPerson;
        public string ToThisPerson;
        public string ReferencingThisPerson;
        public string NearThisPlace;
        public Nullable<int> WithinThisDistance;
        public Nullable<Unit> UnitDistance;
        public Nullable<DateTime> SinceThisDate;
        public Nullable<DateTime> UntilThisDate;
        public bool WithPositiveAttitude = false;
        public bool WithNegativeAttitude = false;
        public bool AskingQuestion = false;
        public bool ContainingLinks = false;
        public bool IncludeRetweets = false;
        public Nullable<int> ResultPerPage;
        public enum Unit { mi, km };

        #endregion

        #region Methods
        public List<Twitter> GetTwitter()
        {

            string SearchFilter = string.Format(@"q=&ands={0}&phrase={1}&ors={2}&nots={3}&tag={4}&lang={5}&from={6}&to={7}&ref={8}&near={9}&within={10}&units={11}&since={12}&until={13}{14}{15}{16}{17}{18}&rpp={19}",
                                            AllTheseWords,
                                            ThisExactPhrase,
                                            AnyTheseWords,
                                            NoneTheseWords,
                                            ThisHastag,
                                            WrittenIn ?? "all",
                                            FromThisPerson,
                                            ToThisPerson,
                                            ReferencingThisPerson,
                                            NearThisPlace,
                                            WithinThisDistance ?? 15,
                                            (UnitDistance != null ? Enum.GetName(typeof(Unit), UnitDistance) : Enum.GetName(typeof(Unit), Unit.mi)),
                                            string.Format("{0:d}", SinceThisDate),
                                            string.Format("{0:d}", UntilThisDate),
                                            (WithPositiveAttitude ? "&tude%5B%5D=%3A%29" : null),
                                            (WithNegativeAttitude ? "&tude%5B%5D=%3A%28" : null),
                                            (AskingQuestion ? "&tude%5B%5D=%3F" : null),
                                            (ContainingLinks ? "&filter%5B%5D=links" : null),
                                            (IncludeRetweets ? "&include%5B%5D=retweets" : null),
                                            ResultPerPage ?? 15
                                        );

            //Faz a pesquisa no twitter de acordo com o filtro passado
            var request = WebRequest.Create(URL_Twitter_Search + SearchFilter) as HttpWebRequest;
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

            DateTime Date;
            if (!DateTime.TryParse(value, out Date))
                throw new ArgumentException("Erro ao converter.");

            // Formata a data para o formato de tempo brasileiro.
            return Date.ToString(ci);
        }


        //Em implementação...
        public static string GetWeatherByLocation()
        {
            //string formattedUri = String.Format(CultureInfo.InvariantCulture, "http://search.twitter.com/search.json?q=twitter", lat, lng);
            HttpWebRequest webRequest = GetWebRequest("http://search.twitter.com/search.json?q=twitter");
            HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
            string jsonResponse = string.Empty;
            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                jsonResponse = sr.ReadToEnd();
            }
            return jsonResponse;
        }
        private static HttpWebRequest GetWebRequest(string formattedUri)
        {
            // Create the request’s URI.
            Uri serviceUri = new Uri(formattedUri, UriKind.Absolute);

            // Return the HttpWebRequest.
            return (HttpWebRequest)System.Net.WebRequest.Create(serviceUri);
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

    public class TwitterJson
    {

        public string stringmax_id { get; set; }
        public string since_id { get; set; }
        public string refresh_url { get; set; }
        public string next_page { get; set; }
        public string results_per_page { get; set; }
        public string page { get; set; }
        public string completed_in { get; set; }
        public string warning { get; set; }
        public string stringsince_id_str { get; set; }
        public string stringmax_id_str { get; set; }
        public string stringquery { get; set; }
    }
}