using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;

namespace PremierLeagueWebScraper.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PremierLeagueAPIContoller : ControllerBase
    {

        public PremierLeagueAPIContoller()
        {

        }

        [HttpGet("GetTopScorers")]
        public async Task<ActionResult<IEnumerable<Player>>> GetTopScorers(int number = 25)
        {

            string url = "https://www.bbc.com/sport/football/premier-league/top-scorers";
            var response = await CallUrl(url);

            List<Player> listOfPlayers = new();


            foreach (var a in ParseHtml(response))
            {

                try
                {
                    if (a.Descendants().Where(t => t.HasClass("gs-u-display-inherit@l")).Count() == 0)
                    {
                        continue;
                    }



                    var player = new Player()
                    {
                        Name = a.Descendants().Where(t => t.HasClass("gs-u-display-inherit@l")).First().InnerText,
                        Club = a.Descendants().Where(t => t.HasClass("gs-o-table__cell")).ToList()[1].FirstChild
                            .ChildNodes[1].FirstChild.ChildNodes[0].InnerText,
                        NumberOfGoals = int.Parse(a.Descendants().Where(t => t.HasClass("gs-o-table__cell")).ToList()[2]
                            .InnerText),
                        NumberOfAssists =
                            int.Parse(a.Descendants().Where(t => t.HasClass("gs-o-table__cell")).ToList()[3].FirstChild
                                .InnerText),
                        GamesPlayed = int.Parse(a.Descendants().Where(t => t.HasClass("gs-o-table__cell")).ToList()[4]
                            .FirstChild.InnerText)

                    };


                    listOfPlayers.Add(player);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }


            }



            return Ok(listOfPlayers.Take(number));




        }


        [HttpGet("GetTopAssisters")]
        public async Task<ActionResult<IEnumerable<Player>>> GetTopAssisters(int number = 25)
        {

            string url = "https://www.bbc.com/sport/football/premier-league/top-scorers/assists";
            var response = await CallUrl(url);

            List<Player> listOfPlayers = new();


            foreach (var a in ParseHtml(response))
            {

                try
                {
                    if (a.Descendants().Where(t => t.HasClass("gs-u-display-inherit@l")).Count() == 0)
                    {
                        continue;
                    }



                    var player = new Player()
                    {
                        Name = a.Descendants().Where(t => t.HasClass("gs-u-display-inherit@l")).First().InnerText,
                        Club = a.Descendants().Where(t => t.HasClass("gs-o-table__cell")).ToList()[1].FirstChild
                            .ChildNodes[1].FirstChild.ChildNodes[0].InnerText,
                        NumberOfAssists = int.Parse(
                            a.Descendants().Where(t => t.HasClass("gs-o-table__cell")).ToList()[2]
                                .InnerText),
                        NumberOfGoals = int.Parse(a.Descendants().Where(t => t.HasClass("gs-o-table__cell")).ToList()[3]
                            .FirstChild.InnerText),
                        GamesPlayed = int.Parse(a.Descendants().Where(t => t.HasClass("gs-o-table__cell")).ToList()[4]
                            .FirstChild.InnerText)

                    };


                    listOfPlayers.Add(player);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }


            }



            return Ok(listOfPlayers.Take(number));




        }


        [HttpGet("GetLeagueTable")]
        public async Task<ActionResult<IEnumerable<Team>>> GetLeagueTable(int number = 20)
        {

            string url = "https://www.bbc.com/sport/football/premier-league/table";
            var response = await CallUrl(url);

            List<Team> listOfTeams = new();


            var teams = ParseHtmlTable(response);

            foreach (var team in teams)
            {

                var newTeam = new Team()
                {
                    Position = int.Parse(team.ChildNodes[0].InnerText), Name = team.ChildNodes[1].InnerText,
                    GamesPlayed = int.Parse(team.ChildNodes[2].InnerText),
                    GamesWon = int.Parse(team.ChildNodes[3].InnerText),
                    GamesLost = int.Parse(team.ChildNodes[4].InnerText),
                    GamesDrawn = int.Parse(team.ChildNodes[5].InnerText),
                    GoalsFor = int.Parse(team.ChildNodes[6].InnerText),
                    GoalsAgainst = int.Parse(team.ChildNodes[7].InnerText),
                    GoalDifference = int.Parse(team.ChildNodes[8].InnerText),
                    Points = int.Parse(team.ChildNodes[9].InnerText)
                };


                listOfTeams.Add(newTeam);

            }

            return Ok(listOfTeams.Take(number));






        }

        [HttpGet("GetNews")]

        public async Task<ActionResult<IEnumerable<Article>>> GetNews(int number=5)
        {

            string url = "https://www.bbc.com/sport/football/premier-league";
            var response = await CallUrl(url);

            List<Article> articles = new();


            var teams = ParseHtmlNews(response);



            foreach (var team in teams)
            {

                

                try
                {


                   


                    //Link
                    Console.WriteLine(team.ChildNodes.ToList()[0].Attributes[6].Value);

                    //Title
                    Console.WriteLine(team.ChildNodes.ToList()[0].Attributes[2].Value);


                    string url2 = team.ChildNodes.ToList()[0].Attributes[6].Value;
                    var res = await CallUrl(url2);

                    var parsed = ParseHtmlArticle(res);


                    string content = "";

                    foreach (var a in parsed.Take(3))
                    {
                        content += a.InnerText;

                    }

                    Article article = new Article()
                    {
                        Url = team.ChildNodes.ToList()[0].Attributes[6].Value,
                        Title = team.ChildNodes.ToList()[0].Attributes[2].Value,
                        Content = content
                    };

                    articles.Add(article);




                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }







            }

            return Ok(articles.Take(number));
        }
    









    private static async Task<string> CallUrl(string fullUrl)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetStringAsync(fullUrl);
            return response;
        }


        private static IEnumerable<HtmlNode> ParseHtml(string html)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var programmerLinks = htmlDoc.DocumentNode.Descendants("gel-long-primer").ToList();

            var topScorers = htmlDoc.DocumentNode.Descendants().Where(t => t.HasClass("gel-long-primer")).ToList()[0]
                .ChildNodes.ToList();


            return topScorers;
        }

        private static IEnumerable<HtmlNode> ParseHtmlTable(string html)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var topScorers =
                htmlDoc.DocumentNode.Descendants().Where(t => t.HasClass("ssrcss-14j0ip6-Table")).ToList()[0].ChildNodes.ToList()[1].ChildNodes.ToList();

            return topScorers;
        }

        private static IEnumerable<HtmlNode> ParseHtmlNews(string html)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var topScorers =
                htmlDoc.DocumentNode.Descendants().Where(t => t.HasClass("sp-o-no-keyline@m")).Skip(1).Take(11).ToList();

            return topScorers;
        }

        private static IEnumerable<HtmlNode> ParseHtmlArticle(string html)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var topScorers =
                htmlDoc.DocumentNode.Descendants().Where(u => u.HasClass("qa-story-body")).ToList()[0].ChildNodes.Take(5).ToList();

            return topScorers;
        }

    }
}