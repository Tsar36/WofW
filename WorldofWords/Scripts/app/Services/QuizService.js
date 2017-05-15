app.service('QuizService', ["$http","$q","HttpRequest", function ($http, $q, HttpRequest) {
    this.getWords = function (id) {
        var deferred = $q.defer();
        HttpRequest.get("api/TrainingWordSuite/Task?id=" + id, deferred);
        return deferred.promise;
    }

    this.sendResult = function (data) {
        var deferred = $q.defer();
        HttpRequest.post('/api/TrainingWordSuite/CheckTask', data, deferred);
        return deferred.promise;
    }

    this.getSynonymTask = function (id) {
        var deferred = $q.defer();
        HttpRequest.get("/api/TrainingWordSuite/SynonymTask?id=" + id, deferred);
        return deferred.promise;
    }

    this.checkSynonymTask = function (data) {
        var deferred = $q.defer();
        HttpRequest.post('/api/TrainingWordSuite/CheckSynonymTask', data, deferred);
        return deferred.promise;
    }

    this.getMixTask = function (id) {
        var deferred = $q.defer();
        HttpRequest.get("/api/TrainingWordSuite/MixTask?id=" + id, deferred);
        return deferred.promise;
    }

    this.checkMixTask = function (data) {
        var deferred = $q.defer();
        HttpRequest.post('/api/TrainingWordSuite/CheckMixTask', data, deferred);
        return deferred.promise;
    }

    this.getDescriptionTask = function (id) {
        var deferred = $q.defer();
        HttpRequest.get("/api/TrainingWordSuite/DescriptionTask?id=" + id, deferred);
        return deferred.promise;
    }
}]);