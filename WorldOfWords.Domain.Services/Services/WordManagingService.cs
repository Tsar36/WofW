using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;
using WorldOfWords.Domain.Models;
using WorldOfWords.Infrastructure.Data.EF.Factory;
using WorldOfWords.API.Models;
using System.Threading.Tasks;
using System.Data.Entity.Migrations;
using WorldOfWords.Infrastructure.Data.EF;

namespace WorldOfWords.Domain.Services
{
    public class WordManagingService : IWordManagingService
    {
        private readonly IWordService wordService;
        private readonly IWordTranslationService wordTranslationService;
        private readonly ITagService tagService;

        public WordManagingService(IWordService wordService, IWordTranslationService wordTranslationService,
                                   ITagService tagService)
        {
            this.wordService = wordService;
            this.wordTranslationService = wordTranslationService;
            this.tagService = tagService;
        }

        public async Task<bool> EditWord(WordTranslationEditModel model)
        {
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool updateBase = await wordService.UpdateAsync(new WordModel
                {
                    Id = model.Id,
                    Description = model.Description,
                    Transcription = model.Transcription,
                    PartOfSpeechId = model.PartOfSpeechId,
                    Comment = model.Comment
                });

                bool translationsRemoved = true;
                if (model.TranslationsToDeleteIdRange != null && model.TranslationsToDeleteIdRange.Count > 0)
                {
                    translationsRemoved = await wordTranslationService.RemoveIdRangeAsync(model.Id, model.TranslationsToDeleteIdRange);
                }

                if (model.TranslationsToAddRange != null && model.TranslationsToAddRange.Count > 0)
                {
                    List<int> translationIds = await CreateNonExistingWords(model.TranslationsToAddRange, model.TranslationLanguageId);
                    await wordTranslationService.AddIdRangeAsync(model.Id, translationIds, model.OwnerId);
                }

                if (model.SynonymsToAddIdRange != null && model.SynonymsToAddIdRange.Count > 0)
                {
                    List<int> synonymIds = await CreateNonExistingWords(model.SynonymsToAddIdRange, model.OriginalLanguageId);
                    await wordTranslationService.AddIdRangeAsync(model.Id, synonymIds, model.OwnerId);
                }

                bool synonymsRemoved = true;
                if (model.SynonymsToDeleteIdRange != null && model.SynonymsToDeleteIdRange.Count > 0)
                {
                    synonymsRemoved = await wordTranslationService.RemoveIdRangeAsync(model.Id, model.SynonymsToDeleteIdRange);
                }

                if (model.TagsToAddRange != null && model.TagsToAddRange.Count > 0)
                {
                    List<int> tagIds = await CreateNonExistingTags(model.TagsToAddRange);
                    await tagService.AddRangeToWordAsync(model.Id, tagIds);
                }

                bool tagsRemoved = true;
                if (model.TagsToDeleteIdRange != null && model.TagsToDeleteIdRange.Count > 0)
                {
                    tagsRemoved = await tagService.RemoveIdRangeAsync(model.Id, model.TagsToDeleteIdRange);
                }

                if (updateBase && translationsRemoved && synonymsRemoved && tagsRemoved)
                {
                    transaction.Complete();
                    return true;
                }
                return false;
            }
        }

        public async Task<int> AddWord(WordModel model)
        {
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                int origWordId = await wordService.AddAsync(model);
                
                if (model.Translations != null && model.Translations.Count > 0)
                {
                    List<int> translationIds = await CreateNonExistingWords(model.Translations, model.TranslationLanguageId);
                    await wordTranslationService.AddIdRangeAsync(origWordId, translationIds, model.OwnerId);
                }

                if (model.Synonyms != null && model.Synonyms.Count > 0)
                {
                    List<int> synonymIds = await CreateNonExistingWords(model.Synonyms, model.LanguageId);
                    await wordTranslationService.AddIdRangeAsync(origWordId, synonymIds, model.OwnerId);
                }

                if (model.Tags != null && model.Tags.Count > 0)
                {
                    List<int> tagIds = await CreateNonExistingTags(model.Tags);
                    await tagService.AddRangeToWordAsync(origWordId, tagIds);
                }

                transaction.Complete();
                return origWordId;
            }

        }

        private async Task<List<int>> CreateNonExistingWords(List<WordValueModel> words, int languageId)
        {
            List<int> translationIds = new List<int>();
            foreach (var translation in words)
            {
                if (translation.Id == null)
                {
                    WordModel wordToAdd = new WordModel
                    {
                        Value = translation.Value,
                        LanguageId = languageId
                    };
                    translation.Id = await wordService.AddAsync(wordToAdd);
                }
                translationIds.Add(translation.Id ?? default(int));
            }
            return translationIds;
        }

        private async Task<List<int>> CreateNonExistingTags(List<TagModel> tags)
        {
            List<int> tagIds = new List<int>();
            foreach (var tag in tags)
            {
                if (tag.Id == null)
                {
                    TagModel wordToAdd = new TagModel
                    {
                        Value = tag.Value
                    };
                    tag.Id = await tagService.AddAsync(wordToAdd);
                    
                    if(tag.Id == -1)
                    {
                        Tag tagToCheck = await tagService.GetFirstTagByNameAsync(tag.Value);
                        tag.Id = tagToCheck.Id;
                    }
                }
                tagIds.Add(tag.Id ?? default(int));
            }
            return tagIds;
        }
    }
}
