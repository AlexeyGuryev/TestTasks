$(document).ready(function () {
    var _dateFormat = "DD.MM.YYYY";

    ko.bindingHandlers.masked = {
        init: function (element, valueAccessor, allBindingsAccessor) {
            var mask = allBindingsAccessor().mask || {};
            $(element).mask(mask);
            ko.utils.registerEventHandler(element, 'focusout', function () {
                var observable = valueAccessor();
                observable($(element).val());
            });
        },
        update: function (element, valueAccessor) {
            var value = ko.utils.unwrapObservable(valueAccessor());
            $(element).val(value);
        }
    };

    var InputDateControl = function (valueSetter) {
        var $controlScope = this;
        $controlScope.GetISOValue = function (value) {
            return value ? moment(value, _dateFormat).toISOString() : null;
        };

        var currentDate = moment().format(_dateFormat);
        $controlScope.ValueSetter = valueSetter;
        $controlScope.Value = ko.observable(currentDate);
        $controlScope.ValueSetter($controlScope.GetISOValue(currentDate));

        $controlScope.Value.subscribe(function(value) {            
            var dateValue = $controlScope.GetISOValue(value);
            $controlScope.ValueSetter(dateValue);
        });
        $controlScope.EndEdit = function (item) {
            if (!moment(item.Value(), _dateFormat, true).isValid())
                item.Value('');
        }
    };

    var RoomsModel = function () {
        var $scope = this;        

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
            return moment().format(_dateFormat);
        };

        $scope.GetInputDateControl = function (valueSetter) {
            return new InputDateControl(valueSetter);
        };

        var MoveDialog = function (room) {
            var $scopeDialog = this;

            $scopeDialog.Room = room;
            $scopeDialog.RoomTo = ko.observable('');
            $scopeDialog.MoveDate = ko.observable($scope.GetDefaultDate());

            $scopeDialog.Furniture = ko.observable('');
            $scopeDialog.Rooms = ko.computed(function (item) {
                return $.grep($scope.Rooms(), function (room) {
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
            $scopeDialog.RemoveDate = ko.observable($scope.GetDefaultDate());

            $scopeDialog.Rooms = ko.computed(function (item) {
                return $.grep($scope.Rooms(), function (room) {
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

        var AddFurnitureDialog = function (room) {

            var $scopeDialog = this;

            $scopeDialog.Room = room.Name;
            $scopeDialog.Furniture = ko.observable('');
            $scopeDialog.CreationDate = ko.observable($scope.GetDefaultDate());

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

        var CreateRoomDialog = function() {
            var $scopeDialog = this;

            $scopeDialog.Room = ko.observable('');
            $scopeDialog.CreationDate = ko.observable($scope.GetDefaultDate()),

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
                var dialogModel = new AddFurnitureDialog(room);
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
            var dialogModel = new CreateRoomDialog();
            $scope.ShowDialog('addRoomDlg', 'Add room', dialogModel);
        };

        $scope.DateInputEndEdit = function (dateValue) {
            if (!moment(dateValue, _dateFormat, true).isValid())
                return;
        }

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