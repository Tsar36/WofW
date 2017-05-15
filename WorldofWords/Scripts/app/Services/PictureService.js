app.service('PictureService', ["$http", "$q", "HttpRequest", function ($http, $q, HttpRequest) {

    this.getPictureByWordId = function (wordId) {
        var deferred = $q.defer();
        HttpRequest.get('/api/Picture?wordId=' + wordId, deferred);
        return deferred.promise;
    }

    this.deletePicture = function (wordId) {
        var deferred = $q.defer();
        HttpRequest.delete('/api/Picture?wordId=' + wordId, deferred);
        return deferred.promise;
    }

    this.addPicture = function (picture) {
        var deferred = $q.defer();
        HttpRequest.post('/api/Picture', picture, deferred);
        return deferred.promise;
    }
}]);