/// <reference path="../../../Umbraco/lib/angular/1.1.5/angular.js" />
/// <reference path="../../../Umbraco/lib/angular/1.1.5/angular-mocks.js" />
/// <reference path="../nameplaceholder.js" />
(function() {

    describe("nameplaceholder", function() {

        var scope,
            controllerFactory,
            editorState = {
                current: {}
            };

        beforeEach(function() {
            angular.module("umbraco.services", []);
        });

        beforeEach(module("testing"));

        beforeEach(inject(function ($controller, $rootScope) {
            controllerFactory = $controller;
            scope = $rootScope.$new();
            scope.model = {};
            editorState.current = {};
        }));

        function createController() {
            controllerFactory("testing.nameplaceholder.controller", { $scope: scope, editorState: editorState });
        }

        it("initializes value if nothing", function () {
            createController();
            expect(scope.model.value).toEqual({
                placeholder: "",
                text: ""
            });
        });

        it("sets placeholder to name of content", function () {
            editorState.current.name = "another name";
            createController();
            scope.$digest();
            expect(scope.model.value.placeholder).toBe("another name");
        });

        it("updates placeholder when name changes", function () {
            createController();
            scope.$digest();
            expect(scope.model.value.placeholder).toBe("");

            editorState.current.name = "another name";
            scope.$digest();
            expect(scope.model.value.placeholder).toBe("another name");
        });

    });

}());