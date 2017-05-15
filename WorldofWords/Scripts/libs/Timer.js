var Timer = function () {
    var startTime = 0;
    var allTime = 0;
    var goalTime = 0;
    var isActive = false;

    this.stop = function () {
        isActive = false;
        startTime = 0;
        allTime = 0;
        goalTime = 0;
    };

    this.start = function () {
        if (isActive) {
            return;
        }
        startTime = Date.now();
        isActive = true;
    };

    this.pause = function () {

        if (!isActive) {
            return;
        }
        isActive = false;
        allTime = allTime + (Date.now() - startTime);
        startTime = 0;
    };

    this.getMiliseconds = function () {
        return allTime + Date.now() - startTime;
    };

    this.getGoalTime = function () {
        return goalTime;
    };

    this.setGoalTime = function (time) {
        goalTime = time;
    };


};