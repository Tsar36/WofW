using WorldOfWords.API.Models;
using WorldOfWords.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace WorldOfWords.API.Models
{
    public class TagMapper : ITagMapper
    {
        public Tag Map(TagModel tag)
        {
            return new Tag()
            {   
                Id = tag.Id ?? default(int),
                Name = tag.Value
            };
        }

        public TagModel Map(Tag tag)
        {
            return new TagModel()
            {
                Id = tag.Id,
                Value = tag.Name
            };
        }

        public List<Tag> MapRange(List<TagModel> tags)
        {
            return tags.Select(t => Map(t)).ToList();
        }
    }
}
