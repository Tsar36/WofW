app.service('WordTranslationService', ["$http", "$q", "HttpRequest", function ($http, $q, HttpRequest) {
    this.searchForWordTranslations = function (searchWord, tags, languageId, searchByTag) {
        var deferred = $q.defer();
        var model = {
            SearchWord: searchWord,
            Tags: tags,
            LanguageId: languageId,
            SearchByTag: searchByTag
        };
        HttpRequest.post('api/WordTranslation/SearchWordTranslations', model, deferred);
        return deferred.promise;
    }

    this.getWordTranslationsByWordSuiteID = function (ID) {
        var deferred = $q.defer();
        HttpRequest.get('api/WordTranslation?ID=' + ID, deferred);
        return deferred.promise;
    }

    this.importWordTranslations = function (wordTranslations) {
        var deferred = $q.defer();
        HttpRequest.post('api/WordTranslation/ImportWordTranslations', wordTranslations, deferred);
        return deferred.promise;
    }

    this.addWordTranslation = function (wordtranslation) {
        var deferred = $q.defer();
        HttpRequest.post('/api/WordTranslation/CreateWordTranslation', wordtranslation, deferred);
        return deferred.promise;
    }

    this.getWordsFromInterval = function (start, end, originalLanguageId, translationLanguageId , partsOfSpeech) {
        var deferred = $q.defer();
        var url = '/api/GlobalDictionary?start=' + start + '&end=' + end +
            '&originalLangId=' + originalLanguageId + '&translationLangId=' + translationLanguageId;
        url += convertPartsOfSpeechToUrl(partsOfSpeech);
        HttpRequest.get(url, deferred);
        return deferred.promise;
    }

    this.getAmountOfWordsBySpecificLanguage = function (lang, translationLanguageId, partsOfSpeech) {
        var deferred = $q.defer();
        var url = '/api/GlobalDictionary?originalLangId=' + lang + '&translationLangId=' + translationLanguageId;
        url += convertPartsOfSpeechToUrl(partsOfSpeech);
        HttpRequest.get(url, deferred);
        return deferred.promise;
    }

    this.getAmountOfWordsBySearchValue = function (searchValue, lang, translationLanguageId, partsOfSpeech) {
        var deferred = $q.defer();
        var url = '/api/GlobalDictionary?searchValue=' + searchValue + '&originalLangId=' + lang + '&translationLangId=' + translationLanguageId;
        url += convertPartsOfSpeechToUrl(partsOfSpeech);
        HttpRequest.get(url, deferred);
        return deferred.promise;
    }

    this.getWordsBySearchValueFromInterval = function(start, end, lang, searchValue, translationLanguageId, partsOfSpeech)
    {
        var deferred = $q.defer();
        var url = '/api/GlobalDictionary?searchValue=' + searchValue + '&startOfInterval=' + start + '&endOfInterval=' + end +
            '&originalLangId=' + lang + '&translationLangId=' + translationLanguageId;
        url += convertPartsOfSpeechToUrl(partsOfSpeech);
        HttpRequest.get(url, deferred);
        return deferred.promise;
    }

    this.editWord = function (word) {
        var deferred = $q.defer();
        HttpRequest.post("api/WordTranslation/EditWord", word, deferred);
        return deferred.promise;
    }

   
    this.getInformationAboutWord = function (word, originLanguageId, translationLanguageId) {
        var deferred = $q.defer();
        HttpRequest.get('api/GlobalDictionary/FullWord?word=' + word + '&originalLangId=' + originLanguageId + '&translationLangId=' + translationLanguageId,
            deferred);
        return deferred.promise;
    }

    this.getInformationAboutWordStrings = function (word, originLanguageId, translationLanguageId) {
        var deferred = $q.defer();
        HttpRequest.get('api/GlobalDictionary/FullWordStrings?wordValue=' + word + '&originalLangId=' + originLanguageId + '&translationLangId=' + translationLanguageId,
            deferred);
        return deferred.promise;
    }
    var convertPartsOfSpeechToUrl = function (partsOfSpeech) {
        var url = '';
        partsOfSpeech.forEach(function (item, index, arr) {
            url += ('&partsOfSpeechId=' + item.Id);
        });
        return url;
    }
}]);