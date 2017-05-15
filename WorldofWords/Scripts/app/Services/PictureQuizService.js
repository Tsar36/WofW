app.service('PictureQuizService', ["$http", "$q", "HttpRequest", function ($http, $q, HttpRequest) {
    this.isPictureQuizAllowed = function (id) {
        var deferred = $q.defer();
        HttpRequest.get("api/TrainingWordSuite/DoesContainAtLeastOnePicture?wordSuiteId=" + id, deferred);
        return deferred.promise;
    }
}]);