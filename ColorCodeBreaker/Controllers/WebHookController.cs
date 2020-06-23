using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ColorCodeBreaker.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static ColorCodeBreaker.Models.FbCommon;
using static ColorCodeBreaker.Models.FbMessengerResponse;

namespace ColorCodeBreaker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebHookController : ControllerBase
    {
        private string _fbUrl = "https://graph.facebook.com/v7.0/me/messages?access_token=<TOKEN_HERE>";
        private ILogger<WebHookController> _logger;
        private static HttpClient _httpClient = new HttpClient();
        private static Dictionary<string, Game> _games = new Dictionary<string, Game>();

        public WebHookController(ILogger<WebHookController> logger)
        {
            _logger = logger;
        }

        [HttpPost("Callback")]
        [HttpGet("Callback")]
        public async Task<IActionResult> Callback([FromBody] FbWebhook req)
        {
            var recipientId = req.entry.First().messaging.First().sender.id;
            var incomingMessage = req.entry.First().messaging.First().message.text;

            if (incomingMessage.Equals("Play", StringComparison.OrdinalIgnoreCase)
                || incomingMessage.Equals("Reset", StringComparison.OrdinalIgnoreCase))
            {
                _games.Remove(recipientId);
                await PostText(recipientId, "We setup a new game for you. You can start from scratch!");
            }

            if (!_games.ContainsKey(recipientId))
            {
                await InitGame(recipientId);
                await PostText(recipientId, "You have 10 tries to find the correct 4 color combination and win." +
                    " Same color may be used more than 1 time." +
                    " Every time you pick the 4th color, the bot will help you giving you hints. Have fun :)");
                await PostChoices(recipientId, 0);
            }
            else
            {
                var game = _games[recipientId];

                if (game.SelectedColorIndex < 3)
                {
                    game.Choices[game.SelectedColorIndex] = req.entry.First().messaging.First().message.text;
                    game.SelectedColorIndex++;
                    await PostChoices(recipientId, game.SelectedColorIndex);
                }
                else
                {
                    game.Choices[game.SelectedColorIndex] = req.entry.First().messaging.First().message.text;

                    game.SelectedColorIndex = 0;

                    var result = game.EvaluateSelection();
                    if (result.Status == Game.GameStatus.Lost || result.Status == Game.GameStatus.Won)
                    {
                        await PostText(recipientId, result.Result);
                        _games.Remove(recipientId);
                    }
                    else
                    {
                        await PostText(recipientId, result.Result);
                        await PostChoices(recipientId, game.SelectedColorIndex);
                    }
                }
            }

            return Ok();
            //var hubChallenge = Request.Query["hub.challenge"][0];

            //string body = "";
            //using (StreamReader stream = new StreamReader(Request.Body))
            //{
            //    body = stream.ReadToEnd();
            //}

            //_logger.LogError(body);
            //System.Diagnostics.Trace.WriteLine(body);

            //return Ok(hubChallenge);
        }

        private async Task InitGame(string recipientId)
        {
            _games.Add(recipientId, new Game(recipientId));
        }

        private async Task PostChoices(string recipientId, int colorSelectIndex)
        {
            string message = "";
            if (colorSelectIndex == 0)
            {
                message = "Pick 1st color:";
            }
            else if (colorSelectIndex == 1)
            {
                message = "Pick 2nd color:";
            }
            else if (colorSelectIndex == 2)
            {
                message = "Pick 3rd color:";
            }
            else if (colorSelectIndex == 3)
            {
                message = "Pick 4th color:";
            }

            var resp = new FbMessengerResponse();
            resp.recipient = new Recipient() { id = recipientId };
            resp.messaging_type = "RESPONSE";
            resp.message = new Message();
            resp.message.text = message;
            resp.message.quick_replies = new List<Quick_Replies>();
            resp.message.quick_replies.Add(new Quick_Replies()
            {
                content_type = "text",
                title = Game.RED,
                payload = Game.RED,
                image_url = "https://colorcodebreaker.azurewebsites.net/images/red.png"
            });
            resp.message.quick_replies.Add(new Quick_Replies()
            {
                content_type = "text",
                title = Game.BLUE,
                payload = Game.BLUE,
                image_url = "https://colorcodebreaker.azurewebsites.net/images/blue.png"
            });
            resp.message.quick_replies.Add(new Quick_Replies()
            {
                content_type = "text",
                title = Game.GREEN,
                payload = Game.GREEN,
                image_url = "https://colorcodebreaker.azurewebsites.net/images/green.png"
            });
            resp.message.quick_replies.Add(new Quick_Replies()
            {
                content_type = "text",
                title = Game.YELLOW,
                payload = Game.YELLOW,
                image_url = "https://colorcodebreaker.azurewebsites.net/images/yellow.png"
            });

            var payload = JsonConvert.SerializeObject(resp);

            var fbResponse = await _httpClient.PostAsync(_fbUrl, new StringContent(payload, Encoding.UTF8, "application/json"));

            var fbResponseString = await fbResponse.Content.ReadAsStringAsync();
        }

        private async Task PostText(string recipientId, string text)
        {
            var resp = new FbMessengerResponse();
            resp.recipient = new Recipient() { id = recipientId };
            resp.messaging_type = "RESPONSE";
            resp.message = new Message();
            resp.message.text = text;

            var payload = JsonConvert.SerializeObject(resp);

            var fbResponse = await _httpClient.PostAsync(_fbUrl, new StringContent(payload, Encoding.UTF8, "application/json"));

            var fbResponseString = await fbResponse.Content.ReadAsStringAsync();
        }
    }
}