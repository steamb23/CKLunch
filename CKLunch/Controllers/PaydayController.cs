using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace CKLunch.Controllers;

[ApiController]
[Route("payday")]
public class PaydayController : ControllerBase
{
    private IList<string> phrases = new[]
    {
        "돈이란 바닷물과 같다. 마시면 마실 수록 목이 말라진다. -쇼펜하우어",
        "돈이란 힘이고 자유이며, 모든 악의 근원이기도 한 동시에, 한편으로는 최대의 행복이 되기도 한다. -칼 샌드버그",
        "두툼한 지갑이 무조건 좋다고 말할 수는 없다. -탈무드 격언",
        "때 묻은 돈도 돈이다. 돈은 돈의 가치를 결정하는 것이다. -유럽 속담",
        "돈이 없으면 방랑자, 돈이 있으면 관광객이라 불린다. -폴 리처",
        "돈과 쾌락과 명예를 사랑하는 사람은 남을 사랑하지 못한다. -에픽테토스",
        "돈에 관한 욕심은 모든 악의 근원으로 여겨지고 있다. 그러나 돈이 없는 것도 이 점에서는 똑같다. -버틀러",
        "돈은 무자비한 주인이지만, 유익한 종이 되기도 한다. -유태 격언",
        "돈은 바닥이 없는 바다와도 같은 것이다. 양심도 명예도 거기에 빠져서 결코 떠오르지 않는다. -벤자민 프랭클린",
        "돈은 비료와 같은 것으로 뿌리지 않으면 쓸모가 없다. -프랜시스 베이컨",
        "돈은 빌려주기 좋아하는 사람은 그냥 주는 사람이다. -조지 허버트",
        "나는 왕이 되어 내 돈을 거지처럼 쓰기보다는, 차라리 거지가 내 마지막 1달러를 왕처럼 쓰겠다. -잉거솔",
        "내 주머니의 푼돈이 남의 주머니에 있는 거금보다 낫다. -세르반테스",
        "돈이란 누구에게도 무한한 것이 아니다. -손자병법",
        "돈을 빌려 달라는 것을 거절함으로써 친구를 잃는 일은 적지만 반대로 돈을 빌려줌으로써 친구를 잃기는 매우 쉽다. -쇼펜하우어"
    };

    [HttpPost]
    public Task<IActionResult> Post([FromForm] string? text, [FromForm(Name = "channel_id")] string? channelId)
    {
        var stringBuilder = new StringBuilder();
        var now = DateTime.Now;
        var lastDay = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));
        var dDayTimeSpan = lastDay - now;
        var dDay = dDayTimeSpan.Days;
        // 올림 표기
        if (dDayTimeSpan.Hours > 0 || dDayTimeSpan.Minutes > 0 || dDayTimeSpan.Seconds > 0)
            dDay += 1;

        switch (dDay)
        {
            case > 1:
                stringBuilder.AppendLine($"{now} 기준 월급날까지 약 {dDay} 일 남았습니다.");
                break;
            case 1:
                stringBuilder.AppendLine($"{now} 기준 월급날까지 약 하루 남았습니다.");
                break;
            case 0:
                stringBuilder.AppendLine("오늘은 월급날입니다!");
                break;
            default:
                stringBuilder.AppendLine("뭔가 문제가 있습니다...");
                break;
        }

        stringBuilder.Append(phrases[Random.Shared.Next(phrases.Count)]);

        return Task.FromResult<IActionResult>(Ok(new
        {
            response_type = "in_channel",
            text = stringBuilder.ToString()
        }));
    }
}