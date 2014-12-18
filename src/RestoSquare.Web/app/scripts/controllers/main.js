'use strict';

angular.module('restosquare')
  .controller('MainCtrl', function ($rootScope, $scope, $http) {
      $scope.range = function (min, max, step) {
          step = step || 1;
          var input = [];
          for (var i = min; i <= max; i += step) input.push(i);
          return input;
      };

      $scope.request = {
          search: "Pizza"
      };

      $scope.$watch('request.address', function (adr) {
          // Reset values.
          $scope.request.lat = null;
          $scope.request.lon = null;
          $scope.message = null;
          if (adr == null || adr == '')
              return;

          // Find my location.
          var geocoder = new google.maps.Geocoder();
          geocoder.geocode({ address: adr }, function (result, status) {
              if (status === google.maps.GeocoderStatus.OK) {
                  $scope.request.lat = result[0].geometry.location.lat();
                  $scope.request.lon = result[0].geometry.location.lng();
              } else {
                  $scope.request.lat = null;
                  $scope.request.lon = null;
                  $scope.message = "Unable to find coordinates for: " + adr;
              }

              if (!$rootScope.$$phase)
                  $rootScope.$apply();

          });
      }, 250);

      $scope.applyFilter = function (type, value) {
          if ($scope.filter) {
              $scope.filter += ' and ';
          } else {
              $scope.filter = '';
          }
          if (type == 'region')
              $scope.filter += type + " eq '" + value + "'";
          else if (type == 'accommodations' || type == 'cuisine')
              $scope.filter += type + "/any(t: t eq '" + value + "')";
          else if (type == 'rating')
              $scope.filter += type + " eq " + value;
          $scope.onSearch(true);
      };

      $scope.onSearch = function (fromFacet) {


          var request = {
              method: 'GET',
              url: "https://svc.search.windows.net/indexes/restosquare/docs",
              headers: {
                  'Content-Type': 'application/json',
                  'api-key': 'mykey'
              },
              params: {
                  search: $scope.request.search,
                  $top: 50,
                  $count: true,
                  'api-version': '2014-10-20-Preview',
                  facet: ['region', 'accommodations', 'cuisine', 'rating'],
                  $filter: $scope.filter
              }
          }

          if (!fromFacet) {
              $scope.filter = null;
          }
          $scope.result = null;

          if ($scope.request.lon && $scope.request.lat) {

              if (request.params.$filter) {
                  request.params.$filter += ' and ';
              } else {
                  request.params.$filter = '';
              }
              request.params.$filter += "geo.distance(location, geography'POINT(" + $scope.request.lon + " " + $scope.request.lat + ")') le 5";
          }

          $http(request).success(function (data) {
              $scope.count = data['@odata.count'];
              $scope.result = data.value;

              var facets = data['@search.facets'];
              if (facets) {
                  if (facets.accommodations) {
                      $scope.accommodationsFacet = facets.accommodations;
                  }
                  if (facets.region) {
                      $scope.regionsFacet = facets.region;
                  }
                  if (facets.rating) {
                      $scope.ratingFacet = facets.rating;
                  }
                  if (facets.cuisine) {
                      $scope.cuisineFacet = facets.cuisine;
                  }
              }
          }).error(function (err) {
              if (err.message)
                  $scope.message = err.message;
              else
                  $scope.message = err;
              $scope.result = null;
          });
      };
  });
