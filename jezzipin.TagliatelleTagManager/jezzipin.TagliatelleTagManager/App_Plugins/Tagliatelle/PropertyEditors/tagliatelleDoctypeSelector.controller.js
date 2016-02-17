angular.module("umbraco")
.controller("Tagliatelle.PropertyEditors.tagliatelleDoctypeSelector",
    function ($http, $scope, notificationsService) {
        $scope.documentTypes = [];
        
        $scope.retreiveDocumentTypes = function () {
            $http.get("/umbraco/backoffice/api/tagliatelle/DocumentTypesApi/GetDocumentTypes")
            .success(function (data) {
                $scope.documentTypes = data;
            });
        }

        $scope.retreiveDocumentTypes();
    }
);

