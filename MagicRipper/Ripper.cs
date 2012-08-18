using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Diagnostics;

namespace MagicRipper
{
    public class Ripper
    {
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
        /// Get the list of all available expansions.
        /// </summary>
        /// <returns>The list of available expansions.</returns>
        public IEnumerable<Expansion> GetExpansions()
        {
            using (var webClient = new WebClient())
            {
                var page = webClient.DownloadString(@"http://gatherer.wizards.com/Pages/Advanced.aspx");
                var lines = page.Split('\n');
                int i = 0;
                while (!expansionsStartRegex.IsMatch(lines[i]))
                    i++;
                i++;
                while (!expansionsEndRegex.IsMatch(lines[i]))
                {
                    var match = expansionRegex.Match(lines[i]);
                    if (match.Groups.Count > 1)
                        yield return new Expansion(match.Groups[1].Value);
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
            new Regex(".*<a id=.*multiverseid=([0-9]+)\".*");
        private static Regex cardsEndRegex =
            new Regex(".*</table.*");

        public IEnumerable<Card> GetCards(Expansion expansion,
            Action<int> numCards)
        {
            bool numCardsCalled = false;
            int numPages = int.MaxValue;
            using (var webClient = new WebClient())
            using(var cardWebClient = new WebClient())
            {
                for (int currentPage = 0; currentPage < numPages; currentPage++)
                {
                    var html = webClient.DownloadString(
                        string.Format(
                            "http://gatherer.wizards.com/Pages/Search/Default.aspx?page={1}&action=advanced&output=compact&set=|[%22{0}%22]",
                            HttpUtility.UrlEncode(expansion.Name),
                            currentPage));
                    var lines = html.Split('\n');
                    
                    int i = 0;

                    if (!numCardsCalled)
                    {
                        for (; ; )
                        {
                            var match = numCardsRegex.Match(lines[i]);
                            if (match.Groups.Count > 1)
                            {
                                numCards(int.Parse(match.Groups[1].Value));
                                numCardsCalled = true;
                                break;
                            }
                            i++;
                        }
                    }
                    
                    while (!pagingStartRegex.IsMatch(lines[i]))
                        i++;
                    for (; ; i++)
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

                    while (!cardsStartRegex.IsMatch(lines[i]))
                        i++;
                    while (!cardsEndRegex.IsMatch(lines[i]))
                    {
                        var match = cardRegex.Match(lines[i]);
                        if (match.Groups.Count > 1)
                            foreach (var variant in getCards(
                                cardWebClient,
                                int.Parse(match.Groups[1].Value),
                                expansion))
                                yield return variant;
                        i++;
                    }
                }
            }
        }

        private const string labelString = "<div class=\"label\"";
        private static Regex manaCostImagesRegex =
            new Regex("<img src=\"[^\"]+\"");
        private static Regex costSymbolRegex =
            new Regex(".*name=(.*)&amp;type=symbol");
        private static Regex textCostRegex =
            new Regex("<img src=\"[^\"]*name=([^\"]*)&amp;type=symbol[^>]*/>");
        private static Regex rarityRegex =
            new Regex(".*>([^<]*)<.*");

        private static Dictionary<string, Rarity> rarityTranslations =
            new Dictionary<string, Rarity>()
            {
                { "Common", Rarity.Common },
                { "Uncommon", Rarity.Uncommon },
                { "Rare", Rarity.Rare },
                { "Mythic Rare", Rarity.MythicRare },
            };

        private IEnumerable<Card> getCards(WebClient webClient, int multiverseId,
            Expansion expansion)
        {
            var htmlBytes = webClient.DownloadData(string.Format(
                "http://gatherer.wizards.com/Pages/Card/Details.aspx?multiverseid={0}",
                multiverseId));
            var html = UTF8Encoding.UTF8.GetString(htmlBytes);
            var lines = html.Split('\n');
            int i = 0;
            var data = new Dictionary<string, string>();
            while (i < lines.Length)
            {
                if (lines[i].Contains(labelString))
                {
                    i++;
                    var name = lines[i].Trim().Replace(":</div>", "");
                    i++;
                    i++;
                    var text = lines[i].Trim();
                    if (text.EndsWith("</div>"))
                        text = text.Substring(0, text.Length - 6);
                    while(text.EndsWith("<br />"))
                        text = text.Substring(0, text.Length - 6);
                    data[name] = text;
                }
                i++;
            }

            var manaCost = string.Join("",
                (from Match symbolMatch in manaCostImagesRegex.Matches(data["Mana Cost"])
                 select "{" + costSymbolRegex.Match(symbolMatch.Value).Groups[1].Value + "}"));

            var allTypes = data["Types"].Split('—');
            string[] types, subtypes;
            if (allTypes.Length == 1)
            {
                types = allTypes[0].Trim().Split(' ');
                subtypes = new string[0];
            }
            else
            {
                types = allTypes[0].Trim().Split(' ');
                subtypes = allTypes[1].Trim().Split(' ');
            }

            var cardText = parseTextTags(data, "Card Text");

            var flavorText = parseTextTags(data, "Flavor Text");

            string[] ptSplit;
            if(data.ContainsKey("P/T"))
            {
                ptSplit = data["P/T"].Split('/');
                ptSplit[0] = ptSplit[0].Trim();
                ptSplit[1] = ptSplit[1].Trim();
            }
            else
                ptSplit = new string[] { null, null };

            var rarity = rarityTranslations[rarityRegex.Match(data["Rarity"]).Groups[1].Value];

            yield return new Card() {
                Name = data["Card Name"],
                ManaCost = manaCost,
                ConvertedManaCost = int.Parse(data["Converted Mana Cost"]),
                Types = types,
                Subtypes = subtypes,
                Text = cardText,
                FlavorText = flavorText,
                Power = ptSplit[0],
                Toughness = ptSplit[1],
                Expansion = expansion,
                Rarity = rarity,
                Number = int.Parse(data["Card #"]),
                Artist = data["Artist"],
                Language = Language.Oracle
            };
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
            cardText = textCostRegex.Replace(cardText, (Match m) => "{" + m.Groups[1].Value + "}");
            cardText = cardText.Trim();
            Debug.Assert(!cardText.Contains("<"), "There are still tags inside card text");
            return cardText;
        }
    }
}
