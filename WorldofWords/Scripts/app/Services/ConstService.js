app.service('ConstService', [function () {
    this.wrongName = 'Wrong Name';
    this.wrongPassword = 'Wrong Password';
    this.wrongEmail = 'Wrong Email';
    this.savedSettings = 'Settings are saved';
    this.nameNotFound = 'Name Not Found';
    this.inputData = 'Fill all fields!';
    this.invalidInput = 'Your input is invalid';
    this.invalidPassword = 'Enter password';
    this.invalidConfirmPassword = 'Passwords are different';
    this.invalidEmail = 'Enter Email';
    this.invalidConfirmPassword = 'Please input new password and confirm password';
    this.invalidName = 'Please write your full name and surname';
    this.invalidStrengthPassword = 'Password must be stronger';
    this.mayLoginMessage = 'You may now login.';
    this.retrievingLanguages = "Retrieving language list...";
    this.pleaseWaitMessage = 'Please wait. Your request is loading.';
    this.existUser = 'User with specified e-mail already exists';
    this.fillInError = 'All fields are required!';
    this.wrongEmailOrPassword = 'Wrong e-mail or password';
    this.cantHandleRequest = 'Sorry, we can\'t handle your request :(';
    this.somethingWrong = 'Something went wrong...';
    this.StudentRole = 'Student';
    this.failureMessage = 'Failure...';
    this.tryAgainMessage = 'Try again later.';
    this.TeacherRole = 'Teacher';
    this.AdminRole = 'Admin';
    this.small = 'sm';
    this.userData = 'userData';
    this.sidebarClosed = 'sidebar-closed';
    this.sidebarMinimized = 'sidebar-minimized';
    this.successMessage = 'Success!';
    this.zero = 0;
    this.name = 0;
    this.surname = 1;
    this.nameLength = 2;
    this.strength = 4;
    this.coursePath = "http://localhost:3138/Index#/Courses";
    this.teacherPath = 'http://localhost:3138/Index#/TeacherManager';
    this.USERS_ON_PAGE = 5;
    this.ID_OF_ADMIN_ROLE = 1;
    this.ID_OF_TEACHER_ROLE = 2;
    this.ID_OF_STUDENT_ROLE = 3;
    this.TEACHER_ROLE_NAME = 'Teacher';
    this.STUDENT_ROLE_NAME = 'Student';
    this.ADMIN_ROLE_NAME = 'Admin';
    this.successTitleForModal = 'Sucess';
    this.failureTitleForModal = 'Failure';
    this.messageWhenWordIsntAdded = 'Sorry, your word has not been added';
    this.messageWhenWordIsAdded = 'Word is successfully added';
    this.messageWhenWordTranslationIsAdded = 'Your word translation has been created';
    this.messageWhenSomeRequiredFieldsAreEmpty = 'Some required fields are empty';
    this.messageWhichSaysThatYouMustChooseValueFromDropdownList = 'Please, choose word from dropdown list or create you own word';
    this.messageErrorOnServerSide = 'Something went wrong on server';
    this.WORDS_ON_PAGE = 10;
    this.subjectsForRequest = ['New Role', 'Bug', 'Problem', 'Other'];
    this.unableToConnectToMicrophone = "Your browser doesn't support recording";
    this.numberOfQuizzes = 6;
    this.quizIndexes = {
        translationQuiz: 0,
        synonymQuiz: 1,
        descriptionQuiz: 2,
        mixQuiz: 3,
        pictureQuiz: 4,
        soundQuiz:5
    }
}]);