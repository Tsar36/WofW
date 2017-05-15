app.controller('PictureQuizController', ["$scope", "QuizService", "PictureService", "PictureQuizService", "$routeParams", "$modal", "$window",
    function ($scope, QuizService, PictureService, PictureQuizService, $routeParams, $modal, $window) {

        $scope.goToCourses = function () {
            $window.location.href = 'Index#/CourseDetail/' + JSON.parse(localStorage.getItem('courseId'));
        };
        $scope.getCurrentIndex = function (index) {
            return ($scope.currentPage - 1) * $scope.itemsPerPage + index;
        };

        $scope.open = function (url, successCallback, data, errorCallback) {
            var modalInstance = $modal.open({
                animation: true,
                templateUrl: url,
                controller: 'ModalStartQuizController',
                size: 'sm',
                resolve: {
                    data: function () {
                        return data;
                    }
                }
            });
            modalInstance.result.then(successCallback, errorCallback);
        };

        $scope.nextPage = function () {
            $scope.currentPage = $scope.currentPage + 1;
            getWordIdsForPage();
            getPicture($scope.wordIds);
        }

        $scope.previousPage = function () {
            $scope.currentPage = $scope.currentPage - 1;
            getWordIdsForPage();
            getPicture($scope.wordIds);
        }

        var getPicture = function (wordIds) {
            PictureService.getPictureByWordId(wordIds[wordIds.length - 1])
                .then(function (response) {
                    $scope.images.push(response.Content);
                    wordIds.pop();
                    if (wordIds.length !== 0) {
                        getPicture(wordIds);
                    } else {
                        $scope.images.reverse();
                    }
                })
        }

        var getWordIdsForPage = function () {
            $scope.wordIds = [];
            $scope.images = [];
            if ($scope.currentPage >= ($scope.totalItems / $scope.itemsPerPage)) {
                var lastPageIds = $scope.words.slice(($scope.currentPage - 1) * $scope.itemsPerPage, $scope.words.length);
                for (i = 0; i < lastPageIds.length; i++) {
                    $scope.wordIds.push(lastPageIds[i].OriginalWordId);
                }
            } else {
                for (i = $scope.itemsPerPage * ($scope.currentPage - 1) ; i < ($scope.currentPage * $scope.itemsPerPage) ; i++) {
                    $scope.wordIds.push($scope.words[i].OriginalWordId);
                }
            }
        }

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

        $scope.Stop = function () {
            clearInterval($scope.timerInterval);
            timer.stop();
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

        $scope.Start = function () {
            $scope.$on('ngRepeatFinished', function (ngRepeatFinishedEvent) {
                var h = document.getElementById("parentQuiz").offsetHeight;
                document.getElementById("left-arrow").style.height = h + "px";
                document.getElementById("right-arrow").style.height = h + "px";
            });
            getWordIdsForPage();
            getPicture($scope.wordIds);
            timer.start();
            $scope.isTimeOut = false;
            $scope.timerInterval = setInterval(showTime, 100);
        }

        var initialize = function () {
            $scope.picture_width = 200;
            $scope.picture_height = 200;
            $scope.isTimeOut = true;
            $scope.isChecked = false;
            $scope.typedWords = [];
            $scope.words = [];
            $scope.currentPage = 1;
            $scope.itemsPerPage = 3;
            PictureQuizService.isPictureQuizAllowed($routeParams.wordSuiteId)
                .then(function (isAllowed) {
                    if (isAllowed) {
                        QuizService.getWords($routeParams.wordSuiteId)
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
                                $scope.open('startQuiz', $scope.Start, $scope.time, $scope.goToCourses);
                                        }
                                },
                            function (error) {
                                $scope.open('errorModal', $scope.goToCourses, (error.ExceptionMessage || error.Message || "Unknown error"));
                            });
                    } else {
                        $scope.open('errorModal', $scope.goToCourses, 'You are not allowed to take this quiz');
                    }
                })

        }
        initialize();
    }]);