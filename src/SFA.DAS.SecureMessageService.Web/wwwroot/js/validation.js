var forms = $('form');
var inputs = forms.find('textarea');

forms.attr('novalidate', 'novalidate');

forms.on('submit', function () {
    inputs.each(function () {
        var result = this.checkValidity();
        if (!result) {
            showValidationMessage(this);
        } else {
            hideValidationMessage(this);
        }
    });
    return this.checkValidity();
});

var showValidationMessage = function (field) {
    var $field = $(field),
        $parent = $field.parent();

    if (!$parent.hasClass('govuk-form-group--error')) {
        var labelText = $field.data('label') || $("label[for='" + $field.attr('id') + "']").text(),
            errorMessageText = 'Enter a ' + labelText.toLowerCase(),
            errorMessage = $('<span />')
                .html(errorMessageText)
                .prop('class', 'govuk-error-message')
                .prop('id', 'validation-' + slugify(labelText));

        $field.before(errorMessage);

        $parent.addClass('govuk-form-group--error');
        $field.attr({
            'aria-describedby': 'validation-' + slugify(labelText),
            'aria-invalid': 'true'
        });
    }
}

var hideValidationMessage = function (field) {
    var $field = $(field),
        $parent = $field.parent();

    $parent.removeClass('govuk-form-group--error').find('span.govuk-error-message').remove();
    $field.removeAttr('aria-describedby').removeAttr('aria-invalid');
}

var slugify = function (text) {
    return text.toString().toLowerCase()
        .replace(/\s+/g, '-')
        .replace(/[^\w\-]+/g, '')
        .replace(/\-\-+/g, '-')
        .replace(/^-+/, '')
        .replace(/-+$/, '');
}
