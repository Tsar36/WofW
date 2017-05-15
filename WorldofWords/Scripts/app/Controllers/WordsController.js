app.controller('WordsController', ["$scope", "$modal", "$timeout", "$modalInstance", "ModalService", "WordsService", 
    "LanguageService", "ConstService", "UserService", "languageId", "translationLanguageId", "toastr", "ShareScopeService", "RecordsService", function ($scope,
        $modal, $timeout, $modalInstance, ModalService, WordsService,LanguageService, ConstService, UserService, 
        languageId, translationLanguageId, toastr, ShareScopeService, RecordsService) {
    $scope.initialize = function () {
        $scope.selectedLanguageId = 0;
        $scope.wordsToSearch = 10;
        LanguageService.getAllLanguages()
        .then(function (response) {
            $scope.languages = response;
        });
        LanguageService.getPartsOfSpeech(languageId)
        .then(function (responce) {
            $scope.partsOfSpeech = responce;
        });
        $scope.selectedWord = '';
        $scope.selectedTranslation = '';
        $scope.selectedSyn = '';
        $scope.descriptionOfWordToAdd = '';
        $scope.transcriptionOfWordToAdd = '';
        $scope.searchSyn = '';
        $scope.selectedTag = '';
        $scope.languageIdOfWordToAdd = languageId;
        $scope.translationLanguageIdToAdd = translationLanguageId;
        $scope.record = [{
            Content: [],
            Description: '',
            WordId: 0
        }];
        $scope.commentOfWordToAdd = '';


        LanguageService.getLanguage(languageId)
        .then(function (language) {
            $scope.originalLanguage = language;
            return LanguageService.getLanguage(translationLanguageId);
        })
       .then(function (language) {
           $scope.translationLanguage = language;
        });
    }

    $scope.synonymsModel = {
        synonyms: [],
        translations: [],
        tags: []        
    }
   
    $scope.addTag = function () {
        if ($scope.selectedTag === ''){
            ModalService.showResultModal(ConstService.failureTitleForModal, ConstService.messageWhenSomeRequiredFieldsAreEmpty, false);
        }
        else if (typeof ($scope.selectedTag) === 'object') {
            $scope.synonymsModel.tags.push({
                Value: $scope.selectedTag.Value,
                Id : $scope.selectedTag.Id
            });
            $scope.selectedTag = '';
        }

        else {
            $scope.synonymsModel.tags.push({ Value: $scope.selectedTag, Id: null })
            $scope.selectedTag = '';
        }

    }

   $scope.addSyn = function () {
       if ($scope.selectedSyn === '') {
           ModalService.showResultModal(ConstService.failureTitleForModal, ConstService.messageWhenSomeRequiredFieldsAreEmpty, false);
       }
       else if (typeof ($scope.selectedSyn) === 'object') {
           $scope.synonymsModel.synonyms.push({ Value: $scope.selectedSyn.Value, Id: $scope.selectedSyn.Id });
           $scope.selectedSyn = '';
       }

       else {
           $scope.synonymsModel.synonyms.push({ Value: $scope.selectedSyn, Id: null });
           $scope.selectedSyn = '';
       }
    }

    $scope.addTran = function () {
        if ($scope.selectedTranslation === '') {
            ModalService.showResultModal(ConstService.failureTitleForModal, ConstService.messageWhenSomeRequiredFieldsAreEmpty, false);
        }
        else if (typeof ($scope.selectedTranslation) === 'object') {
            $scope.synonymsModel.translations.push({ Value: $scope.selectedTranslation.Value, Id: $scope.selectedTranslation.Id });
            $scope.selectedTranslation = '';
        }

        else {
            $scope.synonymsModel.translations.push({ Value: $scope.selectedTranslation, Id: null });
            $scope.selectedTranslation = '';
        }
        
    }

    $scope.deleteSynonym = function (index) {
        $scope.synonymsModel.synonyms.splice(index, 1);
    }

    $scope.deleteTranslation = function (index) {
        $scope.synonymsModel.translations.splice(index, 1);
    }

    $scope.deleteTag = function (index) {
        $scope.synonymsModel.tags.splice(index, 1);
    }

    $scope.selectLanguage = function () {
        WordsService.getWordsByLanguageId($scope.selectedLanguageId)
        .then(function (response) {
            $scope.words = response;
        });
    }

    $scope.close = function () {
        $modalInstance.close();
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    $scope.searchWords = function (searchWord) {
        return WordsService
            .searchForWords(searchWord, languageId, $scope.wordsToSearch)
                .then(function (words) {
                    words.forEach(function (element, index, words) {
                        if (element.Value.toLowerCase().trim() == searchWord.toLowerCase().trim()) {
                            $scope.selectedWord = element;
                        }
                    });
                    return words;
                });
    }

    $scope.searchSynonyms = function (searchWord) {
        return WordsService
            .searchForWords(searchWord, languageId, $scope.wordsToSearch)
                .then(function (words) {
                    words.forEach(function (element, index, words) {
                        if (element.Value.toLowerCase().trim() == searchWord.toLowerCase().trim()) {
                           $scope.selectedSyn = element;
                        }
                    });
                    return words;
                });
    }

    $scope.searchTranslations = function (searchWord) {
        return WordsService
            .searchForTranslations(searchWord, $scope.translationLanguageIdToAdd, $scope.wordsToSearch)
                .then(function (Translations) {
                    Translations.forEach(function (element, index, Translations) {
                        if (element.Value.toLowerCase().trim() == searchWord.toLowerCase().trim()) {
                            $scope.selectedTranslation = element;
                            
                        }
                    });
                    return Translations;
                });
    }

    $scope.searchTags = function (searchTag) {
        return WordsService
            .searchForTags(searchTag,$scope.wordsToSearch)
                .then(function (Tags) {
                    Tags.forEach(function (element, index, Tags) {
                        if (element.Value.toLowerCase().trim() == searchTag.toLowerCase().trim()) {
                            $scope.selectedTag = element;

                        }
                    });
                    return Tags;
                });
    }

   $scope.ManipulateData = function () {
        if ($scope.selectedWord === '') {
            ModalService.showResultModal(ConstService.failureTitleForModal, ConstService.messageWhenSomeRequiredFieldsAreEmpty, false);
            return;
        }

        else if (typeof ($scope.selectedWord) !== 'object') {
            var newWord = {
                value: $scope.selectedWord,
                description: $scope.descriptionOfWordToAdd,
                transcription: $scope.transcriptionOfWordToAdd,
                languageId: $scope.languageIdOfWordToAdd,
                synonyms: $scope.synonymsModel.synonyms,
                translations: $scope.synonymsModel.translations,
                tags: $scope.synonymsModel.tags,
                OwnerId: UserService.getUserData().id,
                translationLanguageId: $scope.translationLanguageIdToAdd,
                comment: $scope.commentOfWordToAdd,
                PartOfSpeechId : $scope.selectedPartOfSpeech
            }
            WordsService.addWord(newWord)
                .then(
                function (ok) {
                    if ($scope.record[0].Content.length !== 0) {
                        $scope.record[0].WordId = ok;
                        RecordsService.addRecord($scope.record[0])
                            .then(function () {
                                toastr.success("Your record was successfully uploaded!");
                            });
                    }

                    ShareScopeService.sendWordId(ok);
                },
                function (badRequest) {
                    toastr.error("Error");
                    $scope.close();
           });
        };
        toastr.success("Your word was successfuly added");
       $timeout(function () { $scope.close(); }, 2000);
    }

    $scope.goToEdit=function(word){
   
     var modalInstance = $modal.open({
            templateUrl: 'confirmModal',
            controller: 'ConfirmModalController',
            size: 'sm',
            backdrop: 'static',
            keyboard: false,
            resolve: {
                titleText: function () {
                    return 'Edit Word';
                },
                bodyText: function () {
                    return 'Such word exists. Do you want to update this word?';
                }
            }
        });

        modalInstance.result.then(function (yes) {
            if (yes) {
                var modalInstance = $modal.open({
                    templateUrl: 'Views/EditWord.html',
                    controller: 'EditWordController',
                    size: 'md',
                    backdrop: 'static',
                    keyboard: false,
                    resolve: {
                        wordV: function () {
                            return word.Value;
                        },
                        originLanguageId: function () {
                            return $scope.languageIdOfWordToAdd;
                        },
                        translationLanguageId: function () {
                            return $scope.translationLanguageIdToAdd;
                        }
                    }
                });
                $scope.close();
            }
        });

     }

    $scope.$watch('selectedWord', function (word) {
        if (word.PartOfSpeechId !== undefined && (word.PartOfSpeechId == null || word.PartOfSpeechId === $scope.selectedPartOfSpeech)) {
            $scope.isDescrFieldDisabled = $scope.isTranscrFieldDisabled = $scope.isTranFieldDisabled = $scope.isSynFieldDisabled = $scope.isTagFieldDisabled = $scope.isButtonDisabled = $scope.isPartOfSpeechDisabled = $scope.isCommentFieldDisabled = true;
            $scope.goToEdit(word);

        } else {
            $scope.isDescrFieldDisabled = $scope.isTranscrFieldDisabled = $scope.isTranFieldDisabled = $scope.isSynFieldDisabled = $scope.isTagFieldDisabled = $scope.isButtonDisabled = $scope.isButtonDisabled = $scope.isPartOfSpeechDisabled = $scope.isCommentFieldDisabled = false;
            $scope.transcriptionOfWordToAdd = '';
            $scope.descriptionOfWordToAdd = '';
            $scope.selectedSyn = '';
            $scope.selectedTag = '';
            $scope.selectedTranslation = '';
            $scope.commentOfWordToAdd = '';
        }
    });

    $scope.$watch('selectedPartOfSpeech', function (selectedPartOfSpeech) {
        if ($scope.selectedWord.PartOfSpeechId !== undefined && selectedPartOfSpeech === $scope.selectedWord.PartOfSpeechId) {
            $scope.isDescrFieldDisabled = $scope.isTranscrFieldDisabled = $scope.isTranFieldDisabled = $scope.isSynFieldDisabled = $scope.isTagFieldDisabled = $scope.isButtonDisabled = $scope.isPartOfSpeechDisabled = $scope.isCommentFieldDisabled = true;
            $scope.goToEdit($scope.selectedWord);
        }
    })
    $scope.initialize();
}]);
