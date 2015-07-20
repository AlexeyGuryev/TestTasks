$(document).ready(function () {
    var RoomsModel = function () {
        var $scope = this;

        $scope.Rooms = ko.observableArray([]);

        var Room = function (data) {
            var me = this;
            me.Name = data.Name;
            me.Furniture = ko.observableArray(data.Furniture);

            me.AddFurniture = function (room) {
                $("#addRoomDlg").modal("show");
            };

            me.MoveFurniture = function (room) {
                $("#moveToRoomDlg").modal("show");
            }

            me.RemoveRoom = function (room) {
                alert(room.Name);
            };
        }

        $scope.AddRoom = function () {
            alert('addRoom!');
        };

        $scope.SelectedRoom = ko.observable(null);

        $scope.InitData = function () {
            var rooms = ([
                { 'Name': 'Living room', 'Furniture': [{ 'Type': 'Desk', 'Count': 1 }] },
                { 'Name': 'Bath', 'Furniture': [{ 'Type': 'Sofa', 'Count': 2 }] },
                { 'Name': 'Kitchen', 'Furniture': [] }
            ]);

            $.each(rooms, function (index, item) {
                $scope.Rooms.push(new Room(item));
            });
        };
    };

    var model = new RoomsModel();
    ko.applyBindings(model, document.getElementById('room-bindable-block'));
    model.InitData();
});