app.service('RecordsService', ['$q','HttpRequest', function ($q, HttpRequest) {
    this.getRecordById = function (id) {
        var deferred = $q.defer();
        HttpRequest.get('/api/Record?recordId=' + id, deferred);
        return deferred.promise;
    };

    this.getRecordByWordId = function (id) {
        var deferred = $q.defer();
        HttpRequest.get('/api/Record?WordId=' + id, deferred);
        return deferred.promise;
    };

    this.isThereRecord = function (id) {
        var deferred = $q.defer();
        HttpRequest.get('/api/Record/IsThereRecord/' + id, deferred);
        return deferred.promise;
    };

    this.addRecord = function (record) {
        var deferred = $q.defer();
        HttpRequest.post('/api/Record', record, deferred);
        return deferred.promise;
    };

    this.deleteRecord = function (wordId) {
        var deferred = $q.defer();
        HttpRequest.delete('/api/Record?wordId=' + wordId, deferred);
        return deferred.promise;
    };
}]);