namespace core.Dtos
{
    public class ListViewDto
    {
        public int NrOfRecs { get; set; }    
        public List<ListViewItemDto> PageData { get; set; } = new List<ListViewItemDto>();
    }
}
