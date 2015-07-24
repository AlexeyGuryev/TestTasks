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

var InputDateControl = function (valueSetter, dateFormat) {
    var $controlScope = this;
    $controlScope.DateFormat = dateFormat;

    $controlScope.GetISOValue = function (value) {
        return value ? moment(value, $controlScope.DateFormat).toISOString() : null;
    };

    var currentDate = moment().format($controlScope.DateFormat);
    $controlScope.ValueSetter = valueSetter;
    $controlScope.Value = ko.observable(currentDate);
    $controlScope.ValueSetter($controlScope.GetISOValue(currentDate));

    $controlScope.Value.subscribe(function (value) {
        var dateValue = $controlScope.GetISOValue(value);
        $controlScope.ValueSetter(dateValue);
    });
    $controlScope.EndEdit = function (item) {
        if (!moment(item.Value(), $controlScope.DateFormat, true).isValid())
            item.Value('');
    }
};