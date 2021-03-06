﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Diagnostics;

namespace MagicRipper
{
    /// <summary>
    /// Handles the HTML scraping of <a href="http://gatherer.wizards.com">Gatherer webpages</a>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The flow of operations within this class is the following:
    /// <list type="bullet">
    /// <item>
    /// <description>Get all the sets with <see cref="Ripper.GetSets"/>. For each set:</description>
    /// </item>
    /// <item>
    /// <description>Invoke <see cref="Ripper.GetCards"/> to get all the cards.</description>
    /// </item>
    /// <item>
    /// <description><see cref="Ripper.SetDownloading"/> gets called.</description>
    /// </item>
    /// <item>
    /// <description>For each card then:
    /// <list type="bullet">
    /// <item>
    /// <description><see cref="Ripper.BaseCardDownloading"/> gets called.</description>
    /// </item>
    /// <item>
    /// <description><see cref="Ripper.CardDownloading"/> gets called.</description>
    /// </item>
    /// </list>
    /// </description>
    /// </item>
    /// </list>
    /// </para>
    /// <para>
    /// During all the steps involving some download, either <see cref="Ripper.HtmlDownloadSucceeded"/> or
    /// <see cref="Ripper.HtmlDownloadError"/> gets called.</para>
    /// <para>
    /// The Un-sets are not supported.
    /// </para>
    /// </remarks>
    public class Ripper
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Ripper"/> class.
        /// </summary>
        public Ripper()
        {
        }

        private static Regex expansionsStartRegex =
            new Regex(".*<div class=\"textboxsmall\".*autoCompleteSourceBoxsetAddText0.*");
        private static Regex expansionRegex =
            new Regex(@".*<a.*>(.*)</a>.*");
        private static Regex expansionsEndRegex =
            new Regex(@".*</div>.*");

        /// <summary>
        /// Gets the available sets.
        /// </summary>
        /// <returns>An enumeration of all the available sets.</returns>
        public IEnumerable<Set> GetSets()
        {
            using (var webClient = new WebClient())
            {
                webClient.Proxy = null;
                var lines = getLines(webClient, @"http://gatherer.wizards.com/Pages/Advanced.aspx");
                int i = 0;
                while (!expansionsStartRegex.IsMatch(lines[i]))
                    i++;
                i++;
                while (!expansionsEndRegex.IsMatch(lines[i]))
                {
                    var match = expansionRegex.Match(lines[i]);
                    if (match.Groups.Count > 1)
                    {
                        var name = match.Groups[1].Value;
                        if(name != "Unglued" && name != "Unhinged")
                            yield return new Set(name);
                    }
                    i++;
                }
            }
        }

        private static Regex numCardsRegex =
            new Regex(".*ctl00_ctl00_ctl00_MainContent_SubContent_SubContentHeader_searchTermDisplay\".*\\(([0-9]+)\\).*");
        private static Regex pagingStartRegex =
            new Regex(".*<div class=\"pagingcontrols\">.*");
        private static Regex pagingRegex =
            new Regex("([0-9]+)</a>");
        private static Regex cardsStartRegex =
            new Regex(".*<table.*");
        private static Regex cardRegex =
            new Regex(".*multiverseid=([0-9]+)\">([^<]+)</a>.*");
        private static string cardsEndString =
            "</table";

        /// <summary>
        /// Occurs just before a set is downloaded.
        /// </summary>
        public event EventHandler<SetDownloadingEventArgs>
            SetDownloading;

        /// <summary>
        /// Occurs just before a card and all its language variants are downloaded.
        /// </summary>
        public event EventHandler<BaseCardDownloadingEventArgs>
            BaseCardDownloading;

        /// <summary>
        /// Occurs just before a card is downloaded.
        /// </summary>
        public event EventHandler<CardDownloadingEventArgs>
            CardDownloading;

        /// <summary>
        /// Gets all the cards from a single expansion set.
        /// 
        /// The cards are returned as an enumeration of collections, as soon as
        /// they are downloaded. Each collection is a group of cards obtained
        /// from a single page. Because of this, a single collection is the
        /// most logical transaction unit.
        /// </summary>
        /// <param name="expansion">The expansion to get the cards from.</param>
        /// <returns>An enumeration of groups of cards, returned as they are
        /// downloaded.</returns>
        public IEnumerable<ICollection<Card>> GetCards(Set expansion)
        {
            bool numCardsCalled = false;
            int numPages = int.MaxValue;
            using (var webClient = new WebClient())
            using(var cardWebClient = new WebClient())
            using(var languageWebClient = new WebClient())
            {
                webClient.Proxy = null;
                cardWebClient.Proxy = null;
                languageWebClient.Proxy = null;

                // run over all pages of current expansion listing
                for (int currentPage = 0; currentPage < numPages; currentPage++)
                {
                    var lines = getLines(
                        webClient,
                        string.Format(
                            "http://gatherer.wizards.com/Pages/Search/Default.aspx?page={1}&action=advanced&output=compact&set=|[%22{0}%22]",
                            HttpUtility.UrlEncode(expansion.Name),
                            currentPage));
                    
                    int i = 0;

                    // invoke the event for sending the number of cards, if
                    // not already done
                    if (!numCardsCalled)
                    {
                        for (; ; )
                        {
                            var match = numCardsRegex.Match(lines[i]);
                            if (match.Groups.Count > 1)
                            {
                                var numCards = int.Parse(match.Groups[1].Value);
                                if (expansion.Name == "Ninth Edition")
                                    numCards--; // Sea Eagle, from 8th edition, appears here
                                                // too, for some unknown reason
                                var handler = SetDownloading;
                                if (handler != null)
                                    handler(this, new SetDownloadingEventArgs(
                                        expansion, numCards));
                                numCardsCalled = true;
                                break;
                            }
                            i++;
                        }
                    }
                    
                    // gets the number of pages (at every page, it doesn't
                    // cost extra downloads)
                    while (!pagingStartRegex.IsMatch(lines[i]))
                        i++;
                    for (; i < lines.Length; i++)
                    {
                        var matches = pagingRegex.Matches(lines[i]);
                        if (matches.Count > 0)
                        {
                            numPages = (from Match match in matches
                                        select int.Parse(match.Groups[1].Value))
                                        .Max();
                            break;
                        }
                    }
                    if (i == lines.Length)
                    {
                        numPages = 1;
                        i = 0;
                    }

                    // find the cards inside the listing
                    while (i < lines.Length && !cardsStartRegex.IsMatch(lines[i]))
                        i++;
                    while (i < lines.Length && !lines[i].Contains(cardsEndString))
                    {
                        if (lines[i].Length > 5000)
                        {
                            // the Match below would miserably get stuck
                            // typically caused by the list of sets
                            // having a basic land card: 48000+ chars with
                            // no match
                            i++;
                            continue;
                        }
                        var match = cardRegex.Match(lines[i]);
                        if (match.Groups.Count > 1)
                        {
                            // got a card (base)
                            var multiverseId = int.Parse(match.Groups[1].Value);
                            var part = match.Groups[2].Value;

                            var handler = BaseCardDownloading;
                            var downloadVariants = true;
                            if (handler != null)
                            {
                                var bcdea = new BaseCardDownloadingEventArgs(
                                    multiverseId, part);
                                handler(this, bcdea);
                                if (bcdea.Cancel)
                                    // no need to download this block: continue
                                    downloadVariants = false;
                            }

                            // download the different languages
                            if (downloadVariants)
                            {
                                bool isMultipart = false;
                                foreach (var variant in getVariants(multiverseId, languageWebClient))
                                {
                                    ICollection<Card> cards = null;
                                    while (cards == null)
                                        try
                                        {
                                            cards = getCard(
                                                cardWebClient,
                                                multiverseId,
                                                variant.Item2,
                                                expansion,
                                                isMultipart ? Language.Oracle : variant.Item1,
                                                part);
                                        }
                                        catch (MultipartLanguageCompatibilityException)
                                        {
                                            isMultipart = true;
                                        }
                                    if (cards.Count > 1)
                                        filterCards(cards);
                                    if (cards.Count != 0)
                                        yield return cards;
                                }
                            }
                        }
                        i++;
                    }
                }
            }
        }

        private void filterCards(ICollection<Card> cards)
        {
            Debug.Assert(cards.Count == 2);
            var toRemove = cards.Where(card => card.Part != card.Name).First();
            cards.Remove(toRemove);
        }

        private static Regex cardIdRegex =
            new Regex(".*Details.aspx.multiverseid=([0-9]+)");

        private IEnumerable<Tuple<Language, int>> getVariants(
            int multiverseId, WebClient webClient)
        {
            // get the language page
            var lines = getLines(webClient, string.Format(
                "http://gatherer.wizards.com/Pages/Card/Languages.aspx?multiverseid={0}",
                multiverseId));

            // return the basic english version
            yield return Tuple.Create(Language.English, multiverseId);

            // return the versions in all the other languages
            var i = 0;
            while (i < lines.Length)
            {
                if (lines[i++].Contains("cardItem evenItem"))
                {
                    // get the id
                    Match match = null;
                    for (; ; )
                    {
                        match = cardIdRegex.Match(lines[i++]);
                        if (match.Groups.Count > 1)
                            break;
                    }
                    var id = int.Parse(match.Groups[1].Value);
                    // get the language
                    while (!lines[i++].Contains("text-align: center;"))
                        ;
                    var languageString = lines[i].Trim()
                        .Replace(" ", "")
                        .Replace("(", "")
                        .Replace(")", "");
                    var language = (Language)Enum.Parse(typeof(Language), languageString);
                    // return the found value
                    yield return Tuple.Create(language, id);
                }
            }

            // return the base oracle version
            yield return Tuple.Create(Language.Oracle, multiverseId);
        }

        private static Regex multipartMarkerRegex =
            new Regex(".*This is one part of the multi-part card <b>([^<]*)</b>.*");
        private const string startCardString = "class=\"cardComponentContainer\"";
        private const string labelString = "<div class=\"label\"";
        private static Regex manaCostImagesRegex =
            new Regex("<img src=\"[^\"]+\"");
        private static Regex costSymbolRegex =
            new Regex(".*name=(.*)&amp;type=symbol");
        private static Regex textCostRegex =
            new Regex("<img src=\"[^\"]*name=([^\"]*)&amp;type=symbol[^>]*/>");
        private static Regex inTagRegex =
            new Regex(".*>([^<]*)<.*");

        private static Dictionary<string, Rarity> rarityTranslations =
            new Dictionary<string, Rarity>()
            {
                { "Basic Land", Rarity.BasicLand },
                { "Common", Rarity.Common },
                { "Uncommon", Rarity.Uncommon },
                { "Rare", Rarity.Rare },
                { "Mythic Rare", Rarity.MythicRare },
                { "Special", Rarity.Special },
                { "", Rarity.Unset },
            };

        private ICollection<Card> getCard(WebClient webClient, int baseMultiverseId,
            int multiverseId, Set expansion, Language language, string part)
        {
            var collectedCards = new List<Card>();

            var handler = CardDownloading;
            if (handler != null)
            {
                var eventArgs = new CardDownloadingEventArgs(multiverseId,
                    baseMultiverseId, part, language);
                handler(this, eventArgs);
                if (eventArgs.Cancel)
                    return collectedCards;
            }

            var url = produceUrl(multiverseId, language, part);
            var lines = getLines(webClient, url);

            int lineNumber = 0;
            var data = new Dictionary<string, string>();
            while (lineNumber < lines.Length)
            {
                data.Clear();

                lineNumber = findCardStart(lines, lineNumber);

                bool isMultipart;
                lineNumber = readCardContent(lines, lineNumber, data, out isMultipart);

                if (isMultipart && language != Language.Oracle)
                    throw new MultipartLanguageCompatibilityException();

                if (!data.ContainsKey("Card Name"))
                    continue;

                string[] types;
                string[] subtypes;
                parseTypesAndSubtypes(language, data, out types, out subtypes);

                var ptSplit = parsePowerAndToughness(data);

                char? variant;
                int? number = parseNumberAndVariant(data, out variant);

                collectedCards.Add(new Card()
                {
                    MultiverseId = multiverseId,
                    BaseMultiverseId = baseMultiverseId,
                    Part = part,
                    Name = data["Card Name"],
                    ManaCost = parseManaCost(data),
                    ConvertedManaCost = parseConvertedManaCost(data),
                    Types = types,
                    Subtypes = subtypes,
                    Text = parseTextTags(data, "Card Text"),
                    FlavorText = parseTextTags(data, "Flavor Text"),
                    Power = ptSplit[0],
                    Toughness = ptSplit[1],
                    Set = expansion,
                    Rarity = parseRarity(data),
                    Number = number,
                    Variant = variant,
                    Artist = parseArtist(data),
                    Language = language
                });
            }

            if (collectedCards.Count > 1 && language != Language.Oracle)
                throw new MultipartLanguageCompatibilityException();

            return collectedCards;
        }

        private static int readCardContent(string[] lines, int lineNumber,
            Dictionary<string, string> data, out bool isMultipart)
        {
            isMultipart = false;
            string overrideName = null;

            while (lineNumber < lines.Length &&
                !lines[lineNumber].Contains(startCardString))
            {
                // check if multipart
                if (lines[lineNumber].Length > 5000)
                {
                    // the Match below would miserably get stuck
                    // typically caused by the list of sets
                    // having a basic land card: 48000+ chars with
                    // no match
                    lineNumber++;
                    continue;
                }
                var match = multipartMarkerRegex.Match(lines[lineNumber]);
                if(match.Groups.Count > 1)
                {
                    isMultipart = true;
                    overrideName = match.Groups[1].Value;
                }
                // find label
                if (lines[lineNumber].Contains(labelString))
                {
                    lineNumber++;
                    var name = lines[lineNumber].Trim().Replace(":</div>", "");
                    lineNumber++;
                    lineNumber++;
                    var text = lines[lineNumber].Trim();
                    if (text.EndsWith("</div>"))
                        text = text.Substring(0, text.Length - 6);
                    while (text.EndsWith("<br />"))
                        text = text.Substring(0, text.Length - 6);
                    data[name] = text;
                }
                // go to the next line
                lineNumber++;
            }

            if(overrideName != null)
                data["Card Name"] = overrideName;

            return lineNumber;
        }

        private static int findCardStart(string[] lines, int lineNumber)
        {
            while (lineNumber < lines.Length &&
                !lines[lineNumber].Contains(startCardString))
                lineNumber++;
            lineNumber++;
            return lineNumber;
        }

        /// <summary>
        /// Occurs whenever there's an error of any kind while downloading an HTML page.
        /// </summary>
        public event EventHandler<HtmlDownloadErrorEventArgs> HtmlDownloadError;

        /// <summary>
        /// Occurs whenever an HTML page is successfully downloaded. Useful to reset
        /// any error indication caused by a <see cref="HtmlDownloadError"/>.
        /// </summary>
        public event EventHandler<HtmlDownloadSucceededEventArgs> HtmlDownloadSucceeded;

        private string[] getLines(WebClient webClient, string url)
        {
            string html;
            int exponentialBackoff = 1;
            for (; ; )
                try
                {
                    var bytes = webClient.DownloadData(url);
                    html = Encoding.UTF8.GetString(bytes);
                    break;
                }
                catch (Exception e)
                {
                    bool ignore = false;

                    var handler = HtmlDownloadError;
                    if (handler != null)
                    {
                        var networkErrorEventArgs = new HtmlDownloadErrorEventArgs(url, e);
                        handler(this, networkErrorEventArgs);
                        ignore = networkErrorEventArgs.Ignore;
                    }

                    if (!ignore)
                        throw;

                    System.Threading.Thread.Sleep(exponentialBackoff);
                    exponentialBackoff = Math.Max(10000, Math.Min(1000, exponentialBackoff * 2));
                }

            var handlerSuccess = HtmlDownloadSucceeded;
            if (handlerSuccess != null)
                handlerSuccess(this, new HtmlDownloadSucceededEventArgs(url));

            return html.Split('\n');
        }

        private static string produceUrl(int multiverseId, Language language,
            string part)
        {
            var args = new Dictionary<string, string>();
            args["multiverseid"] = multiverseId.ToString();
            if (language != Language.Oracle)
                args["printed"] = "true";
            args["part"] = part;
            // TODO: escape part name
            var url = "http://gatherer.wizards.com/Pages/Card/Details.aspx?" +
                string.Join("&", (
                    from entry in args
                    select string.Format("{0}={1}", entry.Key, entry.Value)));
            return url;
        }

        private static int parseConvertedManaCost(Dictionary<string, string> data)
        {
            return data.ContainsKey(
                "Converted Mana Cost") ?
                int.Parse(data["Converted Mana Cost"]) :
                0;
        }

        private int? parseNumberAndVariant(Dictionary<string, string> data, out char? variant)
        {
            if(!data.ContainsKey("Card #"))
            {
                variant = null;
                return null;
            }
            
            var numberAndVariant = data["Card #"];
            
            int number;
            if (int.TryParse(numberAndVariant, out number))
            {
                variant = null;
                return number;
            }
            else
            {
                variant = numberAndVariant[numberAndVariant.Length - 1];
                return int.Parse(numberAndVariant.Substring(0, numberAndVariant.Length - 1));
            }
        }

        private static string parseArtist(Dictionary<string, string> data)
        {
            var artist = inTagRegex.Match(data["Artist"]).Groups[1].Value;
            return artist;
        }

        private static Rarity parseRarity(Dictionary<string, string> data)
        {
            return rarityTranslations[inTagRegex.Match(data["Rarity"]).Groups[1].Value];
        }

        private static string[] parsePowerAndToughness(Dictionary<string, string> data)
        {
            string[] ptSplit;
            if (data.ContainsKey("P/T"))
            {
                ptSplit = data["P/T"].Split('/');
                ptSplit[0] = ptSplit[0].Trim();
                ptSplit[1] = ptSplit[1].Trim();
            }
            else
                ptSplit = new string[] { null, null };
            return ptSplit;
        }

        /// <summary>
        /// A map between languages and the corresponding name of the land
        /// type - we need to distinguish it to treat the subtype differently
        /// (e.g.: "Land - Urza's Mine" has one subtype, "Urza's Mine", and not
        /// two, "Urza's" and "Mine", as the general rule for subtype would
        /// say)
        /// </summary>
        private static Dictionary<Language, string> landTypes = new Dictionary<Language, string>() {
            { Language.ChineseSimplified, "地～" },
            { Language.ChineseTraditional, "地～" },
            { Language.English, "Land" },
            { Language.French, "Terrain" },
            { Language.German, "Land" },
            { Language.Italian, "Terra" },
            { Language.Japanese, "土地" },
            { Language.Korean, "대지" },
            { Language.Oracle, "Land" },
            { Language.Portuguese, "Terreno" },
            { Language.PortugueseBrazil, "Terreno" },
            { Language.Russian, "Земля" },
            { Language.Spanish, "Tierra" }
        };

        private static void parseTypesAndSubtypes(Language language, Dictionary<string, string> data, out string[] types, out string[] subtypes)
        {
            var allTypes = data["Types"].Split('—');
            if (allTypes.Length == 1)
                // french split
                allTypes = data["Types"].Split(new string[] { " : - " }, StringSplitOptions.None);
            if (allTypes.Length == 1)
                // japanese split
                allTypes = data["Types"].Split(new string[] { " ― - " }, StringSplitOptions.None);
            if (allTypes.Length == 1)
                allTypes = data["Types"].Split(new string[] { " - " }, StringSplitOptions.None);

            if (allTypes[0].Trim().Equals(landTypes[language]))
            {
                // special case for lands: subtype is just one (e.g.:
                // Urza's Mine)
                types = new string[] { allTypes[0].Trim() };
                if (allTypes.Length > 1)
                    subtypes = new string[] { allTypes[1].Trim() };
                else
                    subtypes = new string[0];
            }
            else if (allTypes.Length == 1)
            {
                types = allTypes[0].Trim().Split(' ');
                subtypes = new string[0];
            }
            else
            {
                types = allTypes[0].Trim().Split(' ');
                var subtypeString = allTypes[1].Trim();
                switch (language)
                {
                    case Language.ChineseTraditional:
                    case Language.ChineseSimplified:
                        subtypes = subtypeString.Split('／');
                        break;
                    case Language.German:
                        subtypes = subtypeString.Split(' ');
                        // remove ending ',' if necessary
                        for (int j = 0; j < subtypes.Length - 1; j++)
                        {
                            var subtype = subtypes[j];
                            if (subtype[subtype.Length - 1] == ',')
                            {
                                subtype = subtype.Substring(0, subtype.Length - 1);
                                subtypes[j] = subtype;
                            }
                        }
                        break;
                    case Language.French:
                        subtypes = subtypeString.Split(new string[] { " et " },
                            StringSplitOptions.None);
                        break;
                    case Language.Japanese:
                        subtypes = subtypeString.Split(new string[] { " ・ " },
                            StringSplitOptions.None);
                        break;
                    default:
                        subtypes = subtypeString.Split(' ');
                        break;
                }
            }
        }

        private static string parseManaCost(Dictionary<string, string> data)
        {
            return data.ContainsKey("Mana Cost") ?
                string.Join("",
                    (from Match symbolMatch in manaCostImagesRegex.Matches(data["Mana Cost"])
                     select "{" + costSymbolRegex.Match(symbolMatch.Value).Groups[1].Value + "}")) :
                null;
        }

        private static string parseTextTags(Dictionary<string, string> data,
            string key)
        {
            if (!data.ContainsKey(key))
                return null;
            var cardText = data[key]
                .Replace("<div class=\"cardtextbox\">", "")
                .Replace("<div class='cardtextbox'>", "")
                .Replace("</div>", "\n")
                .Replace("<i>", "{-I}")
                .Replace("</i>", "{-/I}");
            if (key == "Flavor Text" && data.ContainsKey("Card Name") &&
                data["Card Name"] == "Drain d'essence")
            {
                // epic fail in this page: invalid html
                cardText = cardText.Replace("<<", "«");
            }
            cardText = textCostRegex.Replace(cardText, (Match m) => "{" + m.Groups[1].Value + "}");
            cardText = cardText.Trim();
            Debug.Assert(!cardText.Contains("<"), "There are still tags inside card text");
            return cardText;
        }
    }
}
