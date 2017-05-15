app.service('CourseService', ["$q", "HttpRequest", "ConstService", function ($q, HttpRequest, ConstService) {
    this.getEnrollCourses = function () {
        var deferred = $q.defer();
        HttpRequest.get('/api/Course/StudentCourses', deferred);
        return deferred.promise;
    };

    this.getUserCourses = function () {
        var deferred = $q.defer();
        HttpRequest.get('/api/course/TeacherCourses', deferred);
        return deferred.promise;
    };

    this.getCourse = function (id) {
        var deferred = $q.defer();
        HttpRequest.get('/api/course/' + id, deferred);
        return deferred.promise;
    };

    this.getCourseDetailWithUserId = function (courseId, userId) {
        var deferred = $q.defer();
        HttpRequest.get('/api/course/progress?courseId=' + courseId + '&userId=' + userId, deferred);
        return deferred.promise;
    }

    this.createCourse = function (course) {
        var deferred = $q.defer();
        HttpRequest.post("api/course/createcourse", course, deferred);
        return deferred.promise;
    }

    this.removeCourse = function (courseId) {
        var deferred = $q.defer();
        HttpRequest.delete('/api/course?courseId='+ courseId, deferred);
        return deferred.promise;
    };

    this.editCourse = function (course) {
        var deferred = $q.defer();
        HttpRequest.post("api/course/editcourse", course, deferred);
        return deferred.promise;
    }

    this.getDisabledQuizzes = function (wordSuites) {
        var flags = [];
        wordSuites.forEach(function (item, wordSuiteIndex, wordSuites) {
            flags[wordSuiteIndex] = [];
            item.ProhibitedQuizzesId.forEach(function (quizId, quizIndex, quizzes) {
                flags[wordSuiteIndex][quizId - 1] = true;
            });
            for (var i = 0; i < ConstService.numberOfQuizzes; ++i) {
                if(flags[wordSuiteIndex][i] === undefined){
                    flags[wordSuiteIndex][i] = false;
                }
            }

        });

        return flags;
    }

}]);