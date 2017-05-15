app.controller('QuizDescController', ["$scope", "$window", "$routeParams", "$modal", "QuizService",
    function ($scope, $window, $routeParams, $modal, QuizService) {
        $scope.$on('$locationChangeStart', function (event) {
            if (!$scope.isTimeOut) {
                event.preventDefault();
                $scope.ConfirmSubmit();
            }
        });

        $scope.open = function (url, successCallback, data, errorCallback) {
            var modalInstance = $modal.open({
                animation: true,
                templateUrl: url,
                controller: 'ModalController',
                size: 'sm',
                resolve: {
                    data: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(successCallback, errorCallback);
        };

        $scope.Stop = function () {
            clearInterval($scope.timerInterval);
            timer.stop();
        };

        $scope.Start = function () {
            $scope.itemsPerPage = 10;
            $scope.$watch('currentPage + itemsPerPage', function () {
                sliceWords();
            });
            $scope.$on('ngRepeatFinished', function (ngRepeatFinishedEvent) {
                var h = document.getElementById("parentQuiz").offsetHeight;
                document.getElementById("left-arrow").style.height = h + "px";
                document.getElementById("right-arrow").style.height = h + "px";
            });
            timer.start();
            $scope.isTimeOut = false;
            $scope.timerInterval = setInterval(showTime, 100);
        };

        $scope.goToCourses = function () {
            $window.location.href = 'Index#/CourseDetail/' + JSON.parse(localStorage.getItem('courseId'));
        };

        $scope.getCurrentIndex = function (index) {
            return ($scope.currentPage - 1) * $scope.itemsPerPage + index;
        };

        $scope.ConfirmSubmit = function () {
            $scope.open('Confirmation', $scope.Submit);
        };

        $scope.Submit = function () {
            if ($scope.isTimeOut) {
                $scope.message += ' Time is out!';
            }
            $scope.isTimeOut = true;
            $scope.Stop();
            var data = {
                Id: $routeParams.wordSuiteId,
                WordTranslations: $scope.typedWords
            };
            $scope.isChecked = true;
            QuizService.sendResult(data)
                    .then(function (response) {
                        $scope.answer = response.WordTranslations;
                        sliceWords();
                        $scope.rightAnswerCount = 0;
                        for (var x in $scope.answer) {
                            if ($scope.answer[x].Result) {
                                ++$scope.rightAnswerCount;
                            };
                        };
                        $scope.isSubmitSuccessful = true;
                        $scope.open('Result', function () { }, ($scope.message + ' Result: ' + $scope.rightAnswerCount + '/' + $scope.totalItems));
                    },
                    function (error) {
                        $window.open('An error has occured', function () { }, (error.ExceptionMessage || error.Message || "Unknown error"));
                    });
        };

        var MilisecondsInSecond = 1000;
        var timer = new Timer();

        var showTime = function () {
            if (timer.getMiliseconds() > timer.getGoalTime() && timer.getGoalTime() !== 0) {
                $scope.isTimeOut = true;
                $scope.Submit();
            }
            else {
                $scope.time = timer.getGoalTime() - timer.getMiliseconds();
            }
            $scope.$apply();
        };

        var sliceWords = function () {
            var begin = (($scope.currentPage - 1) * $scope.itemsPerPage),
                end = begin + $scope.itemsPerPage;
            $scope.filteredWords = $scope.words.slice(begin, end);
        };

        var initialize = function () {
            $scope.isTimeOut = true;
            $scope.typedWords = [];
            $scope.isChecked = false;
            $scope.words = [];
            $scope.currentPage = 1;
            $scope.message = 'Your result are saved!';
            QuizService.getDescriptionTask($routeParams.wordSuiteId)
                .then(function (response) {
                    $scope.label = response.Name;
                    $scope.words = response.WordTranslations;
                    if (!$scope.words.length) {
                        $scope.open('errorModal', $scope.goToCourses, 'Sorry, you learned all words');
                    } else {
                        $scope.totalItems = $scope.words.length;
                        for (i = 0; i < $scope.totalItems; ++i) {
                            $scope.typedWords[i] = {
                                Id: $scope.words[i].Id,
                                TranslationWord: ''
                            }
                        }
                        $scope.time = response.QuizResponseTime * $scope.totalItems * MilisecondsInSecond;
                        timer.setGoalTime($scope.time);
                        $scope.open('startQuiz', $scope.Start, $scope.time, $scope.Start);
                    };
                },
                    function (error) {
                        $scope.open('errorModal', $scope.goToCourses, (error.ExceptionMessage || error.Message || "Unknown error"));
                    });
        };

        initialize();
    }]);

app.controller('ModalController', ["$scope", "$modalInstance", "data", function ($scope, $modalInstance, data) {
    $scope.data = data;
    $scope.close = function () {
        $modalInstance.close();
    };
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}]);

app.directive('onFinishRender', ["$timeout", function ($timeout) {
    return {
        restrict: 'A',
        link: function (scope, element, attr) {
            if (scope.$last === true) {
                $timeout(function () {
                    scope.$emit('ngRepeatFinished');
                });
            }
        }
    }
}]);