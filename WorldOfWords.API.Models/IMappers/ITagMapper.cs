using WorldOfWords.API.Models;
using WorldOfWords.Domain.Models;
using System.Collections.Generic;

namespace WorldOfWords.API.Models
{
    public interface ITagMapper
    {
        Tag Map(TagModel tag);
        TagModel Map(Tag tag);
        List<Tag> MapRange(List<TagModel> tags);
    }
}
