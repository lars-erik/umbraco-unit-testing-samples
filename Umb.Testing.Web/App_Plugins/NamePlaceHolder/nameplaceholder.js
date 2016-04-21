(function (angular) {

    var app;

    function NamePlaceHolderController(scope, editorState) {
        scope.model.value = scope.model.value || { placeholder: "", text: "" };
        scope.state = editorState;

        scope.$watch("state.current.name", function(value) {
            scope.model.value.placeholder = value || "";
        });
    }

    function initModule() {

        app = angular.module("testing", ["umbraco.services"]);

        try {
            var umbraco = angular.module("umbraco");
            umbraco.requires.push("testing");
        } catch(e) {
            // swallow if in unit-tests
        }
    }

    function register() {
        app.controller("testing.nameplaceholder.controller", ["$scope", "editorState", NamePlaceHolderController]);
    }

    initModule();
    register();

}(angular));