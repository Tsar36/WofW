app.service('LanguageService', ["$http", "$q", "HttpRequest", function ($http, $q, HttpRequest) {
    this.getAllLanguages = function () {
        var deferred = $q.defer();
        HttpRequest.get('/api/language', deferred);
        return deferred.promise;
    }

    this.getLanguagesList = function () {
        var deferred = $q.defer();
        HttpRequest.get('/api/language/worldLanguages', deferred);
        return deferred.promise;
    }

    this.addLanguage = function (language) {
        var deferred = $q.defer();
        HttpRequest.post('/api/language', language, deferred);
        return deferred.promise;
    }

    this.removeLanguage = function (id) {
        var deferred = $q.defer();
        HttpRequest.delete('/api/language?id=' + id, deferred);
        return deferred.promise;
    }

    this.getLanguage = function (id) {
        var deferred = $q.defer();
        HttpRequest.get('/api/language?id=' + id, deferred);
        return deferred.promise;
    }

    this.getPartsOfSpeech = function (languageId) {
        var deferred = $q.defer();
        HttpRequest.get('api/Language/partsOfSpeech?id=' + languageId, deferred);
        return deferred.promise;
    }

    this.mapPartsOfSpeechForGlobalDictionary = function (partsOfSpeechOrigin) {
        if(angular.isArray(partsOfSpeechOrigin)){
            var partsOfSpeechNew = [];
            partsOfSpeechNew.push({
                Id : -1,
                Name: 'without'
            });
            
            partsOfSpeechOrigin.forEach(function(item, i, partsOfSpeechOrigin){
                partsOfSpeechNew.push(angular.copy(item));
            });

            partsOfSpeechNew.forEach(function (item, i, partsOfSpeechOrigin) {
                item.selected = true;
            });

            return partsOfSpeechNew;
        }
    }

    this.getIdsFromPartsOfSpeech = function (selectedPartsOfSpeech) {
        var ids = [];
        selectedPartsOfSpeech.forEach(function (item, i, arr) {
            ids.push(item.Id);
        });

        return ids;
    }
}]);