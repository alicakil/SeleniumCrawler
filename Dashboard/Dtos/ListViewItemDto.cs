namespace core.Dtos
{
    public record ListViewItemDto
    {
        public int Id { get; init; }
        public string Title { get; init; }
        public string CoverPhotoURL { get; init; }
        public string ImgZoom { get; init; }
        public double AspectRatio { get; init; }
        public string LocationInfo { get; init; }
        public string CreatedAt { get; init; }
        public string Keywords { get; init; }
        public string PriceInfo { get; init; }
        public string isRegistered { get; init; }
    }
}
