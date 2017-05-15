app.controller('ImportWordTranslationsController', ["$scope", "$modal", "$modalInstance", "WordTranslationService", "languageId", "LanguageService",
    function ($scope, $modal, $modalInstance, WordTranslationService, languageId, LanguageService) {
        $scope.wordTranslations = [];
        $scope.itemsPerPage = 21;
        $scope.currentPage = 1;
        $scope.isChosen = true;

        LanguageService.getPartsOfSpeech(languageId)
                    .then(function (response) {
                        if (response != null) {
                            partsOfSpeech = response;
                        }
                    })

        var partsOfSpeech;

        $scope.getFileName = function () {
            $scope.fileName = document.getElementById('file').value.replace('C:\\fakepath\\', '');
            $scope.$apply();
        }

        $scope.openFile = function () {
            if ($scope.fileName.indexOf('.txt') == $scope.fileName.length - 4) {
                $scope.wordTranslations = [];
                $scope.areAllSelected = true;
           
                var file = document.getElementById('file').files[0],
                    reader = new FileReader();

                reader.onloadend = function (e) {
                    var wordTranslationExp = /([^\[\]\{\}#]+)=([^\[\]\{\}#]+)/,
                        transcriptionExp = (/(\[[^\[\]\{\}#]*\])/),
                        descriptionExp = /({[^\[\]\{\}#]*})/,
                        commentExp = /(<[^\[\]\{\}#]*>)/,
                        tagExp = /#[^\[\]\{\}#]*/g,
                        partOfSpeechExp = /@[^\[\]\{\}#]*/g,

                        data = (e.target.result)
                        .replace(/\t/g, '')
                        .replace(/ +/g, ' ')
                        .replace(/#+/g, '#')
                        .split('\n');

                    for (var i = 0; i < data.length; i++) {
                        if (wordTranslationExp.test(data[i])) {
                            $scope.wordTranslations.push({
                                originalWord: wordTranslationExp.exec(data[i])[1].trim(),

                                //array of object with transaltion and bool isChoosen, alse filter not allowe you add empty translation
                                translationWord: (function () {
                                    this.translationArray = [];
                                    this.returnArray = function () {
                                        if (wordTranslationExp.test(data[i])) {
                                            return wordTranslationExp.exec(data[i])[2].trim().split('|').filter(function (n) { return n != false })
                                            .forEach(function (element, index, array) {
                                                this.translationArray.push({ translation: element, isChoosen: true });
                                            });
                                        }
                                    }();
                                    return this.translationArray;
                                })(),

                                transcription: (function () {
                                    if (transcriptionExp.test(data[i])) {
                                        return transcriptionExp.exec(data[i])[1].trim().replace('[', '').replace(']', '');
                                    }
                                })(),

                                description: (function () {
                                    if (descriptionExp.test(data[i])) {
                                        return descriptionExp.exec(data[i])[1].trim().replace('{', '').replace('}', '');
                                    }
                                })(),

                                comment: (function () {
                                    if (commentExp.test(data[i])) {
                                        return commentExp.exec(data[i])[1].trim().replace('<', '').replace('>', '');
                                    }
                                })(),
                                partOfSpeechId: (function () {
                                    var newPartOfSpeech;
                                    if((newPartOfSpeech = partOfSpeechExp.exec(data[i])) !== null) {
                                        var pos = newPartOfSpeech[0].trim().replace('@', '');
                                        console.log(pos);
                                        for (i = 0; i < partsOfSpeech.length; i++)
                                        {
                                            console.log(i);
                                            if (pos.toLowerCase() == partsOfSpeech[i].Name.toLowerCase()) {
                                                return partsOfSpeech[i].Id;
                                                }
                                            }
                                    }
                                })(),

                                tags: (function () {
                                    var tags = [];
                                    var newTag;
                                    while ((newTag = tagExp.exec(data[i])) !== null) {
                                        tags.push({
                                            Value: newTag[0].trim().replace('#', '')
                                        });
                                    }
                                    return tags;
                                })(),


                                isChoosen: true
                            });
                        }
                    }

                    $scope.showWordTranslations();
                    $scope.$apply();
                }

                reader.readAsText(file, "utf-8");
            }
        }
        //checkbox on transaltion of word
        $scope.checkTranslation = function (isChosen, originalWord, translationWord) {
            for (var i = 0; i < $scope.wordTranslations.length; i++) {
                if (originalWord == $scope.wordTranslations[i].originalWord) {
                    for (var j = 0; j < $scope.wordTranslations[i].translationWord.length; j++) {
                        if (translationWord == $scope.wordTranslations[i].translationWord[j].translation)
                            $scope.wordTranslations[i].translationWord[j].isChoosen = isChosen;
                    }
                };
                $scope.wordTranslations[i].isChoosen = $scope.contain(i);
            };
            $scope.checkIfAllSelected();
        };

        $scope.contain = function (index) {
            for (var j = 0; j < $scope.wordTranslations[index].translationWord.length; j++) {
                if (true == $scope.wordTranslations[index].translationWord[j].isChoosen) { return true; };
            };
            return false;
        };

        $scope.showWordTranslations = function () {
            var begin = (($scope.currentPage - 1) * $scope.itemsPerPage),
                end = begin + $scope.itemsPerPage;
            $scope.wordTranslationsToShow = $scope.wordTranslations.slice(begin, end);
        };

        //checkbox on select all
        $scope.selectAll = function (areAllSelected) {
            for (var i = 0; i < $scope.wordTranslations.length; i++) {
                $scope.wordTranslations[i].translationWord.forEach(function (element, index, array) {
                    element.isChoosen = areAllSelected;
                });
                $scope.wordTranslations[i].isChoosen = areAllSelected;
            };
        };

        //checkbox on word
        $scope.checkAllTaranslationSelected = function (originalWord, wtIsChoosen) {
            for (var i = 0; i < $scope.wordTranslations.length; i++) {
                if (originalWord == $scope.wordTranslations[i].originalWord) {
                    $scope.wordTranslations[i].translationWord.forEach(function (element, index, array) {
                        element.isChoosen = wtIsChoosen;
                    });
                    $scope.wordTranslations[i].isChoosen = wtIsChoosen;
                    $scope.checkIfAllSelected();
                };
            };
        };

        $scope.bool = function (i) {
            for (var j = 0; j < $scope.wordTranslations[i].translationWord.length; j++) {
                if (false == $scope.wordTranslations[i].translationWord[j].isChoosen) { return false; };
            };
            return true;
        };

        $scope.checkIfAllSelected = function () {
            console.log("checkIfAllSelected calling....")
            for (var i = 0; i < $scope.wordTranslations.length; i++) {
                if ($scope.wordTranslations[i].isChoosen == false) $scope.areAllSelected = false;
                else {
                    $scope.areAllSelected = $scope.bool(i);
                };
            };
        };

        $scope.import = function () {
            var wordTranslations = [];

            for (var i = 0; i < $scope.wordTranslations.length; i++) {
                if ($scope.wordTranslations[i].isChoosen) {
                    for (var j = 0; j < $scope.wordTranslations[i].translationWord.length; j++) {
                        if ($scope.wordTranslations[i].translationWord[j].isChoosen)
                        {
                            wordTranslations.push({
                                OriginalWord: $scope.wordTranslations[i].originalWord,
                                TranslationWord: $scope.wordTranslations[i].translationWord[j].translation,
                                Transcription: $scope.wordTranslations[i].transcription,
                                Description: $scope.wordTranslations[i].description,
                                Tags: $scope.wordTranslations[i].tags,
                                LanguageId: languageId,
                                Comment: $scope.wordTranslations[i].comment,
                                PartOfSpeechId: $scope.wordTranslations[i].partOfSpeechId
                            });
                            console.log(wordTranslations);
                        }
                    }
                }
            }

            if (wordTranslations.length) {
                $scope.isProcessing = true;
                WordTranslationService
                    .importWordTranslations(wordTranslations)
                    .then(function (importedWordTranslations) {
                        if (importedWordTranslations) {
                            $modalInstance.close(importedWordTranslations);
                        } else {
                            $scope.openImportResultModal('Import Word Translations', 'Failed to import Word Translations', false);
                        }
                    })
            }
        }

        $scope.openImportResultModal = function (title, body, success) {
            var modalInstance = $modal.open({
                templateUrl: 'messageModal',
                controller: 'MessageModalController',
                size: 'sm',
                backdrop: 'static',
                keyboard: false,
                resolve: {
                    titleText: function () {
                        return title;
                    },
                    bodyText: function () {
                        return body;
                    },
                    success: function () {
                        return success;
                    }
                }
            });
        }

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
        };
    }]);