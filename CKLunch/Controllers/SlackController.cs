using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using SlackAPI;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CKLunch.Controllers;

[ApiController]
[Microsoft.AspNetCore.Mvc.Route("slack")]
public class SlackController : ControllerBase
{
    private ILogger<SlackController> Logger { get; }
    private IConfiguration Config { get; }

    public SlackController(ILogger<SlackController> logger, IConfiguration config)
    {
        Logger = logger;
        Config = config;
    }

    [HttpPost]
    public Task<IActionResult> Post([FromForm] string? text, [FromForm(Name = "channel_id")] string? channelId)
    {
        // 명령어 해석 절차
        var (dayType, mealType) = ParseCommandParameter(text);
        Logger.LogInformation($"channelId: {channelId}");

        Task.Run(async () =>
        {
            try
            {
                var result = GetMenuString(dayType, mealType);

                var slack = new SlackTaskClient(Config["Slack:ApiToken"]);
                var response = await slack.PostMessageAsync(channelId, result);
                if (!response.ok)
                {
                    Logger.LogError(response.error);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        });

        return Task.FromResult<IActionResult>(Ok(new
        {
            response_type = "in_channel",
            text = "잠시만 기다려주세요..."
        }));
    }

    private string GetMenuString(DayType dayType, MealType? mealType)
    {
        var now = CafeteriaParser.GetDateTimeSeoulNow();
        mealType ??= now.Hour < 14 ? MealType.Lunch : MealType.Dinner;

        switch (dayType)
        {
            case DayType.Sunday:
            case DayType.Saturday:
            case DayType.Today when now.DayOfWeek is DayOfWeek.Sunday or DayOfWeek.Saturday:
                return mealType == MealType.Breakfast
                    ? "아침은 제공되지 않습니다."
                    : $"{StringFrom(dayType)}은 {(mealType == MealType.Lunch ? "점심" : "저녁")}이 제공되지 않습니다.";
        }

        string resultText;
        switch (mealType)
        {
            case MealType.Breakfast:
                resultText = "아침은 제공되지 않습니다.";
                break;
            case MealType.Lunch:
                resultText = dayType == DayType.Today
                    ? CafeteriaParser.GetLunchMenuToday()
                    : CafeteriaParser.GetLunchMenu(DayOfWeekFrom(dayType));
                resultText = string.IsNullOrWhiteSpace(resultText)
                    ? $"{StringFrom(dayType)}은 점심이 제공되지 않습니다."
                    : $"{StringFrom(dayType)}의 점심은 아래와 같습니다.\n" +
                      resultText;
                break;
            case MealType.Dinner:
                resultText = dayType == DayType.Today
                    ? CafeteriaParser.GetDinnerMenuToday()
                    : CafeteriaParser.GetDinnerMenu(DayOfWeekFrom(dayType));
                resultText = string.IsNullOrWhiteSpace(resultText)
                    ? $"{StringFrom(dayType)}은 저녁가 제공되지 않습니다."
                    : $"{StringFrom(dayType)}의 저녁은 아래와 같습니다.\n" +
                      resultText;
                break;
            default:
                resultText = "";
                break;
        }

        return resultText;
    }

    private (DayType dayType, MealType? mealType) ParseCommandParameter(string? rawParameter)
    {
        if (rawParameter == null)
        {
            return (DayType.Today, null);
        }

        var parameterList = rawParameter.Split(" ");
        DayType? dayType = null;
        MealType? mealType = null;
        foreach (var parameter in parameterList)
        {
            switch (parameter)
            {
                case "일요일":
                    dayType ??= DayType.Sunday;
                    break;
                case "월요일":
                    dayType ??= DayType.Monday;
                    break;
                case "화요일":
                    dayType ??= DayType.Tuesday;
                    break;
                case "수요일":
                    dayType ??= DayType.Wednesday;
                    break;
                case "목요일":
                    dayType ??= DayType.Thursday;
                    break;
                case "금요일":
                    dayType ??= DayType.Friday;
                    break;
                case "토요일":
                    dayType ??= DayType.Saturday;
                    break;
                case "오늘":
                    dayType ??= DayType.Today;
                    break;
                case "아침":
                    mealType ??= MealType.Breakfast;
                    break;
                case "점심":
                    mealType ??= MealType.Lunch;
                    break;
                case "저녁":
                    mealType ??= MealType.Dinner;
                    break;
            }
        }

        return (dayType ?? DayType.Today, mealType);
    }

    private string StringFrom(DayType dayType)
    {
        return dayType switch
        {
            DayType.Today => "오늘",
            DayType.Sunday => "일요일",
            DayType.Monday => "월요일",
            DayType.Tuesday => "화요일",
            DayType.Wednesday => "수요일",
            DayType.Thursday => "목요일",
            DayType.Friday => "금요일",
            DayType.Saturday => "토요일",
            _ => ""
        };
    }

    private DayOfWeek DayOfWeekFrom(DayType dayType)
    {
        return (DayOfWeek)(dayType - 1);
    }

    private enum DayType
    {
        Today = 0,
        Sunday = 1,
        Monday = 2,
        Tuesday = 3,
        Wednesday = 4,
        Thursday = 5,
        Friday = 6,
        Saturday = 7
    }

    private enum MealType
    {
        Breakfast = 0,
        Lunch = 1,
        Dinner = 2
    }
}