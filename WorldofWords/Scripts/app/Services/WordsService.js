/// <reference path="WordsService.js" />
app.service('WordsService', ["$http","$q","HttpRequest", function ($http, $q, HttpRequest) {

    this.getWordsByLanguageId = function (id) {
        var deferred = $q.defer();
        HttpRequest.get('/api/words?languageId=' + id, deferred);
        return deferred.promise;
    }

    this.deleteWord = function (wordId) {
        var deferred = $q.defer();
        HttpRequest.delete("api/Words/DeleteWord?wordId=" + wordId, deferred);
        return deferred.promise;
    }

    this.addWord = function (word) {
        var deferred = $q.defer();
        HttpRequest.post('/api/Words', word, deferred);
        return deferred.promise;
    }

   this.synonymsModel = function (synonymsModel) {
        var deferred = $q.defer();
        HttpRequest.post('/api/synonymsmodel', synonymsModel, deferred);
        return deferred.promise;
    }

    this.getWordById = function (id) {
        var deferred = $q.defer();
        HttpRequest.get('/api/Words?wordId=' + id, deferred);
        return deferred.promise;
    }

    this.getwordSynonyms = function (id) {
        var deferred = $q.defer();
        HttpRequest.get('/api/Words?WordId + id', deferred);
        return deferred.promise;
    }

    this.searchForTags = function (searchTag, searchResultsCount) {
        var deferred = $q.defer();
        HttpRequest.get('api/Words?searchTag=' + searchTag + '&searchResultsCount=' + searchResultsCount, deferred);
        return deferred.promise;
    }

    this.searchForWords = function (searchWord, languageId, searchResultsCount) {
        var deferred = $q.defer();
        HttpRequest.get('api/Words?searchWord='+searchWord+'&languageId='+languageId+'&searchResultsCount='+searchResultsCount, deferred);
        return deferred.promise;
    }

    this.searchForTranslations = function (searchWord, translationLanguageId, count) {
        var deferred = $q.defer();
        HttpRequest.get('api/Words?searchWord=' + searchWord + '&languageId=' + translationLanguageId + '&searchResultsCount=' + count, deferred);
        return deferred.promise;
    }

    this.editWord = function (word) {
        var deferred = $q.defer();
        HttpRequest.post("/api/Words/EditWord", word, deferred);
        return deferred.promise;
    }

    
}]);