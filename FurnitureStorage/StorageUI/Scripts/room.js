$(document).ready(function () {
    var RoomsModel = function () {
        var $scope = this;

        $scope.Rooms = ko.observableArray([]);

        $scope.QueryDate = ko.observable('');
        $scope.DialogName = ko.observable('');
        $scope.DialogTitle = ko.observable('');
        $scope.DialogErrorText = ko.observable('');
        $scope.DialogData = ko.observable(null);
        $scope.DialogSaveModel = {};

        //$scope.RoomHistoryItems = ko.observableArray([]);
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

        var MoveDialog = function (room) {
            var $scopeDialog = this;

            $scopeDialog.Room = room;
            $scopeDialog.RoomTo = ko.observable('');
            $scopeDialog.MoveDate = ko.observable(new Date());

            $scopeDialog.Furniture = ko.observable('');
            $scopeDialog.Rooms = ko.computed(function (item) {
                return $.grep($scope.Rooms(), function (room) {
                    return room.Name != $scopeDialog.Room.Name;
                });
            });

            // todo date from picker
            $scopeDialog.SaveAction = function (item) {
                if (!item.RoomTo() || !item.Furniture()) {
                    return;
                }

                $.ajax({
                    dataType: 'json',
                    url: '/Room/MoveFurniture',
                    type: "POST",
                    data: {
                        type: item.Furniture().Type,
                        roomNameFrom: item.Room.Name,
                        roomNameTo: item.RoomTo().Name,
                        date: item.MoveDate().toISOString()
                    },
                    error: function (request, error) {
                        $scope.ConsoleLogError(error);
                    },
                    success: function (data) {
                        if (data.hasOwnProperty('Error')) {
                            $scope.DialogErrorText(data.Error);
                        } else {
                            $scope.DialogHide();
                            $scope.UpdateRoomList();
                        }
                    }
                });
            }            
        };

        var RemoveDialog = function (room) {
            var $scopeDialog = this;

            $scopeDialog.Room = room;
            $scopeDialog.RoomTo = ko.observable('');
            $scopeDialog.RemoveDate = ko.observable(new Date());

            $scopeDialog.Rooms = ko.computed(function (item) {
                return $.grep($scope.Rooms(), function (room) {
                    return room.Name != $scopeDialog.Room.Name;
                });
            });

            // todo date from picker
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
                        transfer: item.RoomTo().Name,
                        date: item.RemoveDate().toISOString()
                    },
                    error: function (request, error) {
                        $scope.ConsoleLogError(error);
                    },
                    success: function (data) {
                        if (data.hasOwnProperty('Error')) {
                            $scope.DialogErrorText(data.Error);
                        } else {
                            $scope.DialogHide();
                            $scope.UpdateRoomList();
                        }
                    }
                });
            }
        };

        var Room = function (data) {
            var me = this;
            me.Name = data.Name;
            me.Furnitures = ko.observableArray([]);

            me.AddFurniture = function (room) {
                var dialogModel = {
                    Room: room.Name,
                    CreationDate: ko.observable(new Date()),
                    Furniture: ko.observable(''),                    

                    // todo date from picker
                    SaveAction: function(item) {
                        $.ajax({
                            dataType: 'json',
                            url: '/Room/CreateFurniture',
                            type: "POST",
                            data: {
                                type: item.Furniture(),
                                roomName: item.Room,
                                date: item.CreationDate().toISOString()
                            },
                            error: function(request, error) {
                                $scope.ConsoleLogError(error);
                            },
                            success: function(data) {
                                if (data.hasOwnProperty('Error')) {
                                    $scope.DialogErrorText(data.Error);
                                } else {
                                    $scope.DialogHide();
                                    $scope.UpdateRoomList();
                                }
                            }
                        });
                    }
                };
                $scope.ShowDialog('addFurnitureToRoomDlg', room.Name + ': add furniture', dialogModel);
            };

            me.MoveFurniture = function (room) {
                var dialogModel = new MoveDialog(room);
                $scope.ShowDialog('moveToRoomDlg', room.Name + ': move furniture', dialogModel);
            }

            me.RemoveRoom = function (room) {
                var dialogModel = new RemoveDialog(room);
                $scope.ShowDialog('removeRoomDlg', 'Remove room ' + room.Name, dialogModel);
            };

            for (var furnitureType in data.Furnitures) {
                me.Furnitures.push({ 'Type': furnitureType, 'Count': data.Furnitures[furnitureType] });
            }
        }

        $scope.AddRoom = function () {
            var dialogModel = {
                Room: ko.observable(''),
                CreationDate: ko.observable(new Date()),

                // todo date from picker
                SaveAction: function (item) {
                    $.ajax({
                        dataType: 'json',
                        url: '/Room/CreateRoom',
                        type: "POST",
                        data: {
                            roomName: item.Room,
                            date: item.CreationDate().toISOString()
                        },
                        error: function (request, error) {
                            $scope.ConsoleLogError(error);
                        },
                        success: function (data) {
                            if (data.hasOwnProperty('Error')) {
                                $scope.DialogErrorText(data.Error);
                            } else {
                                $scope.DialogHide();
                                $scope.UpdateRoomList();
                            }
                        }
                    });
                }
            };
            $scope.ShowDialog('addRoomDlg', 'Add room', dialogModel);
        };

        $scope.UpdateRoomList = function () {
            // todo date from picker
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