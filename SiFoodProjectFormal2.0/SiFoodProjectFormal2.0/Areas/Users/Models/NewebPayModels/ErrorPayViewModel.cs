namespace SiFoodProjectFormal2._0.Areas.Users.Models.NewebPayModels
{
    public class ErrorPayViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
