$(document).ready(function () {
    var RoomsModel = function () {
        var $scope = this;

        $scope.Rooms = ko.observableArray([]);

        $scope.DialogName = ko.observable('');
        $scope.DialogTitle = ko.observable('');
        $scope.DialogData = ko.observable(null);
        $scope.DialogSaveModel = {};

        $scope.DialogRendered = function() {
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

        var DialogModel = function(data, saveAction) {
            var me = this;
            me.Data = data;
            me.SaveAction = saveAction;
        }

        var Room = function (data) {
            var me = this;
            me.Name = data.Name;
            me.Furniture = ko.observableArray(data.Furniture);

            me.AddFurniture = function (room) {
                var dialogModel = new DialogModel(
                    { 'room': room, 'message': 'none' },
                    function (item) {
                        alert(item.message);
                        $scope.DialogHide();
                    });
                $scope.ShowDialog('addRoomDlg', 'Add room', dialogModel);
            };

            me.MoveFurniture = function (room) {
                $scope.ShowDialog('addFurnitureToRoomDlg', {});
            }

            me.RemoveRoom = function (room) {
                $scope.ShowDialog('addRoomDlg', { 'room': room, 'message': 'none' }, function (item) {
                    alert(item.message);
                });

            };
        }

        $scope.AddRoom = function () {
            $("#addRoomDlg").modal("show");
        };

        $scope.SelectedRoom = ko.observable(null);

        $scope.InitData = function () {
            //$scope.GlobalBusy.start();
            $.ajax({
                dataType: 'json',
                url: '/Room/RoomsByDate',
                data: {
                    date: new Date()
                },
                error: function (request, error) {
                    //$scope.GlobalBusy.stop();
                    $scope.ConsoleLogError(error);
                },
                success: function (data) {
                    alert(data);
                    //$scope.GlobalBusy.stop();
                }
            });


            var rooms = ([
                { 'Name': 'Living room', 'Furniture': [{ 'Type': 'Desk', 'Count': 1 }] },
                { 'Name': 'Bath', 'Furniture': [{ 'Type': 'Sofa', 'Count': 2 }] },
                { 'Name': 'Kitchen', 'Furniture': [] }
            ]);

            $.each(rooms, function (index, item) {
                $scope.Rooms.push(new Room(item));
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