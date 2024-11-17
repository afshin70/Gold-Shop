using Gold.Domain.Entities.Base;

namespace Gold.Domain.Entities
{
    public class SocialNetwork : IEntity<int>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string ImageName { get; set; }

    }

}
