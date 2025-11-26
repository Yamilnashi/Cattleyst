namespace CattleystWebApi.DTO
{
    public record LocationUpdateRequest
    {
        public string LocationName { get; set; } = string.Empty;
    }
}
