$(document).ready(function () {
    var RoomsModel = function () {
        var $scope = this;        

        $scope.DateFormat = "DD.MM.YYYY";

        $scope.Rooms = ko.observableArray([]);

        $scope.DialogName = ko.observable('');
        $scope.DialogTitle = ko.observable('');
        $scope.DialogErrorText = ko.observable('');
        $scope.DialogData = ko.observable(null);
        $scope.DialogSaveModel = {};

        $scope.ShowShortHistory = ko.observable(false);
        $scope.RoomHistoryItems = ko.observableArray([]);
        $scope.ShowShortHistory.subscribe(function() {
            $scope.UpdateRoomHistory();
        });

        $scope.DialogShow = function() {
            $(".modal").modal("show");
        };
        $scope.DialogHide = function() {
            $(".modal").modal("hide");
        }

        $scope.ShowDialog = function (dialogName, dialogTitle, model) {
            $scope.DialogData(model);
            $scope.DialogTitle(dialogTitle);
            $scope.DialogName(dialogName);
        };

        $scope.GetDefaultDate = function () {
            return moment().format($scope.DateFormat);
        };

        $scope.GetInputDateControl = function (valueSetter) {
            return new InputDateControl(valueSetter, $scope.DateFormat);
        };

        var Room = function (data) {
            var me = this;
            me.Name = data.Name;
            me.Furnitures = ko.observableArray([]);

            me.AddFurniture = function (room) {
                var dialogModel = new AddFurnitureDialog(room, $scope);
                $scope.ShowDialog('addFurnitureToRoomDlg', room.Name + ': add furniture', dialogModel);
            };

            me.MoveFurniture = function (room) {
                var dialogModel = new MoveDialog(room, $scope);
                $scope.ShowDialog('moveToRoomDlg', room.Name + ': move furniture', dialogModel);
            }

            me.RemoveRoom = function (room) {
                var dialogModel = new RemoveDialog(room, $scope);
                $scope.ShowDialog('removeRoomDlg', 'Remove room ' + room.Name, dialogModel);
            };

            for (var furnitureType in data.Furnitures) {
                me.Furnitures.push({ 'Type': furnitureType, 'Count': data.Furnitures[furnitureType] });
            }
        }

        $scope.AddRoom = function () {
            var dialogModel = new CreateRoomDialog($scope);
            $scope.ShowDialog('addRoomDlg', 'Add room', dialogModel);
        };

        $scope.QueryDate = ko.observable($scope.GetDefaultDate());
        $scope.UpdateRoomList = function () {

            $.ajax({
                dataType: 'json',
                url: '/Room/RoomsByDate',
                data: {
                    date: $scope.QueryDate()
                },
                error: function (request, error) {
                    $scope.ConsoleLogError(error);
                },
                success: function (rooms) {
                    var updatedRooms = [];
                    $.each(rooms, function (index, item) {
                        updatedRooms.push(new Room(item));
                    });
                    $scope.Rooms(updatedRooms);
                    $scope.UpdateRoomHistory();
                }
            });
        };

        $scope.UpdateRoomHistory = function () {
            $.ajax({
                dataType: 'json',
                url: '/Room/RoomHistory',
                data: {
                    shortFormat: $scope.ShowShortHistory()
                },
                error: function (request, error) {
                    $scope.ConsoleLogError(error);
                },
                success: function (roomStates) {
                    $scope.RoomHistoryItems(roomStates);
                }
            });
        };

        $scope.InitData = function () {
            $scope.UpdateRoomList();
            $(".modal").modal("hide");
            $(".modal").on('hidden.bs.modal', function() {
                $scope.DialogName('');
            });
        };

        $scope.ConsoleLogError = function(message) {
            if (window.console)
                console.error(message);
        };
    };

    var model = new RoomsModel();
    ko.applyBindings(model, document.getElementById('room-bindable-block'));
    model.InitData();
});