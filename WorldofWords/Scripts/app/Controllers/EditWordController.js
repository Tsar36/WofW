app.controller('EditWordController',
    function ($scope, $window, $routeParams, $modal, $modalInstance, ModalService, WordsService, LanguageService,
        ConstService, WordTranslationService, UserService, originLanguageId, translationLanguageId, ShareScopeService, RecordsService, wordV, toastr) {
    $scope.isWordDisabled = false;
    var initialize = function () {
        WordTranslationService.getInformationAboutWord(wordV, originLanguageId, translationLanguageId)
            .then(function (word) {
                $scope.oldOriginalWord = word;
                if ($scope.oldOriginalWord) {
                    oldOriginalId = $scope.oldOriginalWord.Id;
                    oldTranL = $scope.oldOriginalWord.Translations.length;
                    oldSynL = $scope.oldOriginalWord.Synonims.length;
                    oldTagL = $scope.oldOriginalWord.Tags.length;
                    oldOriginalValue = $scope.oldOriginalWord.Value;
                    oldDescription = $scope.oldOriginalWord.Description;
                    oldTranscription = $scope.oldOriginalWord.Transcription;
                    oldTranslation = $scope.oldOriginalWord.Translations;
                    oldSynonym = $scope.oldOriginalWord.Synonims;
                    oldTags = $scope.oldOriginalWord.Tags;
                    oldPartOfSpeech = $scope.oldOriginalWord.PartOfSpeechId;
                    oldComment = $scope.oldOriginalWord.Comment;

                    $scope.sendWordId();

                    RecordsService.getRecordByWordId($scope.oldOriginalWord.Id)
                    .then(function (record) {
                        if (record === null) {
                            $scope.thereAreRecords = false;
                            $scope.oldRecord = [record];
                        } else {
                            $scope.thereAreRecords = true;
                            $scope.oldRecord = [record];
                }
                        oldRecord = record;
            });
                }
            });
        LanguageService.getPartsOfSpeech(originLanguageId)
        .then(function (error) {
            $scope.partsOfSpeech = error;
        });
    };

  var oldOriginalId,
   oldTranL,
   oldSynL,
   oldTagL,
   oldOriginalValue,
   oldDescription,
   oldTranscription,
   oldTranslation,
   oldSynonym,
   oldTags,
   isBasicInfoChanged,
   oldPartOfSpeech,
   oldRecord,
   oldComment;


    $scope.sendWordId = function () {
        ShareScopeService.sendWordId(oldOriginalId);
    };
    $scope.transLanguageOfWordId = translationLanguageId;
    $scope.originalLanguageOfWordId = originLanguageId;
    $scope.selectedSyn = '';
    $scope.selectedTranslation = '';
    $scope.selectedTag = '';
    $scope.wordsToSearch = 10;
    $scope.languageIsOn = false;

    LanguageService.getLanguage(originLanguageId)
    .then(function (language) {
        $scope.originalLanguage = language;

        return LanguageService.getLanguage(translationLanguageId);
    })
    .then(function (language) {
        $scope.translationLanguage = language;
        $scope.languageIsOn = true;
    });

    $scope.editModel = {
        synonymsToAdd:[],
        translationsToAdd: [],
        tagsToAdd: [],
        synonymsToDelete: [],
        translationsToDelete: [],
        tagsToDelete:[]
    }


    $scope.editSyn = function () {

        if ($scope.selectedSyn === '') {
            ModalService.showResultModal(ConstService.failureTitleForModal, ConstService.messageWhenSomeRequiredFieldsAreEmpty, false);
        }

        else if (typeof ($scope.selectedSyn) === 'object') {
            if ($scope.editModel.synonymsToDelete.indexOf($scope.selectedSyn.Id) > -1) {
                $scope.editModel.synonymsToDelete.splice($scope.editModel.synonymsToDelete.indexOf($scope.selectedSyn.Id), 1);
                oldSynL++;
            }
            else {
                $scope.editModel.synonymsToAdd.push({ Id: $scope.selectedSyn.Id, Value: $scope.selectedSyn.Value });
            }
            $scope.oldOriginalWord.Synonims.push($scope.selectedSyn);
            $scope.selectedSyn = '';
        }
        else {

            $scope.oldOriginalWord.Synonims.push({ Value: $scope.selectedSyn });
            $scope.editModel.synonymsToAdd.push({ Value: $scope.selectedSyn, Id: null });
            $scope.selectedSyn = '';
        }
    }

    $scope.editTran = function () {

        if ($scope.selectedTranslation === '') {
            ModalService.showResultModal(ConstService.failureTitleForModal, ConstService.messageWhenSomeRequiredFieldsAreEmpty, false);
        }

        else if (typeof ($scope.selectedTranslation) === 'object') {
            if($scope.editModel.translationsToDelete.indexOf($scope.selectedTranslation.Id)>-1){
                $scope.editModel.translationsToDelete.splice($scope.editModel.translationsToDelete.indexOf($scope.selectedTranslation.Id), 1);
                oldTranL++;
            }
       else{
                $scope.editModel.translationsToAdd.push({Id:$scope.selectedTranslation.Id,Value:$scope.selectedTranslation.Value});
            }
            $scope.oldOriginalWord.Translations.push($scope.selectedTranslation);
            $scope.selectedTranslation = '';
        }
        else {
            
            $scope.oldOriginalWord.Translations.push({Value:$scope.selectedTranslation});
            $scope.editModel.translationsToAdd.push({ Value: $scope.selectedTranslation, Id: null });
            $scope.selectedTranslation = '';
        }
    }

    $scope.editTag = function () {
        if ($scope.selectedTag === '') {
            ModalService.showResultModal(ConstService.failureTitleForModal, ConstService.messageWhenSomeRequiredFieldsAreEmpty, false);
        }

        else if (typeof ($scope.selectedTag) === 'object') {
            if ($scope.editModel.tagsToDelete.indexOf($scope.selectedTag.Id) > -1) {
                $scope.editModel.tagsToDelete.splice($scope.editModel.tagsToDelete.indexOf($scope.selectedTag.Id), 1);
                oldTagL++;
            }
            else {
                $scope.editModel.tagsToAdd.push({ Id: $scope.selectedTag.Id, Value: $scope.selectedTag.Value });
            }
            $scope.oldOriginalWord.Tags.push($scope.selectedTag);
            $scope.selectedTag = '';
        }
        else {

            $scope.oldOriginalWord.Tags.push({ Value: $scope.selectedTag });
            $scope.editModel.tagsToAdd.push({ Value: $scope.selectedTag, Id: null });
            $scope.selectedTag = '';
        }
    }

    $scope.deleteSynonymsToEdit = function (index) {
        if (index > oldSynL - 1) {
            $scope.oldOriginalWord.Synonims.splice(index, 1);
            $scope.editModel.synonymsToAdd.splice(index-oldSynL, 1);
           
        }
        else {
            $scope.editModel.synonymsToDelete.push($scope.oldOriginalWord.Synonims[index].Id);
            $scope.oldOriginalWord.Synonims.splice(index, 1);
            oldSynL--;
           
        }
    }

    $scope.deleteTranslationsToEdit = function (index) {
        if (index > oldTranL - 1) {
            $scope.oldOriginalWord.Translations.splice(index, 1);
            $scope.editModel.translationsToAdd.splice(index-oldTranL, 1);
            
        }
        else {
            $scope.editModel.translationsToDelete.push($scope.oldOriginalWord.Translations[index].Id);
            $scope.oldOriginalWord.Translations.splice(index, 1);
            oldTranL--;
        }
       
    }

    $scope.deleteTagsToEdit = function (index) {
        if (index > oldTagL - 1) {
            $scope.oldOriginalWord.Tags.splice(index, 1);
            $scope.editModel.tagsToAdd.splice(index - oldTranL, 1);

        }
        else {
            $scope.editModel.tagsToDelete.push($scope.oldOriginalWord.Tags[index].Id);
            $scope.oldOriginalWord.Tags.splice(index, 1);
            oldTagL--;
        }

    }

    $scope.deleteRecordToEdit = function () {
        $scope.oldRecord[0] = null;
        $scope.thereAreRecords = false;
    }

    $scope.searchWords = function (searchWord) {
        return WordsService
            .searchForWords(searchWord, originLanguageId, $scope.wordsToSearch)
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
            .searchForTranslations(searchWord, translationLanguageId, $scope.wordsToSearch)
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
            .searchForTags(searchTag, $scope.wordsToSearch)
                .then(function (Tags) {
                    Tags.forEach(function (element, index, Tags) {
                        if (element.Value.toLowerCase().trim() == searchTag.toLowerCase().trim()) {
                            $scope.selectedTag = element;

                        }
                    });
                    return Tags;
                });
    }


    var checkIfAreAnyChangesInOriginalWord = function () {
        isBasicOriginalInfoChanged = oldDescription != $scope.oldOriginalWord.Description ||
                                     oldTranscription != $scope.oldOriginalWord.Transcription ||
                                     oldComment != $scope.oldOriginalWord.Comment ||
                                     oldPartOfSpeech != $scope.oldOriginalWord.PartOfSpeechId||
                                     $scope.editModel.translationsToAdd.length != 0 ||
                                     $scope.editModel.translationsToDelete.length != 0 ||
                                     $scope.editModel.synonymsToAdd.length != 0 ||
                                     $scope.editModel.synonymsToDelete.length != 0 ||
                                     $scope.editModel.tagsToAdd.length != 0 ||
                                     $scope.editModel.tagsToDelete.length != 0;

        return isBasicOriginalInfoChanged;
    }
    
    var checkifAreAnyChangesInRecord = function () {
        return (oldRecord !== null && $scope.oldRecord[0] === null)
            || (oldRecord === null && $scope.oldRecord[0] !== null)
            || ((oldRecord !== null && $scope.oldRecord[0]!== null) 
                &&(oldRecord.Content.toString() !== $scope.oldRecord[0].Content.toString() || oldRecord.Description !== $scope.oldRecord[0].Description));
    }
    

       
    $scope.saveWord = function () {
        if (!checkIfAreAnyChangesInOriginalWord() && !checkifAreAnyChangesInRecord()) {
            toastr.error("You haven't changed anything");
            $scope.close();
        }
        else {
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
                        return 'Are you sure you want to update this word?';
                    }
                }
            });
            modalInstance.result.then(function (yes) {
                if (yes) {
                    if (checkifAreAnyChangesInRecord()) {
                        if (oldRecord !== null && $scope.oldRecord[0] === null) {
                            RecordsService.deleteRecord($scope.oldOriginalWord.Id);
                        } else if (oldRecord === null && $scope.oldRecord[0] !== null) {
                            $scope.oldRecord[0].WordId = $scope.oldOriginalWord.Id;
                            RecordsService.addRecord($scope.oldRecord[0]);
                        } else {
                            RecordsService.deleteRecord($scope.oldOriginalWord.Id)
                                .then(function () {
                                    $scope.oldRecord[0].WordId = $scope.oldOriginalWord.Id
                                    RecordsService.addRecord($scope.oldRecord[0]);
                                })
                        }
                    }
                    if (checkIfAreAnyChangesInOriginalWord()) {
                        WordsService
                            .editWord({
                                Id: $scope.oldOriginalWord.Id,
                                Description: $scope.oldOriginalWord.Description,
                                Transcription: $scope.oldOriginalWord.Transcription,
                                OwnerId: UserService.getUserData().id,
                                SynonymsToAddIdRange: $scope.editModel.synonymsToAdd,
                                TranslationsToAddRange: $scope.editModel.translationsToAdd,
                                TagsToAddRange:$scope.editModel.tagsToAdd,
                                SynonymsToDeleteIdRange: $scope.editModel.synonymsToDelete,
                                TranslationsToDeleteIdRange: $scope.editModel.translationsToDelete,
                                TagsToDeleteIdRange:$scope.editModel.tagsToDelete,
                                TranslationLanguageId: $scope.transLanguageOfWordId,
                                OriginalLanguageId: $scope.originalLanguageOfWordId,
                                Comment: $scope.oldOriginalWord.Comment,
                                PartOfSpeechId : $scope.oldOriginalWord.PartOfSpeechId
                                
                            })
                            .then(
                            function (ok) {
                                toastr.success("Your word was successfully edited");
                            },
                            function (badRequest) {
                                toastr.error("Fail");
                            });
                    }

                    $scope.close();
                }
            });
        }
      }

    $scope.close = function () {
        ShareScopeService.modalClosed();
        $modalInstance.close();
    };

    $scope.cancel = function () {
        ShareScopeService.modalClosed();
        $modalInstance.dismiss('cancel');
    };

    initialize();

});