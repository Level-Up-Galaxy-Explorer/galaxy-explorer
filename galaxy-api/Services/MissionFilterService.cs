using galaxy_api.Models;

namespace galaxy_api.Services.Filters;

public static class MissionFilterService
{
    public static IEnumerable<Missions> FilterMissions(
        IEnumerable<Missions> missions,
        Func<Missions, bool> predicate)
    {
        return missions.Where(predicate);
    }

    public static IEnumerable<string> GetTopFeedbacks(
        IEnumerable<Missions> missions,
        int count = 5)
    {
        return missions
            .Where(m => !string.IsNullOrWhiteSpace(m.Feedback))
            .OrderByDescending(m => m.Launch_Date)
            .Take(count)
            .Select(m => m.Feedback);
    }
}
