var MoveDialog = function (room, context) {
    var $scopeApp = context;
    var $scopeDialog = this;

    $scopeDialog.Room = room;
    $scopeDialog.RoomTo = ko.observable('');
    $scopeDialog.MoveDate = ko.observable($scopeApp.GetDefaultDate());

    $scopeDialog.Furniture = ko.observable('');
    $scopeDialog.Rooms = ko.computed(function (item) {
        return $.grep($scopeApp.Rooms(), function (room) {
            return room.Name != $scopeDialog.Room.Name;
        });
    });

    $scopeDialog.SaveAction = function (item) {
        if (!item.RoomTo() || !item.Furniture()) {
            return;
        }

        $.ajax({
            dataType: 'json',
            url: '/Room/MoveFurniture',
            type: "POST",
            data: {
                type: item.Furniture() ? item.Furniture().Type : null,
                roomNameFrom: item.Room.Name,
                roomNameTo: item.RoomTo() ? item.RoomTo().Name : null,
                date: item.MoveDate()
            },
            error: function (request, error) {
                $scopeApp.ConsoleLogError(error);
            },
            success: function (data) {
                if (data.hasOwnProperty('Error')) {
                    $scopeApp.DialogErrorText(data.Error);
                } else {
                    $scopeApp.DialogHide();
                    $scopeApp.UpdateRoomList();
                }
            }
        });
    }
};

var RemoveDialog = function (room, context) {
    var $scopeApp = context;
    var $scopeDialog = this;

    $scopeDialog.Room = room;
    $scopeDialog.RoomTo = ko.observable('');
    $scopeDialog.RemoveDate = ko.observable($scopeApp.GetDefaultDate());

    $scopeDialog.Rooms = ko.computed(function (item) {
        return $.grep($scopeApp.Rooms(), function (room) {
            return room.Name != $scopeDialog.Room.Name;
        });
    });

    $scopeDialog.SaveAction = function (item) {
        if (!item.RoomTo()) {
            return;
        }

        $.ajax({
            dataType: 'json',
            url: '/Room/RemoveRoom',
            type: "POST",
            data: {
                roomName: item.Room.Name,
                transfer: item.RoomTo() ? item.RoomTo().Name : null,
                date: item.RemoveDate()
            },
            error: function (request, error) {
                $scopeApp.ConsoleLogError(error);
            },
            success: function (data) {
                if (data.hasOwnProperty('Error')) {
                    $scopeApp.DialogErrorText(data.Error);
                } else {
                    $scopeApp.DialogHide();
                    $scopeApp.UpdateRoomList();
                }
            }
        });
    }
};

var AddFurnitureDialog = function (room, context) {
    var $scopeApp = context;

    var $scopeDialog = this;

    $scopeDialog.Room = room.Name;
    $scopeDialog.Furniture = ko.observable('');
    $scopeDialog.CreationDate = ko.observable($scopeApp.GetDefaultDate());

    $scopeDialog.SaveAction = function (item) {
        $.ajax({
            dataType: 'json',
            url: '/Room/CreateFurniture',
            type: "POST",
            data: {
                type: item.Furniture(),
                roomName: item.Room,
                date: item.CreationDate()
            },
            error: function (request, error) {
                $scopeApp.ConsoleLogError(error);
            },
            success: function (data) {
                if (data.hasOwnProperty('Error')) {
                    $scopeApp.DialogErrorText(data.Error);
                } else {
                    $scopeApp.DialogHide();
                    $scopeApp.UpdateRoomList();
                }
            }
        });
    }
};

var CreateRoomDialog = function (context) {
    var $scopeApp = context;
    var $scopeDialog = this;

    $scopeDialog.Room = ko.observable('');
    $scopeDialog.CreationDate = ko.observable($scopeApp.GetDefaultDate()),

    $scopeDialog.SaveAction = function (item) {
        var date =
        $.ajax({
            dataType: 'json',
            url: '/Room/CreateRoom',
            type: "POST",
            data: {
                roomName: item.Room,
                date: item.CreationDate()
            },
            error: function (request, error) {
                $scopeApp.ConsoleLogError(error);
            },
            success: function (data) {
                if (data.hasOwnProperty('Error')) {
                    $scopeApp.DialogErrorText(data.Error);
                } else {
                    $scopeApp.DialogHide();
                    $scopeApp.UpdateRoomList();
                }
            }
        });
    }
};