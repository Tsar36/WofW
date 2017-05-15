app.service('TagService', ["$http","$q","HttpRequest", function ($http, $q, HttpRequest){

this.getAmountOfTagsBySearchValue = function (searchValue, lang, translationLanguageId) {
       var deferred = $q.defer();
       HttpRequest.get('/api/GlobalDictionary/GetAmountByTags?searchValue=' + searchValue + '&originalLangId=' + lang + '&translationLangId=' + translationLanguageId, deferred);
       return deferred.promise;
}

             
this.getWordsByTagValue = function (start, end, lang, searchValue, translationLanguageId) {
         var deferred = $q.defer();
         HttpRequest.get('/api/GlobalDictionary/GetWordsByTag?startOfInterval=' + start + '&endOfInterval=' + end +
         '&originalLangId=' + lang + '&searchValue=' + searchValue + '&translationLangId=' + translationLanguageId,
         deferred);
         return deferred.promise;
        }


}]);