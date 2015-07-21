$(document).ready(function () {
    var RoomsModel = function () {
        var $scope = this;

        $scope.Rooms = ko.observableArray([]);

        $scope.DialogName = ko.observable('');
        $scope.DialogTitle = ko.observable('');
        $scope.DialogData = ko.observable(null);
        $scope.DialogSaveModel = {};

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

        var Room = function (data) {
            var me = this;
            me.Name = data.Name;
            me.Furnitures = ko.observableArray([]);

            me.AddFurniture = function (room) {
                var dialogModel = {
                    Room: room.Name,
                    Date: ko.observable(new Date()),
                    Furniture: ko.observable(''),

                    SaveAction: function(item) {

                        $.ajax({
                            dataType: 'json',
                            url: '/Room/CreateFurniture',
                            type: "POST",
                            data: {
                                type: item.Furniture,
                                roomName: item.Room,
                                date: item.Date().toISOString()
                            },
                            error: function(request, error) {
                                $scope.ConsoleLogError(error);
                            },
                            success: function(data) {
                                if (data.hasOwnProperty('Error')) {
                                    alert(data.Error);
                                } else {
                                    alert('FurnitureCreated!');
                                }
                                $scope.DialogHide();
                            }
                        });
                    }
                };
                $scope.ShowDialog('addFurnitureToRoomDlg', room.Name + ': add furniture', dialogModel);
            };

            me.MoveFurniture = function (room) {
                $scope.ShowDialog('addFurnitureToRoomDlg', {});
            }

            me.RemoveRoom = function(room) {
                $scope.ShowDialog('addRoomDlg', { 'room': room, 'message': 'none' }, function(item) {
                    alert(item.message);
                });
            };

            for (var furnitureType in data.Furnitures) {
                me.Furnitures.push({ 'Type': furnitureType, 'Count': data.Furnitures[furnitureType] });
            }
        }

        $scope.AddRoom = function () {
            $("#addRoomDlg").modal("show");
        };

        $scope.SelectedRoom = ko.observable(null);

        $scope.InitData = function () {
            var date = new Date();
            $.ajax({
                dataType: 'json',
                url: '/Room/RoomsByDate',
                data: {
                    date: date.toISOString()
                },
                error: function (request, error) {
                    $scope.ConsoleLogError(error);
                },
                success: function (rooms) {
                    $.each(rooms, function (index, item) {
                        $scope.Rooms.push(new Room(item));
                    });
                }
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