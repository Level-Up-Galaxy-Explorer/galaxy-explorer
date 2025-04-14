namespace galaxy_api.Delegates
{
    public delegate bool FeedbackValidator(string feedback);

    public static class FeedbackValidation
    {
        public static bool IsValidFeedback(string feedback)
        {
            return !string.IsNullOrWhiteSpace(feedback) && feedback.Length >= 10;
        }
    }
}
