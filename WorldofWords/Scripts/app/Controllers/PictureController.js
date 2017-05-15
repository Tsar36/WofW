app.controller('PictureController', ['$scope','$window', 'PictureService', 'ShareScopeService', 'toastr', function ($scope, $window, PictureService, ShareScopeService, toastr) {
    var dropbox = document.getElementById("dropbox")
    $scope.dropText = 'Drop files here...'

    const MAX_IMG_SIZE = 200 * 1024; //weight picture formula: kb * 1024
    const MAX_ALLOWED_FILES = 1;
    const ALLOWED_TYPES = ['.png', '.jpg', '.jpeg', '.gif'];

    $scope.isPictureSelected = false;
    $scope.preview = null;
    $scope.isPicture = false;
    $scope.pictureToShow = '';
    $scope.files = [];
    var wordId = null;

    $scope.$on('data_shared', function () {
        wordId = ShareScopeService.getWordId();
        if ($scope.files[0] != null)
        {
            $scope.uploadfile();
            $scope.close();
        } else {
            showPicture();
        }
    });

    var showPicture = function () {
        PictureService.getPictureByWordId(wordId)
            .then(function (response) {
                if (response != null) {
                    $scope.isPicture = true;
                    $scope.pictureToShow = response.Content;
                }
                else {
                    $scope.isPicture = false;
                }
            });
    }
    
    function dragEnterLeave(evt) {
        evt.stopPropagation()
        evt.preventDefault()
    }
    dropbox.addEventListener("dragenter", dragEnterLeave, false)
    dropbox.addEventListener("dragleave", dragEnterLeave, false)
    dropbox.addEventListener("dragover", function (evt) {
        evt.stopPropagation()
        evt.preventDefault()
    }, false)
    dropbox.addEventListener("drop", function (evt) {
        evt.stopPropagation()
        evt.preventDefault()
        $scope.$apply(function () {
            $scope.dropText = 'Drop files here...'
        })
        var files = evt.dataTransfer.files

        if (files.length > 0 && files.length <= MAX_ALLOWED_FILES) {
            $scope.$apply(function () {
                $scope.files = []
                for (var i = 0; i < files.length; i++) {

                    if (checkType(files[i].name)) {
                        if (files[i].size < MAX_IMG_SIZE) {
                            $scope.isPictureSelected = true;
                            reader.readAsDataURL(files[i]);
                        } else {
                            $scope.dropText = 'Picture is too heavy!\nTry picture smaller ' + (MAX_IMG_SIZE/1024).toString() + "kb"
                        }
                    } else {
                        $scope.dropText = 'Invalid data format. Allowed data format:' + ALLOWED_TYPES.toString();
                    }
                }
            })
        }
    }, false)
    $scope.setFiles = function (element) {
        $scope.$apply(function (scope) {
            console.log('files:', element.files);
            for (var i = 0; i < element.files.length; i++) {
                $scope.files = []
                if (checkType(element.files[i].name)) {
                    if (element.files[i].size < MAX_IMG_SIZE) {
                        $scope.isPictureSelected = true;
                        reader.readAsDataURL(element.files[i]);
                    } else {
                        $scope.dropText = 'Picture is too heavy!\nTry picture smaller ' + (MAX_IMG_SIZE / 1024).toString() + "kb"
                    }
                } else {
                    $scope.dropText = 'Invalid data format. Allowed data format:' + ALLOWED_TYPES.toString();
                }
            }
        });
    };

    $scope.uploadfile = function () {
        var content = [];

        for (var i = 0; i < $scope.files[0].length; i++) {
            content.push($scope.files[0].charCodeAt(i));
        }
        
        var pictureForUpload =
        {
            Content: content,
            WordId: wordId
        }

        PictureService.addPicture(pictureForUpload)
            .then(
            function (ok) {
                showPicture();
                $scope.cancel();
                toastr.success("Your picture was successfully uploaded!");
            },
            function (badRequest) {
               toastr.error("Fail to upload picture");
            });
    };

    $scope.deleteCurrentPicture = function () {
        PictureService.deletePicture(wordId)
            .then(
            function (ok) {
                showPicture();
                toastr.success("Your picture was successfully deleted!");
            },
            function (badRequest) {
                toastr.error("Fail to delete picture");
            });
    };

    $scope.cancel = function () {
        $scope.isPictureSelected = false;
        $scope.preview = null;
        $scope.files[0] = null;
    };

    var reader = new FileReader();
    reader.onload = function (onLoadEvent) {
        $scope.$apply(function () {
            $scope.files.push(reader.result)
            $scope.preview = reader.result;
            $scope.dropText = 'Drop files here...'
        });
    };

    var checkType = function (name) {
        for (var i = 0; i < ALLOWED_TYPES.length; i++) {
            if (name.toLowerCase().indexOf(ALLOWED_TYPES[i]) > -1) {
                return true;
            }
        }
        return false;
    }
}]);