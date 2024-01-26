namespace blog_api.Models
{
    public class PagesDto
    {
        // initializing ensures that the property is always ready for use,
        // preventing null reference issues and providing a convient starting point for adding elements;
        public List<PageDto> Pages { get; set; } = new List<PageDto>();
    }
}
