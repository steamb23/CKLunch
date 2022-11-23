using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace CKLunch;

/// <summary>
/// 식당 메뉴 파서
/// </summary>
public static class CafeteriaParser
{
    /// <summary>
    /// 점심 메뉴를 가져옵니다.
    /// </summary>
    /// <param name="dayOfWeek"></param>
    /// <returns></returns>
    public static string GetLunchMenu(DayOfWeek dayOfWeek)
    {
        return ParseMenu(DayOfWeekToRow(dayOfWeek), 1);
    }

    /// <summary>
    /// 오늘의 점심 메뉴를 가져옵니다.
    /// </summary>
    /// <returns></returns>
    public static string GetLunchMenuToday()
    {
        return ParseMenuToday(1);
    }

    /// <summary>
    /// 저녁 메뉴를 가져옵니다.
    /// </summary>
    /// <param name="dayOfWeek"></param>
    /// <returns></returns>
    public static string GetDinnerMenu(DayOfWeek dayOfWeek)
    {
        return ParseMenu(DayOfWeekToRow(dayOfWeek), 2);
    }

    /// <summary>
    /// 오늘의 저녁 메뉴를 가져옵니다.
    /// </summary>
    /// <returns>오늘의 저녁 메뉴</returns>
    public static string GetDinnerMenuToday()
    {
        return ParseMenuToday(2);
    }

    /// <summary>
    /// 현재 서울 시각을 가져옵니다.
    /// </summary>
    /// <returns>현재 서울 시각</returns>
    public static DateTime GetDateTimeSeoulNow()
    {
        var currentDateTimeUtc = DateTime.UtcNow;
        return TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Asia/Seoul"));
    }

    private static int DayOfWeekToRow(DayOfWeek dayOfWeek)
    {
        return (int)dayOfWeek;
    }

    private static int GetTodayRow()
    {
        var dateTime = GetDateTimeSeoulNow();
        return DayOfWeekToRow(dateTime.DayOfWeek);
    }

    private static string ParseMenuToday(int row)
    {
        var column = GetTodayRow();

        return ParseMenu(column, row);
    }

    private static string ParseMenu(int column, int row)
    {
        row += 1;
        
        // if layoutchanged
        (column, row) = (row, column);
        
        var option = new ChromeOptions();
        option.AddArguments("--headless", "--no-sandbox", "--disable-dev-shm-usage");
        using IWebDriver driver = new ChromeDriver(option)
        {
            Url = "https://www.ck.ac.kr/univ-life/menu"
        };

        var td = driver.FindElement(
            By.XPath($"/html/body/div[1]/div[3]/div[2]/div/div/table/tbody[2]/tr[{row}]/td[{column}]"));

        return td.Text;
    }
}