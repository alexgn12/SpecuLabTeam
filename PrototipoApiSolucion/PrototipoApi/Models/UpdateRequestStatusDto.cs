namespace PrototipoApi.Models
{
    public class UpdateRequestStatusDto
    {
        public string StatusType { get; set; } = string.Empty;
        public string? Comment { get; set; }
        public DateTime? ChangeDate { get; set; }
        public string? ChangeDateFormatted => ChangeDate.HasValue ? Helpers.DateFormatHelper.ToExternalFormat(ChangeDate.Value) : null;
    }
}
